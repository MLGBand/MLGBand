using NNWorkbench.NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NNWorkbench.Testing;
using NNWorkbench.Datasets;
using NNWorkbench.Preprocessing;
namespace NNWorkbench
{
    class Program
    {
        static Queue<NetworkConfiguration> tests;

        static void Main(string[] args)
        {
            NetworkConfiguration configuration = new NetworkConfiguration()
            {
                DetectionValue = 1.0,
                Epochs = 100000,
                LearningCoefficient = 0.000005,
                Momentum = 0.9,
                Name = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"),
                NoDetectionValue = 0.0,
                Seed = 0,
                WeightInitialisationMethod = RandomType.Linear,
                WeightInitialisationSize = 1.0
            };
            
            NetworkEvaluator evaluator = new NetworkEvaluator()
            {
                LogToConsole = true,
                Dataset = GestureDataset.Load(),
                Train = false,
                InitialModel= @"C:\Users\Patrick\Source\deco3801teammlg\NNWorkspace\Models\2016-10-23 09-03-00-600.neuralnetwork"
            };
            evaluator.Evaluate(configuration);

            //tests = new Queue<NetworkConfiguration>(NetworkConfiguration.Read());
            //new Thread(TestConfiguration).Start();
            //new Thread(TestConfiguration).Start();
            //new Thread(TestConfiguration).Start();
            //new Thread(TestConfiguration).Start();

            Console.ReadLine();
        }
        
        static void TestConfiguration()
        {
            NetworkEvaluator evaluator = new NetworkEvaluator();
            while (true)
            {
                NetworkConfiguration configuration;
                lock (tests)
                {
                    if (tests.Count == 0) break;
                    configuration = tests.Dequeue();
                }
                Console.WriteLine("[{0}] Starting test of {1}", DateTime.Now, configuration.Name);
                evaluator.Evaluate(configuration);
            }
        }
    }
}
