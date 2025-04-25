using System;
using System.Drawing;
using SkiaSharp;
class Program
{
    static void Main(String[] Args)
    {
        if (Args.Length < 1)
        {
            Console.WriteLine("Please provide this format:\n[File Path]");
            return;
        }
        using var input = File.OpenRead(Args[0]);
        using var bitmap = SKBitmap.Decode(input);
        Console.WriteLine("Starting Processing");
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
                var newPix = new SKColor((byte)avg,(byte)avg,(byte)avg);
                bitmap.SetPixel(x,y,newPix);
            }
        }
        var img = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes("processed-img.png", img.ToArray());
    }
}