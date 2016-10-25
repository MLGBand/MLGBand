using NNWorkbench.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class FeedForwardLayer : Layer
    {
        private readonly double[] biasWeights;
        private readonly double[,] inputWeights;

        public FeedForwardLayer(NetworkConfiguration configuration, int size, int inputSize)
            : base(configuration, size, inputSize)
        {
            this.biasWeights = new double[size];
            Randomise(biasWeights, inputSize + 1);

            this.inputWeights = new double[inputSize, size];
            Randomise(inputWeights, inputSize + 1);
        }

        public void FeedForward(FeedForwardState state)
        {
            double[] weightedSums = state.WeightedSums;

            Array.Clear(weightedSums, 0, weightedSums.Length);
            Sum(biasWeights, weightedSums);
            WeightedInputSum(state.Inputs, inputWeights, weightedSums);
            OutputFromActivation(weightedSums, state.Outputs);        
        }

        public void BackPropogate(FeedForwardTrainingState state)
        {
            Debug.Assert(state.Now.WeightedSums != state.Now.Outputs);

            // Initially, state.Errors contains just those backpropogated from the next layer.
            // These are d(E)/d(outputs of this layer).

            // Multiply by the derivative of the activation function to finalise
            // errors with respect to the weighted sum for the units in this layer.
            if (state.Now.Outputs.Length != 1)
            {
                MultiplyByActivationDerivative(state.Now.WeightedSums, state.Errors);
            }

            // Propogate the errors back to the input.
            Array.Clear(state.InputErrors, 0, state.InputErrors.Length);
            WeightedOutputSum(state.Errors, inputWeights, state.InputErrors);
            
            // Add the error of each weight. This is its value of the input it connects to
            // times the error of the output it connects to.
            Sum(state.Errors, state.BiasWeightErrors);
            SumProducts(state.Now.Inputs, state.Errors, state.InputWeightErrors);
        }

        protected override double Activation(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        protected override double ActivationDerivative(double x)
        {
            double output = Activation(x);
            return output * (1.0 - output);
        }
        

        public void ApplyWeightChanges(FeedForwardTrainingState state, double learningCoefficient)
        {
            Layer.ApplyWeightChanges(biasWeights, state.BiasWeightErrors, learningCoefficient);
            Layer.ApplyWeightChanges(inputWeights, state.InputWeightErrors, learningCoefficient);
        }

        public override double SumAbsoluteWeight
        {
            get { return Layer.SumAbsolute(inputWeights) + Layer.SumAbsolute(biasWeights); }
        }

        public override double WeightCount
        {
            get { return InputSize * Size + Size; }
        }

        public override void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(biasWeights);
            writer.Write(inputWeights);
        }

        public override void Load(System.IO.BinaryReader reader)
        {
            reader.Read(biasWeights);
            reader.Read(inputWeights);
        }

        public override void Export(StringBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
