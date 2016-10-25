using NNWorkbench.Datasets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Preprocessing
{
    struct InputData
    {
        public double Heading;
        public double Pitch;
        public double Roll;
        public double LinearX;
        public double LinearY;
        public double LinearZ;
        public double GravityX;
        public double GravityY;
        public double GravityZ;
    }

    class GesturePreprocessor
    {
        public GesturePreprocessor()
        {
        }

        // 500ms, 600ms, 750ms, 1 sec, 1.5 sec,
        static readonly int[] skipFrames = new int[] { 12, 10, 8, 6, 4 };

        private const int GestureCount = 8;

        //public static void Realign(string filePath)
        //{
        //    StreamReader reader = new StreamReader(filePath);
        //    string[] columns = reader.ReadLine().Split(',');
        //    string[] inputColumns = new string[]
        //    {
        //        "EulerR", "EulerP", "EulerH", "Euler_H_Diff1Sec",
        //        "LinearAccelerationX", "LinearAccelerationY", "LinearAccelerationZ",
        //        "X_MeanAdjIntegral_0.995", "Y_MeanAdjIntegral_0.995", "Z_MeanAdjIntegral_0.995",
        //        "GravityX", "GravityY", "GravityZ"
        //    };
        //    int gestureColumnIndex = Array.IndexOf(columns, "GestureNumber");
        //    int gestureCount = 9;

        //    int[] inputIndicies = new int[inputColumns.Length];
        //    for (int i = 0; i < inputColumns.Length; i++)
        //    {
        //        inputIndicies[i] = Array.IndexOf(columns, inputColumns[i]);
        //    }

        //    List<string> lines = new List<string>();
        //    List<int> lineGestureNumbers = new List<int>();
        //    while (!reader.EndOfStream)
        //    {
        //        string line = reader.ReadLine();
        //        lines.Add(line);
        //        string[] lineParts = line.Split(',');
        //        int gestureNumber = int.Parse(lineParts[gestureColumnIndex]);
        //        lineGestureNumbers.Add(gestureNumber);
        //    }

        //    // Realigned position.


        //    for (int i = 0; i < lines.Count; i++)
        //    {
        //        string[] lineParts = lines[i].Split(',');
        //        double[] parsedParts = new double[inputColumns.Length];
        //        for (int i = 0; i < inputColumns.Length; i++)
        //        {
        //            string linePart = lineParts[inputIndicies[i]];
        //            if (string.IsNullOrEmpty(linePart))
        //            {
        //                parsedParts[i] = 0.0;
        //            }
        //            else
        //            {
        //                parsedParts[i] = double.Parse(linePart);
        //            }
        //        }
        //        InputData data;
        //        data.Heading = parsedParts[Array.IndexOf(columns, "EulerH")];
        //        data.Pitch = parsedParts[Array.IndexOf(columns, "EulerP")];
        //        data.Roll = parsedParts[Array.IndexOf(columns, "EulerR")];
        //        data.LinearX = parsedParts[Array.IndexOf(columns, "LinearAccelerationX")];
        //        data.LinearY = parsedParts[Array.IndexOf(columns, "LinearAccelerationY")];
        //        data.LinearZ = parsedParts[Array.IndexOf(columns, "LinearAccelerationZ")];
        //    }
        //}


        public static List<InputDataFrame> Realign(string filePath)
        {
            var inputFrames = ReadInputFrames(filePath);
            RealignmentTransformation realignment = new RealignmentTransformation();
            var outputFrames = realignment.Transform(inputFrames);

            StreamReader reader = new StreamReader(filePath);
            StreamWriter realignedFile = new StreamWriter(filePath + ".csv.realigned", false);
            string columns = reader.ReadLine();
            columns += ",RealignedGesture,Roll,Pitch,Heading,HeadingDiff";
            realignedFile.WriteLine(columns);
            
            for (int i = 0; i < inputFrames.Count; i++)
            {
                string line = reader.ReadLine();
                line += "," + outputFrames[i].OriginalGestureNumber;
                line += "," + outputFrames[i].Roll;
                line += "," + outputFrames[i].Pitch;
                line += "," + outputFrames[i].Heading;
                line += "," + outputFrames[i].PostProcessedValues["Realignment_HeadingDifferential"];
                realignedFile.WriteLine(line);
            }
            realignedFile.Flush();
            realignedFile.Close();
            return outputFrames;
        }

        private static List<InputDataFrame> ReadInputFrames(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            string[] columns = reader.ReadLine().Split(',');
            string[] inputColumns = new string[]
            {
                "EulerR", "EulerP", "EulerH",
                "LinearAccelerationX", "LinearAccelerationY", "LinearAccelerationZ",
                "QuaternionW", "QuaternionX", "QuaternionY", "QuaternionZ",
                //"X_MeanAdjIntegral_0.995", "Y_MeanAdjIntegral_0.995", "Z_MeanAdjIntegral_0.995",
                "GravityX", "GravityY", "GravityZ", "GestureNumber"
            };
            int[] inputIndicies = new int[inputColumns.Length];
            for (int i = 0; i < inputColumns.Length; i++)
            {
                inputIndicies[i] = Array.IndexOf(columns, inputColumns[i]);
            }

            List<InputDataFrame> inputFrames = new List<InputDataFrame>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] lineParts = line.Split(',');
                double[] parsedParts = new double[inputColumns.Length];
                for (int i = 0; i < inputColumns.Length; i++)
                {
                    string linePart = lineParts[inputIndicies[i]];
                    if (string.IsNullOrEmpty(linePart))
                    {
                        parsedParts[i] = 0.0;
                    }
                    else
                    {
                        parsedParts[i] = double.Parse(linePart);
                    }
                }

                Quaternion quaterion = new Quaternion()
                {
                    w = parsedParts[Array.IndexOf(inputColumns, "QuaternionW")],
                    x = parsedParts[Array.IndexOf(inputColumns, "QuaternionX")],
                    y = parsedParts[Array.IndexOf(inputColumns, "QuaternionY")],
                    z = parsedParts[Array.IndexOf(inputColumns, "QuaternionZ")],
                };
                InputDataFrame dataFrame = new InputDataFrame
                {
                    Roll = quaterion.Roll,// parsedParts[Array.IndexOf(inputColumns, "EulerR")],
                    Pitch = quaterion.Pitch,// parsedParts[Array.IndexOf(inputColumns, "EulerP")],
                    Heading = quaterion.Yaw,//parsedParts[Array.IndexOf(inputColumns, "EulerH")],
                    QuaterionA = parsedParts[Array.IndexOf(inputColumns, "QuaternionW")],
                    QuaterionB = parsedParts[Array.IndexOf(inputColumns, "QuaternionX")],
                    QuaterionC = parsedParts[Array.IndexOf(inputColumns, "QuaternionY")],
                    QuaterionD = parsedParts[Array.IndexOf(inputColumns, "QuaternionZ")],
                    LinearX = parsedParts[Array.IndexOf(inputColumns, "LinearAccelerationX")],
                    LinearY = parsedParts[Array.IndexOf(inputColumns, "LinearAccelerationY")],
                    LinearZ = parsedParts[Array.IndexOf(inputColumns, "LinearAccelerationZ")],
                    GravityX = parsedParts[Array.IndexOf(inputColumns, "GravityX")],
                    GravityY = parsedParts[Array.IndexOf(inputColumns, "GravityY")],
                    GravityZ = parsedParts[Array.IndexOf(inputColumns, "GravityZ")],
                    OriginalGestureNumber = (int)parsedParts[Array.IndexOf(inputColumns, "GestureNumber")]
                };

                inputFrames.Add(dataFrame);
            }
            return inputFrames;
        }

        public static List<TrainingSample> Load(string filePath)
        {
            var inputFrames = Realign(filePath);

            SubsamplingTransformation subsampling = new SubsamplingTransformation();
            NormalisationTransformation standardPreprocessing = new NormalisationTransformation();
            List<TrainingSample> result = new List<TrainingSample>();

            for (int j = 0; j < 5; j++)
            {
                var processedFrames = subsampling.Transform(inputFrames);
                processedFrames = standardPreprocessing.Transform(processedFrames);

                double time = 0;
                int lastGestureNumber = 0;
                List<TrainingFrame> trainingFrames = new List<TrainingFrame>();
                for (int i = 0; i < processedFrames.Count; i++)
                {
                    InputDataFrame inputFrame = processedFrames[i];
                    double[] inputData = new double[]
                    {
                        inputFrame.Roll,
                        inputFrame.Pitch,
                        inputFrame.PostProcessedValues["HeadingDifferential"],
                        inputFrame.PostProcessedValues["RollDifferential"],
                        inputFrame.PostProcessedValues["PitchDifferential"],
                        inputFrame.LinearX,
                        inputFrame.LinearY,
                        inputFrame.LinearZ,
                        inputFrame.PostProcessedValues["LinearXIntegral"],
                        inputFrame.PostProcessedValues["LinearYIntegral"],
                        inputFrame.PostProcessedValues["LinearZIntegral"],
                        //inputFrame.GravityX,
                        //inputFrame.GravityY,
                        //inputFrame.GravityZ,
                    };

                    // Assume input data at 10Hz.
                    Frame frame = new Frame(time, time + 0.1, inputData);
                    time += 0.1;

                    double[] targets = new double[GestureCount + 1];
                    targets[inputFrame.OriginalGestureNumber] = 1.0;

                    bool isTarget = (lastGestureNumber != inputFrame.OriginalGestureNumber);
                    lastGestureNumber = inputFrame.OriginalGestureNumber;

                    TrainingFrame trainingFrame = new TrainingFrame(frame, targets, isTarget);
                    trainingFrames.Add(trainingFrame);
                }

                result.Add(new TrainingSample(trainingFrames.ToArray()));
            }
            return result;
        }


        //public static List<TrainingSample> Load(string filePath)
        //{
        //    StreamReader reader = new StreamReader(filePath);
        //    string[] columns = reader.ReadLine().Split(',');
        //    string[] inputColumns = new string[]
        //    {
        //        "EulerR", "EulerP", "Euler_H_Diff1Sec",
        //        "LinearAccelerationX", "LinearAccelerationY", "LinearAccelerationZ",
        //        "X_MeanAdjIntegral_0.995", "Y_MeanAdjIntegral_0.995", "Z_MeanAdjIntegral_0.995",
        //        "GravityX", "GravityY", "GravityZ"
        //    };
        //    int gestureColumnIndex = Array.IndexOf(columns, "GestureNumber");
        //    int gestureCount = 9;

        //    int[] inputIndicies = new int[inputColumns.Length];
        //    for (int i = 0; i < inputColumns.Length; i++)
        //    {
        //        inputIndicies[i] = Array.IndexOf(columns, inputColumns[i]);
        //    }
            
        //    List<string> lines = new List<string>();
        //    List<int> lineGestureNumbers = new List<int>();
        //    while (!reader.EndOfStream)
        //    {
        //        string line = reader.ReadLine();
        //        lines.Add(line);
        //        string[] lineParts = line.Split(',');
        //        int gestureNumber = int.Parse(lineParts[gestureColumnIndex]);
        //        lineGestureNumbers.Add(gestureNumber);
        //    }

        //    Random random = new Random(0);
        //    List<TrainingSample> result = new List<TrainingSample>();
        //    for (int s = 0; s < skipFrames.Length; s++)
        //    {
        //        //int stepSize = skipFrames[s];
        //        //int offset = s % stepSize;
        //        int stepSize = 10;
        //        int skipSpeed = 1;
        //        int framesUntilStepSizeAdjustment = 100;


        //        double time = 0;
        //        int lastGestureNumber = 0;
        //        List<TrainingFrame> trainingFrames = new List<TrainingFrame>();

        //        //while (!reader.EndOfStream)
        //        //foreach (var line in lines.Where((l, index) => (index % stepSize == offset)))
        //        for (int lineIndex = 0; lineIndex < lines.Count; )  
        //        {
        //            string[] lineParts = lines[lineIndex].Split(',');
        //            double[] parsedParts = new double[inputColumns.Length];
        //            for (int i = 0; i < inputColumns.Length; i++)
        //            {
        //                string linePart = lineParts[inputIndicies[i]];
        //                if (string.IsNullOrEmpty(linePart))
        //                {
        //                    parsedParts[i] = 0.0;
        //                }
        //                else
        //                {
        //                    parsedParts[i] = double.Parse(linePart);
        //                }
        //            }
        //            Frame frame = new Frame(time, time + 0.1, parsedParts);
        //            time += 0.1;
                
        //            int gestureNumber = int.Parse(lineParts[gestureColumnIndex]);
        //            double[] targets = new double[gestureCount];
        //            targets[gestureNumber] = 1.0;

        //            bool isTarget = (lastGestureNumber != gestureNumber);
        //            lastGestureNumber = gestureNumber;

        //            TrainingFrame trainingFrame = new TrainingFrame(frame, targets, isTarget);
        //            trainingFrames.Add(trainingFrame);

        //            // Set a new stepsize.
        //            framesUntilStepSizeAdjustment--;
        //            if (framesUntilStepSizeAdjustment <= 0 && gestureNumber == 0)
        //            {
        //                framesUntilStepSizeAdjustment = 100;
        //                stepSize = skipFrames[random.Next(0, skipFrames.Length)];
        //            }

        //            // If current gesture is nil, and not immediately near gesture, go faster.
        //            if (gestureNumber == 0 && (lineIndex > (3 * stepSize)) && (lineIndex + (3 * stepSize) < lines.Count)
        //                && lineGestureNumbers[lineIndex - stepSize * 3] == 0 && lineGestureNumbers[lineIndex + stepSize * 3] == 0)
        //            {
        //                lineIndex += stepSize * skipSpeed;
        //            }
        //            else
        //            {
        //                skipSpeed = random.Next(1, 4);
        //                lineIndex += stepSize;
        //            }

        //            //lineIndex += stepSize;
        //        }

        //        //TrainingSample sample = new TrainingSample(trainingFrames.Where((f, index) => (index % stepSize) == offset).ToArray());
        //        result.Add(new TrainingSample(trainingFrames.ToArray()));
        //    }

        //    return result;
        //}
    }
}
