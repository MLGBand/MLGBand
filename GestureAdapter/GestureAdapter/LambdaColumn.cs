using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Represents a custom data logger column whose value is derived from a simple lambda expression on the sample.
    /// </summary>
    class LambdaColumn : IDataLoggerColumn
    {
        public LambdaColumn(string name, Func<Sample, string> function)
        {
            this.Name = name;
            this.function = function;
        }

        private Func<Sample, string> function;

        public string Name { get; private set; }

        public string GetValue(Sample sample)
        {
            return function(sample);
        }
    }
}
