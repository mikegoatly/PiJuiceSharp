namespace PiJuiceSharp
{
    public enum BatteryStatus
    {
        /// <summary>
        /// Battery is present, but not charging
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Battery is charging from the its power input
        /// </summary>
        ChargingFromIn = 1,

        /// <summary>
        /// Battery is charging from the 5V IO pin
        /// </summary>
        ChargingFrom5vIo = 2,

        /// <summary>
        /// The PiJuice board is present, but no battery is connected
        /// </summary>
        NotPresent = 3,

        /// <summary>
        /// No PiJuice board is present - no readings will be available. This is an extension to the
        /// original PiJuice Python library.
        /// </summary>
        NoPiJuice = 8000,
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
