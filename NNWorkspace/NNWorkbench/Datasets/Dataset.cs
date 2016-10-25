using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Datasets
{
    public abstract class Dataset
    {
        private List<TrainingSample> trainingSamples;
        private List<TrainingSample> validationSamples;
        private List<TrainingSample> testingSamples;

        protected Dataset()
        {
            trainingSamples = new List<TrainingSample>();
            validationSamples = new List<TrainingSample>();
            testingSamples = new List<TrainingSample>();
        }

        protected void Build(string path)
        {
            Directory.CreateDirectory(path);

            //List<TrainingSample> samples = new List<TrainingSample>();
            foreach (var trackInfo in GetFiles())
            {
                if (trackInfo.Contains("TEST_")) {
                    testingSamples.AddRange(GetSample(path, trackInfo));
                }
                else if (trackInfo.Contains("VALIDATE_"))
                {
                    validationSamples.AddRange(GetSample(path, trackInfo));
                }
                else if (trackInfo.Contains("TRAIN_"))
                {
                    trainingSamples.AddRange(GetSample(path, trackInfo));
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
//                samples.Add(GetSample(path, trackInfo));
            }

            Randomise(trainingSamples);
            Randomise(validationSamples);
            Randomise(testingSamples);

            //var samplesByLength = samples.OrderByDescending(x => x.Frames.Length).ToList();
            //Debug.Assert(samplesByLength.Count == 4);
            //trainingSamples.Add(samplesByLength[0]);
            //trainingSamples.Add(samplesByLength[1]);
            //testingSamples.Add(samplesByLength[2]);
            //validationSamples.Add(samplesByLength[3]);


            //Randomise(samples);
            //for (int i = 0; i < samples.Count; i++)
            //{
            //    int bin = i % 20;
            //    if (bin < 15)
            //    {
            //        trainingSamples.Add(samples[i]);
            //    }
            //    else if (bin < 18)
            //    {
            //        validationSamples.Add(samples[i]);
            //    }
            //    else
            //    {
            //        testingSamples.Add(samples[i]);
            //    }
            //}
        }

        private static void Randomise(List<TrainingSample> samples)
        {
            Random random = new Random(0);
            for (int i = 0; i < samples.Count; i++)
            {
                int newLocation = random.Next(samples.Count);
                TrainingSample temp = samples[newLocation];
                samples[newLocation] = samples[i];
                samples[i] = temp;
            }
        }

        private List<TrainingSample> GetSample(string directory, string trackName)
        {
            string datasetFilePath = Path.Combine(directory, trackName);

            List<TrainingSample> results = new List<TrainingSample>();
            for (int i = 0; ; i++)
            {
                string filePath = datasetFilePath + "_" + i + ".trainingsample";
                if (File.Exists(filePath)) {
                    results.Add(TrainingSample.Load(filePath));
                } else {
                    break;
                }
            }

            if (results.Count == 0)
            {
                results = BuildSample(trackName);
                for (int i = 0; i < results.Count; i++)
                {
                    string filePath = datasetFilePath + "_" + i + ".trainingsample";
                    results[i].Save(filePath);
                }
                
            }
            return results;
        }

        public abstract IEnumerable<string> GetFiles();

        protected abstract List<TrainingSample> BuildSample(string trackName);

        public IList<TrainingSample> TrainingSamples
        {
            get { return trainingSamples; }
        }

        public IList<TrainingSample> ValidationSamples
        {
            get { return validationSamples; }
        }

        public IList<TrainingSample> TestingSamples
        {
            get { return testingSamples; }
        }
    }
}
