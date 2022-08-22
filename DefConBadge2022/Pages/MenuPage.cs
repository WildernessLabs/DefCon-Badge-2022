using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TextDisplayMenu;
using Meadow.Foundation.Graphics;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DefConBadge2022
{
    public delegate void RequestPageChange(string pageName);
    public class MenuPage : IBadgePage
    {
        IHardwareConfig config;
        MicroGraphics graphics;

        public bool IsUpdating = false;
        Menu menu;
        MenuItem[] menuItems;
        Version version = Assembly.GetExecutingAssembly().GetName().Version;

        public event RequestPageChange RequestPageChange;

        public void StartUpdating(IHardwareConfig config)
        {
            this.config = config;
            graphics = this.config.Graphics;

            menuItems = new MenuItem[]
            {
                new MenuItem("Environment", command: "startEnvironmentPage", id: nameof(EnvironmentPage)),
                new MenuItem("WiFi Tracker", command: "startWiFiTrackerPage", id: nameof(WiFiTrackerPage)),
                new MenuItem("Display Test", command: "startDisplayTestPage", id: nameof(GraphPage)),
                new MenuItem("Options", subItems: new MenuItem[]
                {
                    // TODO: TextDisplayMenuLibrary: does menu example need `$` prefix on strings: `$"Sound {value}", or is that a placeholder system?
                    //new MenuItem("Sound {value}", id: "sound", type: "OnOff", value: true),
                    //new MenuItem("Volume {value}", id: "volume", type: "Numerical", value: 5),
                    new MenuItem($"Version {version}"),
                }),
            };
            // Loading a menu file from embedded assembly file
            //byte[] LoadFromJson(string filename)
            //{
            //    var assembly = Assembly.GetExecutingAssembly();
            //    var resourceName = $"MeadowMenu.{filename}";
            //    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //    {
            //        using (var ms = new MemoryStream())
            //        {
            //            stream.CopyTo(ms);
            //            return ms.ToArray();
            //        }
            //    }
            //}
            menu = new Menu(graphics, menuItems, showBackOnRoot: false);
            menu.Selected += Menu_Selected;
            menu.Enable();

            Task.Run(() =>
            {
                while (true)
                {
                    Draw();
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            });
        }

        public void StopUpdating()
        {
            IsUpdating = false;
            menu.Selected -= Menu_Selected;
        }

        public void Init(IHardwareConfig hardware)
        {
        }

        private void Menu_Selected(object sender, MenuSelectedEventArgs e)
        {
            menu.Disable();
            Console.WriteLine($"Page selected: {e?.PageClass}");
            RequestPageChange(e?.PageClass);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Down()
        {
            menu.Select();
        }

        public void Left()
        {
            menu.Previous();
        }

        public void Right()
        {
            menu.Next();
        }

        public void Up()
        {
            menu.Back();
        }

        void DrawStatus(string label, string value, Color color, int yPosition)
        {
            graphics.DrawText(x: 0, y: yPosition, label, color: WildernessLabsColors.AzureBlue);
            graphics.DrawText(x: 10, y: yPosition + 18, value,
                scaleFactor: ScaleFactor.X2,
                alignment: TextAlignment.Left,
                color: color);
        }

        void Draw()
        {
            //graphics.Clear();
            //DrawStatus("x", "3", Color.BurlyWood, 120);
            //graphics.Show();
        }
    }
}

//PushButton next, previous, select;

// Menu system
//var version = Assembly.GetExecutingAssembly().GetName().Version;
//var menuItems = new MenuItem[]
//{
//    new MenuItem("FrogIt", command: "startFrogIt"),
//    new MenuItem("Pong", command: "startPong"),
//    new MenuItem("Span4", command: "startSpan4"),
//    new MenuItem("Snake", command: "startSnake"),
//    new MenuItem("Options", subItems: new MenuItem[]
//    {
//        // TODO: TextDisplayMenuLibrary: does menu example need `$` prefix on strings: `$"Sound {value}", or is that a placeholder system?
//        new MenuItem("Sound {value}", id: "sound", type: "OnOff", value: true),
//        new MenuItem("Volume {value}", id: "volume", type: "Numerical", value: 5),
//        new MenuItem($"Version {version}"),
//    }),
//};
//var menu = new Menu(graphics, menuItems, showBackOnRoot: false);
//next = new PushButton(Device, Device.Pins.D03, ResistorMode.InternalPullUp);
//next.Clicked += (s, e) => {
//    menu.Next();
//};
//select = new PushButton(Device, Device.Pins.D04, ResistorMode.InternalPullUp);
//select.Clicked += (s, e) => { menu.Select(); };
//previous = new PushButton(Device, Device.Pins.D02, ResistorMode.InternalPullUp);
//previous.Clicked += (s, e) => { menu.Previous(); };
//menu.Enable();
// Loading a menu file from embedded assembly file
//byte[] LoadFromJson(string filename)
//{
//    var assembly = Assembly.GetExecutingAssembly();
//    var resourceName = $"MeadowMenu.{filename}";
//    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
//    {
//        using (var ms = new MemoryStream())
//        {
//            stream.CopyTo(ms);
//            return ms.ToArray();
//        }
//    }
//}
