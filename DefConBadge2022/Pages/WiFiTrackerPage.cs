using Meadow.Foundation.Graphics;
using System.Threading;
using System;
using System.Threading.Tasks;
using Meadow.Foundation;
using Meadow.Gateways.Bluetooth;
using Meadow.Hardware;
using Meadow.Gateway.WiFi;
using System.Net.Http;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DefConBadge2022
{
    public class WiFiTrackerPage : IBadgePage
    {
        IHardwareConfig config;

        MicroGraphics graphics;
        IWiFiNetworkAdapter wifi;

        public bool IsUpdating = false;

        public void StartUpdating(IHardwareConfig config)
        {
            this.config = config;

            graphics = config.Graphics;

            IsUpdating = true;

            Task.Run(async () =>
            {
                while (IsUpdating)
                {
                    await ScanForAndDisplayAccessPoints(wifi);
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
            });
        }

        public void StopUpdating()
        {
            IsUpdating = false;
        }

        void WiFiAdapter_NetworkConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Connection request completed.");
        }

        async Task ScanForAndDisplayAccessPoints(IWiFiNetworkAdapter wifi)
        {
            Console.WriteLine("Getting list of access points.");
            var networks = await wifi.Scan(TimeSpan.FromSeconds(60));
            Draw(networks);
        }

        public async Task GetWebPageViaHttpClient(string uri)
        {
            Console.WriteLine($"Requesting {uri} - {DateTime.Now}");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 5, 0);

                HttpResponseMessage response = await client.GetAsync(uri);

                try
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request time out.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }

        void DrawMessage(string label, string value, Color color, int yPosition)
        {
            graphics.DrawText(x: 0, y: yPosition, label, color: WildernessLabsColors.AzureBlue);
            graphics.DrawText(x: 10, y: yPosition + 18, value,
                alignmentH: HorizontalAlignment.Left,
                color: color);
        }

        void Draw(IList<WifiNetwork> networks)
        {
            graphics.Clear();

            if (networks.Count > 0)
            {
                Console.WriteLine("|-------------------------------------------------------------|---------|");
                Console.WriteLine("|         Network Name             | RSSI |       BSSID       | Channel |");
                Console.WriteLine("|-------------------------------------------------------------|---------|");

                int yPosition = 0;
                DrawMessage("| Name | RSSI | BSSID | Ch |", null, WildernessLabsColors.GalleryWhite, yPosition);
                graphics.DrawText(x: 0, y: yPosition, "| Name | RSSI | BSSID | Ch |", color: WildernessLabsColors.AzureBlue);
                yPosition += 18;
                foreach (WifiNetwork accessPoint in networks)
                {
                    Console.WriteLine($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |");
                    //DrawMessage("Temperature:", $"{temp.Celsius:N1}C", WildernessLabsColors.GalleryWhite, 0);
                }
            }
            else
            {
                Console.WriteLine($"No access points detected.");
            }

            if (config.TemperatureSensor.Temperature is { } temp)
            {
                DrawMessage("Temperature:", $"{temp.Celsius:N1}C", WildernessLabsColors.GalleryWhite, 0);
            }

            if (config.PressureSensor.Pressure is { } pressure)
            {
                DrawMessage("Pressure:", $"{pressure.StandardAtmosphere:N2}atm", WildernessLabsColors.GalleryWhite, 60);
            }

            if (config.HumiditySensor.Humidity is { } humidity)
            {
                DrawMessage("Humidity:", $"{humidity.Percent:N1}%", WildernessLabsColors.GalleryWhite, 120);
            }


            if (config.LightSensor.Illuminance is { } light)
            {
                if (light is { } lightReading)
                {
                    DrawMessage("Lux:", $"{lightReading:N0}Lux", WildernessLabsColors.GalleryWhite, 180);
                }
            }

            graphics.Show();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public void Up()
        {
            throw new System.NotImplementedException();
        }

        public void Down()
        {
            throw new System.NotImplementedException();
        }

        public void Left()
        {
            throw new System.NotImplementedException();
        }

        public void Right()
        {
            throw new System.NotImplementedException();
        }

        public void Init(IHardwareConfig config)
        {
        }
    }
}