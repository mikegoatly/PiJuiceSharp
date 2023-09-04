namespace PiJuiceSharp
{
    public enum Led
    {
        D1 = 0,
        D2 = 1
    }

    public record struct Color(byte R, byte G, byte B)
    {
        public Color(Span<byte> data) : this(data[0], data[1], data[2])
        {
        }
    };

    public record struct LedBlinkState(byte Count, Color Rgb1, short Period1, Color Rgb2, short Period2);
}