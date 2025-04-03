﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole.operations
{
    public class CPUStressOperation
    {
        BenchmarkClient benchmarkClient;
        BenchmarkAppContext context;

        const string REQUEST_NAME = "cpuStress";
        CSVFile<CPUStressOperationModel> csvFile;


        public CPUStressOperation(BenchmarkAppContext context)
        {
            this.context = context;
            benchmarkClient = new BenchmarkClient(context);
            csvFile = new($"{REQUEST_NAME}.csv");
            csvFile.resetFile();
        }

        public void run()
        {
            foreach (var operationConfig in context.config.cpuStressOperationConfigs)
            {
                run(operationConfig.loadIterations, operationConfig.loadThreads, operationConfig.iterations, operationConfig.threads);
            }
        }


        public void run(int loadIterations, int loadThreads, int iterations, int threads)
        {
            context.logger.Info($"Running Request '{REQUEST_NAME}' with load Iterations: {loadIterations}. and loadThreads:{loadThreads}");

            //https://localhost:7167/api/Operations/cpuStress?operations=100&threads=10
            string relativeURL = $"/api/Operations/cpuStress?operations={loadIterations}&threads={loadThreads}" ;
            List<BenchmarkRequestRecord> records = benchmarkClient.Run(REQUEST_NAME, HttpMethod.Get, relativeURL, "",
                threads, iterations);

            IEnumerable<CPUStressOperationModel> operationRecords = records.Select(x => new CPUStressOperationModel()
            {
                period = x.period,
                responseBody = x.responseBody,
                StatusCode = x.StatusCode,
                iteration = x.iteration,
                threadId = x.threadId,
                hostName = x.hostName,
                requestName = x.requestName,
                hasError = x.hasError,
                loadIterations=loadIterations,
                loadThreads=loadThreads
            });
            csvFile.append(operationRecords);
        }

    }

    public class CPUStressOperationModel : BenchmarkRequestRecord
    {
        public required int loadThreads { get; set; }
        public required int loadIterations { get; set; }
    }
}
