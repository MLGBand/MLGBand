using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Datasets
{
    public class Frame
    {
        public Frame(double start, double end, double[] values)
        {
            this.Start = start;
            this.End = end;
            this.Values = values;
        }

        public readonly double Start;
        public readonly double End;
        public readonly double[] Values;

        public void Write(BinaryWriter writer)
        {
            writer.Write(Start);
            writer.Write(End);
            writer.Write((ushort)Values.Length);
            for (int i = 0; i < Values.Length; i++)
            {
                writer.Write(Values[i]);
            }
        }

        public static Frame Read(BinaryReader reader)
        {
            double start = reader.ReadDouble();
            double end = reader.ReadDouble();
            int length = reader.ReadUInt16();
            double[] values = new double[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = reader.ReadDouble();
            }
            return new Frame(start, end, values);
        }

        public double Energy
        {
            get
            {
                double energy = 0.0;
                for (int i = 0; i < Values.Length; i++)
                {
                    energy += Math.Exp(Values[i]) - 1.0;
                }
                return energy;
            }
        }

        public override string ToString()
        {
            return String.Format("{0:0.0000}:{1:0.0000}", Energy, Values.Average());
        }
    }
}
