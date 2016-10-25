using NNWorkbench.Datasets;
using NNWorkbench.NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Testing
{
    class NetworkScorer
    {
        public NetworkScorer(int classCount)
        {
            rocTrackers = new List<RocTracker>();
            this.ClassCount = classCount;

            double threshold = 0.02;

            for (; threshold < 1.0; threshold += 0.02)
            {
                rocTrackers.Add(new RocTracker(threshold, classCount));
            }

            ResetScores();
        }

        private List<RocTracker> rocTrackers;
        private double sumSquaredError;
        private double sampleCount;
        public readonly int ClassCount;
        private double sumCrossEntropyError;
        private object syncLock = new object();

        public void Score(List<NetworkState> states, TrainingSample sample)
        {
            var availableTargets = sample.Frames.Where(frame => frame.IsTarget).ToList();
            int totalSelection = availableTargets.Count(); // tp + fn

            double deltaSamples = 0.0;
            double deltaSquaredError = 0.0;
            double deltaCrossEntropy = 0.0;
            for (int i = 1; i < states.Count; i++)
            {
                NetworkState state = states[i];
                TrainingFrame frame = sample.Frames[i - 1];
                double time = sample.Frames[i - 1].Frame.Start;

                for (int j = 0; j < state.Output.Length; j++) {
                    deltaSquaredError += Math.Abs(state.Output[j] - frame.ExpectedOutputs[j]) / ((double)state.Output.Length);
                    deltaCrossEntropy += -(Math.Log(state.Output[j]) * frame.ExpectedOutputs[j]) / ((double)state.Output.Length);
                }
                deltaSamples++;
            }
            
            lock (syncLock)
            {
                sumSquaredError += deltaSquaredError;
                sumCrossEntropyError += deltaCrossEntropy;
                sampleCount += deltaSamples;
            }

            foreach (var rocTracker in rocTrackers)
            {
                rocTracker.Score(states, sample);
            }
        }

        public void ResetScores()
        {
            sumSquaredError = 0;
            sumCrossEntropyError = 0;
            sampleCount = 0;
            foreach (var rocTracker in rocTrackers)
            {
                rocTracker.ResetScores();
            };
        }

        public IList<RocTracker> RocCurve
        {
            get { return rocTrackers; }
        }
         

        public double MSE
        {
            get { return sumSquaredError / sampleCount; }
        }

        public double CrossEntropyError
        {
            get { return sumCrossEntropyError / sampleCount; }
        }

        private List<RocTracker> GetOptimalRocTrackerForEachClass()
        {
            List<RocTracker> result = new List<RocTracker>();
            for (int i = 0; i < ClassCount; i++)
            {
                result.Add(rocTrackers.OrderByDescending(x => x.GetFScore(i)).First());
            }
            return result;
        }

        public double Precision
        {
            get
            {
                double precision = 0.0;
                var optimalTrackers = GetOptimalRocTrackerForEachClass();
                for (int i = 0; i < ClassCount; i++)
                {
                    precision += optimalTrackers[i].GetPrecision(i);
                }
                return precision / ClassCount;
            }
        }

        public double Recall
        {
            get
            {
                double recall = 0.0;
                var optimalTrackers = GetOptimalRocTrackerForEachClass();
                for (int i = 0; i < ClassCount; i++)
                {
                    recall += optimalTrackers[i].GetTruePositiveRate(i);
                }
                return recall / ClassCount;
            }
        }

        public double FScore
        {
            get
            {
                double fscore = 0.0;
                var optimalTrackers = GetOptimalRocTrackerForEachClass();
                for (int i = 0; i < ClassCount; i++)
                {
                    fscore += optimalTrackers[i].GetFScore(i);
                }
                return fscore / ClassCount;
            }
        }

        public double Threshold
        {
            get
            {
                return GetOptimalRocTrackerForEachClass().Average(x => x.Threshold);
            }
        }
    }
}

class RocTracker
{
    public RocTracker(double threshold, int classCount)
    {
        this.Threshold = threshold;
        this.ClassCount = classCount;
        this.falseNegatives = new int[ClassCount];
        this.truePositives = new int[ClassCount];
        this.falsePositives = new int[ClassCount];
        ResetScores();
    }

    public readonly double Threshold;
    public readonly int ClassCount;
    private int[] falsePositives;
    private int[] truePositives;
    private int[] falseNegatives;
    private object syncLock = new object();

    private const double MatchingTolerance = 1.0;// 0.070; //0.025;

    public void Score(List<NetworkState> states, TrainingSample sample)
    {
        var availableTargets = sample.Frames.Where(frame => frame.IsTarget && frame.ExpectedOutputs[0] < 0.001).ToList();
        int totalSelection = availableTargets.Count(); // tp + fn
        bool[] suppressRedetection = new bool[ClassCount];
        int[] falsePositives = new int[ClassCount];
        int[] truePositives = new int[ClassCount];
        int[] falseNegatives = new int[ClassCount];

        for (int i = 1; i < states.Count; i++)
        {
            NetworkState state = states[i];
            TrainingFrame frame = sample.Frames[i - 1];
            double time = sample.Frames[i - 1].Frame.Start;
            
            for (int j = 1; j < state.Output.Length; j++)
            {
                // Apply hysteresis to gesture detection.
                if (state.Output[j] > this.Threshold && !suppressRedetection[j-1])
                {
                    // Detect
                    TrainingFrame matchedTarget = availableTargets.FirstOrDefault(t => Math.Abs(t.Frame.Start - time) < MatchingTolerance);
                    if (matchedTarget != null)
                    {
                        // If correct classification.
                        if (matchedTarget.ExpectedOutputs[j] > 0.999)
                        {
                            availableTargets.Remove(matchedTarget);
                            truePositives[j-1]++;
                        }
                        else
                        {
                            falsePositives[j-1]++;
                        }
                    }
                    else
                    {
                        falsePositives[j-1]++;
                    }

                    suppressRedetection[j-1] = true;
                }
                else if (state.Output[j] < this.Threshold * 0.75)
                {
                    suppressRedetection[j-1] = false;
                }
            }

        }

        for (int j = 1; j <= ClassCount; j++)
        {
            falseNegatives[j - 1] += availableTargets.Count(x => x.ExpectedOutputs[j] > 0.999);
        }

        lock (syncLock)
        {
            Sum(this.falseNegatives, falseNegatives);
            Sum(this.truePositives, truePositives);
            Sum(this.falsePositives, falsePositives);
        }
    }

    private void Sum(int[] existingArray, int[] newArray)
    {
        for (int i = 0; i < newArray.Length; i++) {
            existingArray[i] += newArray[i];
        }
    }

    public double GetPrecision(int classNumber)
    {
        return ((double)truePositives[classNumber]) / (truePositives[classNumber] + falsePositives[classNumber]);
    }
    
    public double GetFalsePositiveRate(int classNumber)
    {
        return 1.0 - GetPrecision(classNumber); 
    }

    public double GetTruePositiveRate(int classNumber)
    {
        return ((double)truePositives[classNumber]) / (truePositives[classNumber] + falseNegatives[classNumber]);
    }

    public double GetFScore(int classNumber)
    {
        return 2.0 * (GetPrecision(classNumber) * GetTruePositiveRate(classNumber)) / (GetPrecision(classNumber) + GetTruePositiveRate(classNumber));
    }

    public void ResetScores()
    {
        Array.Clear(truePositives, 0, ClassCount);
        Array.Clear(falseNegatives, 0, ClassCount);
        Array.Clear(falsePositives, 0, ClassCount);
    }
}