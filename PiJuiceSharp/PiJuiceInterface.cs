using System.Device.I2c;

namespace PiJuiceSharp
{
    public sealed class PiJuiceInterface : IDisposable
    {
        private static readonly SemaphoreSlim semaphore = new(1, 1);
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
            this.Read(command, readBuffer);
            return readBuffer[0];
        }

        public (byte, byte) ReadPair(byte command)
        {
            Span<byte> readBuffer = stackalloc byte[2];
            this.Read(command, readBuffer);
            return (readBuffer[0], readBuffer[1]);
        }

        public void Read(byte command, Span<byte> readBuffer)
        {
            try
            {

                Span<byte> writeBuffer = stackalloc byte[1];

                writeBuffer[0] = command;

                if (!semaphore.Wait(1000))
                {
                    throw new PiJuiceException("COMMUNICATION_TIMEOUT");
                }

                try
                {
                    this.i2cDevice.WriteRead(writeBuffer, readBuffer);
                }
                finally
                {
                    _ = semaphore.Release();
                }
            }
            catch (IOException ex)
            {
                throw new PiJuiceException("COMMUNICATION_ERROR", ex);
            }
        }

        public void Write(byte command, Span<byte> data)
        {
            try
            {
                Span<byte> writeBuffer = new byte[data.Length + 2];
                writeBuffer[0] = command;
                data.CopyTo(writeBuffer[1..]);
                writeBuffer[^1] = GetChecksum(data);

                if (!semaphore.Wait(1000))
                {
                    throw new PiJuiceException("COMMUNICATION_TIMEOUT");
                }

                try
                {
                    this.i2cDevice.Write(writeBuffer);
                }
                finally
                {
                    _ = semaphore.Release();
                }
            }
            catch (IOException ex)
            {
                throw new PiJuiceException("COMMUNICATION_ERROR", ex);
            }
        }

        public static byte GetChecksum(Span<byte> data)
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