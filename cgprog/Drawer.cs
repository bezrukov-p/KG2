using lib;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cgprog
{
    class Drawer
    {
        public Drawer(Data d) {
            Data = d;
            loaded = new bool[d.SizeZ];


            vboTexture = Enumerable.Range(0, d.SizeZ).ToArray();
        }

        public Data Data { get; } 
        int Clamp(int val)
        {
            return Math.Max(0, Math.Min(val, 255));
        }

        bool[] loaded;
        int[] vboTexture;

        int vmin = 0;
        public int VMin
        {
            get => vmin;
            set
            {
                vmin = value;

                loaded = new bool[Data.SizeZ];
                vboTexture = Enumerable.Range(0, Data.SizeZ).ToArray();

            }
        }

        int vmax = 2000;
        public int VMax
        {
            get => vmax;
            set
            {
                vmax = value;

                loaded = new bool[Data.SizeZ];
                vboTexture = Enumerable.Range(0, Data.SizeZ).ToArray();
            }
        }

        public Color TransferFunction(short val)
        {
            var c = Clamp((val - VMin) * 255 / (VMax - VMin));
            return Color.FromArgb(c, c, c);
        }

        public void Init(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Data.SizeX, 0, Data.SizeY, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        public void DrawQuadStrips(int z)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);            

            for (int y = 0; y < Data.SizeY -1; ++y)
            {
                GL.Begin(BeginMode.QuadStrip);

                GL.Color3(TransferFunction(Data.G(0, y+1, z)));
                GL.Vertex2(0, y+1);

                GL.Color3(TransferFunction(Data.G(0, y, z)));
                GL.Vertex2(0, y);

                for (int x = 1; x < Data.SizeX - 1; ++x)
                {                 

                    GL.Color3(TransferFunction(Data.G(x, y, z)));
                    GL.Vertex2(x, y);

                    GL.Color3(TransferFunction(Data.G(x, y+1, z)));
                    GL.Vertex2(x, y+1);
                }

                GL.End();
            }            
        }

        public int Mode { get; set; } = 0;

        public void Draw(int z)
        {
            if (Mode == 0)
                DrawQuads(z);

            if (Mode == 1)
                DrawQuadStrips(z);

            if (Mode == 2)
                DrawTexture(z);
        }

        public void DrawQuads(int z)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);

            for (int x = 0; x < Data.SizeX - 1; ++x)
            {
                for (int y = 0; y < Data.SizeY - 1; ++y)
                {
                    GL.Color3(TransferFunction(Data.G(x, y, z)));
                    GL.Vertex2(x, y);

                    GL.Color3(TransferFunction(Data.G(x, y+1, z)));
                    GL.Vertex2(x, y+1);


                    GL.Color3(TransferFunction(Data.G(x+1, y+1, z)));
                    GL.Vertex2(x+1, y+1);

                    GL.Color3(TransferFunction(Data.G(x+1, y, z)));
                    GL.Vertex2(x+1, y);
                }

               
            }

            GL.End();
        }
        public void DrawTexture(int z)
        {
            if (!loaded[z])
            {

                Bitmap texture = new Bitmap(Data.SizeX, Data.SizeY);
                for (int i = 0; i < Data.SizeX; ++i)
                {
                    for (int j = 0; j < Data.SizeY; ++j)
                    {
                        texture.SetPixel(i, j, TransferFunction(Data.G(i, j, z)));
                    }
                }

                GL.BindTexture(TextureTarget.Texture2D, vboTexture[z]);
                var data = texture.LockBits(
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                    );

                GL.TexImage2D(
                    TextureTarget.Texture2D, 
                    0, 
                    PixelInternalFormat.Rgba, 
                    data.Width, data.Height, 
                    0, 
                    PixelFormat.Bgra, 
                    PixelType.UnsignedByte, 
                    data.Scan0
                    );

                texture.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

                loaded[z] = true;
            }


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, vboTexture[z]);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);

            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Data.SizeY);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Data.SizeX, Data.SizeY);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Data.SizeX, 0);

            GL.End();

            GL.Disable(EnableCap.Texture2D);

        }
    }
}
