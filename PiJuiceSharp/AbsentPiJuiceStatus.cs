
namespace PiJuiceSharp
{
    /// <summary>
    /// A placeholder implementation of <see cref="IPiJuiceStatus"/> that returns default values for when no PiJuice
    /// interface is present at all.
    /// </summary>
    public sealed class AbsentPiJuiceStatus : IPiJuiceStatus
    {
        public void Dispose()
        {
        }

        public float GetBatteryCurrent()
        {
            return 0F;
        }

        public int GetBatteryTemperature()
        {
            return 0;
        }

        public float GetBatteryVoltage()
        {
            return 0F;
        }

        public int GetChargeLevel()
        {
            return 0;
        }

        public Dictionary<string, object> GetFaultStatus()
        {
            return new();
        }

        public float GetGpioCurrent()
        {
            return 0F;
        }

        public float GetGpioVoltage()
        {
            return 0F;
        }

        public LedBlinkState GetLedBlinkState(Led led)
        {
            return new();
        }

        public Color GetLedState(Led led)
        {
            return new();
        }

        public StatusInfo GetStatus()
        {
            return new StatusInfo(false, false, BatteryStatus.NoPiJuice, PowerInputStatus.NotPresent, PowerInputStatus.NotPresent);
        }

        public void SetLedBlinkState(Led led, LedBlinkState state)
        {
        }

        public void SetLedState(Led led, Color color)
        {
        }
    }
}