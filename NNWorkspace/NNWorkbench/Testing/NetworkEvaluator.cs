using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NNWorkbench.NeuralNetworks;
using System.IO;
using NNWorkbench.Datasets;
using NNWorkbench.Training;

namespace NNWorkbench.Testing
{
    class NetworkEvaluator
    {
        public Dataset Dataset { get; set; }
        public NetworkEvaluator()
        {
        }

        public bool LogToConsole { get; set; }

        public bool Train { get; set; }

        public string InitialModel { get; set; }

        public void Evaluate(NetworkConfiguration configuration)
        {
            //15,
            RecurrentNetwork network = new RecurrentNetwork(configuration, 11, 18, 9);
            
            if (!String.IsNullOrEmpty(InitialModel))
            {
                network.Load(InitialModel);
            }
            StringBuilder exportedModel = new StringBuilder();
            network.Export(exportedModel);
            File.WriteAllText("export.txt", exportedModel.ToString());

            ParallelNetworkTrainer trainer = new ParallelNetworkTrainer(network);

            trainer.LearningCoefficient = configuration.LearningCoefficient;
            trainer.Momentum = configuration.Momentum;

            string logPath = Path.Combine(Paths.LogsDirectory, configuration.Name + ".csv");
            StreamWriter writer = new StreamWriter(logPath);
            configuration.WriteConfiguration(writer);
            writer.Flush();

            if (!Directory.Exists(Paths.Models))
            {
                Directory.CreateDirectory(Paths.Models);
            }

            writer.WriteLine("Time,Epoch,Mean Abs. Weight,Gradient Norm,Train F-Score,Validation F-Score,Test F-Score,Train MSE,Validation MSE,Test MSE,Train Recall,Train Precision,Train ACE,Train Thresh,Validation Recall,Validation Precision,Validation ACE,Validation Thresh,Test Recall, Test Precision,Test ACE,Test Thresh");

            NetworkScorer trainingScorer = new NetworkScorer(8);
            NetworkScorer validationScorer = new NetworkScorer(8);
            NetworkScorer testingScorer = new NetworkScorer(8);

            try
            {
                for (int i = 0; i < configuration.Epochs; i++)
                {
                    if (Train)
                    {
                        trainer.Train(Dataset.TrainingSamples, trainingScorer);
                    }
                    else
                    {
                        trainer.FeedForward(Dataset.TrainingSamples, trainingScorer);
                    }

                    trainer.FeedForward(Dataset.ValidationSamples, validationScorer);
                    trainer.FeedForward(Dataset.TestingSamples, testingScorer);

                    if (i % 10 == 0)
                    {
                        if (Train)
                        {
                            string modelPath = Path.Combine(Paths.Models, configuration.Name + "-" + i + ".neuralnetwork");
                            network.Save(modelPath);
                            network.Load(modelPath);
                        }

                        string trainingActivationsPath = Path.Combine(Paths.Activations, configuration.Name + "-" + i + "-train.csv");
                        System.IO.File.WriteAllLines(trainingActivationsPath, trainer.GetActivations(Dataset.TrainingSamples.First()).Select(a => a.ToString()));
                        WriteRocCurve(configuration.Name + "-" + i + "-train-roc.csv", trainingScorer);
                        WriteRocCurve(configuration.Name + "-" + i + "-validate-roc.csv", validationScorer);
                        WriteRocCurve(configuration.Name + "-" + i + "-test-roc.csv", testingScorer);

                        //string validationActivationsPath = Path.Combine(Paths.Activations, configuration.Name + "-" + i + "-validate.csv");
                        //System.IO.File.WriteAllLines(validationActivationsPath, trainer.GetActivations(Dataset.ValidationSamples.First()).Select(a => a.ToString()));
                        //string testActivationsPath = Path.Combine(Paths.Activations, configuration.Name + "-" + i + "-test.csv");
                        //System.IO.File.WriteAllLines(testActivationsPath, trainer.GetActivations(Dataset.TestingSamples.First()).Select(a => a.ToString()));

                        //WriteRocCurve(configuration.Name + "-" + i + "-train-roc.csv", trainingScorer);
                        //WriteRocCurve(configuration.Name + "-" + i + "-validate-roc.csv", validationScorer);
                    }

                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}", DateTime.Now, i, network.MeanAbsoluteWeight, trainer.LastGradientNorm,
                        trainingScorer.FScore, validationScorer.FScore, testingScorer.FScore,
                        trainingScorer.MSE, validationScorer.MSE, testingScorer.MSE,
                        trainingScorer.Recall, trainingScorer.Precision, trainingScorer.CrossEntropyError, trainingScorer.Threshold,
                        validationScorer.Recall, validationScorer.Precision,validationScorer.CrossEntropyError, validationScorer.Threshold,
                        testingScorer.Recall, testingScorer.Precision, testingScorer.CrossEntropyError, testingScorer.Threshold
                        );
                    writer.Flush();

                    if (LogToConsole)
                    {
                        //Mean Abs. Weight:{4:0.0000}   Gradient Norm: {8:00000}   MSE:{9:0.0000}
                        //    F-Score:{1:0.0000}   Recall:{2:0.0000}   Precision:{3:0.0000}
                        Console.WriteLine("{0:D3}   MAW:{4:0.0000}   Gradient: {8:00000}   Rec:{2:0.0000}   Prec:{3:0.0000}  ACE:{5:0.0000}   Val. ACE:{6:0.0000}   Test ACE:{7:0.0000}", i, trainingScorer.FScore, trainingScorer.Recall, trainingScorer.Precision,
                            network.MeanAbsoluteWeight, trainingScorer.CrossEntropyError, validationScorer.CrossEntropyError, testingScorer.CrossEntropyError, trainer.LastGradientNorm, trainingScorer.MSE);
                    }
                    trainingScorer.ResetScores();
                    validationScorer.ResetScores();
                    testingScorer.ResetScores();
                }
            }
            catch (Exception ex)
            {
                if (LogToConsole)
                {
                    Console.WriteLine("{0},Aborted {1} at {2}", DateTime.Now, ex.Message, ex.StackTrace);
                }
                writer.WriteLine("{0},Aborted {1} at {2}", DateTime.Now, ex.Message, ex.StackTrace);
            }
        }

        private static void WriteRocCurve(string name, NetworkScorer scorer) //, NetworkScorer validationScorer, NetworkScorer testScorer)
        {
            List<string> lines = new List<string>();
            string header = "Threshold";
            for (int i = 0; i < scorer.ClassCount; i++)
            {
                header += string.Format(",FPR-{0},TPR-{0}", i);
            }
            lines.Add(header);

            foreach (var point in scorer.RocCurve)
            {
                string line = point.Threshold.ToString();
                for (int i = 0; i < scorer.ClassCount; i++)
                {
                    line += string.Format(",{0},{1}", point.GetFalsePositiveRate(i), point.GetTruePositiveRate(i));
                }
                lines.Add(line);
            }

            string rocPath = Path.Combine(Paths.Activations, name + ".csv");
            System.IO.File.WriteAllLines(rocPath, lines);
        }
    }
}
