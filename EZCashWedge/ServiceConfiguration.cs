using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZCashWedge
{
    public class ServiceConfiguration
    {

        public static string GetFileLocation(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public static string GetDecryptedToken(string name)
        {
            var encryptToken = string.Empty;
            try
            {
                var decryptedToken = ConfigurationManager.AppSettings[name];
                encryptToken = TokenEncryptDecrypt.Decrypt(decryptedToken);

                return encryptToken;
            }
            catch (Exception ex)
            {
                Logger.LogExceptionWithNoLock(" Exception at GetDecryptedToken(). ", ex);
                return encryptToken;
            }

        }
    }
}
