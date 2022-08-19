using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Displays;
using Meadow.Units;
using SimpleJpegDecoder;
using System;
using System.Linq;
using System.Diagnostics;
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

        MenuPage menuPage;

        IBadgePage[] pages;

        BufferRgb888 splash;
        BufferRgb888 logo;

        PwmLed buttonLed;

        //Random rand = new Random();

        // TODO: Possibly worth making these part of some inherited system of hardware config classes?
        const bool isProjectLabBoardV1 = false;
        const bool useTertiaryDisplay = false;
        const bool useSecondaryDisplay = true;
        const bool useMikroBusButtonLed = true;

        const float defaultBrightness = 0.2f;
        const float brightnessChangeAmount = 0.1f;
        const double lowestAmbientBrightnessThreshold = 40;
        const double highestAmbientBrightnessThreshold = 1500;
        float _tertiaryDisplayBrightness = defaultBrightness;
        float tertiaryDisplayBrightness {
            get { return _tertiaryDisplayBrightness; }
            set { _tertiaryDisplayBrightness = Math.Clamp(value, 0.0f, 1.0f); }
        }
        float AdjustDisplayBrightnessToAmbientBrightness()
        {
            float adjustedBrightness = defaultBrightness;
            if (hardware.LightSensor.Illuminance is { } light)
            {
                if (light is { } lightReading)
                {
                    adjustedBrightness = (float)Math.Clamp(lightReading.Lux, lowestAmbientBrightnessThreshold, highestAmbientBrightnessThreshold).Map(lowestAmbientBrightnessThreshold, highestAmbientBrightnessThreshold, 0.1, 1.0);
                    Console.WriteLine($"Lux: {lightReading.Lux}, Adjusted: {adjustedBrightness}");
                }
            }
            return adjustedBrightness;
        }

        public override async Task Run()
        {
            hardware.Graphics.Clear();
            hardware.Graphics.DrawText(0, 0, "Initializing ...", Color.White);
            hardware.Graphics.Show();

            if (useSecondaryDisplay)
            {
                _ = Task.Run(() =>
                {
                    var message = "  D3FC0N + Meadow!!";

                    while (true)
                    {
                        for (int x = 0; x < message.Length * (hardware.GraphicsSecondary.Width + 1); x++)
                        {
                            hardware.GraphicsSecondary.Clear();
                            hardware.GraphicsSecondary.DrawText(0 - x, 0, message);
                            hardware.GraphicsSecondary.Show();

                            Thread.Sleep(75);
                        }

                        Thread.Sleep(2000);
                    }
                }).ConfigureAwait(false);
            }

            if (useTertiaryDisplay)
            {
                _ = Task.Run(() =>
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    int maxCompleteCharsPerScreen = (int)Math.Floor(hardware.GraphicsTertiary.Width / (hardware.GraphicsTertiary.CurrentFont.Width + 1.0));
                    var message = (new string(' ', maxCompleteCharsPerScreen + 1)) + "Meadow@D3FC0N";
                    Console.WriteLine(message);

                    while (true)
                    {
                        for (int x = 0; x < message.Length * (hardware.GraphicsTertiary.CurrentFont.Width + 1); x++)
                        {
                            if (stopwatch.ElapsedMilliseconds > 500)
                            {
                                Console.WriteLine("Adjusting brightness");
                                tertiaryDisplayBrightness = AdjustDisplayBrightnessToAmbientBrightness();
                                stopwatch.Restart();
                            }
                            hardware.GraphicsTertiary.Clear();
                            hardware.GraphicsTertiary.DrawText(0 - x, 1, message, new Color(tertiaryDisplayBrightness, tertiaryDisplayBrightness, tertiaryDisplayBrightness));
                            hardware.GraphicsTertiary.Show();

                            Thread.Sleep(0);
                        }

                        Thread.Sleep(1000);
                    }
                }).ConfigureAwait(false);
            }


            splash = LoadImage("mask.jpg");
            logo = LoadImage("d3fc0n.jpg");

            while (true)
            {
                DrawSplash(splash);
                await Task.Delay(10000).ConfigureAwait(false);

                currentPage.StartUpdating(hardware);
                await Task.Delay(20000).ConfigureAwait(false);
                currentPage.StopUpdating();

                DrawSplash(logo);
                await Task.Delay(3000).ConfigureAwait(false);

                currentPage.StartUpdating(hardware);
                await Task.Delay(20000).ConfigureAwait(false);
                currentPage.StopUpdating();
            }
        }

        public override Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            if (useMikroBusButtonLed)
            {
                buttonLed = new PwmLed(Device, Device.Pins.D03, new Voltage(3.3));

                buttonLed.StartPulse(TimeSpan.FromMilliseconds(2000), 1, 0);
            }

            hardware = new ProjLabHardwareConfig(Device, isProjectLabBoardV1);

            hardware.ButtonLeft.Clicked += ButtonLeft_Clicked;
            hardware.ButtonRight.Clicked += ButtonRight_Clicked;
            // Button down/up interrupt issues on v1 Project Lab Boards. Resolved in v2.
            if (!isProjectLabBoardV1)
            {
                hardware.ButtonDown.Clicked += ButtonDown_Clicked;
                hardware.ButtonUp.Clicked += ButtonUp_Clicked;
            }

            hardware.RgbPwmLed.SetColor(Color.Green);

            menuPage = new MenuPage();
            menuPage.RequestPageChange += MenuPage_RequestPageChange;

            pages = new IBadgePage[]
            {
                menuPage,
                new EnvironmentPage(),
                new GraphPage(),
                new WiFiTrackerPage(),
            };

            currentPage = pages[0];

            return base.Initialize();
        }

        private void MenuPage_RequestPageChange(string pageName)
        {
            IBadgePage page = pageName switch
            {
                nameof(MenuPage) => pages.FirstOrDefault(p => p.GetType() == typeof(MenuPage)),
                nameof(EnvironmentPage) => pages.FirstOrDefault(p => p.GetType() == typeof(EnvironmentPage)),
                nameof(WiFiTrackerPage) => pages.FirstOrDefault(p => p.GetType() == typeof(MenuPage)),
                _ => null,
            };

            // TODO: Figure out how to switch pages.
            //       This will result in a non-existent breakpoint triggering at SamplingSensorBase.cs:67 (a closing brace?).
            currentPage.StopUpdating();
            currentPage = page;
            currentPage.StartUpdating(hardware);
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
            tertiaryDisplayBrightness += brightnessChangeAmount;
            Console.WriteLine($"Brightness: {tertiaryDisplayBrightness}");
            currentPage.Right();
        }

        private void ButtonLeft_Clicked(object sender, EventArgs e)
        {
            tertiaryDisplayBrightness -= brightnessChangeAmount;
            Console.WriteLine($"Brightness: {tertiaryDisplayBrightness}");
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