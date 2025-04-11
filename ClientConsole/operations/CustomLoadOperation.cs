using ClientConsole.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ClientConsole.operations
{
    public class CustomLoadOperation
    {
        BenchmarkClient benchmarkClient;
        BenchmarkAppContext context;

        const string REQUEST_NAME = "customLoad";
        CSVFile<ClientConsole.DataOperationModel> csvFile;


        public CustomLoadOperation(BenchmarkAppContext context, CSVFile<DataOperationModel> csvFile_)
        {
            this.context = context;
            benchmarkClient = new BenchmarkClient(context);
            csvFile = csvFile_;
        }

        public void run()
        {
            foreach (var operationConfig in context.config.customLoadOperationConfigs)
            {
                run(operationConfig);
            }
        }


        public void run(CustomLoadOperationConfig customLoadOperationConfig)
        {
            context.logger.Info($"Running Request '{REQUEST_NAME}', with name '{customLoadOperationConfig.name}'");

            var fileContent = new ByteArrayContent(File.ReadAllBytes(customLoadOperationConfig.filePath));

            var content = new MultipartFormDataContent();
            content.Add(fileContent, "PythonFile", "pythonCode.py");
            content.Add(new StringContent(customLoadOperationConfig.resultVarName), "ResultVariableName");


            //post https://localhost:7167/api/Operations/runLoad
            string relativeURL = $"/api/Operations/runLoad";
            List<BenchmarkRequestRecord> records = benchmarkClient.Run(REQUEST_NAME, HttpMethod.Post, relativeURL, content,
                customLoadOperationConfig.threads, customLoadOperationConfig.iterations);

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
                name = customLoadOperationConfig.name,
            });
            csvFile.append(operationRecords);
        }
    }

}
