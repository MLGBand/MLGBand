using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Logs samples to a CSV file for later analysis.
    /// </summary>
    class DataLogger : IDisposable
    {
        public DataLogger(string name, int frequencyInHz)
        {
            period = Math.Max(1, 100 / frequencyInHz);
            logFile = new StreamWriter(name + "_" + frequencyInHz + "Hz_" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".csv", false);
            CustomColumns = new List<IDataLoggerColumn>();

            WriteFileHeaders();
        }

        // How many frames (including the sampled frame) between sampled frames.
        int period;
        StreamWriter logFile;

        private void WriteFileHeaders()
        {
            string headers = Sample.FileHeaders;
            foreach (var customColumn in CustomColumns)
            {
                headers += "," + customColumn.Name;
            }
            logFile.WriteLine(headers);
        }

        public void WriteToFile(Sample sample)
        {
            if (sample.Timings.SamplesReceived % period != 0) return;

            string line = sample.ToFileString();
            foreach (var customColumn in CustomColumns)
            {
                line += "," + customColumn.GetValue(sample);
            }
            logFile.WriteLine(line);
        }

        public List<IDataLoggerColumn> CustomColumns { get; private set; }
        
        public void Dispose()
        {
            logFile.Dispose();
        }
    }
}
