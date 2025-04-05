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
    CSVFile<DataOperationModel> csvFile = new($"ClientOperationsOutput.csv");
    csvFile.resetFile();

    new FileWriteOperation(appContext, csvFile).run();
    new FileReadOperation(appContext, csvFile).run();
    new MemoryWriteOperation(appContext, csvFile).run();
    new MemoryReadOperation(appContext, csvFile).run();
    new CPUStressOperation(appContext, csvFile).run();
    new NetworkStressOperation(appContext, csvFile).run();


    appContext.logger.Success("*********************Execution Succeed**************************");
}
catch (Exception ex)
{
    appContext.logger.Error($""+ ex.Message);
}

appContext.logger.Info("*********************Execution Finished**************************");
Console.ReadLine();
