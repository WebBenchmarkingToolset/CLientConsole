using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole.utilities
{

    public class CSVFile<T>
    {
        public CSVFile(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
        char delimiter = ',';

        public void resetFile()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var csvBuilder = new StringBuilder();

            // Header row
            csvBuilder.AppendLine(string.Join(delimiter, properties.Select(p => p.Name)));

            File.WriteAllText(FilePath, csvBuilder.ToString(), Encoding.UTF8);
        }


        public void append(IEnumerable<T> data)
        {
            if (!File.Exists(FilePath))
            {
                resetFile();
            }

            if (data == null)
                throw new ArgumentException("Data collection is empty or null.");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var csvBuilder = new StringBuilder();

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

            File.AppendAllText(FilePath, csvBuilder.ToString(), Encoding.UTF8);
        }

    }

}
