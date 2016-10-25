using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Represents an exponentially-decaying, mean-corrected integral on a given property.
    /// Used to integrate acceleration to get speed.
    /// </summary>
    class AccelerationIntegralColumn : IDataLoggerColumn
    {
        private double average;
        private double exponent;
        private Func<Sample, double> property;

        private int historyPos = 0;
        private double[] history = new double[500];
        private double integral = 0;

        public string Name { get; private set; }

        public AccelerationIntegralColumn(string name, double exponent, Func<Sample, double> property)
        {
            this.average = 0;
            this.Name = name;
            this.exponent = exponent;
            this.property = property;
        }

        public string GetValue(Sample sample)
        {
            double value = property(sample);
            double meanCorrectedValue = value - history.Average();

            integral = integral * exponent + (meanCorrectedValue / 100.0);

            history[historyPos] = value;
            historyPos = (historyPos + 1) % history.Length;
            return integral.ToString();
        }
    }
}
