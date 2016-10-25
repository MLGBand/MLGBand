using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Preprocessing
{
    class InputDataFrame
    {
        public InputDataFrame()
        {
            PostProcessedValues = new Dictionary<string, double>();
        }

        public double Heading;
        public double Pitch;
        public double Roll;
        public double QuaterionA;
        public double QuaterionB;
        public double QuaterionC;
        public double QuaterionD;
        public double LinearX;
        public double LinearY;
        public double LinearZ;
        public double GravityX;
        public double GravityY;
        public double GravityZ;
        public int OriginalGestureNumber;

        public Dictionary<string, double> PostProcessedValues;        

        public InputDataFrame Clone()
        {
            return new InputDataFrame
            {
                Heading = Heading,
                Pitch = Pitch,
                Roll = Roll,
                LinearX = LinearX,
                LinearY = LinearY,
                LinearZ = LinearZ,
                GravityX = GravityX,
                GravityY = GravityY,
                GravityZ = GravityZ,
                OriginalGestureNumber = OriginalGestureNumber,
                PostProcessedValues = new Dictionary<string, double>(PostProcessedValues)
            };
        }
    }
}
