using NNWorkbench.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class SoftmaxLayer : Layer
    {
        private readonly double[] biasWeights;
        private readonly double[,] inputWeights;

        public SoftmaxLayer(NetworkConfiguration configuration, int size, int inputSize)
            : base(configuration, size, inputSize)
        {
            this.biasWeights = new double[size];
            Randomise(biasWeights, inputSize + 1);

            this.inputWeights = new double[inputSize, size];
            Randomise(inputWeights, inputSize + 1);
        }

        public void FeedForward(SoftmaxState state)
        {
            double[] weightedSums = state.WeightedSums;

            Array.Clear(weightedSums, 0, weightedSums.Length);
            Sum(biasWeights, weightedSums);
            WeightedInputSum(state.Inputs, inputWeights, weightedSums);
            //OutputFromActivation(weightedSums, state.Outputs);

            double denominator = 0;
            for (int i = 0; i < state.Outputs.Length; i++)
            {
                state.Outputs[i] = Math.Exp(state.WeightedSums[i]);
                denominator += state.Outputs[i];
            }
            for (int i = 0; i < state.Outputs.Length; i++)
            {
                state.Outputs[i] /= denominator;
            }
        }

        public void BackPropogate(SoftmaxTrainingState state)
        {
            Debug.Assert(state.Now.WeightedSums != state.Now.Outputs);

            // Initially, state.Errors contains just those backpropogated from the next layer.
            // These are d(E)/d(outputs of this layer).
            // Since this is the last layer, the error should be
            // output - target.

            // Multiply by the derivative of the activation function to finalise
            // errors with respect to the weighted sum for the units in this layer.
            // This is one for softmax classifier.

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
            throw new NotImplementedException();
            //return 1.0 / (1.0 + Math.Exp(-x));
        }

        protected override double ActivationDerivative(double x)
        {
            throw new NotImplementedException();
            //double output = Activation(x);
            //return output * (1.0 - output);
        }

        public void ApplyWeightChanges(SoftmaxTrainingState state, double learningCoefficient)
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
            ExportWeights(builder, inputWeights);
            ExportWeights(builder, biasWeights);
        }
    }
}
