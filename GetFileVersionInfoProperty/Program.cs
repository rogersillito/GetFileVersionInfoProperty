using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GetFileVersionInfoProperty
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Environment.ExitCode = 0;
            if (!args.Any() || args.Length < 2 || new[] { "/?", "-h", "--help" }.Contains(args[0].ToLower()))
            {
                ShowUsage();
                return;
            }

            var filePath = args[0];
            var propertyName = args[1];
            if (!File.Exists(filePath))
            {
                Error($"file not found at path ({filePath})");
                return;
            }

            var fv = FileVersionInfo.GetVersionInfo(filePath);
            if (new[] { "/l", "-l", "--list" }.Contains(propertyName.ToLower()))
            {
                var properties = typeof(FileVersionInfo).GetProperties();
                foreach (var prop in properties.OrderBy(p => p.Name))
                {
                    Console.WriteLine(prop.Name);
                }
                return;
            }

            var propertyInfo = typeof(FileVersionInfo).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propertyInfo == null)
            {
                Error($"file property not found ({propertyName})");
                return;
            }

            var value = propertyInfo.GetValue(fv);
            Console.WriteLine(value);
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: FileProp.exe FILEPATH PROPERTY_NAME");
            Console.WriteLine("       FileProp.exe FILEPATH --list");
            Console.WriteLine("       FileProp.exe FILEPATH -l");
            Console.WriteLine("       FileProp.exe FILEPATH /l");
        }

        private static void Error(string msg)
        {
            Console.WriteLine("ERROR: " + msg);
            Environment.ExitCode = 1;
        }
    }
}