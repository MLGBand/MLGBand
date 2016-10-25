using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Represents a sensor data sample recieved from the gesture band.
    /// </summary>
    class Sample
    {
        public long Timestamp { get; private set; }
        public double EulerH { get; private set; }
        public double EulerR { get; private set; }
        public double EulerP { get; private set; }
        public double QuaternionW { get; private set; }
        public double QuaternionX { get; private set; }
        public double QuaternionY { get; private set; }
        public double QuaternionZ { get; private set; }
        public double LinearAccelerationX { get; private set; }
        public double LinearAccelerationY { get; private set; }
        public double LinearAccelerationZ { get; private set; }
        public double GravityX { get; private set; }
        public double GravityY { get; private set; }
        public double GravityZ { get; private set; }
        public int TemperatureInCelcius { get; private set; }
        public int SystemCalibration { get; private set; }
        public int GyroCalibration { get; private set; }
        public int AccelerometerCalibration { get; private set; }
        public int MagnotometerCalibration { get; private set; }
        public double BatteryLevel { get; private set; }
        public int PredictedClass { get; private set; }
        public double[] GestureProbibilities { get; private set; }
        
        public SampleTimings Timings { get; internal set; }

        public static Sample Deserialise(byte[] data)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data));
            double eulerScale = 1.0 / 16.0; // 16 LSB = 1 degree.
            double quaternionScale = 1.0 / (1 << 14); // As per bosch doco
            double gravityScale = 1.0 / 100.0;
            var sample = new Sample
            {
                Timestamp = reader.ReadUInt32(),
                EulerH = reader.ReadInt16() * eulerScale,
                EulerR = reader.ReadInt16() * eulerScale,
                EulerP = reader.ReadInt16() * eulerScale,
                QuaternionW = reader.ReadInt16() * quaternionScale,
                QuaternionX = reader.ReadInt16() * quaternionScale,
                QuaternionY = reader.ReadInt16() * quaternionScale,
                QuaternionZ = reader.ReadInt16() * quaternionScale,
                LinearAccelerationX = reader.ReadInt16() * gravityScale,
                LinearAccelerationY = reader.ReadInt16() * gravityScale,
                LinearAccelerationZ = reader.ReadInt16() * gravityScale,
                GravityX = reader.ReadInt16() * gravityScale,
                GravityY = reader.ReadInt16() * gravityScale,
                GravityZ = reader.ReadInt16() * gravityScale,
                TemperatureInCelcius = reader.ReadSByte(),
            };

            int calibrationStatus = reader.ReadByte();
            sample.SystemCalibration = (calibrationStatus >> 6) & 0x03;
            sample.GyroCalibration = (calibrationStatus >> 4) & 0x03;
            sample.AccelerometerCalibration = (calibrationStatus >> 2) & 0x03;
            sample.MagnotometerCalibration = (calibrationStatus) & 0x03;
            sample.BatteryLevel = reader.ReadUInt16();
            sample.PredictedClass = reader.ReadByte();

            // Read and discard byte to ensure stream alignment.
            reader.ReadByte();

            sample.GestureProbibilities = new double[9];
            for (int i = 0; i <sample.GestureProbibilities.Length; i++)
            {
                sample.GestureProbibilities[i] = reader.ReadSingle();
            }
            return sample;
        }

        public string ToString()
        {
            return string.Format("L(X):{0,6:0.00}  L(Y):{1,6:0.00}  L(Z):{2,6:0.00}  E(X):{3,4:0}   E(Y):{4,4:0}   E(Z):{5,4:0}   T:{6,2:0}  Cal(S,G,A,M): {7},{8},{9},{10}  P:{11}",
                LinearAccelerationX, LinearAccelerationY, LinearAccelerationZ, EulerH, EulerR, EulerP, TemperatureInCelcius, SystemCalibration, GyroCalibration, AccelerometerCalibration, MagnotometerCalibration, PredictedClass);
        }

        public const string FileHeaders = "Timestamp,EulerH,EulerR,EulerP,"
            + "QuaternionW,QuaternionX,QuaternionY,QuaternionZ,"
            + "LinearAccelerationX,LinearAccelerationY,LinearAccelerationZ,"
            + "GravityX,GravityY,GravityZ,"
            + "TemperatureInCelcius,SystemCalibration,GyroCalibration,AccelerometerCalibration,MagnotometerCalibration,"
            + "BatteryLevel,Received,Skipped";

        public string ToFileString()
        {
            return string.Format(
                                // Timestamp + Euler
                                "{0},{1},{2},{3}," +
                                // Quaternion
                                "{4},{5},{6},{7}," +
                                // Linear Acceleration
                                "{8},{9},{10}," +
                                // Gravity
                                "{11},{12},{13}," +
                                // Temperature
                                "{14}," +
                                // Calibration
                                "{15},{16},{17},{18}," +
                                // Battery level
                                "{19}," +
                                // Samples recieved and skipped
                                "{20},{21}",
                                Timestamp, EulerH, EulerR, EulerP,
                                QuaternionW, QuaternionX, QuaternionY, QuaternionZ,
                                LinearAccelerationX, LinearAccelerationY, LinearAccelerationZ,
                                GravityX, GravityY, GravityZ,
                                TemperatureInCelcius,
                                SystemCalibration, GyroCalibration, AccelerometerCalibration, MagnotometerCalibration,
                                BatteryLevel,
                                Timings.SamplesReceived, Timings.SamplesSkipped);
        }
    }
}
