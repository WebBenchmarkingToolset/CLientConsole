using ClientConsole.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole
{
    public static class Utilities
    {
        public static int ToInt(this double value)
        {
            return (int)value; // Truncates the decimal part
        }


        public static void SaveToCsv<T>(IEnumerable<T> data, string filePath, char delimiter = ',')
        {
            if (data == null || !data.Any())
                throw new ArgumentException("Data collection is empty or null.");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var csvBuilder = new StringBuilder();

            // Header row
            csvBuilder.AppendLine(string.Join(delimiter, properties.Select(p => p.Name)));

            // Data rows
            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item, null);
                    return value != null ? $"\"{value.ToString().Replace("\"", "\"\"")}\"" : "";
                });

                csvBuilder.AppendLine(string.Join(delimiter, values));
            }

            File.WriteAllText(filePath, csvBuilder.ToString(), Encoding.UTF8);
        }
    }

    public class BenchmarkAppContext
    {
        public required ConfigModel config;
        public required Logger logger;
    }



    public class DataOperationModel : BenchmarkRequestRecord
    {
        public int? dataSizeMB { get; set; }
        public int? loadThreads { get; set; }
        public int? loadIterations { get; set; }
        public int? fileSize { get; set; }

    }

}
