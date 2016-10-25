using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Represents the application of a linear function to a series of inputs.
    /// Models the MATLAB mapminmax_apply() function.
    /// </summary>
    class OffsetsAndGains
    {
        public OffsetsAndGains(double[] offsets, double[] gains, double ymin)
        {
            this.offsets = offsets;
            this.gains = gains;
            this.ymin = ymin;
        }

        double[] offsets;
        double[] gains;
        double ymin;

        /// <summary>
        /// Applies the gains and offsets to the given raw data inputs, storing
        /// them in another array.
        /// </summary>
        public void Apply(double[] rawInputs, double[] processedInputs)
        {
            for (int i = 0; i < rawInputs.Length; i++)
            {
                processedInputs[i] = ((rawInputs[i] - offsets[i]) * gains[i]) + ymin;
            }
        }

        /// <summary>
        /// Reservses the gains and offsets applied to a set of process inputs,
        /// providing the raw inputs that were used.
        /// Models mapminmax_reserse() function.
        /// </summary>
        public void Reverse(double[] processedInputs, double[] rawInputs)
        {
            for (int i = 0; i < rawInputs.Length; i++)
            {
                rawInputs[i] = ((processedInputs[i] - ymin) / gains[i]) + offsets[i];
            }
        }
    }
}
