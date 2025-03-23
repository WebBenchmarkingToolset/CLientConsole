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
    public class HttpBenchmarkTools
    {
        public int SleepAfterHost { get; set; } = 2000;
        public int SleepAfterIteration { get; set; } = 2000;



        public async Task<BenchmarkHttpLog> Send(HttpMethod method, string url, string requestBody)
        {
            HttpClient? HttpClient = null;
            HttpClient = new HttpClient();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, url) { Content = content };

            Stopwatch sw = Stopwatch.StartNew();
            HttpResponseMessage response = await HttpClient.SendAsync(requestMessage);
            string responseStr = await response.Content.ReadAsStringAsync();
            sw.Stop();

            return new() {
                StatusCode= response.StatusCode.ToString(),
                responseBody = responseStr,
                period= Convert.ToInt64(sw.Elapsed.TotalMilliseconds)
            };
        }
        
        
        public async Task<List<BenchmarkRequestRecord>> Run(string requestName, HttpMethod method, string relativeUrl
            , string requestBody, int threads, int iterations)
        {
            List<BenchmarkRequestRecord> logs = new();

            foreach (var host in Env.i.Hosts) {
                string url = host.BaseUrl + relativeUrl;

                for (int i = 0; i < iterations; i++)
                {
                    Parallel.For(0, threads, async threardNumber =>
                    {
                        var response = await Send(method, url, requestBody);
                        logs.Add(new()
                        {
                            period = response.period,
                            responseBody = response.responseBody,
                            StatusCode = response.StatusCode,
                            iteration = i,
                            threadId= threardNumber,
                            nodeName=host.Name,
                            requestName= requestName

                        });
                    });
                    Thread.Sleep(SleepAfterIteration);
                }
                Thread.Sleep(SleepAfterHost);
            }
            return logs;
        }
    }

    public class BenchmarkHttpLog
    {
        public required string StatusCode;
        public required string responseBody;
        public required long period;
    }


    public class BenchmarkRequestRecord
    {
        public required string StatusCode;
        public required string responseBody;
        public required long period;
        public required int threadId;
        public required int iteration;
        public required string nodeName;
        public required string requestName;

    }


    public class HttpBenckmarkRequest
    {
        public required string realativeUrl;
        public required HttpMethod method;
        public required string requestBody;

    }
}
