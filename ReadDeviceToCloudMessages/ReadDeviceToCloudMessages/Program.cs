using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ReadDeviceToCloudMessages
{
    class Program
    {

        static string connectionString = "HostName=srramiothub.azure-devices.net;DeviceId=rb2;SharedAccessKey=owGSUyvpUR2Cg19itAskk44/hkHbka6Dk4rgf8A0lnY=";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;
        private async static Task ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine(string.Format("Message received. Partition: {0} Data: '{1}'", partition, data));
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Receive messages\n");
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }
           // Console.ReadLine();

        }
    }
}
