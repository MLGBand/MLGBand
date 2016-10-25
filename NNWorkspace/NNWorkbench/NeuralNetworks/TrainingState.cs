using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public abstract class TrainingState
    {
        /// <summary>
        /// Multiplies accumulated errors by the specified factor.
        /// </summary>
        /// <param name="factor">The factor to multiply accumulated errors by.</param>
        public abstract void MultiplyError(double factor);

        protected static void Multiply(double[] weights, double scalar)
        {
            for (int i= 0; i < weights.Length; i++)
            {
                weights[i] *= scalar;
            }
        }

        protected static void Multiply(double[,] weights, double scalar)
        {
            for (int x = 0; x < weights.GetLength(0); x++)
            {
                for (int y = 0; y < weights.GetLength(1); y++)
                {
                    weights[x, y] *= scalar;
                }
            }
        }

        public abstract double GetSumSquaredGradients();
    }
}
