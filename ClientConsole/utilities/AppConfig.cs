using ClientConsole.operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientConsole.utilities
{
    public class AppConfig
    {
        const string CONFIG_FILE_NAME = "Config.json";
        public static ConfigModel ReadConfig()
        {
            if (!File.Exists(CONFIG_FILE_NAME))
                throw new FileNotFoundException(CONFIG_FILE_NAME);

            ConfigModel? config = JsonSerializer.Deserialize<ConfigModel>(File.ReadAllText(CONFIG_FILE_NAME));
            if (config==null)
                throw new NullReferenceException("Faild parsing config file");
            return config;
        }

        public static void GenerateConfigFile()
        {
            File.WriteAllText($"Config.template.json",JsonSerializer.Serialize<ConfigModel>(new ConfigModel()));
        }
    }


    public class ConfigModel
    {
        public HostModel[] Hosts { get; set; } = { new()};
        public int SleepAfterHost { get; set; } = 2000;
        public int SleepBeforeIteration { get; set; } = 2000;

        public FileOperationConfig[]  fileWriteOperationConfigs { get; set; } =[];
        public FileOperationConfig[]  fileReadOperationConfigs { get; set; } = [];
        public DataOperationConfig[] memoryWriteOperationConfigs { get; set; } = [];
        public DataOperationConfig[] memoryReadOperationConfigs { get; set; } = [];
        public CpuStressOperationConfig[] cpuStressOperationConfigs { get; set; } = [];
        public DataOperationConfig[] networkStressOperationConfigs { get; set; } = [];
    }


    public class HostModel
    {
        public string BaseUrl { get; set; } = "https://localhost:7167";
        public string Name { get; set; } = "Host1";

    }


    public class OperationConfig
    {
        public int threads { get; set; } = 1;
        public int iterations { get; set; } = 1;
    }

    public class FileOperationConfig: OperationConfig
    {
        public int fileSize { get; set; } = 100;
    }
    
    public class DataOperationConfig : OperationConfig
    {
        public int dataSizeMB { get; set; } = 100;
    }
    
    public class CpuStressOperationConfig : OperationConfig
    {
        public int loadIterations { get; set; } = 1000;
        public int loadThreads { get; set; } = 1;
    }
}
