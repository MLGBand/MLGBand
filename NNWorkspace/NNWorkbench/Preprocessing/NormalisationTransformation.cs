using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Preprocessing
{
    class NormalisationTransformation
    {
        public List<InputDataFrame> Transform(List<InputDataFrame> inputs)
        {
            AccelerationIntegralPreprocessor accelerationXPreprocessor = new AccelerationIntegralPreprocessor();
            AccelerationIntegralPreprocessor accelerationYPreprocessor = new AccelerationIntegralPreprocessor();
            AccelerationIntegralPreprocessor accelerationZPreprocessor = new AccelerationIntegralPreprocessor();
            AngleDifferentialPreprocessor headingPreprocessor = new AngleDifferentialPreprocessor(6);
            AngleDifferentialPreprocessor rollPreprocessor = new AngleDifferentialPreprocessor(6);
            AngleDifferentialPreprocessor pitchPreprocessor = new AngleDifferentialPreprocessor(6);

            List<InputDataFrame> result = new List<InputDataFrame>();
            for (int i = 0; i < inputs.Count; i++)
            {
                InputDataFrame inputFrame = inputs[i].Clone();
                inputFrame.PostProcessedValues["LinearXIntegral"] = accelerationXPreprocessor.GetValue(inputFrame.LinearX);
                inputFrame.PostProcessedValues["LinearYIntegral"] = accelerationYPreprocessor.GetValue(inputFrame.LinearY);
                inputFrame.PostProcessedValues["LinearZIntegral"] = accelerationZPreprocessor.GetValue(inputFrame.LinearZ);
                inputFrame.PostProcessedValues["HeadingDifferential"] = headingPreprocessor.GetValue(inputFrame.Heading);
                inputFrame.PostProcessedValues["RollDifferential"] = rollPreprocessor.GetValue(inputFrame.Roll);
                inputFrame.PostProcessedValues["PitchDifferential"] = pitchPreprocessor.GetValue(inputFrame.Pitch);
                result.Add(inputFrame);
            }
            return result;
        }

        class AccelerationIntegralPreprocessor
        {
            // 5 seconds of history for mean correction.
            private double[] history = new double[50];
            private int historyPos = 0;
            private double integral = 0.0;

            public double GetValue(double input)
            {
                double meanCorrectedValue = input - history.Average();
                integral = integral * 0.95 + (meanCorrectedValue / 10.0);

                history[historyPos] = input;
                historyPos++;
                if (historyPos >= history.Length)
                {
                    historyPos = 0;
                }
                return integral;
            }
        }
    }
}
