using Microsoft.Win32;
using System;

namespace mitoSoft.Common.UriSchema
{
    public class RegistryHelper
    {
        public static void RegisterScheme(string uriSchema, string applicationPath, bool asFirstArg)
        {
            using var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + uriSchema);

            key.SetValue("", "URL:" + $"{uriSchema} Protocol"); //FriendlyName
            key.SetValue("URL Protocol", "");

            using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
            {
                defaultIcon.SetValue("", applicationPath + ",1");
            }

            string extension = "";
            if (asFirstArg)
            {
                extension = " \"%1\"";
            }
            using var commandKey = key.CreateSubKey(@"shell\open\command");
            commandKey.SetValue("", $"\"{applicationPath}\"{extension}");
        }

        public static string? IsRegistered(string uriSchema)
        {
            using var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + uriSchema);

            if (key == null)
            {
                return null;
            }

            using var commandKey = key.CreateSubKey(@"shell\open\command");
            if (commandKey.ValueCount <= 0)
            {
                return null;
            }

            var value = commandKey.GetValue("");

            string path = ParseValueToPath(value);

            return path;
        }

        private static string ParseValueToPath(object? value)
        {
            try
            {
                if (value == null)
                {
                    throw new Exception("No value to parse");
                }
                var path = value.ToString();
                path = path.Replace("\"", "");
                path = path.Replace(" %1", "");
                return path;
            }
            catch (Exception ex)
            {
                throw new Exception($"Value '{value?.ToString()}' not possible to parse: {ex.Message}");
            }
        }
    }
}