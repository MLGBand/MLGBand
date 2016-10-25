using NNWorkbench.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class Gate : Layer
    {
        public Gate(NetworkConfiguration configuration, int size, int inputSize) : base(configuration, size, inputSize)
        {
            inputWeights = new double[inputSize, size];
            temporalWeights = new double[size, size];
            cellWeights = new double[size];
            biases = new double[size];

            Randomise(inputWeights, inputSize + size + 2);
            Randomise(temporalWeights, inputSize + size + 2);
            Randomise(cellWeights, inputSize + size + 2);
            Randomise(biases, inputSize + size + 2);
            
            
        }

        private double[,] inputWeights;
        private double[,] temporalWeights;
        private double[] cellWeights;
        private double[] biases;

        public double ForcedOutput { get; set; }

        public void FeedForward(GateState state)
        {
            // Implements equations 4.2, 4.4, 4.8
            Array.Copy(biases, state.WeightedSums, state.WeightedSums.Length);
            Layer.AddDotProduct(state.CellStates, cellWeights, state.WeightedSums);
            Layer.WeightedInputSum(state.Inputs, inputWeights, state.WeightedSums);
            Layer.WeightedInputSum(state.LastCellOutputs, temporalWeights, state.WeightedSums);

            // Implements equations 4.3, 4.5, 4.9.
            this.OutputFromActivation(state.WeightedSums, state.Outputs);

////            //for (int i = 0; i < state.Outputs.Length; i++)
////            //{
////            //    state.Outputs[i] = ForcedOutput;
////            //}
        }

        public void Backpropogate(GateTrainingState state)
        {
            // Calculate the error with respect to the Objective function for each of the weighted sums on the gate control-side.  
            // Implements part of Equations 4.13, 4.16, 4.17.
            MultiplyByActivationDerivative(state.Now.WeightedSums, errors: state.Errors);

            // Propogate the errors back to the input. Implements part of 3.32 (one of the sums, without multiplying by activation derivative).
            WeightedOutputSum(state.Errors, inputWeights, state.InputErrors);       //// ****
            WeightedOutputSum(state.Errors, temporalWeights, state.TemporalErrors); //// ****
            DotProduct(state.Errors, cellWeights, state.CellErrors);                //// ****

            // Add the error of each weight. This is its value of the input it connects to
            // times the error of the output it connects to. Implements Equation 3.34.
            Sum(state.Errors, state.BiasWeightErrors);
            SumProducts(state.Now.Inputs, state.Errors, state.InputWeightErrors);
            SumProducts(state.Now.LastCellOutputs, state.Errors, state.TemporalWeightErrors);
            AddDotProduct(state.Now.CellStates, state.Errors, state.CellWeightErrors);
        }

        protected override double Activation(double x)
        {
            //double ePowerNegativeX = Math.Exp(-x);
            //return (1.0 / (1.0 + ePowerNegativeX));


            if (x > 0)
            {
                return 1.0 / (1.0 + Math.Exp(-x));
            }
            else
            {
                return 1.0 - (1.0 / (1.0 + Math.Exp(x)));
            }
        }

        protected override double ActivationDerivative(double x)
        {
            double sigmoid = Activation(x);
            return sigmoid * (1.0 - sigmoid);
            //double activation = Activation(x);
            //return activation * (1.0 - activation);
            //double ePowerX = Math.Exp(x);
            //double divisor = (ePowerX + 1);
            //return (1.0 * ePowerX) / (divisor * divisor);

        }


        public override double SumAbsoluteWeight
        {
            get { return Layer.SumAbsolute(inputWeights) + Layer.SumAbsolute(temporalWeights) + Layer.SumAbsolute(biases); }
        }

        public override double WeightCount
        {
            get { return InputSize * Size + Size * Size + Size; }
        }

        public void ApplyWeightChanges(GateTrainingState state, double learningCoefficient)
        {
            //Bound(state.BiasWeightErrors, 10.0);
            //Bound(state.InputWeightErrors, 10.0);
            //Bound(state.TemporalWeightErrors, 10.0);
            //Bound(state.CellWeightErrors, 10.0);

            Layer.ApplyWeightChanges(inputWeights, state.InputWeightErrors, learningCoefficient);
            Layer.ApplyWeightChanges(temporalWeights, state.TemporalWeightErrors, learningCoefficient);

            Layer.ApplyWeightChanges(cellWeights, state.CellWeightErrors, learningCoefficient);
            Layer.ApplyWeightChanges(biases, state.BiasWeightErrors, learningCoefficient);
        }

        public override void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(inputWeights);
            writer.Write(temporalWeights);
            writer.Write(cellWeights);
            writer.Write(biases);
        }

        public override void Load(System.IO.BinaryReader reader)
        {
            reader.Read(inputWeights);
            reader.Read(temporalWeights);
            reader.Read(cellWeights);
            reader.Read(biases);
        }

        public override void Export(StringBuilder builder)
        {
            ExportWeights(builder, inputWeights);
            ExportWeights(builder, temporalWeights);
            ExportWeights(builder, cellWeights);
            ExportWeights(builder, biases);
        }
    }
}
