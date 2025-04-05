using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientConsole.utilities;

namespace ClientConsole.operations
{
    public class MemoryWriteOperation
    {
        BenchmarkClient benchmarkClient;
        BenchmarkAppContext context;

        const string REQUEST_NAME = "MemoryWrite";
        CSVFile<ClientConsole.DataOperationModel> csvFile;


        public MemoryWriteOperation(BenchmarkAppContext context, CSVFile<DataOperationModel> csvFile_)
        {
            this.context = context;
            benchmarkClient = new BenchmarkClient(context);
            csvFile = csvFile_;
        }

        public void run()
        {
            foreach (var memoryWriteConfig in context.config.memoryWriteOperationConfigs)
            {
                run(memoryWriteConfig.dataSizeMB, memoryWriteConfig.iterations, memoryWriteConfig.threads);
            }
        }


        public void run(int dataSizeMB, int iterations, int threads)
        {
            context.logger.Info($"Running Request '{REQUEST_NAME}' with data size: {dataSizeMB}MB");

            //https://localhost:7167/api/Operations/memoryWrite?dataSizeMB=100
            string relativeURL = "/api/Operations/memoryWrite?dataSizeMB=" + dataSizeMB;
            List<BenchmarkRequestRecord> records = benchmarkClient.Run(REQUEST_NAME, HttpMethod.Get, relativeURL, "",
                threads, iterations);

            IEnumerable<ClientConsole.DataOperationModel> operationRecords = records.Select(x => new ClientConsole.DataOperationModel()
            {
                period = x.period,
                responseBody = x.responseBody,
                StatusCode = x.StatusCode,
                iteration = x.iteration,
                threadId = x.threadId,
                hostName = x.hostName,
                requestName = x.requestName,
                hasError = x.hasError,
                dataSizeMB = dataSizeMB
            });
            csvFile.append(operationRecords);
        }

    }

}
