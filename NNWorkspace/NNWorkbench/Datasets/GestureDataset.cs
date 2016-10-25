using NNWorkbench.Preprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Datasets
{
    class GestureDataset : Dataset
    {
        public override IEnumerable<string> GetFiles()
        {
            string beatsDirectory = Path.Combine(Environment.CurrentDirectory, Paths.GestureDirectory);
            var files = new DirectoryInfo(beatsDirectory).EnumerateFiles("*.csv", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                yield return file.Name.Substring(0, file.Name.Length - file.Extension.Length);
            }
        }

        public static IEnumerable<string> GetFileNames()
        {
            return new GestureDataset().GetFiles();
        }

        protected override List<TrainingSample> BuildSample(string trackName)
        {
            Console.WriteLine("Processing {0}...", trackName);
            string filePath = Path.Combine(Environment.CurrentDirectory, Paths.GestureDirectory, trackName + ".csv");
            return GesturePreprocessor.Load(filePath);
        }

        public static GestureDataset Load()
        {
            GestureDataset result = new GestureDataset();
            result.Build(Paths.GestureDatasetDirectory);
            return result;
        }
    }
}
