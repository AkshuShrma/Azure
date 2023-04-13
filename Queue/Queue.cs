using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queue
{
    public class Queue
    {
        public static string connstring = "DefaultEndpointsProtocol=https;AccountName=bloblumakin;AccountKey=Y61FT79SlbgigtBEvoylHBmBjrC33gi1GFYVV0tveZ67BX7xZxJeGrKOlNNN4LX1nG/ncKI0ammq+AStDGsDQw==;EndpointSuffix=core.windows.net";
        static void Main(string[] args)
        {
            AddMessage();
            Console.ReadKey();
        }

        public static void AddMessage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connstring);
            CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference("aaaaa");
            CloudQueueMessage queueMessage = new CloudQueueMessage("Hello, Message Created by Console Application");
            cloudQueue.AddMessage(queueMessage);
            Console.WriteLine("Message sent");

        }
    }
}
