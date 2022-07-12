using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DefConBadge2022
{
    public class EnvironmentPage : IBadgePage
    {
        IHardwareConfig config;

        MicroGraphics graphics;

        public bool IsUpdating = false;

        public void StartUpdating(IHardwareConfig config)
        {
            this.config = config;

            graphics = config.Graphics;

            IsUpdating = true;

            Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine("Draw");

                    Draw();
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            });
        }

        public void StopUpdating()
        {
            IsUpdating = false;
        }

        public void Init(IHardwareConfig hardware)
        {
         
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Down()
        {
            throw new NotImplementedException();
        }

        public void Left()
        {
            throw new NotImplementedException();
        }

        public void Right()
        {
            throw new NotImplementedException();
        }

        public void Up()
        {
            throw new NotImplementedException();
        }

        //helper method
        void DrawStatus(string label, string value, Color color, int yPosition)
        {
            graphics.DrawText(x: 5, y: yPosition, label, color: color);
            graphics.DrawText(x: 240, y: yPosition, value, alignment: TextAlignment.Right, color: color);
        }

        void Draw()
        {
            graphics.Clear();

            graphics.DrawText(x: 5, y: 5, "Hello PROJ LAB!", WildernessLabsColors.AzureBlue);
          
            if (config.TemperatureSensor.Temperature is { } temp)
            {
                DrawStatus("Temperature:", $"{temp.Celsius:N1}C", WildernessLabsColors.GalleryWhite, 40);
            }

            if (config.PressureSensor.Pressure is { } pressure)
            {
                DrawStatus("Pressure:", $"{pressure.StandardAtmosphere:N1}atm", WildernessLabsColors.GalleryWhite, 60);
            }

            if (config.HumiditySensor.Humidity is { } humidity)
            {
                DrawStatus("Humidity:", $"{humidity.Percent:N1}%", WildernessLabsColors.GalleryWhite, 80);
            }
            

            if (config.LightSensor.Illuminance is { } light)
            {
                if (light is { } lightReading)
                {
                    DrawStatus("Lux:", $"{lightReading:N0}Lux", WildernessLabsColors.GalleryWhite, 100);
                }
            }

            graphics.Show();
         }
    }
}