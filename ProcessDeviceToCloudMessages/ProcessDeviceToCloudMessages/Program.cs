using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace ProcessDeviceToCloudMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            string iotHubConnectionString = "HostName=srramdemo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=np+WT8KRP41II8lxdvoSXkTEXTlW1v3glwIQR9wo2CU=";
            string iotHubD2cEndpoint = "messages/events";
            StoreEventProcessor.StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storagefordemodfsrram;AccountKey=RUTukQbuykqb1LS1+3Az4rubAbuS/gY1N8b3nNvKg+HPdSW0TZtbk6PvCOyvQqNj8SOJvAYv7f/T+5icX+5/nQ==;BlobEndpoint=https://storagefordemodfsrram.blob.core.windows.net/;TableEndpoint=https://storagefordemodfsrram.table.core.windows.net/;QueueEndpoint=https://storagefordemodfsrram.queue.core.windows.net/;FileEndpoint=https://storagefordemodfsrram.file.core.windows.net/";
            StoreEventProcessor.ServiceBusConnectionString = "HostName=srramdemo.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=Wj7vlYAiMTQIjdDe+niS3IRmn6ZoTUU9rYeEisZjmHU=";

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost(eventProcessorHostName, iotHubD2cEndpoint, EventHubConsumerGroup.DefaultGroupName, iotHubConnectionString, StoreEventProcessor.StorageConnectionString, "messages-events");
            Console.WriteLine("Registering EventProcessor...");
            eventProcessorHost.RegisterEventProcessorAsync<StoreEventProcessor>().Wait();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
