using System;
using System.IO;
using Yarhl.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

namespace LibNep.FileFormats
{
    public class CL3
    {
        static byte[] magic = { 0x43, 0x4C, 0x33 };
        DataReader reader;

        public CL3(string path)
        {
            var stream = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            reader = new DataReader(stream);
            throw new NotImplementedException();
        }
        public CL3(Stream stream)
        {
            var stream2 = DataStreamFactory.FromStream(stream);
            reader = new DataReader(stream2);
            throw new NotImplementedException();
        }
        public CL3(DataStream stream)
        {
            reader = new DataReader(stream);
            throw new NotImplementedException();
        }

        internal void load()
        {

        }
    }
}
