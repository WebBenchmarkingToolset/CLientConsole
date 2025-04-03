using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole.operations
{
    public class NetworkStressOperation
    {
        BenchmarkClient benchmarkClient;
        BenchmarkAppContext context;

        const string REQUEST_NAME = "NetworkStress";
        CSVFile<DataOperationModel> csvFile;


        public NetworkStressOperation(BenchmarkAppContext context)
        {
            this.context = context;
            benchmarkClient = new BenchmarkClient(context);
            csvFile = new($"{REQUEST_NAME}.csv");
            csvFile.resetFile();
        }

        public void run()
        {
            foreach (var operationConfig in context.config.networkStressOperationConfigs)
            {
                run(operationConfig.dataSizeMB, operationConfig.iterations, operationConfig.threads);
            }
        }


        public void run(int dataSizeMB, int iterations, int threads)
        {
            context.logger.Info($"Running Request '{REQUEST_NAME}' with data size: {dataSizeMB}");

            long fileSize = dataSizeMB * 1024 * 1024; // to bytes
            byte[] file = new byte[fileSize];
            MemoryStream memStream = new MemoryStream();
            memStream.Write(file, 0, file.Length);
            memStream.Seek(0, SeekOrigin.Begin);


            var content = new MultipartFormDataContent();
            
            content.Add(new StreamContent(memStream), "DataFile", "data.bin");

            //https://localhost:7167/api/Operations/dataFile
            string relativeURL = $"/api/Operations/dataFile" ;
            List<BenchmarkRequestRecord> records = benchmarkClient.Run(REQUEST_NAME, HttpMethod.Post, relativeURL, content,
                threads, iterations);

            IEnumerable<DataOperationModel> operationRecords = records.Select(x => new DataOperationModel()
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
