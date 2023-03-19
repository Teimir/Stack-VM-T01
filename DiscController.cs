using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    class DiscController
    {

        private const int PageSize = 256;
        private const int PageCount = 256;

        private readonly string _filename;

        public DiscController(string filename)
        {
            _filename = filename;
        }

        public byte ReadFromDisk(int page, int offset)
        {
            using (var fs = new FileStream(_filename, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(page * PageSize + offset, SeekOrigin.Begin);
                return (byte)fs.ReadByte();
            }
        }

        public void WriteToDisk(int page, int offset, byte data)
        {
            using (var fs = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Seek(page * PageSize + offset, SeekOrigin.Begin);
                fs.WriteByte(data);
            }
        }

        public void GenerateFile()
        {
            using (var fs = new FileStream(_filename, FileMode.Create, FileAccess.Write))
            {
                var data = new byte[PageSize * PageCount];
                for (int i = 0; i < PageSize * PageCount; i++)
                {
                    data[i] = 0x30;
                }
                fs.Write(data, 0, data.Length);
            }
        }

    }
}