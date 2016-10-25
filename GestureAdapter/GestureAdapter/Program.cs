using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestureAdapter
{
    /// <summary>
    /// A program for collecting neural network training data. To be used in conjunction with
    /// the DataCollection Arduino application.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        { 
            if (!args.Contains("-noprompts"))
            {
                DataCollectionForm.ShowPrompts();
            }
            
            Console.WriteLine("Enter a name for the data collection:");
            string name = Console.ReadLine();

            outputFile = new DataLogger(name, 100);
            outputFile.CustomColumns.AddRange(CustomColumns);
            lowerFrequencyFile = new DataLogger(name, 10);
            lowerFrequencyFile.CustomColumns.AddRange(CustomColumns);

            DeviceConnection device = new DeviceConnection();
            device.Connect("192.168.4.1");
            device.SampleReceived += Device_SampleReceived;
                       
            while (device.Connected)
            {
                while (!device.DataAvailable) Thread.Sleep(1);
                device.Read();
            }
        }

        private static void Device_SampleReceived(object sender, SampleReceivedEventArgs e)
        {
            var sample = e.Sample;
            outputFile.WriteToFile(sample);
            lowerFrequencyFile.WriteToFile(sample);
            Console.WriteLine(sample.ToString());
            //RunNeuralNetwork(sample);
        }

        static DataCollectionForm promptUser;
        static DataLogger outputFile;
        static DataLogger lowerFrequencyFile;
        static NeuralNetwork network = new NeuralNetwork();

        static readonly List<IDataLoggerColumn> CustomColumns = new List<IDataLoggerColumn>() {
            new AccelerationIntegralColumn("X_MeanAdjIntegral_0.995", 0.995, s => s.LinearAccelerationX),
            new AccelerationIntegralColumn("Y_MeanAdjIntegral_0.995", 0.995, s => s.LinearAccelerationY),
            new AccelerationIntegralColumn("Z_MeanAdjIntegral_0.995", 0.995, s => s.LinearAccelerationZ),
            new HeadingDifferentialColumn("Euler_H_Diff1Sec", 100),
            new HeadingDifferentialColumn("Euler_H_Diff2Sec", 200),
            new HeadingDifferentialColumn("Euler_H_Diff3Sec", 300),

            new LambdaColumn("GestureNumber", s => ((int)DataCollectionForm.GetGestureAt(s.Timings.Time)).ToString() ),
            new LambdaColumn("GestureName", s => (DataCollectionForm.GetGestureAt(s.Timings.Time)).ToString() ),
            new LambdaColumn("Bad", s => DataCollectionForm.IsDataBad() ? "Bad" : "OK")
            };
        
        static void RunNeuralNetwork(Sample sample)
        {
            // Run Neural Network at 10Hz.
            if (sample.Timings.SamplesReceived % 10 != 0) return;

            Console.WindowWidth = 160;
            network.Step(new double[] { sample.EulerP, sample.EulerR, sample.LinearAccelerationX,
                sample.LinearAccelerationY, sample.LinearAccelerationZ, sample.GravityX, sample.GravityY, sample.GravityZ });

            // Print Neural Network outputs, with the highest in white.
            for (int i = 0; i < network.Outputs.Length; i++)
            {
                if (network.Outputs[i] == network.Outputs.Max())
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.Write("{0,6:0.00} ", network.Outputs[i]);
            }
            Console.WriteLine();

        }
    }
}
