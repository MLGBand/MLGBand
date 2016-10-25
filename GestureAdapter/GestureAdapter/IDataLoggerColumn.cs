using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureAdapter
{
    /// <summary>
    /// Represents a custom data logger column.
    /// </summary>
    interface IDataLoggerColumn
    {
        string Name { get;  }

        string GetValue(Sample sample);
    }
}
