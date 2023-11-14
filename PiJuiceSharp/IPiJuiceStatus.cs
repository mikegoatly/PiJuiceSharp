
namespace PiJuiceSharp
{
    public interface IPiJuiceStatus : IDisposable
    {
        float GetBatteryCurrent();
        int GetBatteryTemperature();
        float GetBatteryVoltage();
        int GetChargeLevel();
        Dictionary<string, object> GetFaultStatus();
        float GetGpioCurrent();
        float GetGpioVoltage();
        LedBlinkState GetLedBlinkState(Led led);
        Color GetLedState(Led led);
        StatusInfo GetStatus();
        void SetLedBlinkState(Led led, LedBlinkState state);
        void SetLedState(Led led, Color color);
    }
}