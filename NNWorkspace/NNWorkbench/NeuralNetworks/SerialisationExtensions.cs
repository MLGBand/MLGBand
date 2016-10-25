using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    static class SerialisationExtensions
    {
        public static void Write(this BinaryWriter writer, double[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(array[i]);
            }
        }

        public static void Write(this BinaryWriter writer, double[,] array)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    writer.Write(array[x, y]);
                }
            }
        }

        public static void Read(this BinaryReader reader, double[] array)
        {
            for (int i =0; i < array.Length; i++)
            {
                array[i] = reader.ReadDouble();
            }
        }

        public static void Read(this BinaryReader reader, double[,] array)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = reader.ReadDouble();
                }
            }
        }
    }
}
