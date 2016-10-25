using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Preprocessing
{
    class RealignmentTransformation
    {
        public int MinimumSearchedOffset { get; set; } = -100;
        public int MaximumSearchedOffset { get; set; } = +200;
        public int MinimumAllowedOffset { get; set; } = -70;
        public int MaximumAllowedOffset { get; set; } = +160;

        public Func<List<InputDataFrame>, int, double>[] AlignmentFunctions = new Func<List<InputDataFrame>, int, double>[]
        {
            // Push. Focus on in peak -ve Y acceleration.
            (frames, index) => { return -frames.Skip(index).Take(10).Average(x => x.LinearY); },
            
            // Pull. Focus on in peak +ve Y acceleration.
            (frames, index) => { return frames.Skip(index).Take(10).Average(x => x.LinearY); },
            
            // Screw-in. Focus on in peak +ve roll rate.
            (frames, index) => { return -(frames[index].Roll - frames[index - 10].Roll); },
            
            // Screw-out. Focus on in peak -ve roll rate.
            (frames, index) => { return (frames[index].Roll - frames[index - 10].Roll); },
            
            // Hit. Focus on in peak -ve pitch rate.
            (frames, index) => { return -(frames[index].Pitch - frames[index - 10].Pitch); },
            
            // Lift. Focus on in peak +ve pitch rate.
            (frames, index) => { return (frames[index].Pitch - frames[index - 10].Pitch); },
            
            // Drag Left. Focus on in peak pitch delta.
            (frames, index) => { return frames[index].PostProcessedValues["Realignment_HeadingDifferential"]; },

            // Drag Right. Focus on in peak pitch delta.
            (frames, index) => { return -frames[index].PostProcessedValues["Realignment_HeadingDifferential"]; },
        };
        
        public List<InputDataFrame> Transform(List<InputDataFrame> inputs)
        {
            AngleDifferentialPreprocessor headingDifferential = new AngleDifferentialPreprocessor(60);
            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i].PostProcessedValues["Realignment_HeadingDifferential"] = headingDifferential.GetValue(inputs[i].Heading);
            }

            List<InputDataFrame> results = new List<InputDataFrame>();
            results.AddRange(inputs.Select(i => {
                InputDataFrame clonedFrame = i.Clone();
                clonedFrame.OriginalGestureNumber = 0;
                return clonedFrame;
            }));


            List<int>[] deltaByClass = new List<int>[AlignmentFunctions.Length];
            for (int i = 0; i < AlignmentFunctions.Length; i++)
            {
                deltaByClass[i] = new List<int>();
            }

            int lastGestureNumber = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                InputDataFrame frame = inputs[i];
                if (frame.OriginalGestureNumber != lastGestureNumber && frame.OriginalGestureNumber != 0)
                {
                    // This is a target!
                    // Realign the gesture.
                    int newPosition = RealignTarget(inputs, frame.OriginalGestureNumber, i);
                    for (int j = 0; j < 60; j++)
                    {
                        if ((newPosition + j) < results.Count) {
                            results[newPosition + j].OriginalGestureNumber = frame.OriginalGestureNumber;
                        }
                    }
                    // Track changes made to facilitate statistics collection.
                    deltaByClass[frame.OriginalGestureNumber - 1].Add(newPosition - i);
                }
                lastGestureNumber = frame.OriginalGestureNumber;
            }

            for (int i = 0; i < deltaByClass.Length; i++)
            {
                double mean = deltaByClass[i].Select(value => (double)value).Average();
                double stdDeviation = Math.Sqrt(deltaByClass[i].Average(value => Math.Pow(value - mean, 2)));
                Console.WriteLine("Re-aligned Targets. Class {0}: Mean: {1}, Std Dev.:{2}", i + 1, mean, stdDeviation);
            }
            return results;
        }

        private int RealignTarget(List<InputDataFrame> inputs, int gestureNumber, int approximatePosition)
        {
            int bestOffset = int.MinValue;
            double bestValue = double.MinValue;
            for (int offset = MinimumSearchedOffset; offset < MaximumSearchedOffset; offset++)
            {
                int position = approximatePosition + offset;
                if (position < 0 || position >= inputs.Count) continue;
                
                double result = AlignmentFunctions[gestureNumber - 1](inputs, position);
                if (result > bestValue)
                {
                    bestValue = result;
                    bestOffset = offset;
                }
            }

            if (bestOffset < MinimumAllowedOffset || bestOffset > MaximumAllowedOffset)
            {
                Console.WriteLine("Re-alignment beyond authority. {0} for Gesture {1}", bestOffset, gestureNumber);
                return approximatePosition;
            }
            return approximatePosition + bestOffset;
        }
    }
}
