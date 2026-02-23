using Serilog.Sinks.File;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace EZCashWedge
{
    public class ArchiveFileHook : FileLifecycleHooks
    {
        private readonly string _archiveDirectory;
        private readonly int _rollOutDays;
        private static bool _isProcessing = false;

        public ArchiveFileHook(string archiveDirectory, int rollOutDays)
        {
            _archiveDirectory = archiveDirectory;
            if (!Directory.Exists(_archiveDirectory)) Directory.CreateDirectory(_archiveDirectory);
            this._rollOutDays = rollOutDays;
        }

        public override Stream OnFileOpened(string path, Stream underlyingStream, Encoding encoding)
        {
            if (_isProcessing)
                return base.OnFileOpened(path, underlyingStream, encoding);

            try
            {
                string fileName = Path.GetFileName(path);

                // Only act on the numbered roll file
                if (fileName.Contains("_00") || fileName.Contains("_0"))
                {
                    _isProcessing = true;

                    string folder = Path.GetDirectoryName(path);
                    string staticPath = Path.Combine(folder, "EZCashWedgeServer.txt");

                    // 1. Close the stream Serilog just opened for the _001 file
                    underlyingStream.Dispose();

                    // 2. Delete the _001 file immediately so it doesn't clutter the folder
                    if (File.Exists(path)) File.Delete(path);

                    if (File.Exists(staticPath))
                    {
                        // 3. INSTANT SWAP: Rename the 100MB file to a temp name
                        // This frees up "EZCashWedgeServer.txt" immediately
                        string tempFile = Path.Combine(folder, $"temp_{Guid.NewGuid()}.tmp");
                        File.Move(staticPath, tempFile);

                        // 4. BACKGROUND ZIP: Run the heavy work on another thread
                        Task.Run(() =>
                        {
                            try
                            {
                                string timestamp = DateTime.Now.ToString("yyyyMMdd_hhmm_tt");
                                string zipPath = Path.Combine(_archiveDirectory, $"EZCashWedgeServer-{timestamp}.zip");

                                using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                                {
                                    archive.CreateEntryFromFile(tempFile, "EZCashWedgeServer.txt");
                                }
                                File.Delete(tempFile); // Delete temp after zipping
                                CleanupOldArchives(_rollOutDays);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Trace.WriteLine("Background Zip Error: " + ex.Message);
                            }
                        });
                    }

                    // 5. IMMEDIATE RETURN: Give Serilog a fresh EZCashWedgeServer.txt right away
                    var newStream = new FileStream(staticPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    _isProcessing = false;
                    return newStream;
                }
            }
            catch (Exception ex)
            {
                _isProcessing = false;
                System.Diagnostics.Trace.WriteLine("Hook Error: " + ex.Message);
            }

            return base.OnFileOpened(path, underlyingStream, encoding);
        }

        private void CleanupOldArchives(int daysToKeep)
        {
            try
            {
                DateTime cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                DirectoryInfo directory = new DirectoryInfo(_archiveDirectory);
                FileInfo[] files = directory.GetFiles("*.zip");

                foreach (FileInfo file in files)
                {
                    if (file.LastWriteTime < cutoffDate)
                    {
                        file.Delete();
                        LogEvents($"Deleting {file.Name}, {daysToKeep} days older file from archived folder.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Cleanup Error: " + ex.Message);
            }
        }

        private static void LogEvents(string input)
        {
            Logger.LogWithNoLock($"{input}");
        }
    }
}
