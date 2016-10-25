using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class GateState
    { 
        public GateState(double[] inputs, double[] lastCellOutputs, double[] cellStates)
        {
            Inputs = inputs;
            Outputs = new double[cellStates.Length];
            LastCellOutputs = lastCellOutputs;
            CellStates = cellStates;
            WeightedSums = new double[cellStates.Length];
        }

        /// <summary>
        /// The control output of the gate.
        /// </summary>
        public readonly double[] Outputs;

        /// <summary>
        /// The weighted inputs prior to going through the gate activation function to give the gate control output.
        /// </summary>
        public readonly double[] WeightedSums;

        /// <summary>
        /// The inputs to the Gate from the previous layer.
        /// </summary>
        public readonly double[] Inputs;
        
        /// <summary>
        /// The inputs to the Gate from the last timestep. 
        /// </summary>
        public readonly double[] LastCellOutputs;

        /// <summary>
        /// The state of the internal cell memory cell at the last timestep (for input and forget gates), or the current timestep (for output gates).
        /// </summary>
        public readonly double[] CellStates;
    }
}
