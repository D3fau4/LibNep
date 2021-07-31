using System;
using System.Collections.Generic;
using Yarhl.IO;
using System.IO;

namespace LibNep.TID
{
    class TID
    {
        static byte[] magic = { 0x54, 0x49, 0x44 };
        DataReader reader;
        public TID(string path)
        {
            var stream = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            reader = new DataReader(stream);
            load();
        }

        public TID(Stream file)
        {
            var stream = DataStreamFactory.FromStream(file);
            reader = new DataReader(stream);
            load();
        }
        public TID(DataStream stream)
        {
            reader = new DataReader(stream);
            load();
        }

        internal void load()
        {
            var _magic = reader.ReadBytes(3);
            if (!magic.Equals(_magic))
                throw new Exception("Sistema de archivos no reconocido");
        }
    }
}
