using NNWorkbench.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public abstract class Layer
    {
        public readonly int Size;
        public readonly int InputSize;
        private readonly NetworkConfiguration configuration;

        public Layer(NetworkConfiguration configuration, int size, int inputSize)
        {
            this.configuration = configuration;
            this.Size = size;
            this.InputSize = inputSize;
        }

        public abstract double SumAbsoluteWeight { get; }

        public abstract double WeightCount { get; }

        public abstract void Save(BinaryWriter writer);

        public abstract void Load(BinaryReader reader);

        public abstract void Export(StringBuilder builder);

        protected virtual double Activation(double x)
        {
            return 1.7159 * tanh((2.0 / 3.0) * x);
        }

        private static double tanh(double x)
        {
            double ePow2x = Math.Exp(2 * x);
            return (ePow2x - 1) / (ePow2x + 1);
        }

        private static double sech(double x)
        {
            double ePowx = Math.Exp(x);
            return 2.0 / (ePowx + (1.0 / ePowx));
        }

        protected virtual double ActivationDerivative(double x)
        {
            // derivative for 1.7159 * tanh(2.0/3.0)
            double s = sech((2.0 / 3.0) * x);
            return (2.0 / 3.0) * 1.7159 * s * s;
        }

        protected void OutputFromActivation(double[] input, double[] output)
        {
            Debug.Assert(input.Length == output.Length);

            for (int i = 0; i < input.Length; i++)
            {
                double activated = Activation(input[i]);
                if (double.IsNaN(activated))
                    throw new ArgumentException("NaN hit");
                output[i] = activated;
            }
        }

        protected void MultiplyByActivationDerivative(double[] inputs, double[] errors)
        {
            Debug.Assert(inputs.Length == errors.Length);

            for (int i = 0; i < inputs.Length; i++)
            {
                errors[i] *= ActivationDerivative(inputs[i]);
            }
        }

        protected static void Sum(double[] weights, double[] outputs)
        {
            Debug.Assert(weights.Length == outputs.Length);
            
            for (int o = 0; o < outputs.Length; o++)
            {
                Debug.Assert(!double.IsNaN(weights[o]));
                outputs[o] += weights[o];
            }
        }

        protected static void WeightedInputSum(double[] inputs, double[,] weights, double[] output)
        {
            Debug.Assert(weights.GetLength(0) == inputs.Length);
            Debug.Assert(weights.GetLength(1) == output.Length);

            for (int o = 0; o < output.Length; o++)
            {
                double sum = output[o];
                for (int i = 0; i < inputs.Length; i++)
                {
                    sum += inputs[i] * weights[i, o];
                }
                output[o] = sum;
            }
        }

        protected static void WeightedOutputSum(double[] outputs, double[,] weights, double[] inputs)
        {
            Debug.Assert(weights.GetLength(0) == inputs.Length);
            Debug.Assert(weights.GetLength(1) == outputs.Length);

            for (int i = 0; i < inputs.Length; i++)
            {
                double sum = inputs[i];
                for (int o = 0; o < outputs.Length; o++)
                {
                    sum += outputs[o] * weights[i, o];
                }
                inputs[i] = sum;
            }
        }

        protected static void SumProducts(double[] inputs, double[] outputs, double[,] weights)
        {
            Debug.Assert(weights.GetLength(0) == inputs.Length);
            Debug.Assert(weights.GetLength(1) == outputs.Length);

            for (int i = 0; i < inputs.Length; i++)
            {
                for (int o = 0; o < outputs.Length; o++)
                {
                    weights[i, o] += inputs[i] * outputs[o];
                }
            }
        }

        protected static void DotProduct(double[] a, double[] b, double[] output)
        {
            Debug.Assert(a.Length == b.Length && b.Length == output.Length);
            for (int i = 0; i < output.Length; i++)
            {
                Debug.Assert(!double.IsNaN(a[i]) && !double.IsNaN(b[i]));
                output[i] = a[i] * b[i];
            }
        }

        protected static void AddDotProduct(double[] a, double[] b, double[] output)
        {
            Debug.Assert(a.Length == b.Length);
            Debug.Assert(a.Length == output.Length);

            for (int i = 0; i < output.Length; i++)
            {
                output[i] += a[i] * b[i];
            }
        }

        protected static void ApplyWeightChanges(double[] weights, double[] errors, double learningCoefficient)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] -= errors[i] * learningCoefficient;
            }
        }

        protected static void ApplyWeightChanges(double[,] weights, double[,] errors, double learningCoefficient)
        {
            for (int x = 0; x < weights.GetLength(0); x++)
            {
                for (int y = 0; y < weights.GetLength(1); y++)
                {
                    Debug.Assert(!double.IsNaN(errors[x,y]));
                    weights[x, y] -= errors[x, y] * learningCoefficient;
                }
            }
        }

        protected void Randomise(double[,] weights, int fanIn)
        {
            double fanInDouble = (double)fanIn;
            for (int x = 0; x < weights.GetLength(0); x++)
            {
                for (int y = 0; y < weights.GetLength(1); y++)
                {
                    weights[x, y] = Random(fanIn);
                }
            }
        }

        protected void Randomise(double[] weights, int fanIn)
        {
            double fanInDouble = (double)fanIn;
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Random(fanIn);
            }
        }

        private double Random(double fanIn)
        {
            if (configuration.WeightInitialisationMethod == RandomType.Linear)
            {
                return (configuration.NextRandom() * 0.4) - 0.2;
                //return (configuration.NextRandom() - 0.5) * (4.8 / fanIn) * configuration.WeightInitialisationSize;
            }
            else if (configuration.WeightInitialisationMethod == RandomType.Guassian)
            {
                return RandomNormal() * configuration.WeightInitialisationSize;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Generates normally distributed random numbers, with a mean of zero and variance of one.
        /// </summary>
        /// <returns>a normally distributed random number, with a mean of zero and variance of one.</returns>
        private double RandomNormal()
        {
            double uniform1 = configuration.NextRandom();
            double uniform2 = configuration.NextRandom();

            // Use Box-Muller transform to generate normally distributed random numbers.
            return Math.Sqrt(-2.0 * Math.Log(uniform1)) * Math.Cos(2.0 * Math.PI * uniform2);
        }

        protected static double SumAbsolute(double[,] weights)
        {
            double sum = 0.0;
            for (int x = 0; x < weights.GetLength(0); x++)
            {
                for (int y = 0; y < weights.GetLength(1); y++)
                {
                    sum += Math.Abs(weights[x, y]);
                }
            }
            return sum;
        }

        protected static double SumAbsolute(double[] weights)
        {
            return weights.Sum(weight => Math.Abs(weight));
        }

        public static void Bound(double[] values, double max)
        {
            //double scaling = 1.0;
            for (int i = 0; i < values.Length; i++)
            {
                double value = Math.Abs(values[i]);
                if (value > max)
                {
                    values[i] *= (max / value);
                }
                //if (value > scaling)
                //    scaling = value;
            }
            //if (scaling == 1.0) return;
            //scaling = 1.0 / scaling;

            //for (int i = 0; i < values.Length; i++)
            //{
            //    values[i] *= scaling;
            //}
        }


        public static void Bound(double[,] values, double max)
        {
            //double scaling = max;
            for (int x = 0; x < values.GetLength(0); x++)
            {
                for (int y = 0; y < values.GetLength(1); y++)
                {
                    double value = Math.Abs(values[x, y]);
                    if (value > max)
                    {
                        values[x, y] *= (max / value);
                    }
                    //if (value > scaling)
                    //    scaling = value;
                }
            }
            //if (scaling == max) return;
            //scaling = max / scaling;

            //for (int x = 0; x < values.GetLength(0); x++)
            //{
            //    for (int y = 0; y < values.GetLength(1); y++)
            //    {
            //        values[x,y] *= scaling;
            //    }
            //}
        }

        public static double SumSquared(double[] values)
        {
            double sum = 0.0;
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i] * values[i];
            }
            return sum;
        }

        public static double SumSquared(double[,] values)
        {
            double sum = 0.0;
            for (int x = 0; x < values.GetLength(0); x++)
            {
                for (int y = 0; y < values.GetLength(1); y++)
                {
                    double value = values[x, y];
                    sum += value * value;
                }
            }
            return sum;
        }


        public void ExportWeights(StringBuilder builder, double[,] values)
        {
            for (int o = 0; o < values.GetLength(1); o++)
            {
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    builder.Append(((float)values[i, o]) + "f, ");
                }
            }
        }

        public void ExportWeights(StringBuilder builder, double[] values)
        {
            for (int o = 0; o < values.Length; o++)
            {
                builder.Append(((float)values[o]) + "f, ");
            }
        }
    }
}
