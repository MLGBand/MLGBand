using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    /// <summary>
    /// A the state of a recurrent neural network layer that also supports training via backpropogation.
    /// </summary>
    public class RecurrentTrainingState : TrainingState
    {
        public RecurrentTrainingState(double[] inputErrors, double[] errors)
        {
            this.InputErrors = inputErrors;
            this.Errors = errors;
            this.InternalErrors = new double[errors.Length];
            this.BiasWeightErrors = new double[errors.Length];
            this.InputWeightErrors = new double[inputErrors.Length, errors.Length];
            this.InternalWeightErrors = new double[errors.Length, errors.Length];
        }

        //public RecurrentState Last { get; set; }
        public RecurrentState Now { get; set; }

        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to 
        /// the input of the output unit.
        /// </summary>
        public readonly double[] Errors;

        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to
        /// the input of the input unit.
        /// </summary>
        public readonly double[] InputErrors;

        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to
        /// the input of the output unit at the previous step in time.
        /// </summary>
        public readonly double[] InternalErrors;


        public readonly double[] BiasWeightErrors;

        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to 
        /// each of the weights between the input and this layer.
        /// </summary>
        public readonly double[,] InputWeightErrors;

        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to 
        /// each of the weights between the this layer at the previous timestep and this layer at
        /// the current timestep.
        /// </summary>
        public readonly double[,] InternalWeightErrors;


        public override void MultiplyError(double factor)
        {
            TrainingState.Multiply(BiasWeightErrors, factor);
            TrainingState.Multiply(InputWeightErrors, factor);
            TrainingState.Multiply(InternalWeightErrors, factor);
        }

        /// <summary>
        /// The sum of the squares of the error gradients.
        /// Used to bound the gradients (via a reduced learning coefficient) if they get too large.
        /// </summary>
        public override double GetSumSquaredGradients()
        {
            return Layer.SumSquared(BiasWeightErrors) + Layer.SumSquared(InputWeightErrors)
                + Layer.SumSquared(InternalWeightErrors);
        }
    }
}
