using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Datasets
{
    public class TrainingFrame
    {
        public TrainingFrame(Frame frame, bool isOnset)
        {
            this.Frame = frame;
            this.ExpectedOutputs = new double[1] { isOnset ? 1.0 : 0.0 };
            this.IsTarget = isOnset;
        }


        public TrainingFrame(Frame frame, double[] expectedOutputs, bool isTarget)
        {
            this.Frame = frame;
            this.ExpectedOutputs = expectedOutputs;
            this.IsTarget = isTarget;
        }

        public readonly Frame Frame;
        public readonly double[] ExpectedOutputs;
        public readonly bool IsTarget;

        //public readonly bool IsOnset;

        public void Write(BinaryWriter writer)
        {
            Frame.Write(writer);
            writer.Write(ExpectedOutputs.Length);
            for (int i = 0; i < ExpectedOutputs.Length; i++)
            {
                writer.Write(ExpectedOutputs[i]);
            }
            writer.Write(IsTarget);
        }

        public static TrainingFrame Read(BinaryReader reader)
        {
            Frame frame = Frame.Read(reader);


            int count = reader.ReadInt32();
            double[] expectedOutputs = new double[count];
            for (int i = 0; i < count; i++)
            {
                expectedOutputs[i] = reader.ReadDouble();
            }
            bool isTarget = reader.ReadBoolean();
            
            //frame.Values[0] = isOnset ? 1.0 : 0.0;
            return new TrainingFrame(frame, expectedOutputs, isTarget);
        }

        public override string ToString()
        {
            return String.Format("{0},{1}", Frame, IsTarget);
        }
    }
}
