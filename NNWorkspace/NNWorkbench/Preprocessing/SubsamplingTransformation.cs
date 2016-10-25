using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Preprocessing
{
    class SubsamplingTransformation
    {
        Random random = new Random();

        // 500ms, 600ms, 750ms, 1 sec, 1.5 sec,
        static readonly int[] skipFrames = new int[] { 12, 10, 8, 6, 4 };

        public List<InputDataFrame> Transform(List<InputDataFrame> inputs)
        {
            List<InputDataFrame> result = new List<InputDataFrame>();

            int stepSize = 10;
            int skipSpeed = 1;
            int framesUntilStepSizeAdjustment = 100;
            int nextFrameIndex = 0;

            double averageLinearX = 0.0;
            double averageLinearY = 0.0;
            double averageLinearZ = 0.0;
            int averageCount = 0;

            for (int i = 0; i < inputs.Count; i++)
            {
                averageLinearX += inputs[i].LinearX;
                averageLinearY += inputs[i].LinearY;
                averageLinearZ += inputs[i].LinearZ;
                averageCount++;

                if (i >= nextFrameIndex) {

                    InputDataFrame frame = inputs[i].Clone();
                    // Do not correct for the number of frames this is an average over.
                    // Always assume it was 10 -- this way, if time is going slower, 
                    // we will also see a slower rate of acceleration... as expected.
                    frame.LinearX = averageLinearX / 10.0; /// ((double)averageCount);
                    frame.LinearY = averageLinearY / 10.0; /// ((double)averageCount);
                    frame.LinearZ = averageLinearZ / 10.0; /// ((double)averageCount);
                    averageCount = 0;
                    averageLinearX = 0.0;
                    averageLinearY = 0.0;
                    averageLinearZ = 0.0;

                    int gestureNumber = frame.OriginalGestureNumber;

                    // Set a new stepsize.
                    framesUntilStepSizeAdjustment--;
                    if (framesUntilStepSizeAdjustment <= 0 && gestureNumber == 0)
                    {
                        framesUntilStepSizeAdjustment = 100;
                        stepSize = skipFrames[random.Next(0, skipFrames.Length)];
                    }

                    // If current gesture is nil, and not immediately near gesture, go faster.
                    if (gestureNumber == 0 && (i > (3 * stepSize)) && (i + (3 * stepSize) < inputs.Count)
                        && inputs[i - stepSize * 3].OriginalGestureNumber == 0 && inputs[i + stepSize * 3].OriginalGestureNumber == 0)
                    {
                        nextFrameIndex += stepSize * skipSpeed;
                    }
                    else
                    {
                        skipSpeed = random.Next(1, 4);
                        nextFrameIndex += stepSize;
                    }
                    result.Add(frame);
                }
            }
            return result;
        }
    }
}
