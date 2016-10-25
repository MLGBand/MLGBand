using NNWorkbench.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class LSTMLayer : Layer
    {
        private double[,] cellInputWeights;
        private double[,] cellTemporalWeights;
        private double[] cellBiases;
        private Gate inputGate;
        private Gate outputGate;
        private Gate forgetGate;
        private NetworkConfiguration configuration;

        public LSTMLayer(NetworkConfiguration configuration, int size, int inputSize) : base(configuration, size, inputSize)
        {
            this.configuration = configuration;
            cellInputWeights = new double[inputSize, size];
            cellTemporalWeights = new double[size, size];
            cellBiases = new double[size];
            Randomise(cellBiases, inputSize + size + 1);
            Randomise(cellInputWeights, inputSize + size + 1);
            Randomise(cellTemporalWeights, inputSize + size + 1);

            inputGate = new Gate(configuration, Size, InputSize);
            outputGate = new Gate(configuration, Size, InputSize);
            forgetGate = new Gate(configuration, Size, InputSize);
        }

        public void FeedForward(LSTMState state)
        {
           // DelayInitialise();

            // Equations 4.2, 4.3
            inputGate.FeedForward(state.InputGate);

            // Equations 4.4, 4.5
            forgetGate.FeedForward(state.ForgetGate);
            
            // Equation 4.6
            Debug.Assert(state.WeightedInputs.Length == Size && cellBiases.Length == Size);
            Array.Copy(cellBiases, state.WeightedInputs, Size);
            Layer.WeightedInputSum(state.Inputs, cellInputWeights, output: state.WeightedInputs);
            Layer.WeightedInputSum(state.LastOutputs, cellTemporalWeights, output: state.WeightedInputs);

            // Equation 4.7
            OutputFromInputActivation(state.WeightedInputs, state.ActivatedInputs);
            SumOfTwoDotProducts(state.ActivatedInputs, state.InputGate.Outputs, state.LastCellStates, state.ForgetGate.Outputs, outputs: state.CellStates);

         //   Layer.Bound(state.CellStates, 100.0);

            // Equations 4.8, 4.9
            outputGate.FeedForward(state.OutputGate);

            // Equation 4.10
////            Array.Copy(state.CellStates, state.ActivatedOutputs, Size);
            OutputFromActivation(state.CellStates, output: state.ActivatedOutputs); ////***************

            DotProduct(state.ActivatedOutputs, state.OutputGate.Outputs, output: state.Outputs);

        }

        public void Backpropogate(LSTMTrainingState state)
        {
            // Implements the part of Equation 4.12 related to summing errors coming in from the next timestamp.
            // (excl. zero-ing of error.)
            Layer.Sum(state.TemporalErrors, state.Errors);

            // Existing values not needed anymore, must be cleared since backpropogating the output gate errors
            // will place errors here.
            Array.Clear(state.TemporalErrors, 0, state.TemporalErrors.Length);
            Array.Clear(state.InputErrors, 0, state.InputErrors.Length);

            // Implements the right-hand part of Equation 4.13
            Layer.DotProduct(state.Now.ActivatedOutputs, state.Errors, output: state.OutputGate.Errors);    

            // Implements the left-hand part of Equation 4.13 and all the backpropogation to the Temporal and Input Errors.
            outputGate.Backpropogate(state.OutputGate);

            // Turns the errors with respect to the cell output to be with respect to the cell states.
            MultiplyByActivationDerivative(state.Now.CellStates, state.Errors); ////***************
            
            
            // Implements most of Equation 4.14 (except for statement directly above).
            DotProduct(state.Now.OutputGate.Outputs, state.Errors, output: state.StateErrors);
            Sum(state.TemporalStateErrors, outputs: state.StateErrors);
            Sum(state.InputGate.CellErrors, state.StateErrors);
            Sum(state.ForgetGate.CellErrors, state.StateErrors);
            Sum(state.OutputGate.CellErrors, state.StateErrors);

            DotProduct(state.StateErrors, state.Now.ForgetGate.Outputs, output: state.TemporalStateErrors);
            
            // Implements Equation 4.15. 
            // Use state.Errors to store d(O)/d(Ac) i.e. error with respect to weighted cell inputs.
            DotProduct(state.Now.InputGate.Outputs, state.StateErrors, state.Errors);
            MultiplyByInputActivationDerivative(state.Now.WeightedInputs, state.Errors);
            
            // Backpropogate the errors with respect to weighted cell inputs back to the input layers.
            Layer.WeightedOutputSum(state.Errors, cellInputWeights, state.InputErrors);
            Layer.Bound(state.InputErrors, 1.0); // 1.
            Layer.WeightedOutputSum(state.Errors, cellTemporalWeights, state.TemporalErrors);
            Layer.Bound(state.TemporalErrors, 1.0); // 1.

            // And backpropogate the errors to the individual weights on the input to the cell.
            Sum(state.Errors, state.BiasWeightErrors);
            SumProducts(state.Now.Inputs, state.Errors, state.InputWeightErrors);
            SumProducts(state.Now.LastOutputs, state.Errors, state.TemporalWeightErrors);

            // These cell error derivatives are no longer required. 
            // Clear them so we start from a clean slate on the previous (next in backpropogation) timestamp.
            //Array.Clear(state.Errors, 0, state.Errors.Length);


            // Implements Equation 4.16 and associated backpropogation to input, temporal, bias and cell input layers.
            DotProduct(state.Now.LastCellStates, state.StateErrors, output: state.ForgetGate.Errors);
            forgetGate.Backpropogate(state.ForgetGate);

            // Implements Equation 4.17 and associated backpropogation to input, temporal, bias and cell input layers.
            DotProduct(state.Now.ActivatedInputs, state.StateErrors, output: state.InputGate.Errors);
            inputGate.Backpropogate(state.InputGate);
        }


        private void OutputFromInputActivation(double[] weightedInputs, double[] outputs)
        {
            OutputFromActivation(weightedInputs, outputs);
            //Debug.Assert(weightedInputs.Length == outputs.Length);
            //for (int i = 0; i < outputs.Length; i++)
            //{
            //    outputs[i] = InputActivation(weightedInputs[i]);
            //}
        }

        private void MultiplyByInputActivationDerivative(double[] weightedInputs, double[] errors)
        {
            MultiplyByActivationDerivative(weightedInputs, errors);

            //Debug.Assert(weightedInputs.Length == errors.Length);
            //for (int i = 0; i < weightedInputs.Length; i++)
            //{
            //    errors[i] *= InputActivationDerivative(weightedInputs[i]);
            //}
        }

        private void SumOfTwoDotProducts(double[] inputA, double[] gateA, double[] inputB, double[] gateB, double[] outputs)
        {
            for (int i = 0; i < outputs.Length; i++)
            {
                double output  = inputA[i] * gateA[i] + inputB[i] * gateB[i];
                outputs[i] = output;
            }
        }

        //private double InputActivation(double x)
        //{
        //    return base.Activation(x);
        //}

        //private double InputActivationDerivative(double x)
        //{
        //    return base.ActivationDerivative(x);
        //}

        protected double Sigmoid(double x)
        {
            if (x > 0)
            {
                return 1.0 / (1.0 + Math.Exp(-x));
            }
            else
            {
                return 1.0 - (1.0 / (1.0 + Math.Exp(x)));
            }
        }

        protected override double Activation(double x)
        {
            return 4.0 * Sigmoid(x) - 2.0;
        }

        protected override double ActivationDerivative(double x)
        {
            double sigmoid = Sigmoid(x);
            return 4.0 * (sigmoid) * (1.0 - sigmoid);
            //double ePowerX = Math.Exp(x);
            //double divisor = (ePowerX + 1);
            //return (4.0 * ePowerX) / (divisor * divisor);

        }


        //protected override double Activation(double x)
        //{
        //    return 1.0 / (1.0 + Math.Exp(-x));
        //}

        //protected override double ActivationDerivative(double x)
        //{
        //    double output = Activation(x);
        //    return output * (1.0 - output);
        //}


        public override double SumAbsoluteWeight
        {
            get { //return inputGate.SumAbsoluteWeight + forgetGate.SumAbsoluteWeight + outputGate.SumAbsoluteWeight + 
                return SumAbsolute(cellInputWeights) + SumAbsolute(cellTemporalWeights) + SumAbsolute(cellBiases); }
        }

        public override double WeightCount
        {
            get
            { //return inputGate.WeightCount + forgetGate.WeightCount + outputGate.WeightCount + 
                return InputSize * Size + Size * Size + Size; 
            }
        }

        public void ApplyWeightChanges(LSTMTrainingState state, double learningCoefficient)
        {
            //Bound(state.BiasWeightErrors, 5.0); //10.
            //Bound(state.InputWeightErrors, 5.0); //10.
            //Bound(state.TemporalWeightErrors, 5.0); //10.

            Layer.ApplyWeightChanges(cellBiases, state.BiasWeightErrors, learningCoefficient);
            Layer.ApplyWeightChanges(cellInputWeights, state.InputWeightErrors, learningCoefficient);
            Layer.ApplyWeightChanges(cellTemporalWeights, state.TemporalWeightErrors, learningCoefficient);

            inputGate.ApplyWeightChanges(state.InputGate, learningCoefficient);
            forgetGate.ApplyWeightChanges(state.ForgetGate, learningCoefficient);
            outputGate.ApplyWeightChanges(state.OutputGate, learningCoefficient);

            Array.Clear(state.TemporalErrors, 0, state.TemporalErrors.Length);
            Array.Clear(state.TemporalStateErrors, 0, state.TemporalStateErrors.Length);
        }

        public override void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(cellInputWeights);
            writer.Write(cellTemporalWeights);
            writer.Write(cellBiases);
            inputGate.Save(writer);
            forgetGate.Save(writer);
            outputGate.Save(writer);
        }

        public override void Load(System.IO.BinaryReader reader)
        {
            reader.Read(cellInputWeights);
            reader.Read(cellTemporalWeights);
            reader.Read(cellBiases);
            inputGate.Load(reader);
            forgetGate.Load(reader);
            outputGate.Load(reader);
        }

        public override void Export(StringBuilder builder)
        {
            ExportWeights(builder, cellInputWeights);
            ExportWeights(builder, cellTemporalWeights);
            ExportWeights(builder, cellBiases);
            inputGate.Export(builder);
            forgetGate.Export(builder);
            outputGate.Export(builder);
//            throw new NotImplementedException();
        }
    }
}
