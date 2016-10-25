using NNWorkbench.Datasets;
using NNWorkbench.NeuralNetworks;
using NNWorkbench.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Training
{
    /// <summary>
    /// Provides deterministic parallel training of Neural Networks.
    /// </summary>
    class ParallelNetworkTrainer
    {
        private const int BatchSize = 8;

        private NetworkTrainer[] trainers;
        private RecurrentNetwork network;


        public double LearningCoefficient { get; set; }
        public double Momentum { get; set; }

        // Last SQRT(Sum of Squared Error Gradients).
        public double LastGradientNorm { get; set; }

        public ParallelNetworkTrainer(RecurrentNetwork network)
        {
            this.network = network;
            trainers = new NetworkTrainer[BatchSize];
            for (int i = 0; i < BatchSize; i++)
            {
                NetworkTrainer trainer = new NetworkTrainer(network);
                trainer.LearningCoefficient = LearningCoefficient;
                trainer.Momentum = Momentum;
                trainers[i] = trainer;
            }
        }

        public void Train(IList<TrainingSample> samples, NetworkScorer scorer)
        {
            // To ensure deterministic training behaviour, ensure that the same batches of
            // training samples are trained together each time. 
            // This ensures that the resulting weight changes will be the same each time.
            //  
            // We also ensure each thread gets allocated the same item in each batch, since
            // momentum is applied at a per-thread level. Mathematically, this should
            // make zero difference -- although floating-point rounding errors may mean
            // there is infact a very minute difference.
            for (int i = 0; i < samples.Count; i+= BatchSize)
            {
                var batch = samples.Skip(i).Take(BatchSize);

                Console.WriteLine("I@{0:D3}  F-Score:{1:0.0000}   Recall:{2:0.0000}   Precision:{3:0.0000}   Mean Abs. Weight:{4:0.0000}", i, scorer.FScore, scorer.Recall, scorer.Precision, network.MeanAbsoluteWeight);
                Console.CursorTop -= 1;

                ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = BatchSize };
                Parallel.ForEach(batch, parallelOptions,(sample, state, index) => {
                    NetworkTrainer trainer = trainers[index];
                    TrainOne(trainer, sample, scorer);
                });

                UpdateWeights();
            }
        }

        public void FeedForward(IList<TrainingSample> samples, NetworkScorer scorer)
        {
            ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = BatchSize};
            Parallel.ForEach(samples, parallelOptions, sample =>
            {
                trainers.First().FeedForward(sample, scorer);
            });
        }

        private void TrainOne(NetworkTrainer trainer, TrainingSample sample, NetworkScorer scorer)
        {
            NetworkConfiguration configuration = network.Configuration;

            // Apply momentum.
            trainer.TrainingState.MultiplyError(momentum: Momentum);

            // Feed forward and back.
            trainer.FeedForwardAndBack(sample, scorer);
        }

        private void UpdateWeights()
        {
            LastGradientNorm = 0.0;
            foreach (NetworkTrainer trainer in trainers)
            {
                NetworkTrainingState trainingState = trainer.TrainingState;
                trainingState.ApplyWeightChanges(LearningCoefficient);
                LastGradientNorm = Math.Max(LastGradientNorm, trainingState.LastGradientNorm);
            }
        }

        public List<string> GetActivations(TrainingSample sample)
        {
            return trainers.First().GetActivations(sample);
        }
    }
}
