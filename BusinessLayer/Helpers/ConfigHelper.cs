using System;
using System.Configuration;
namespace BusinessLayer.Helpers
{
    public static class ConfigHelper
    {
        public static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
