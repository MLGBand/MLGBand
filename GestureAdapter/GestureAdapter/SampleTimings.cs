using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Contains information on when a how was received.
    /// </summary>
    class SampleTimings
    {
        public SampleTimings(long received, long skipped, DateTime time)
        {
            this.SamplesReceived = received;
            this.SamplesSkipped = skipped;
            this.Time = time;
        }

        /// <summary>
        /// The total number of samples recieved up to this point.
        /// </summary>
        public long SamplesReceived { get; private set; }

        /// <summary>
        /// The number of samples that have been dropped due to
        /// lack of WiFi throughput.
        /// </summary>
        public long SamplesSkipped { get; private set; }

        /// <summary>
        /// The estimated time at which the sample was taken. This
        /// is based on the estimated clock synchronisation of the device
        /// and the computer.
        /// </summary>
        public DateTime Time { get; private set; }
    }
}
