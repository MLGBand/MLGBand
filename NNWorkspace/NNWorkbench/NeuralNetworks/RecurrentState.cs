using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class RecurrentState
    {
        public RecurrentState(double[] inputs, double[] outputs)
            : this(inputs, outputs, null)
        {
        }

        public RecurrentState(double[] inputs, double[] outputs, RecurrentState lastState)
        {
            Inputs = inputs;
            Outputs = outputs;
            if (lastState != null)
                LastOutputs = lastState.Outputs;
            else
                LastOutputs = null;// new double[outputs.Length];
            WeightedSums = new double[outputs.Length];
        }

        public readonly double[] Inputs;
        public readonly double[] Outputs;
        public readonly double[] WeightedSums;
        public readonly double[] LastOutputs;
    }
}
