using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class LSTMTrainingState : TrainingState
    {
        public LSTMTrainingState(double[] inputErrors, double[] errors)
        {
            this.InputErrors = inputErrors;
            this.Errors = errors;
            this.StateErrors = new double[errors.Length];
            this.TemporalStateErrors = new double[errors.Length];
            this.TemporalErrors = new double[errors.Length];
            this.BiasWeightErrors = new double[errors.Length];
            this.InputWeightErrors = new double[inputErrors.Length, errors.Length];
            this.TemporalWeightErrors = new double[errors.Length, errors.Length];

            this.InputGate = new GateTrainingState(InputErrors, TemporalErrors);
            this.ForgetGate = new GateTrainingState(InputErrors, TemporalErrors);
            this.OutputGate = new GateTrainingState(InputErrors, TemporalErrors);
        }

        private LSTMState state;
        public LSTMState Now
        {
            get { return state; }
            set
            {
                state = value;
                InputGate.Now = state.InputGate;
                ForgetGate.Now = state.ForgetGate;
                OutputGate.Now = state.OutputGate;
            }
        }

        public readonly GateTrainingState InputGate;
        public readonly GateTrainingState ForgetGate;
        public readonly GateTrainingState OutputGate;
        
        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to 
        /// the output of the output unit.
        /// </summary>
        public readonly double[] Errors;

        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to
        /// the input of the input unit.
        /// </summary>
        public readonly double[] InputErrors;

        /// <summary>
        /// The partial derivative of the so-called "Objective" or "Error" function with respect to
        /// the input of the output unit at the next step in time.
        /// </summary>
        public readonly double[] TemporalErrors;

        public readonly double[] StateErrors;

        /// <summary>
        /// The proportion of the partial derivative of the so-called "Objective" or "Error" function 
        /// with respect to the cell state at the next timestep that was attributable to the
        /// current timestep.
        /// </summary>
        public readonly double[] TemporalStateErrors;

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
        public readonly double[,] TemporalWeightErrors;


        public override void MultiplyError(double factor)
        {
            TrainingState.Multiply(BiasWeightErrors, factor);
            TrainingState.Multiply(InputWeightErrors, factor);
            TrainingState.Multiply(TemporalWeightErrors, factor);
            InputGate.MultiplyError(factor);
            ForgetGate.MultiplyError(factor);
            OutputGate.MultiplyError(factor);
        }


        /// <summary>
        /// The sum of the squares of the error gradients.
        /// Used to bound the gradients (via a reduced learning coefficient) if they get too large.
        /// </summary>
        public override double GetSumSquaredGradients()
        {
            return Layer.SumSquared(BiasWeightErrors) + Layer.SumSquared(InputWeightErrors)
                + Layer.SumSquared(TemporalWeightErrors) + InputGate.GetSumSquaredGradients()
                + ForgetGate.GetSumSquaredGradients() + OutputGate.GetSumSquaredGradients();
        }

    }
}
