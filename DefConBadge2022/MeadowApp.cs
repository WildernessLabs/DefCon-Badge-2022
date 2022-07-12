using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using System;
using System.Threading.Tasks;

namespace DefConBadge2022
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        IHardwareConfig hardware;

        IBadgePage currentPage;

        IBadgePage[] pages;

        public override Task Run()
        {
            currentPage.StartUpdating(hardware);
            return Task.CompletedTask;
        }

        public override Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            hardware = new HardwareConfig(Device);

            hardware.ButtonLeft.Clicked += ButtonLeft_Clicked;
            hardware.ButtonRight.Clicked += ButtonRight_Clicked;
            hardware.ButtonDown.Clicked += ButtonDown_Clicked;
            hardware.ButtonUp.Clicked += ButtonUp_Clicked;

            hardware.RgbPwmLed.SetColor(Color.Green);

            pages = new IBadgePage[]
            {
                new EnvironmentPage(),
                new WiFiTrackerPage(),
            };

            currentPage = pages[0];

            return Task.CompletedTask;
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
    }
}