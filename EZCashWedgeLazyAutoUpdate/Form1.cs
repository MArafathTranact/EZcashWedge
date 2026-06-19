using System.ComponentModel;
using System.Diagnostics;

namespace EZCashWedgeLazyAutoUpdate
{
    public partial class frmLazyUpdate : Form
    {
        public frmLazyUpdate()
        {
            InitializeComponent();
        }

        private async void frmLazyUpdate_Load(object sender, EventArgs e)
        {

            // 2. Get the working area of the primary display (excludes the Taskbar)
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            // 3. Calculate Coordinates:
            // X = Left edge of the screen (0)
            // Y = Bottom edge of the working area minus the height of this form
            int xCoordinate = workingArea.Right - this.Width;
            int yCoordinate = workingArea.Bottom - this.Height;

            // 4. Assign the calculated point to the form's location
            this.Location = new Point(xCoordinate, yCoordinate);


            CopyAutoUpdater();

            await Task.Delay(1500);

            string tempFolder = Path.Combine(Path.GetTempPath(), "EZCashWedgeAutoUpdate");
            var destinationExecutablePath = Path.Combine(tempFolder, "EZCashWedgeAutoUpdater.exe");

            LaunchExecutable(destinationExecutablePath);

            await Task.Delay(2000);
            Environment.Exit(0);
        }

        private void CopyAutoUpdater()
        {
            try
            {

#if DEBUG
                var servicePath = @"C:\Program Files (x86)\EZCash\EZCashWedge";

                var sourceConfigPath = Path.Combine(servicePath, "UpdateEZCashServiceConfig.config");
                var SourceExecutablePath = Path.Combine(servicePath, "EZCashWedgeAutoUpdater.exe");
#else
                var servicePath = Path.GetDirectoryName(Application.ExecutablePath);

                var sourceConfigPath = Path.Combine(servicePath, "UpdateEZCashServiceConfig.config");
                var SourceExecutablePath = Path.Combine(servicePath, "EZCashWedgeAutoUpdater.exe");
#endif

                // Use a specific subfolder in Temp to avoid clutter
                string tempFolder = Path.Combine(Path.GetTempPath(), "EZCashWedgeAutoUpdate");

                if (Directory.Exists(tempFolder))
                {
                    try
                    {
                        Directory.Delete(tempFolder, true);
                    }
                    catch { /* Ignore or log: file might be locked */ }
                }

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                var destinationConfigPath = Path.Combine(tempFolder, "UpdateEZCashServiceConfig.config");
                var destinationExecutablePath = Path.Combine(tempFolder, "EZCashWedgeAutoUpdater.exe");


                File.Copy(SourceExecutablePath, destinationExecutablePath, true);


                // Config is often required for the app to run/connect to DB
                if (File.Exists(sourceConfigPath))
                {
                    File.Copy(sourceConfigPath, destinationConfigPath, true);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private static void LaunchExecutable(string path)
        {
            try
            {

                //ApplicationLoader.PROCESS_INFORMATION procInfo;
                //ApplicationLoader.StartProcessAndBypassUAC(path, "", out procInfo);


                ProcessStartInfo startInfo = new()
                {
                    FileName = path,
                    UseShellExecute = true, // Set to true to use the OS shell (standard for EXEs)
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                using Process exeProcess = Process.Start(startInfo);

                // Optional: If you want the C# app to wait until the EXE closes:
                // exeProcess.WaitForExit(); 
            }
            catch (Win32Exception ex)
            {
                // Specifically catches "File not found" or "Access denied"
                MessageBox.Show($"Platform error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
