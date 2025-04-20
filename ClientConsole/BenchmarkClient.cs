using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClientConsole
{
    public class BenchmarkClient
    {

        BenchmarkAppContext context;
        private static long initialTimeStamp = DateTime.UtcNow.Ticks;

        public BenchmarkClient(BenchmarkAppContext context)
        {
            this.context = context;
            var x=initialTimeStamp;
        }
        
        public async Task<BenchmarkHttpResponse> Send(HttpMethod method, string url, HttpContent content)
        {
            HttpClient? HttpClient = new HttpClient();
            HttpClient.Timeout= TimeSpan.FromMinutes(21); ;
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, url) { Content = content };

            Stopwatch sw = Stopwatch.StartNew();
            HttpResponseMessage response = await HttpClient.SendAsync(requestMessage);
            string responseStr = await response.Content.ReadAsStringAsync();
            sw.Stop();

            return new() {
                StatusCode= (int)response.StatusCode,
                responseBody = responseStr,
                period= Convert.ToInt64(sw.Elapsed.TotalMilliseconds)
            };
        }

        public List<BenchmarkRequestRecord> Run(string requestName, HttpMethod method,
            string relativeUrl, string requestBody, int threads, int iterations)
        {
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            return Run( requestName,  method, relativeUrl,  content,  threads,  iterations);
        }

        public List<BenchmarkRequestRecord> Run(string requestName, HttpMethod method, 
            string relativeUrl, HttpContent content, int threads, int iterations)
        {
            List<BenchmarkRequestRecord> responses = new();

            foreach (var host in context.config.Hosts) {
                string url = host.BaseUrl + relativeUrl;

                for (int i = 0; i < iterations; i++)
                {
                    context.logger.Info($"Sleeping {context.config.SleepBeforeIteration}ms before Iteration {i}");
                    Thread.Sleep(context.config.SleepBeforeIteration);

                    Parallel.For(0, threads, threardNumber =>
                    {
                        BenchmarkHttpResponse response = Send(method, url, content).Result;
                        //context.logger.Info($"Request Host Iteration Thread StatusCode period");
                        if (response.StatusCode==200)
                            context.logger.Info($"{requestName} {host.Name} {i} {threardNumber} ({response.period}ms)");
                        else
                            context.logger.Error($"{requestName} {host.Name} {i} {threardNumber} ({response.period}ms). Error Details: {response.StatusCode}, {response.responseBody}");

                        responses.Add(new()
                        {
                            TimeStamp=DateTime.UtcNow.Ticks- initialTimeStamp,
                            period = response.period,
                            responseBody = response.responseBody,
                            StatusCode = response.StatusCode,
                            iteration = i,
                            threadId = threardNumber,
                            hostName = host.Name,
                            requestName = requestName,
                            hasError = (response.StatusCode == 200) ? 0 : 1
                        });
                    });
                }
                context.logger.Info($"Sleeping after Host {context.config.SleepAfterHost}ms");
                Thread.Sleep(context.config.SleepAfterHost);
            }
            return responses;
        }
    }

    public class BenchmarkHttpResponse
    {
        public required int StatusCode { get; set; }
        public required string responseBody { get; set;}
        public required long period { get; set;}
    }


    public class BenchmarkRequestRecord
    {
        public required long TimeStamp { get; set;}
        public required int StatusCode { get; set;}
        public required string responseBody { get; set; }
        public required long period { get; set;}
        public required int threadId { get; set;}
        public required int iteration { get; set;}
        public required string hostName { get; set;}
        public required string requestName { get; set;}
        public required int hasError { get; set; } = 1;

    }


    public class HttpBenckmarkRequest
    {
        public required string realativeUrl { get; set; }
        public required HttpMethod method { get; set;}
        public required string requestBody { get; set;}

    }
}
