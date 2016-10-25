using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Testing
{
    public class NetworkConfiguration
    {
        public string Name { get; set; }
        public int Seed { get; set; }
        public int Epochs { get; set; }
        public double LearningCoefficient { get; set; }
        public double Momentum { get; set; }
        public RandomType WeightInitialisationMethod { get; set; }
        public double WeightInitialisationSize { get; set; }
        public double DetectionValue { get; set; }
        public double NoDetectionValue { get; set; }

        private Random random;

        /// <summary>
        /// Initialises the Random number generator to the initial seed value,
        /// so that all following calls to NextRandom() are deterministically
        /// generated.
        /// </summary>
        public void InitialiseRandom()
        {
            this.random = new Random(Seed);
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random number between 0.0 and 1.0.</returns>
        public double NextRandom()
        {
            return random.NextDouble();
        }

        private static NetworkConfiguration Parse(string line)
        {
            var parts = line.Split(',');
            NetworkConfiguration result = new NetworkConfiguration();
           
            result.Name = parts[0];
            result.Seed = int.Parse(parts[1]);
            result.Epochs = int.Parse(parts[2]);
            result.LearningCoefficient = double.Parse(parts[3]);
            result.Momentum = double.Parse(parts[4]);
            result.WeightInitialisationMethod = (RandomType)Enum.Parse(typeof(RandomType), parts[5], true);
            result.WeightInitialisationSize = double.Parse(parts[6]);
            result.DetectionValue = double.Parse(parts[7]);
            result.NoDetectionValue = double.Parse(parts[8]);
            return result;
        }

        public void WriteConfiguration(StreamWriter writer)
        {
            writer.WriteLine("Network Configuration Details");
            writer.WriteLine("Name,\"{0}\"", Name);
            writer.WriteLine("Seed,{0}", Seed);
            writer.WriteLine("Epochs,{0}", Epochs);
            writer.WriteLine("Learning Coefficient,{0}", LearningCoefficient);
            writer.WriteLine("Momentum,{0}", Momentum);
            writer.WriteLine("Weight Initialisation Method,{0}", Enum.GetName(typeof(RandomType), WeightInitialisationMethod));
            writer.WriteLine("Weight Initialisation Size,{0}", WeightInitialisationSize);
            writer.WriteLine("Detection Value,{0}", DetectionValue);
            writer.WriteLine("No Detection Value,{0}", NoDetectionValue);
            writer.WriteLine();
        }

        public static List<NetworkConfiguration> Read()
        {
            string path = Path.Combine(Paths.LogsDirectory, "schedule.csv");
            return File.ReadAllLines(path).Skip(1).Select(line => Parse(line)).ToList();
        }
    }
}
