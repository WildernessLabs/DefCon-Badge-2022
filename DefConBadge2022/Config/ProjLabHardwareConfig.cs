using Meadow.Devices;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Light;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Peripherals.Sensors.Light;
using Meadow.Peripherals.Speakers;
using Meadow.Units;
using System;

namespace DefConBadge2022
{
    public class ProjLabHardwareConfig : IHardwareConfig
    {
        public RgbPwmLed RgbPwmLed { get; protected set; }

        public MicroGraphics Graphics { get; protected set; } 

        public ITemperatureSensor TemperatureSensor { get; protected set; }

        public IBarometricPressureSensor PressureSensor { get; protected set; }

        public IHumiditySensor HumiditySensor { get; protected set; }

        public ILightSensor LightSensor { get; protected set; }

        public IGraphicsDisplay Display { get; protected set; }

        public IButton ButtonLeft { get; protected set; }
        public IButton ButtonRight { get; protected set; }
        public IButton ButtonUp { get; protected set; }
        public IButton ButtonDown { get; protected set; }

        public IToneGenerator Speaker { get; protected set; }

        public ProjLabHardwareConfig(F7FeatherBase device)
        {
            RgbPwmLed = new RgbPwmLed(device: device,
                redPwmPin: device.Pins.OnboardLedRed,
                greenPwmPin: device.Pins.OnboardLedGreen,
                bluePwmPin: device.Pins.OnboardLedBlue);

            Speaker = new PiezoSpeaker(device, device.Pins.D11);

            var config = new SpiClockConfiguration(
                new Frequency(48000, Frequency.UnitType.Kilohertz),
                SpiClockConfiguration.Mode.Mode3);

            var spiBus = device.CreateSpiBus(
                device.Pins.SCK,
                device.Pins.MOSI,
                device.Pins.MISO,
                config);

            Display = new St7789(
                device: device,
                spiBus: spiBus,
                chipSelectPin: device.Pins.A03,
                dcPin: device.Pins.A04,
                resetPin: device.Pins.A05,
                width: 240, height: 240,
                displayColorMode: ColorType.Format16bppRgb565);

            Graphics = new MicroGraphics(Display)
            {
                Rotation = RotationType._90Degrees,
                CurrentFont = new Font12x16()
            };

            var i2cBus = device.CreateI2cBus();

            try
            {
                var bh1750 = new Bh1750(
                    i2cBus: i2cBus,
                    measuringMode: Bh1750.MeasuringModes.ContinuouslyHighResolutionMode, // the various modes take differing amounts of time.
                    lightTransmittance: 0.5, // lower this to increase sensitivity, for instance, if it's behind a semi opaque window
                    address: (byte)Bh1750.Addresses.Address_0x23
                );
                bh1750.StartUpdating();
                LightSensor = bh1750;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bh1750: {e.Message}");
            }

            try
            {
                var bme688 = new Bme688(i2cBus, (byte)Bme680.Addresses.Address_0x76);
                bme688.StartUpdating();

                TemperatureSensor = bme688;
                HumiditySensor = bme688;
                PressureSensor = bme688;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bme688: {e.Message}");
            }

            ButtonUp = new PushButton(device, device.Pins.D15, ResistorMode.InternalPullDown);
            ButtonRight = new PushButton(device, device.Pins.D05, ResistorMode.InternalPullDown);
            ButtonDown = new PushButton(device, device.Pins.D02, ResistorMode.InternalPullDown);
            ButtonLeft = new PushButton(device, device.Pins.D10, ResistorMode.InternalPullDown);
        }
    }
}