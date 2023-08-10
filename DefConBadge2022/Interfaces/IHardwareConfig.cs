using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Peripherals.Sensors.Light;
using Meadow.Peripherals.Speakers;

namespace DefConBadge2022
{
    public interface IHardwareConfig
    {
        public RgbPwmLed RgbPwmLed { get; }
        public MicroGraphics Graphics { get; }

        public MicroGraphics GraphicsSecondary { get; }
        public MicroGraphics GraphicsTertiary { get; }

        public IGraphicsDisplay Display { get; }
        public IGraphicsDisplay DisplaySecondary { get; }

        public IToneGenerator Speaker { get; }

        public ITemperatureSensor TemperatureSensor { get; }
        public IBarometricPressureSensor PressureSensor { get; }
        public IHumiditySensor HumiditySensor { get; }

        public ILightSensor LightSensor { get; }

        public INetworkAdapter WifiAdapter { get; }

        public IButton ButtonLeft { get; }
        public IButton ButtonRight { get; }
        public IButton ButtonUp { get; }
        public IButton ButtonDown { get; }
    }
}