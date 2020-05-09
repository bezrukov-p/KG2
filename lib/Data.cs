using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib
{
    public class Data
    {
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }

        public short G(int x, int y, int z) => mem[x, y, z];

        short[,,] mem;

        public Data() { }

        public void Load(string path)
        {
            if (!File.Exists(path))
                return;


            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                int x = br.ReadInt32();
                int y = br.ReadInt32();
                int z = br.ReadInt32();

                float scaleX = BitConverter.ToSingle(br.ReadBytes(4),0);
                float scaleY = BitConverter.ToSingle(br.ReadBytes(4), 0);
                float scaleZ = BitConverter.ToSingle(br.ReadBytes(4), 0);

                SizeX = x;
                SizeY = y;
                SizeZ = z;

                mem = new short[x, y, z];
                for(int i = 0; i < x*y*z; ++i)
                {
                    mem[i % (x), ((i) / x) % y, i / (x * y)] = br.ReadInt16();
                }

                Console.WriteLine("Loaded!");

            }            

        }
    }
}
