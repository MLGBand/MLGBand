using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class SoftmaxTrainingState : TrainingState
    {
        public SoftmaxTrainingState(double[] inputErrors, double[] errors)
        {
            this.InputErrors = inputErrors;
            this.Errors = errors;
            this.BiasWeightErrors = new double[errors.Length];
            this.InputWeightErrors = new double[inputErrors.Length, errors.Length];
        }

        public SoftmaxState Now { get; set; }

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


        public readonly double[] BiasWeightErrors;

        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to 
        /// each of the weights between the input and this layer.
        /// </summary>
        public readonly double[,] InputWeightErrors;

        public override void MultiplyError(double factor)
        {
            TrainingState.Multiply(BiasWeightErrors, factor);
            TrainingState.Multiply(InputWeightErrors, factor);
        }

        public override double GetSumSquaredGradients()
        {
            return Layer.SumSquared(BiasWeightErrors) + Layer.SumSquared(InputWeightErrors);
        }
    }
}
