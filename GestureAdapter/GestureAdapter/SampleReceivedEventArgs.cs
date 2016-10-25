using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Provides information about a sample that was recieved.
    /// </summary>
    class SampleReceivedEventArgs : EventArgs
    {
        public SampleReceivedEventArgs(Sample sample)
        {
            this.Sample = sample;
        }

        /// <summary>
        /// The sample that was received.
        /// </summary>
        public Sample Sample { get; private set; }
    }
}
