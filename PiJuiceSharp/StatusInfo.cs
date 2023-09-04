namespace PiJuiceSharp
{
    public enum BatteryStatus
    {
        Normal = 0,
        ChargingFromIn = 1,
        ChargingFrom5vIo = 2,
        NotPresent = 3
    }

    public enum PowerInputStatus
    {
        NotPresent = 0,
        Bad = 1,
        Weak = 2,
        Present = 3
    }

    public record struct StatusInfo(bool IsFault, bool IsButton, BatteryStatus BatteryStatus, PowerInputStatus PowerInput, PowerInputStatus PowerInput5vIo);
}
