using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Uchu.Core.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationPathAttribute : Attribute
    {
        /// <summary>
        /// Returns an absolute path relative to a configuration path.
        /// </summary>
        /// <param name="configurationLocation">File location of the configuration.</param>
        /// <param name="relativePath">Relative path to make absolute.</param>
        /// <returns>Absolute path of the relative path.</returns>
        public static string ReplaceRelativePath(string configurationLocation, string relativePath)
        {
            // Normalize the relative path.
            configurationLocation ??= "";
            relativePath ??= "";
            relativePath = relativePath.Replace(configurationLocation.Contains('/', StringComparison.CurrentCulture) ? "\\" : "/", configurationLocation.Contains('/', StringComparison.CurrentCulture) ? "/" : "\\", StringComparison.CurrentCulture);
            
            // Return the replaced path.
            if (Path.IsPathRooted(relativePath)) return relativePath;
            return Path.Join(Path.GetDirectoryName(configurationLocation), relativePath);
        }
        
        /// <summary>
        /// Replaces local configuration paths relative to the configuration file.
        /// </summary>
        /// <param name="configurationLocation">File location of the configuration.</param>
        /// <param name="configurationObject">Object to replace the paths in.</param>
        public static void ReplaceFilePaths(string configurationLocation, object configurationObject)
        {
            // Return if there is no object to replace.
            if (configurationObject == null)
            {
                return;
            }
        
            // Iterate over the properties.
            foreach (var property in configurationObject.GetType().GetProperties())
            {
                if (property.GetIndexParameters().Length != 0) continue;
                var value = property.GetValue(configurationObject);
                if (value == null) continue;
                if (property.GetCustomAttribute<ConfigurationPathAttribute>() != null)
                {
                    // Replace the strings or list of strings.
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(configurationObject, ReplaceRelativePath(configurationLocation, (string) value));
                    }
                    else if (property.PropertyType == typeof(string[]))
                    {
                        var paths = (string[]) value;
                        for (var i = 0; i < paths.Length; i++)
                        {
                            paths[i] = ReplaceRelativePath(configurationLocation, paths[i]);
                        }
                    }
                    else if (property.PropertyType == typeof(List<string>))
                    {
                        var paths = (List<string>) value;
                        for (var i = 0; i < paths.Count; i++)
                        {
                            paths[i] = ReplaceRelativePath(configurationLocation, paths[i]);
                        }
                    }
                }
                else
                {
                    ReplaceFilePaths(configurationLocation, value);
                }
            }
        }
    } 
}
