using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using System;
using System.Linq;
using System.Reflection;
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

        public override Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            hardware = new ProjLabHardwareConfig(Device);

            hardware.ButtonLeft.Clicked += ButtonLeft_Clicked;
            hardware.ButtonRight.Clicked += ButtonRight_Clicked;
            //hardware.ButtonDown.Clicked += ButtonDown_Clicked;
            hardware.ButtonUp.Clicked += ButtonUp_Clicked;

            hardware.RgbPwmLed.SetColor(Color.Green);

            menuPage = new MenuPage();
            menuPage.RequestPageChange += MenuPage_RequestPageChange;

            pages = new IBadgePage[]
            {
                menuPage,
                new EnvironmentPage(),
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

        public override Task Run()
        {
            currentPage.StartUpdating(hardware);
            return base.Run();
        }

        private void ButtonUp_Clicked(object sender, EventArgs e)
        {
            currentPage.Up();
        }

        //private void ButtonDown_Clicked(object sender, EventArgs e)
        //{
        //    currentPage.Down();
        //}

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