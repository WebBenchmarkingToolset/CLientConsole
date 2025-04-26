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
    CSVFile<DataOperationModel> csvFile = new($"ClientOperationsOutput - {DateTime.Now:yyyy-MM-dd HHmmss}.csv");
    csvFile.resetFile();

    new FileWriteOperation(appContext, csvFile).run();
    new FileReadOperation(appContext, csvFile).run();
    new MemoryWriteOperation(appContext, csvFile).run();
    new MemoryReadOperation(appContext, csvFile).run();
    new CPUStressOperation(appContext, csvFile).run();
    new NetworkStressOperation(appContext, csvFile).run();
    new CustomLoadOperation(appContext, csvFile).run();


    appContext.logger.Success("*********************Execution Succeed**************************");
}
catch (Exception ex)
{
    appContext.logger.Error(ex.ToDetailedString());
}

appContext.logger.Info("*********************Execution Finished**************************");

string logFileName = $"logs\\{DateTime.Now:yyyy-MM-dd HHmmss}.log";
appContext.logger.saveFile(logFileName);
appContext.logger.Log("log file saved " + logFileName);

Console.ReadLine();
