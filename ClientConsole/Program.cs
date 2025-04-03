using ClientConsole;
using ClientConsole.operations;
using ClientConsole.utilities;
using System.Text.Json;

Console.WriteLine("Hello,");

BenchmarkAppContext appContext = new()
{
    config = AppConfig.ReadConfig(),
    logger = new Logger()
};

try
{


    new FileWriteOperation(appContext).run();
    new FileReadOperation(appContext).run();
    new MemoryWriteOperation(appContext).run();
    new MemoryReadOperation(appContext).run();
    new CPUStressOperation(appContext).run();
    new NetworkStressOperation(appContext).run();


    appContext.logger.Success("*********************Execution Succeed**************************");
}
catch (Exception ex)
{
    appContext.logger.Error($""+ ex.Message);
}

appContext.logger.Info("*********************Execution Finished**************************");
Console.ReadLine();
