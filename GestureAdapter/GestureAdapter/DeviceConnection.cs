using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Represents a TCP connection to the gesture band device.
    /// </summary>
    class DeviceConnection
    {
        private const int DevicePort = 80;
        private const int SampleSizeInBytes = 36 + 36;

        private TcpClient client;
        private NetworkStream stream;

        // Our recieve buffer (sufficient for one sample) and our current
        // position in filling it.
        private int position = 0;
        private byte[] recieveBuffer = new byte[SampleSizeInBytes];

        // The device timestamp of the first sample that was recieved.
        long firstSampleTime = int.MinValue;

        // Our current best estimate of the device's start time. This is based
        // on the fact that we can never recieve a sample before it has been
        // taken. 
        DateTime deviceStartTime = DateTime.MaxValue;

        // The number of samples that have been recieved to date.
        long samplesReceived = 0;

        public DeviceConnection()
        {
            client = new TcpClient();
        }

        /// <summary>
        /// Connects to the device at the specified IP Address.
        /// </summary>
        /// <param name="ipAddress">The IP Address to connect to.</param>
        public void Connect(string ipAddress)
        {
            client.Connect(ipAddress, DevicePort);
            stream = client.GetStream();
            position = 0;
        }

        /// <summary>
        /// Reads samples from the device, firing <see cref="SampleReceived"/> for each sample.
        /// </summary>
        public void Read()
        {
            int bytesRead = 0;
            do
            {
                // Fill recieve buffer.
                bytesRead = stream.Read(recieveBuffer, position, SampleSizeInBytes - position);
                position += bytesRead;

                // If filled, deserialise and process the sample.
                if (position == recieveBuffer.Length)
                {
                    position = 0;
                    ProcessSample(recieveBuffer);
                }
            } while (bytesRead > 0);
        }
        
        /// <summary>
        /// Whether there any samples to be read.
        /// </summary>
        public bool DataAvailable
        {
            get { return (stream != null) && stream.DataAvailable; }
        }

        /// <summary>
        /// Whether the device is currently connected.
        /// </summary>
        public bool Connected
        {
            get { return client.Connected; }
        }

        private void ProcessSample(byte[] sampleBuffer)
        {
            // Deserialise the sample.
            var sample = Sample.Deserialise(recieveBuffer);

            if (firstSampleTime == int.MinValue)
            {
                firstSampleTime = sample.Timestamp;
            }

            // Predict when the device started, assuming this sample was not buffered and there was
            // no network delay. This represents an upper bound on when the device could have started.
            // With buffering, it may have started sooner.
            DateTime deviceStartUpperBound = DateTime.Now.AddMilliseconds(-sample.Timestamp);

            // Check our current best estimate with this upper bound -- revise our estimate if need be.
            if (deviceStartUpperBound < deviceStartTime)
            {
                deviceStartTime = deviceStartUpperBound;
            }

            // Expect a sample every 10ms.
            samplesReceived++;
            long samplesExpected = (sample.Timestamp - firstSampleTime) / 10;
            long skipped = (samplesExpected - samplesReceived);

            sample.Timings = new SampleTimings(samplesReceived, skipped,
                    deviceStartTime.AddMilliseconds(sample.Timestamp));

            // Fire the sample recieved event.
            OnSampleReceived(new SampleReceivedEventArgs(sample));
        }

        protected void OnSampleReceived(SampleReceivedEventArgs e)
        {
            var sampleRecievedHandler = SampleReceived;
            if (sampleRecievedHandler != null)
            {
                sampleRecievedHandler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a motion sample has been recieved from the device.
        /// </summary>
        public event EventHandler<SampleReceivedEventArgs> SampleReceived;
    }
}
