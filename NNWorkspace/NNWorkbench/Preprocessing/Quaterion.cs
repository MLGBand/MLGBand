using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.Preprocessing
{
    struct Quaternion
    {
        public double w;
        public double x;
        public double y;
        public double z;

        private const double RadiansToDegrees = 180.0 / Math.PI;

        // Body z-x-y
        // Quaternion to Euler rotation conversion.
        // i.e. yaw-pitch-roll, because x is our pitch axis and y is our roll.
        // (This is equivalent to the system-fixed rotation Y-X-Z.)
        //
        // If you need to change this conversion, refer to:
        // http://stackoverflow.com/questions/11103683/euler-angle-to-quaternion-then-quaternion-to-euler-angle

        // Rotate around x-axis. 
        public double Roll
        {
            get
            {
                return Math.Atan2(-2 * (x * z - w * y), w * w - x * x - y * y + z * z);
            }
        }

        public double RollInDegrees
        {
            get { return Roll * RadiansToDegrees; }
        }

        // Rotation around y-axis.
        public double Pitch
        {
            get
            {
                return Math.Asin(2 * (y * z + w * x)); 
            }
        }

        public double PitchInDegrees
        {
            get { return Pitch * RadiansToDegrees; }
        }

        // Rotation around z-axis
        public double Yaw
        {
            get
            {
                return Math.Atan2(-2 * (x * y - w * z), w * w - x * x + y * y - z * z); 
            }
        }

        public double YawInDegrees
        {
            get { return Yaw * RadiansToDegrees; }
        }

    }
}
