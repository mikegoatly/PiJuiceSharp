
namespace PiJuiceSharp
{
    public sealed class PiJuiceStatus : IPiJuiceStatus
    {
        private const byte STATUS_CMD = 0x40;
        private const byte CHARGE_LEVEL_CMD = 0x41;
        private const byte FAULT_EVENT_CMD = 0x44;
        private const byte BATTERY_TEMPERATURE_CMD = 0x47;
        private const byte BATTERY_VOLTAGE_CMD = 0x49;
        private const byte BATTERY_CURRENT_CMD = 0x4b;
        private const byte IO_VOLTAGE_CMD = 0x4d;
        private const byte IO_CURRENT_CMD = 0x4f;
        private const byte LED_STATE_CMD = 0x66;
        private const byte LED_BLINK_CMD = 0x68;
        private readonly PiJuiceInterface piJuiceInterface;

        private PiJuiceStatus(PiJuiceInterface piJuiceInterface)
        {
            this.piJuiceInterface = piJuiceInterface;
        }

        public static IPiJuiceStatus Create()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope - the PiJuiceInterface is disposed by the returned object
            if (PiJuiceInterface.TryConnect(out PiJuiceInterface? piJuiceInterface))
            {
                return new PiJuiceStatus(piJuiceInterface);
            }
#pragma warning restore CA2000 // Dispose objects before losing scope

            return new AbsentPiJuiceStatus();
        }

        public StatusInfo GetStatus()
        {
            byte d = this.piJuiceInterface.Read(STATUS_CMD);

            return new StatusInfo(
                (d & 0x01) != 0,
                (d & 0x02) != 0,
                (BatteryStatus)((d >> 2) & 0x03),
                (PowerInputStatus)((d >> 4) & 0x03),
                (PowerInputStatus)((d >> 6) & 0x03));
        }

        public int GetChargeLevel()
        {
            return this.piJuiceInterface.Read(CHARGE_LEVEL_CMD);
        }
        public int GetBatteryTemperature()
        {
            (byte d0, byte _) = this.piJuiceInterface.ReadPair(BATTERY_TEMPERATURE_CMD);

            int temp = d0;
            if ((d0 & (1 << 7)) == (1 << 7))
            {
                temp -= 1 << 8;
            }

            return temp;
        }

        public float GetBatteryVoltage()
        {
            return this.ReadFloat(BATTERY_VOLTAGE_CMD);
        }

        public float GetBatteryCurrent()
        {
            return this.ReadSignedFloat(BATTERY_CURRENT_CMD);
        }

        public float GetGpioVoltage()
        {
            return this.ReadFloat(IO_VOLTAGE_CMD);
        }

        public float GetGpioCurrent()
        {
            return this.ReadSignedFloat(IO_CURRENT_CMD);
        }

        public Dictionary<string, object> GetFaultStatus()
        {
            var fault = new Dictionary<string, object>();

            try
            {
                byte d = this.piJuiceInterface.Read(FAULT_EVENT_CMD);

                if ((d & 0x01) == 0x01)
                {
                    fault["button_power_off"] = true;
                }

                if ((d & 0x02) == 0x02)
                {
                    fault["forced_power_off"] = true;
                }

                if ((d & 0x04) == 0x04)
                {
                    fault["forced_sys_power_off"] = true;
                }

                if ((d & 0x08) == 0x08)
                {
                    fault["watchdog_reset"] = true;
                }

                if ((d & 0x20) == 0x20)
                {
                    fault["battery_profile_invalid"] = true;
                }

                byte tempEnum = (byte)((d >> 6) & 0x03);
                if (tempEnum != 0)
                {
                    fault["charging_temperature_fault"] = tempEnum switch
                    {
                        1 => "NORMAL",
                        2 => "SUSPEND",
                        3 => "COOL",
                        4 => "WARM",
                        _ => $"UNKNOWN ({tempEnum})",
                    };

                }
            }
            catch (IOException ex)
            {
                throw new PiJuiceException("COMMUNICATION_ERROR", ex);
            }

            return fault;
        }

        public Color GetLedState(Led led)
        {
            Span<byte> ledData = stackalloc byte[3];
            this.piJuiceInterface.Read((byte)(LED_STATE_CMD + (byte)led), ledData);
            return new Color(ledData);
        }

        public void SetLedState(Led led, Color color)
        {
            Span<byte> ledData = stackalloc byte[] { color.R, color.G, color.B };
            this.piJuiceInterface.Write((byte)(LED_STATE_CMD + (byte)led), ledData);
        }

        public LedBlinkState GetLedBlinkState(Led led)
        {
            Span<byte> ledData = stackalloc byte[9];
            this.piJuiceInterface.Read((byte)(LED_BLINK_CMD + (byte)led), ledData);
            return new LedBlinkState(
                ledData[0],
                new Color(ledData[1..4]),
                (short)(ledData[4] * 10),
                new Color(ledData[5..8]),
                (short)(ledData[8] * 10));
        }

        public void SetLedBlinkState(Led led, LedBlinkState state)
        {
            Span<byte> ledData = stackalloc byte[9]
            {
                (byte)(state.Count & 0xFF),
                state.Rgb1.R,
                state.Rgb1.G,
                state.Rgb1.B,
                (byte)((state.Period1 / 10) & 0xFF),
                state.Rgb2.R,
                state.Rgb2.G,
                state.Rgb2.B,
                (byte)((state.Period2 / 10) & 0xFF)
            };

            this.piJuiceInterface.Write((byte)(LED_BLINK_CMD + (byte)led), ledData);
        }

        public void Dispose()
        {
            this.piJuiceInterface.Dispose();
        }

        private float ReadFloat(byte command)
        {
            (byte d0, byte d1) = this.piJuiceInterface.ReadPair(command);

            return ((d1 << 8) | d0) / 1000F;
        }

        private float ReadSignedFloat(byte command)
        {
            (byte d0, byte d1) = this.piJuiceInterface.ReadPair(command);
            int current = (d1 << 8) | d0;
            if ((current & (1 << 15)) == (1 << 15))
            {
                current -= 1 << 16;
            }

            return current / 1000F;
        }
    }
}