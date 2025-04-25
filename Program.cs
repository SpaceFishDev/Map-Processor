using System;
using System.Drawing;
using SkiaSharp;
using System.Runtime.InteropServices;
using System.Diagnostics;
class Program
{
    static SKBitmap bitmap;
    static byte[] bitmap_result;
    static void ReadImage(string path){
        var input = File.OpenRead(path);
        bitmap = SKBitmap.Decode(input);
        Console.WriteLine("Starting Processing");
        bitmap_result = new Byte[4*bitmap.Width*bitmap.Height];
    }
    static void ProcessNormal(){
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
    static void ProcessScaled(int scale){
        bitmap_result = new byte[scale*scale*bitmap.Width*bitmap.Height*4];
        int idx_x = 0;
        int idx_y = 0;
        Random rnd = new();
        for(int x = 0; x < bitmap.Width; ++x)
        {
            if(x % 100 == 0){
                Console.WriteLine($"{(float)x/(float)bitmap.Width*100.0f}%");
            }
            for(int y = 0; y < bitmap.Height; ++y)
            {
                    var pixelColor = bitmap.GetPixel(x,y);
                    float avg = pixelColor.Red + pixelColor.Green;
                    avg /= 2;
                    avg = 255-avg;
                    for(int i = 0; i < scale; ++i)
                    {
                        for(int j = 0; j < scale; ++j)
                        {
                            float random = (float)rnd.NextDouble();
                            random *= 5;

                            float n = random+avg;
                            if(avg == 0){
                                n = 0;
                            }
                            int scaleX = scale*x + i;
                            int scaleY = scale*y + j;
                            int scaleWidth = bitmap.Width*scale;
                            int idx = 4*(scaleX + (scaleY*scaleWidth));
                            n = Math.Clamp(n, 0,255);
                            bitmap_result[idx + 0] = (byte)n;
                            bitmap_result[idx + 1] = (byte)n;
                            bitmap_result[idx + 2] = (byte)n;
                            bitmap_result[idx + 3] = 255;
                        }
                    }
            }
        }
        var gcHandle = GCHandle.Alloc(bitmap_result, GCHandleType.Pinned);
        var info = new SKImageInfo(bitmap.Width*scale, bitmap.Height*scale, SKImageInfo.PlatformColorType, SKAlphaType.Opaque);
        bitmap.InstallPixels(info, gcHandle.AddrOfPinnedObject(),info.RowBytes, null);
        gcHandle.Free();
        var img = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes("processed-img.png", img.ToArray());
    }
    static void Process(int scale = 0){
        if(scale < 2){
            ProcessNormal();
            return;
        }
        ProcessScaled(scale);
    }
    static void Main(String[] Args)
    {
        if (Args.Length < 1)
        {
            Console.WriteLine("Please provide this format:\n[File Path] or [File Path] [scale (scale scales the image by any integer >= 2)]\nNote: The scale is artificial and adds some randomness.");
            return;
        }
        ReadImage(Args[0]);
        if(Args.Length == 2){
            Process(int.Parse(Args[1]));
            return;
        }
        Process();

    
    }
}