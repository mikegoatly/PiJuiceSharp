using System.Device.I2c;

namespace PiJuiceSharp
{
    public class PiJuiceInterface : IDisposable
    {
        private readonly I2cDevice i2cDevice;
        private readonly int address;

        public PiJuiceInterface(int busId = 1, int deviceAddress = 0x14)
        {
            var settings = new I2cConnectionSettings(busId, deviceAddress);
            this.i2cDevice = I2cDevice.Create(settings);
            this.address = deviceAddress;
        }

        public void Dispose()
        {
            this.i2cDevice?.Dispose();
        }

        public byte Read(byte command)
        {
            Span<byte> readBuffer = stackalloc byte[1];
            Read(command, readBuffer);
            return readBuffer[0];
        }

        public (byte, byte) ReadPair(byte command)
        {
            Span<byte> readBuffer = stackalloc byte[2];
            Read(command, readBuffer);
            return (readBuffer[0], readBuffer[1]);
        }

        public void Read(byte command, Span<byte> readBuffer)
        {
            Span<byte> writeBuffer = stackalloc byte[1];

            writeBuffer[0] = command;

            this.i2cDevice.WriteRead(writeBuffer, readBuffer);
        }

        public void Write(byte command, Span<byte> data)
        {
            Span<byte> writeBuffer = new byte[data.Length + 2];
            writeBuffer[0] = command;
            data.CopyTo(writeBuffer[1..]);
            writeBuffer[^1] = GetChecksum(data);

            this.i2cDevice.Write(writeBuffer);
        }

        public byte GetChecksum(Span<byte> data)
        {
            byte fcs = 0xFF;
            foreach (byte x in data)
            {
                fcs = (byte)(fcs ^ x);
            }

            return fcs;
        }
    }
}