using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //A class which wraps the barometric sensor
        BME280Sensor BME280;

        public MainPage()
        {
            this.InitializeComponent();
        }

        // This method will be called by the application framework when the page is first loaded
        protected override async void OnNavigatedTo(NavigationEventArgs navArgs)
        {
            Debug.WriteLine("MainPage::OnNavigatedTo");



            try
            {
                // Create a new object for our sensor class
                BME280 = new BME280Sensor();
                //Initialize the sensor
                await BME280.Initialize();

                //Create variables to store the sensor data: temperature, pressure, humidity and altitude. 
                //Initialize them to 0.
                float temp = 0;
                float pressure = 0;
                float altitude = 0;
                float humidity = 0;

                //Create a constant for pressure at sea level. 
                //This is based on your local sea level pressure (Unit: Hectopascal)
                const float seaLevelPressure = 1022.00f;

                //Read 10 samples of the data
                //for (int i = 0; i < 31; i++)
                while(true)
                {
                    temp = await BME280.ReadTemperature();
                    pressure = await BME280.ReadPreasure();
                    altitude = await BME280.ReadAltitude(seaLevelPressure);
                    humidity = await BME280.ReadHumidity();

                    //Write the values to your debug console
                    Debug.WriteLine("Temperature: " + temp.ToString() + " deg C");
                    Debug.WriteLine("Humidity: " + humidity.ToString() + " %");
                    Debug.WriteLine("Pressure: " + pressure.ToString() + " Pa");
                    Debug.WriteLine("Altitude: " + altitude.ToString() + " m");
                    Debug.WriteLine("");

                    var telemetryDataPoint = new
                    {
                        deviceId = "rb2",
                        sentDttm = DateTime.UtcNow.ToString(),
                        measPressure = pressure.ToString(),
                        measTemperature = temp.ToString(),
                        measAltitude = altitude.ToString(),
                        measHumidity = humidity.ToString()
                    };

                    DeviceClient deviceClient;
                    string iotHubUri = "srramiothub.azure-devices.net";
                    string deviceKey = "aao7QiNVZr5mGblgn8Lf/LxOW9SW4CQvLQ7m83knBbQ=";
                    deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("rb2", deviceKey));
                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    var message = new Message(Encoding.ASCII.GetBytes(messageString));
                    Debug.WriteLine("Message sent is : " + messageString);


                    await deviceClient.SendEventAsync(message);

                    Task.Delay(60000).Wait();

                   
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


    }
}
