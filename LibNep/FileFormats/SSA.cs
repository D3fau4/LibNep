using System;
using System.IO;
using Yarhl.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNep.FileFormats
{
    public class SSA
    {
        static byte[] magic = { 0x53, 0x53, 0x41, 0x44 };
        DataReader reader;

        public SSA(string path)
        {
            var stream = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            reader = new DataReader(stream);
            throw new NotImplementedException();
        }
        public SSA(Stream stream)
        {
            var stream2 = DataStreamFactory.FromStream(stream);
            reader = new DataReader(stream2);
            throw new NotImplementedException();
        }
        public SSA(DataStream stream)
        {
            reader = new DataReader(stream);
            throw new NotImplementedException();
        }

        internal void load()
        {

        }
    }
}
