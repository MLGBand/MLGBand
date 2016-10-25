using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// A data logger column that is an differential of the Euler heading, over the specified
    /// time window. Used to avoid Neural Network overfitting; ensuring that behaviour is never
    /// dependent on direction the user is facing relative to Magnetic North.
    /// </summary>
    class HeadingDifferentialColumn : IDataLoggerColumn
    {
        private Func<Sample, double> property;
        private int historyPos = 0;
        private int ticksSinceStart = 0;
        private double[] history;

        public string Name { get; private set; }

        public HeadingDifferentialColumn(string name, int durationInSamples)
        {
            this.history = new double[durationInSamples];
            this.Name = name;
        }

        public string GetValue(Sample sample)
        {
            ticksSinceStart++;

            double value = sample.EulerH;
            int nextPos = (historyPos + 1) % history.Length;
            double difference = value - history[nextPos];
            if (difference > 180)
            {
                difference -= 360;
            }
            else if (difference < -180)
            {
                difference += 360;
            }

            history[nextPos] = value;
            historyPos = nextPos;

            // Insufficient data.
            if (ticksSinceStart < history.Length) return string.Empty;
            return difference.ToString();
        }
    }
}
