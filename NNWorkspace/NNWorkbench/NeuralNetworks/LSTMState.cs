using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class LSTMState
    {
        public LSTMState(double[] inputs, double[] outputs, LSTMState lastState)
        {
            Inputs = inputs;
            Outputs = outputs;
            WeightedInputs = new double[outputs.Length];
            ActivatedInputs = new double[outputs.Length];
            ActivatedOutputs = new double[outputs.Length];
            CellStates = new double[outputs.Length];
            if (lastState != null)
            {
                LastCellStates = lastState.CellStates;
                LastOutputs = lastState.Outputs;
                InputGate = new GateState(Inputs, LastOutputs, LastCellStates);
                ForgetGate = new GateState(Inputs, LastOutputs, LastCellStates);
                OutputGate = new GateState(Inputs, LastOutputs, CellStates);
            }
            else
            {
                LastCellStates = null;
                LastOutputs = null;
                InputGate = null;
                ForgetGate = null;
                OutputGate = null;
            }
        }

        public readonly GateState InputGate;
        public readonly GateState ForgetGate;
        public readonly GateState OutputGate;

        public readonly double[] Inputs;
        public readonly double[] WeightedInputs;

        /// <summary>
        /// The activated, but not gated inputs.
        /// </summary>
        public readonly double[] ActivatedInputs;

        public readonly double[] CellStates;
        public readonly double[] LastCellStates;

        /// <summary>
        /// The activated, but not gated outputs.
        /// </summary>
        public readonly double[] ActivatedOutputs;
        public readonly double[] Outputs;
        public readonly double[] LastOutputs;
    }
}
