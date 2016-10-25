using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class GateTrainingState : TrainingState
    {
        public GateTrainingState(double[] inputErrors, double[] temporalErrors)
        {
            this.Errors = new double[temporalErrors.Length];
            this.InputErrors = inputErrors;
            this.TemporalErrors = temporalErrors;
            this.CellErrors = new double[temporalErrors.Length];
            this.InputWeightErrors = new double[inputErrors.Length, temporalErrors.Length];
            this.TemporalWeightErrors = new double[temporalErrors.Length, temporalErrors.Length];
            this.CellWeightErrors = new double[temporalErrors.Length];
            this.BiasWeightErrors = new double[temporalErrors.Length];
        }

        
        public GateState Now { get; set; }

        /// <summary>
        /// The partial derivative of the Objective function with respect to the gate's control output.
        /// </summary>
        public readonly double[] Errors;
        public readonly double[] InputErrors;
        public readonly double[] TemporalErrors;
        public readonly double[] CellErrors;

        public readonly double[,] InputWeightErrors;
        public readonly double[,] TemporalWeightErrors;
        public readonly double[] CellWeightErrors;
        public readonly double[] BiasWeightErrors;

        public override void MultiplyError(double factor)
        {
            TrainingState.Multiply(InputWeightErrors, factor);
            TrainingState.Multiply(TemporalWeightErrors, factor);
            TrainingState.Multiply(CellWeightErrors, factor);
            TrainingState.Multiply(BiasWeightErrors, factor);
        }
        
        /// <summary>
        /// The sum of the squares of the error gradients.
        /// Used to bound the gradients (via a reduced learning coefficient) if they get too large.
        /// </summary>
        public override double GetSumSquaredGradients()
        {
            return Layer.SumSquared(BiasWeightErrors) + Layer.SumSquared(InputWeightErrors)
                + Layer.SumSquared(TemporalWeightErrors) + Layer.SumSquared(BiasWeightErrors);
        }
    }
}
