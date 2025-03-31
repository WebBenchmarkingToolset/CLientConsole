using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole.operations
{
    public class FileWriteOperation
    {
        BenchmarkClient benchmarkClient;
        BenchmarkAppContext context;

        //https://localhost:7167/api/Operations/fileWrite?fileSizeMB=100

        const string REQUEST_NAME = "FileWrite";
        CSVFile<FileWriteOperationModel> csvFile;


        public FileWriteOperation(BenchmarkAppContext context)
        {
            this.context = context;
            benchmarkClient = new BenchmarkClient(context);
            csvFile = new($"{REQUEST_NAME}.csv");
            csvFile.resetFile();
        }

        public void run()
        {
            foreach (var fileWriteConfig in context.config.fileWriteOperationConfigs)
            {
                run(fileWriteConfig.fileSize, fileWriteConfig.iterations, fileWriteConfig.threads);
            }
        }


        public void run(int fileSize, int iterations, int threads)
        {
            context.logger.Info($"Running Request '{REQUEST_NAME}' with file size: {fileSize}MB");

            string relativeURL = "/api/Operations/fileWrite?fileSizeMB=" + fileSize;
            List<BenchmarkRequestRecord> records = benchmarkClient.Run(REQUEST_NAME, HttpMethod.Get, relativeURL, "",
                threads, iterations);

            /*            IEnumerable<ReportModel> avg = records
                            .Where(x=>x.StatusCode==200)

                            .GroupBy(x => 
                            x.hostName,
                            x => x.responseBody,
                            (hostName, val) => new ReportModel(){ 
                                hostName= hostName,
                                avg=(val.Select(x=>Int32.Parse(x)).Average().ToInt()),
                                fileSize= fileSize
                                }
                            );*/

            IEnumerable<FileWriteOperationModel> operationRecords = records.Select(x => new FileWriteOperationModel()
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

    public class FileWriteOperationModel : BenchmarkRequestRecord
    {
        public required int fileSize { get; set; }
    }
    /*
        internal class ReportModel
        {
            public required string hostName { get; set;}
            public required int avg { get; set;}
            public required int fileSize { get; set;}
        }
    */
}
