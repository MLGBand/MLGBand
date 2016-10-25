using NNWorkbench.NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class NetworkTrainingState
    {
        public readonly RecurrentNetwork Network;
        private List<TrainingState> states;

        public readonly double[] Errors;

        public NetworkTrainingState(RecurrentNetwork network)
        {
            Network = network;
            double[] inputErrors = new double[network.Layers[0].InputSize];
            double[] errors = null;

            states = new List<TrainingState>();
            for (int i = 0; i < network.Layers.Count; i++)
            {
                Layer layer = network.Layers[i];
                errors = new double[layer.Size];

                if (layer is LSTMLayer)
                {
                    states.Add(new LSTMTrainingState(inputErrors, errors));
                }
                else if (layer is RecurrentLayer)
                {
                    //RecurrentState lastState = last != null ? (RecurrentState)last.states[i] : null;
                    states.Add(new RecurrentTrainingState(inputErrors, errors));
                }
                else if (layer is SoftmaxLayer)
                {
                    states.Add(new SoftmaxTrainingState(inputErrors, errors));
                }
                else
                {
                    states.Add(new FeedForwardTrainingState(inputErrors, errors));
                }

                // The input errors next layer are the errors of the last.
                inputErrors = errors;
            }
            this.Errors = errors;
        }

        public void MultiplyError(double momentum)
        {
            foreach (TrainingState state in states)
            {
                // Keep a proportion of the the total error backpropogating the last sample.
                // This functions as a momentum term.
                state.MultiplyError(momentum);
            }
        }

        public void BackPropogate(NetworkState now)
        {
            for (int i = states.Count - 1; i >= 0; i--)
            {
                object state = states[i];
                Layer layer = Network.Layers[i];

                LSTMLayer ltsmLayer = layer as LSTMLayer;
                RecurrentLayer recurrentLayer = layer as RecurrentLayer;
                SoftmaxLayer softmaxLayer = layer as SoftmaxLayer;
                FeedForwardLayer feedForwardLayer = layer as FeedForwardLayer;
               
                if (ltsmLayer != null)
                {
                    LSTMTrainingState ltsmState = (LSTMTrainingState)state;
                    ltsmState.Now = (LSTMState)now.states[i];
                    ltsmLayer.Backpropogate(ltsmState);
                }
                else if (recurrentLayer != null)
                {
                    RecurrentTrainingState recurrentState = (RecurrentTrainingState)state;
                    //recurrentState.Last = (RecurrentState)last.states[i];
                    recurrentState.Now = (RecurrentState)now.states[i];
                    recurrentLayer.BackPropogate(recurrentState);
                }
                else if (softmaxLayer != null)
                {
                    SoftmaxTrainingState softmaxState = (SoftmaxTrainingState)state;
                    softmaxState.Now = (SoftmaxState)now.states[i];
                    softmaxLayer.BackPropogate(softmaxState);
                }
                else
                {
                    FeedForwardTrainingState feedForwardState = (FeedForwardTrainingState)state;
                    feedForwardState.Now = (FeedForwardState)now.states[i];
                    feedForwardLayer.BackPropogate(feedForwardState);
                }
            }
        }

        private const double Threshold = 1000.0;
        public double LastGradientNorm { get; private set; }

        public void ApplyWeightChanges(double learningCoefficient)
        {
            double sumSquaredErrorGradient = 0;
            for (int i = 0; i < states.Count; i++)
            {
                sumSquaredErrorGradient += states[i].GetSumSquaredGradients();
            }

            double normalisedErrorGradient = Math.Sqrt(sumSquaredErrorGradient);
            if (normalisedErrorGradient > Threshold)
            {
                learningCoefficient *= (Threshold / normalisedErrorGradient);
            }
            LastGradientNorm = normalisedErrorGradient;
            //
            for (int i = 0; i < states.Count; i++)
            {
                object state = states[i];
                Layer layer = Network.Layers[i];

                LSTMTrainingState ltsmState = state as LSTMTrainingState;
                RecurrentTrainingState recurrantState = state as RecurrentTrainingState;
                FeedForwardTrainingState feedForwardState = state as FeedForwardTrainingState;
                SoftmaxTrainingState softmaxState = state as SoftmaxTrainingState;
                if (ltsmState != null)
                {
                    ((LSTMLayer)layer).ApplyWeightChanges(ltsmState, learningCoefficient);
                }
                else if (recurrantState != null)
                {
                    ((RecurrentLayer)layer).ApplyWeightChanges(recurrantState, learningCoefficient);
                }
                else if (softmaxState != null)
                {
                    ((SoftmaxLayer)layer).ApplyWeightChanges(softmaxState, learningCoefficient);
                }
                else
                {
                    ((FeedForwardLayer)layer).ApplyWeightChanges(feedForwardState, learningCoefficient);
                }
            }
        }
    }
}
