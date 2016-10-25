using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Datasets
{
    public class TrainingSample
    {
        public TrainingSample(IEnumerable<Frame> frames, IEnumerable<double> onsets)
        {
            Frames = frames.Select(frame =>
            {
                bool anyOnset = onsets.Any(onset => frame.Start <= (onset + 441.0 / 44100) && (onset + 441.0 / 44100) < frame.End);
                return new TrainingFrame(frame, anyOnset);
            }
            ).ToArray();
        }

        public TrainingSample(TrainingFrame[] frames)
        {
            this.Frames = frames;
        }

        public readonly TrainingFrame[] Frames;

        public void Save(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Frames.Length);
                foreach (TrainingFrame frame in Frames)
                {
                    frame.Write(writer);
                }
            }
        }

        public static TrainingSample Load(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int length = reader.ReadInt32();
                TrainingFrame[] frames = new TrainingFrame[length];
                for (int i = 0; i < length; i++)
                {
                    frames[i] = TrainingFrame.Read(reader);
                }
                return new TrainingSample(frames);
            }
        }

    }
}
