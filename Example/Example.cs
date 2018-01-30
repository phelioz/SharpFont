using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SharpFont;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Bmp;
using System.Numerics;

namespace Example
{
    class Example
    {
        public void Run()
        {
            var font = new FontFace(File.OpenRead("Fonts/OpenSans-Regular.ttf"));

            for(int i = 33; i < 127; i++)
            {
                var surface = RenderSurface((char)i, font);

                var image = SaveAsImage(surface);

                image.Save("char" + i + ".png");
            }
        }

        public unsafe Surface RenderSurface(char c, FontFace font)
        {
            var glyph = font.GetGlyph(c, 32);
            var surface = new Surface
            {
                Bits = Marshal.AllocHGlobal(glyph.RenderWidth * glyph.RenderHeight),
                Width = glyph.RenderWidth,
                Height = glyph.RenderHeight,
                Pitch = glyph.RenderWidth
            };

            var stuff = (byte*)surface.Bits;
            for (int i = 0; i < surface.Width * surface.Height; i++)
                *stuff++ = 0;

            glyph.RenderTo(surface);

            return surface;
        }

        private Image<Rgb24> SaveAsImage(Surface surface)
        {
            int width = surface.Width;
            int height = surface.Height;
            int len = width * height;
            byte[] data = new byte[len];
            Marshal.Copy(surface.Bits, data, 0, len);
            byte[] pixels = new byte[len * 3];

            int index = 0;
            for(int i = 0; i < len; i++)
            {
                byte c = data[i];
                pixels[index++] = c;
                pixels[index++] = c;
                pixels[index++] = c;
            }

            return Image.LoadPixelData<Rgb24>(pixels, width, height);
        }
    }
}
