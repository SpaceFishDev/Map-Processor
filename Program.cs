using System;
using System.Drawing;
using SkiaSharp;
using System.Runtime.InteropServices;
class Program
{
    static void Main(String[] Args)
    {
        if (Args.Length < 1)
        {
            Console.WriteLine("Please provide this format:\n[File Path]");
            return;
        }
        var input = File.OpenRead(Args[0]);
        Console.WriteLine(input.GetType());
        var bitmap = SKBitmap.Decode(input);
        Console.WriteLine("Starting Processing");
        Byte[] bitmap_result = new Byte[4*bitmap.Width*bitmap.Height];
    
        for(int x = 0; x < bitmap.Width; x++)
        {
            if(x % 100 == 0){
                Console.WriteLine($"{(float)x/(float)bitmap.Width*100.0f}%");
            }
            for(int y = 0; y < bitmap.Height; y++)
            {
                var pixelColor = bitmap.GetPixel(x,y);
                float avg = pixelColor.Red + pixelColor.Green;
                avg /= 2;
                avg = 255-avg;
                byte col = (byte)avg;
                int idx = 4*(x+(y*bitmap.Width));
                bitmap_result[idx+0] = col;
                bitmap_result[idx+1] = col;
                bitmap_result[idx+2] = col;
                bitmap_result[idx+3] = 255;
            }
        }
        var gcHandle = GCHandle.Alloc(bitmap_result, GCHandleType.Pinned);
        var info = new SKImageInfo(bitmap.Width, bitmap.Height, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);
        bitmap.InstallPixels(info, gcHandle.AddrOfPinnedObject(),info.RowBytes, null);
        gcHandle.Free();
        var img = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes("processed-img.png", img.ToArray());
    }
}