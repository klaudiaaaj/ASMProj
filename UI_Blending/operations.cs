using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace UI_app
{
    public class ImageClass
    {


        public Image Result;
        public ImageSource background { get; set; }
        public ImageSource source { get; set; }
        public Image result { get; set; }
        public int test { get; set; }
        public Bitmap bmpBackground { get; set; }
        public Bitmap bmpSource { get; set; }
        public int widthBackground { get; set; }
        public int heightBacground { get; set; }
        public int widthSource { get; set; }
        public int heightSource { get; set; }
        public byte alpha { get; set; }
        public short[] redBackground { get; set; }

        public short[] greenBackground { get; set; }

        public short[] blueBackground { get; set; }

        public short[] redSource { get; set; }

        public short[] greenSource { get; set; }

        public short[] blueSource { get; set; }

        public byte[] redResult { get; set; }

        public byte[] greenResult { get; set; }

        public byte[] blueResult { get; set; }
        



        int pixels { get; set; }
        public float time { get; set; }
        const int size = 8;
        private BitmapData bmpDataBackground;
        private BitmapData bmpDataSource;

        [DllImport("C:/Users/48573/Downloads/Brightness-20201012T104727Z-001/Brightness/x64/Debug/C_PROC.dll")]
        //[DllImport("C:/Users/48573/source/repos/TestProj/x64/Debug/Project_C.dll")]
        static extern void LightenC(IntPtr tab, byte ratio);
        [DllImport("C_PROC.dll")]
        static extern void DimC(IntPtr tab, byte ratio);

        [DllImport("C:/Users/48573/Downloads/Brightness-20201012T104727Z-001/Brightness/x64/Debug/ASM_PROC.dll")]
        static extern void Blend(IntPtr tab, byte ratio);
        [DllImport("ASM_PROC.dll")]
        static extern void DimASM(IntPtr tab, byte ratio);

        [DllImport("C:/Users/48573/Downloads/Brightness-20201012T104727Z-001/Brightness/x64/Debug/C_PROC.dll")]
        static extern void BlendC(IntPtr bg, IntPtr src, byte alpha);


        public void setSize()
        {
            heightBacground = this.bmpBackground.Height;
            widthBackground = this.bmpBackground.Width;
            heightSource = this.bmpSource.Height;
            widthSource = this.bmpSource.Width;
        }
        public void createRGB_source()
        {
            BitmapData bmpDataImg;
            Rectangle rect = new Rectangle(0, 0, bmpSource.Width, bmpSource.Height);
            bmpDataImg = bmpSource.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //Gets pointer to the first pixel in the bitmap.  
            IntPtr ptr = bmpDataImg.Scan0;
            //Gets the number of necessary space in the bytes's array.
            int pixels = bmpDataImg.Width * bmpSource.Height;
            int bytes = bmpDataImg.Stride * bmpSource.Height;
            byte[] rgbValues = new byte[bytes];
            redSource = new short[pixels];
            greenSource = new short[pixels];
            blueSource = new short[pixels];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            int count = 0;
            int stride = bmpDataImg.Stride;


            // petla tworzące tablice r g b na podstawie bitmap 
            for (int row = 0; row < bmpDataImg.Height; row++)
            {
                for (int column = 0; column < bmpDataImg.Width; column++)
                {
                    blueSource[count] = (short)(rgbValues[(column * 3) + row * stride]);
                    greenSource[count] = (short)(rgbValues[(column * 3 + 1) + row * stride]);
                    redSource[count++] = (short)(rgbValues[(column * 3 + 2) + row * stride]);
                }
            }
            bmpDataSource = bmpDataImg;
            bmpSource.UnlockBits(bmpDataImg);
        }
        public void createRGB_Background()
        {
            BitmapData bmpDataImg;
            Rectangle rect = new Rectangle(0, 0, bmpBackground.Width, bmpBackground.Height);
            bmpDataImg = bmpBackground.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            //Gets pointer to the first pixel in the bitmap.  
            IntPtr ptr = bmpDataImg.Scan0;
            //Gets the number of necessary space in the bytes's array.
            int pixels = bmpDataImg.Width * bmpSource.Height;
            int bytes = bmpDataImg.Stride * bmpSource.Height;
            byte[] rgbValues = new byte[bytes];
            redBackground = new short[pixels];
            greenBackground = new short[pixels];
            blueBackground = new short[pixels];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            int count = 0;
            // int width = bmpDataImg.Width;
            int stride = bmpDataImg.Stride;


            // petla tworzące tablice r g b na podstawie bitmap 
            for (int row = 0; row < bmpDataImg.Height; row++)
            {
                for (int column = 0; column < bmpDataImg.Width; column++)
                {
                    blueBackground[count] = (short)(rgbValues[(column * 3) + row * stride]);
                    greenBackground[count] = (short)(rgbValues[(column * 3 + 1) + row * stride]);
                    redBackground[count++] = (short)(rgbValues[(column * 3 + 2) + row * stride]);
                }
            }
            bmpDataBackground = bmpDataImg;
            bmpBackground.UnlockBits(bmpDataImg);
        }

        public void Blend_C()
        {
            int pixels = bmpBackground.Width * bmpBackground.Height;
            int arraySize = pixels / size;

            //pointers to the background arrays 
            IntPtr[] redArrayBackground = new IntPtr[arraySize];
            IntPtr[] greenArrayBackground = new IntPtr[arraySize];
            IntPtr[] blueArrayBackground = new IntPtr[arraySize];

            //pointers to the background arrays 
            IntPtr[] redArraySource = new IntPtr[arraySize];
            IntPtr[] greenArraySource = new IntPtr[arraySize];
            IntPtr[] blueArraySource = new IntPtr[arraySize];

            //Alocating memory for background image 
            int startIndex = 0;
            for (int i = 0; i < arraySize; i++)
            {
                // każda z tablic musi przechowywać 16 pikseli = bajt * 16
                redArrayBackground[i] = Marshal.AllocHGlobal(sizeof(short) * size );
                greenArrayBackground[i] = Marshal.AllocHGlobal(sizeof(short) * size );
                blueArrayBackground[i] = Marshal.AllocHGlobal(sizeof(short) * size );

                // kopiowanie wartości r g b do nowych tablic 
                Marshal.Copy(redBackground, startIndex, redArrayBackground[i], size );
                Marshal.Copy(greenBackground, startIndex, greenArrayBackground[i], size);
                Marshal.Copy(blueBackground, startIndex, blueArrayBackground[i], size );

                startIndex += size;
            }

            //Alocating memory for source image 
            startIndex = 0;
            for (int i = 0; i < arraySize; i++)
            {
                //każda z tablic musi przechowywać 16 pikseli = bajt * 16
                redArraySource[i] = Marshal.AllocHGlobal(sizeof(short) * size);
                greenArraySource[i] = Marshal.AllocHGlobal(sizeof(short) * size);
                blueArraySource[i] = Marshal.AllocHGlobal(sizeof(short) * size);

                // kopiowanie wartości r g b do nowych tablic 
                Marshal.Copy(redSource, startIndex, redArraySource[i], size);
                Marshal.Copy(greenSource, startIndex, greenArraySource[i], size);
                Marshal.Copy(blueSource, startIndex, blueArraySource[i], size);

                startIndex += size;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();

            {
                Parallel.For(0, arraySize, i =>
                {

                    BlendC(redArrayBackground[i], redArraySource[i], alpha);
                    BlendC(greenArrayBackground[i], greenArraySource[i], alpha);
                    BlendC(blueArrayBackground[i], blueArraySource[i], alpha);

                });
            }

            stopwatch.Stop();
            //time = stopwatch.Elapsed.TotalMilliseconds;
            AssignNewValues(redArrayBackground, greenArrayBackground, blueArrayBackground);


        }




        public void Blend_ASM()
        {

        }


        private void AssignNewValues(IntPtr[] redArray, IntPtr[] greenArray, IntPtr[] blueArray)
        {
            int pixels = bmpBackground.Width * bmpBackground.Height;
            redResult = new byte[pixels];
            greenResult = new byte[pixels];
            blueResult = new byte[pixels];


            int count = 0;
            for (int y = 0; y < pixels / size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    //redResult[count] = Convert.ToByte((Convert.ToInt32(redBackground[count]) * (100 - alpha) + Convert.ToInt32(redSource[count] )* alpha) / 100);  
                    //greenResult[count] = Convert.ToByte((Convert.ToInt32(greenBackground[count]) * (100 - alpha) + Convert.ToInt32(greenSource[count]) * alpha) / 100);
                    //blueResult[count] = Convert.ToByte((Convert.ToInt32(blueBackground[count]) * (100 - alpha) + Convert.ToInt32(blueSource[count++]) * alpha) / 100);
                    redResult[count] = Marshal.ReadByte(redArray[y] + 2*x);
                    greenResult[count] = Marshal.ReadByte(greenArray[y] + 2*x);
                    blueResult[count++] = Marshal.ReadByte(blueArray[y] + 2*x);
                    // przypisanie do r g b


                }
            }
        }

        public Bitmap AfterImageFromRGB()
        {

            Bitmap result_bmp = new Bitmap(bmpBackground.Width, bmpBackground.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Color c;
            int arrayIndex;

            for (int x = 0; x < bmpBackground.Width; x++)
            {
                for (int y = 0; y < bmpBackground.Height; y++)
                {
                    arrayIndex = y * bmpBackground.Width + x; // obliczenie indeksu
                    c = System.Drawing.Color.FromArgb(255, redResult[arrayIndex], greenResult[arrayIndex], blueResult[arrayIndex]); // znajdywanie koloru na podstawie wartości rgb
                    //c = System.Drawing.Color.FromArgb(255,128, 255, 6); // znajdywanie koloru na podstawie wartości rgb
                    result_bmp.SetPixel(x, y, c); // ustawianie piksela
                    //var handle = result_bmp.GetHbitmap();
                }
            }
            return result_bmp;
        }
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

    }
}