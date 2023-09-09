# PiJuiceSharp

![PiJuiceSharp logo](/docs/favicon-256.png)

An implementation of the PiJuice client library based on the original [PiJuice Python library](https://github.com/PiSupply/PiJuice/blob/master/Software/Source/pijuice.py) in pure .NET. Currently packaged as a .NET 6 library with only a dependency on `System.Device.Gpio`.

This library currently only supports a subset of the Status APIs, and other APIs such as the RTC and Power Management are not yet implemented. Implementing others should be fairly straightforward, but I just don't have a need for them at the moment - contributions are welcome if you need to extend it!

## Installation

You can install the library from [NuGet](https://www.nuget.org/packages/PiJuiceSharp/):

``` bash
dotnet add package PiJuiceSharp
```

## PiJuice Status APIs

You can use the `PiJuiceStatus` class to get various point-in-time status values of the PiJuice board.

To construct a `PiJuiceStatus`, you need to pass in an instance of `PiJuiceInterface`.

``` csharp
var piJuiceInterface = new PiJuiceInterface();

var piJuiceStatus = new PiJuiceStatus(piJuiceInterface);
```

`GetStatus()` returns an object containing basic status information, defined as:

``` csharp
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

    public record struct StatusInfo(
      bool IsFault,
      bool IsButton,
      BatteryStatus BatteryStatus,
      PowerInputStatus PowerInput,
      PowerInputStatus PowerInput5vIo);
```

`GetChargeLevel()` returns the current charge level of the battery, as a percentage.

`GetFaultStatus()` returns a dictionary containing the current fault status information of the PiJuice board:

```
forced_power_off: True
```

`GetBatteryVoltage()` returns the current voltage of the battery, in volts.

`GetBatteryCurrent()` returns the current current of the battery, in amps.

`GetBatteryTemperature()` returns the current temperature of the battery, in degrees Celsius.

`GetIoVoltage()` returns the current voltage of the GPIO pins, in volts.

`GetIoCurrent()` returns the current current of the GPIO pins, in amps.

### Working with LEDs

The status API also allows you to read and set the LED colors, including a temporary blinking state.

`GetLedState(Led led)` returns a `Color` struct containing the RGB state of either the `Led.D1` or `Led.D2` LED

`SetLedState(Led led, Color color)` sets the RGB state of either the `Led.D1` or `Led.D2` LED to the given `Color`. (Note that if the LED is not in a user configurable state, this will have no effect.)

`GetLedBlinkState(Led led)` returns an `LedBlinkState` struct containing the current blinking state of either the `Led.D1` or `Led.D2` LED. `LedBlinkState` is defined as:

``` csharp
public record struct LedBlinkState(byte Count, Color Rgb1, short Period1, Color Rgb2, short Period2);
```

`SetLedBlinkState(Led led, LedBlinkState state)` sets the blinking state of either the `Led.D1` or `Led.D2` LED to the given `LedBlinkState`. (Note that if the LED is not in a user configurable state, this will have no effect.)

## Thread safety

By using a single `SemaphoreSlim` instance across all instances of `PiJuiceInterface`, the library guarantees that thread safety is enforced 
when communicating with the PiJuice.