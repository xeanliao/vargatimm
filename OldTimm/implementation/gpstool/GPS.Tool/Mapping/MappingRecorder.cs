using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace GPS.Tool.Mapping
{
    class MappingRecorder
    {
        public static int GetCurrentId(MappingTables table)
        {
            string key = string.Format("{0}CurrentId", table);
            string value = ConfigurationManager.AppSettings[key];
            if (value != null)
            {
                try
                {
                    return int.Parse(value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return 0;
        }

        public static void SetCurrentId(MappingTables table, int currentId)
        {
            string key = string.Format("{0}CurrentId", table);
            UpdateAppConfig(key, currentId.ToString());
        }

        private static void UpdateAppConfig(string newKey, string newValue)
        {
            bool isModified = false;
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == newKey)
                {
                    isModified = true;
                }
            }

            // Open App.Config of executable
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // You need to remove the old settings object before you can replace it
            if (isModified)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            // Add an Application Setting.
            config.AppSettings.Settings.Add(newKey, newValue);
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
        }


    }
}
