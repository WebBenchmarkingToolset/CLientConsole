using ClientConsole;
using ClientConsole.utilities;
using System.Text.Json;

Console.WriteLine("Hello, World!");


BenchmarkAppContext appContext = new()
{
    config = AppConfig.ReadConfig(),
    logger = new Logger()
};

new FileWriteOperation(appContext).run();


appContext.logger.Info("*********************Execution Finished**************************");
Console.ReadLine();