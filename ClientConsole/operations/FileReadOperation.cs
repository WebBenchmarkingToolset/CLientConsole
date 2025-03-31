using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole.operations
{
    public class FileReadOperation
    {
        BenchmarkClient benchmarkClient;
        BenchmarkAppContext context;

        //https://localhost:7167/api/Operations/fileRead?fileSizeMB=100

        const string REQUEST_NAME = "FileRead";
        CSVFile<FileOperationModel> csvFile;


        public FileReadOperation(BenchmarkAppContext context)
        {
            this.context = context;
            benchmarkClient = new BenchmarkClient(context);
            csvFile = new($"{REQUEST_NAME}.csv");
            csvFile.resetFile();
        }

        public void run()
        {
            foreach (var fileReadConfig in context.config.fileReadOperationConfigs)
            {
                run(fileReadConfig.fileSize, fileReadConfig.iterations, fileReadConfig.threads);
            }
        }


        public void run(int fileSize, int iterations, int threads)
        {
            context.logger.Info($"Running Request '{REQUEST_NAME}' with file size: {fileSize}MB");

            string relativeURL = "/api/Operations/fileRead?fileSizeMB=" + fileSize;
            List<BenchmarkRequestRecord> records = benchmarkClient.Run(REQUEST_NAME, HttpMethod.Get, relativeURL, "",
                threads, iterations);

            IEnumerable<FileOperationModel> operationRecords = records.Select(x => new FileOperationModel()
            {
                period = x.period,
                responseBody = x.responseBody,
                StatusCode = x.StatusCode,
                iteration = x.iteration,
                threadId = x.threadId,
                hostName = x.hostName,
                requestName = x.requestName,
                hasError = x.hasError,
                fileSize = fileSize
            });
            csvFile.append(operationRecords);
        }

    }
}
