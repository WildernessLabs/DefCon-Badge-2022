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
using Meadow;
using Meadow.Devices.Esp32.MessagePayloads;

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
            Resolver.Log.Info("Connection request completed.");
        }

        async Task ScanForAndDisplayAccessPoints(IWiFiNetworkAdapter wifi)
        {
            Resolver.Log.Info("Getting list of access points.");
            var networks = await wifi.Scan(TimeSpan.FromSeconds(60));
            Draw(networks);
        }

        public async Task GetWebPageViaHttpClient(string uri)
        {
            Resolver.Log.Info($"Requesting {uri} - {DateTime.Now}");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 5, 0);

                HttpResponseMessage response = await client.GetAsync(uri);

                try
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Resolver.Log.Info(responseBody);
                }
                catch (TaskCanceledException)
                {
                    Resolver.Log.Warn("Request time out.");
                }
                catch (Exception e)
                {
                    Resolver.Log.Error($"Request went sideways: {e.Message}");
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
                Resolver.Log.Info("|-------------------------------------------------------------|---------|");
                Resolver.Log.Info("|         Network Name             | RSSI |       BSSID       | Channel |");
                Resolver.Log.Info("|-------------------------------------------------------------|---------|");

                int yPosition = 0;
                DrawMessage("| Name | RSSI | BSSID | Ch |", null, WildernessLabsColors.AzureBlue, yPosition);
                yPosition += 18;
                foreach (WifiNetwork accessPoint in networks)
                {
                    Resolver.Log.Info($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |");
                    DrawMessage($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |", null, WildernessLabsColors.GalleryWhite, 0);
                    yPosition += 18;
                }
            }
            else
            {
                Resolver.Log.Info($"No access points detected.");
                DrawMessage($"No access points detected.", null, WildernessLabsColors.GalleryWhite, 0);
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