using System;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace QueueTrigger
{
    public class Trigger
    {
        [FunctionName("Trigger")]
        public void Run([QueueTrigger("queue", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            var data = myQueueItem;
            // Deserialize Data here
            var finalData = JsonConvert.DeserializeObject<FileData>(myQueueItem);
            // send data to the table
            sendDataToTable(finalData);
        }

        private static bool sendDataToTable(FileData queueData)
        {
            // now we will call owr web api to send the data to the table.
            HttpClient httpClient = new HttpClient();
            using var content = new StringContent(JsonConvert.SerializeObject(queueData), Encoding.UTF8, "application/json");
            HttpResponseMessage response = httpClient.PostAsync(Environment.GetEnvironmentVariable("URL"), content).Result;
            var dataStatus = response.RequestMessage;
            return true;
        }
    }
}
