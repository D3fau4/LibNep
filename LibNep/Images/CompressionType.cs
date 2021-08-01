using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNep.FileFormats
{
    public enum CompressionType
    {
        None = 0,
        DTX1 = 0x31545844,
        DTX5 = 0x35545844,
    }
}
