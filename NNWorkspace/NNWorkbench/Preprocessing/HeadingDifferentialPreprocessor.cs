using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Preprocessing
{
    class AngleDifferentialPreprocessor
    {
        public AngleDifferentialPreprocessor(int historySize)
        {
            this.history = new double[historySize];
        }

        private double[] history;
        private int historyPos = 0;
        private bool initialised = false;

        public double GetValue(double input)
        {
            if (!initialised)
            {
                for (int i = 0; i < history.Length; i++)
                {
                    history[i] = input;
                }
                initialised = true;
                return 0.0;
            }

            int nextPos = (historyPos + 1) % history.Length;
            double difference = input - history[nextPos];
            if (difference > Math.PI)
            {
                difference -= 2.0 * Math.PI;
            }
            else if (difference < -Math.PI)
            {
                difference += 2.0 * Math.PI;
            }
            history[nextPos] = input;
            historyPos = nextPos;
            return difference;
        }
    }
}
