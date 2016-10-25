using NNWorkbench.Datasets;
using NNWorkbench.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    class NetworkTrainer
    {
        private RecurrentNetwork network;
        private NetworkTrainingState trainingState;

        public double LearningCoefficient { get; set; }
        public double Momentum { get; set; }

        public NetworkTrainer(RecurrentNetwork network)
        {
            this.network = network;
            this.trainingState = new NetworkTrainingState(network);
        }

        private NetworkConfiguration Configuration
        {
            get { return network.Configuration; }
        }

        public RecurrentNetwork Network
        {
            get { return network; }
        }

        public NetworkTrainingState TrainingState
        {
            get { return trainingState; }
        }
        
        public void Train(TrainingSample sample, NetworkScorer scorer)
        {
            // Apply momentum.
            trainingState.MultiplyError(momentum: Momentum);

            FeedForwardAndBack(sample, scorer);

            // Apply the required weight changes.
            trainingState.ApplyWeightChanges(LearningCoefficient);
        }

        public void FeedForwardAndBack(TrainingSample sample, NetworkScorer scorer)
        {
            List<NetworkState> states = FeedForward(sample, scorer);

            // Backpropogate through time.
            for (int i = states.Count - 1; i >= 1; i--)
            {
                // Get the corresponding frame.
                TrainingFrame frame = sample.Frames[i - 1];
                NetworkState current = states[i];
                
                for (int j = 0; j < current.Output.Length; j++)
                {
                    trainingState.Errors[j] = (current.Output[j] - frame.ExpectedOutputs[j]);
                }

                trainingState.BackPropogate(now: current);
            }
        }


        public List<NetworkState> FeedForward(TrainingSample sample, NetworkScorer scorer)
        {
            List<NetworkState> states = new List<NetworkState>();

            // Initialise with a zero before-starting state.
            NetworkState state = new NetworkState(Network);
            states.Add(state);

            // Forward propogate through time
            foreach (TrainingFrame trainingFrame in sample.Frames)
            {
                state = new NetworkState(Network, state);
                Array.Copy(trainingFrame.Frame.Values, state.Input, state.Input.Length);
                state.FeedForward();
                states.Add(state);
            }

            if (scorer != null)
            {
                 scorer.Score(states, sample);
            }
            return states;
        } 

        public List<string> GetActivations(TrainingSample sample)
        {
            var states = FeedForward(sample, null);
            return states.Select(state => string.Join(",", state.Output.Select(o => o.ToString()))).ToList();
        }
    }
}
