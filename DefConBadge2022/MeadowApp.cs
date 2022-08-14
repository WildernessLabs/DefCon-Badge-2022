using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DefConBadge2022
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        IHardwareConfig hardware;

        IBadgePage currentPage;

        IBadgePage[] pages;

        BufferRgb888 splash;
        BufferRgb888 logo;

        PwmLed buttonLed;

        Random rand = new Random();

        public override async Task Run()
        {
            /*
            _ = Task.Run(() =>
            {
                var message = "  D3FC0N Meadow F7 Micro V2";

                while (true)
                {
                    for (int x = 0; x < message.Length * 4; x++)
                    {
                        hardware.GraphicsSecondary.Clear();
                        hardware.GraphicsSecondary.DrawText(0 - x, 1, message);
                        hardware.GraphicsSecondary.Show();

                        Thread.Sleep(75);
                    }

                    Thread.Sleep(2000 + rand.Next(3000));
                }
            });
            */

            while (true)
            {
                DrawSplash(splash);
                await Task.Delay(3000);

                currentPage.StartUpdating(hardware);
                await Task.Delay(20000);
                currentPage.StopUpdating();

                DrawSplash(logo);
                await Task.Delay(3000);

                currentPage.StartUpdating(hardware);
                await Task.Delay(20000);
                currentPage.StopUpdating();
            }
        }

        public override Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            buttonLed = new PwmLed(Device, Device.Pins.D03, new Voltage(3.3));

            buttonLed.StartPulse(TimeSpan.FromMilliseconds(2000), 1, 0);

            hardware = new ProjLabHardwareConfig(Device);

            hardware.ButtonLeft.Clicked += ButtonLeft_Clicked;
            hardware.ButtonRight.Clicked += ButtonRight_Clicked;
        //    hardware.ButtonDown.Clicked += ButtonDown_Clicked;
        //    hardware.ButtonUp.Clicked += ButtonUp_Clicked;

            hardware.RgbPwmLed.SetColor(Color.Green);

            pages = new IBadgePage[]
            {
                new GraphPage(),
                new EnvironmentPage(),
                new WiFiTrackerPage(),
            };

            currentPage = pages[0];

            hardware.Graphics.Clear();
            hardware.Graphics.DrawText(0, 0, "Initializing ...", Color.White);
            hardware.Graphics.Show();

            splash = LoadImage("mask.jpg");
            logo = LoadImage("d3fc0n.jpg");

            return Task.CompletedTask;
        }

        void DrawSplash(IPixelBuffer buffer)
        {
            hardware.Graphics.Clear();

            hardware.Graphics.DrawBuffer(0, 0, buffer);

            Console.WriteLine("Jpeg show");

            hardware.Graphics.Show();
        }

        BufferRgb888 LoadImage(string name)
        {
            var jpgData = LoadResource(name);

            Console.WriteLine($"Loaded {jpgData.Length} bytes, decoding jpeg ...");

            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            Console.WriteLine($"Jpeg decoded is {jpg.Length} bytes");
            Console.WriteLine($"Width {decoder.Width}");
            Console.WriteLine($"Height {decoder.Height}");

            return new BufferRgb888(decoder.Width, decoder.Height, jpg);
        }

        private void ButtonUp_Clicked(object sender, EventArgs e)
        {
            currentPage.Up();
        }

        private void ButtonDown_Clicked(object sender, EventArgs e)
        {
            currentPage.Down();
        }

        private void ButtonRight_Clicked(object sender, EventArgs e)
        {
            currentPage.Right();
        }

        private void ButtonLeft_Clicked(object sender, EventArgs e)
        {
            currentPage.Left();
        }

        byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"DefConBadge2022.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}