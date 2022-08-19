using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DefConBadge2022
{
    public class GraphPage : IBadgePage
    {
        IHardwareConfig config;

        MicroGraphics graphics;

        int index = 0;
        int[] data;


        public bool IsUpdating = false;

        public GraphPage()
        {
        }

        public void StartUpdating(IHardwareConfig config)
        {
            if(data == null || data.Length == 0)
            {
                Init(config);
            }

            this.config = config;

            graphics = config.Graphics;

            IsUpdating = true;

            Task.Run(() =>
            {
                while (IsUpdating)
                {
                    Draw();
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                }
            });
        }

        public void StopUpdating()
        {
            IsUpdating = false;
        }

        public void Init(IHardwareConfig hardware)
        {
            Console.WriteLine("Init");

            data = new int[hardware.Graphics.Width];

            for (int i = 0; i < hardware.Graphics.Width; i++)
            {
                data[i] = hardware.Graphics.Height /2 +  (int)(hardware.Graphics.Height / 3 * Math.Sin(i / (4 * Math.PI)));
            }
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

        void Draw()
        {
            graphics.Clear();

            int dataIndex1, dataIndex2;

            for (int i = 0; i < data.Length - 1; i++)
            {
                dataIndex1 = (index + i) % data.Length;
                dataIndex2 = (index + i + 1) % data.Length;
                //  graphics.DrawPixel(i, data[dataIndex], Color.Cyan);

                graphics.DrawLine(i, data[dataIndex1], i+1, data[dataIndex2], Color.Cyan);
            }

            graphics.Show();

            index++;
        }
    }
}