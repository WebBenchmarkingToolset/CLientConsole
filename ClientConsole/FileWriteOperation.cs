﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole
{
    public class FileWriteOperation
    {
        BenchmarkClient benchmarkClient;
        BenchmarkAppContext context;

        //https://localhost:7167/api/Operations/fileWrite?fileSizeMB=100

        const string REQUEST_NAME = "FileWrite";
        CSVFile<ReportModel> csvFile;


        public FileWriteOperation(BenchmarkAppContext context)
        {
            this.context = context;
            this.benchmarkClient = new BenchmarkClient(context);
            csvFile = new($"{REQUEST_NAME}.csv");
            csvFile.resetFile();
        }

        public void run()
        {
            foreach(var fileWriteConfig in context.config.fileWriteOperationConfigs)
            {
                run(fileWriteConfig.fileSize, fileWriteConfig.iterations, fileWriteConfig.threads);
            }
        }


        public void run(int fileSize,int iterations, int threads)
        {
            context.logger.Info($"Running Request '{REQUEST_NAME}' with file size: {fileSize}MB");

            string relativeURL = "/api/Operations/fileWrite?fileSizeMB="+ fileSize;
            List<BenchmarkRequestRecord> records = benchmarkClient.Run(REQUEST_NAME, HttpMethod.Get, relativeURL, "",
                threads, iterations);

            IEnumerable<ReportModel> avg = records
                .Where(x=>x.StatusCode==200)
                
                .GroupBy(x => 
                x.hostName,
                x => x.responseBody,
                (hostName, val) => new ReportModel(){ 
                    hostName= hostName,
                    avg=(val.Select(x=>Int32.Parse(x)).Average().ToInt()),
                    fileSize= fileSize
                    }
                );

            csvFile.append(avg);
        }

    }

    public class FileWriteOperationModel
    {

    }

    internal class ReportModel
    {
        public required string hostName { get; set;}
        public required int avg { get; set;}
        public required int fileSize { get; set;}
    }
}
