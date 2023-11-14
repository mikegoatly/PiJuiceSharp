using PiJuiceSharp;

using static System.Console;

IPiJuiceStatus piJuiceStatus = PiJuiceStatus.Create();

#region Console writing function
static void WriteHeader(string header)
{
    WriteLine();
    ForegroundColor = ConsoleColor.DarkYellow;
    WriteLine(header);
    ResetColor();
}

static void WriteDict(string header, Dictionary<string, object> data)
{
    WriteHeader(header);

    foreach (KeyValuePair<string, object> entry in data)
    {
        WriteLine("{0}: {1}", entry.Key, entry.Value);
    }
}

static void WriteValue(string header, object value, string? suffix)
{
    WriteHeader(header);

    Write(value);

    if (suffix != null)
    {
        Write(suffix);
    }

    WriteLine();
}

static void WriteColorValueWithHeader(string header, Color color)
{
    WriteHeader(header);

    WriteColorValue(color);
}

static void WriteColorValue(Color color)
{
    Write($"R: {color.R} G: {color.G} B: {color.B} ");

    // Set the console foreground color to the color of the LED using terminal commands
    Write($"\u001b[38;2;{color.R};{color.G};{color.B}m");
    Write("▬▬▬▬▬▬");

    // Reset the console foreground color
    Write("\u001b[0m");

    WriteLine();
}
#endregion

StatusInfo status = piJuiceStatus.GetStatus();
WriteHeader("Status");
WriteLine($"Is faulted: {status.IsFault}");
WriteLine($"Is button: {status.IsButton}");
WriteLine($"Battery status: {status.BatteryStatus}");
WriteLine($"Power input: {status.PowerInput}");
WriteLine($"Power input 5V IO: {status.PowerInput5vIo}");

WriteValue($"PiJuice Charge Level", piJuiceStatus.GetChargeLevel(), "%");

WriteDict("Fault event info", piJuiceStatus.GetFaultStatus());

WriteValue($"Battery temperature", piJuiceStatus.GetBatteryTemperature(), "°C");
WriteValue($"Battery voltage", piJuiceStatus.GetBatteryVoltage(), "V");
WriteValue($"Battery current", piJuiceStatus.GetBatteryCurrent(), "A");
WriteValue($"GPIO voltage", piJuiceStatus.GetGpioVoltage(), "V");
WriteValue($"GPIO current", piJuiceStatus.GetGpioCurrent(), "A");

WriteColorValueWithHeader($"LED D1 State", piJuiceStatus.GetLedState(Led.D1));
WriteColorValueWithHeader($"LED D2 State", piJuiceStatus.GetLedState(Led.D2));

WriteLine();
WriteLine("Randomly setting D2 LED colors until key pressed....");

do
{
    piJuiceStatus.SetLedState(Led.D2, new Color((byte)Random.Shared.Next(0, 256), (byte)Random.Shared.Next(0, 256), (byte)Random.Shared.Next(0, 256)));
    Thread.Sleep(500);
}
while (!KeyAvailable);

piJuiceStatus.SetLedState(Led.D2, new Color(0, 0, 0));

WriteLine();
WriteLine("Setting D2 LED to blink 5 times....");

piJuiceStatus.SetLedBlinkState(
    Led.D2,
    new LedBlinkState(5, new Color(255, 0, 0), 500, new Color(0, 0, 255), 1000));

LedBlinkState blinkStatus = piJuiceStatus.GetLedBlinkState(Led.D2);

WriteHeader("Blink status");
WriteLine($"Count: {blinkStatus.Count}");
Write($"Color 1: {blinkStatus.Period1}ms - ");
WriteColorValue(blinkStatus.Rgb1);
Write($"Color 2: {blinkStatus.Period2}ms - ");
WriteColorValue(blinkStatus.Rgb2);
