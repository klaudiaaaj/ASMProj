using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace ConsoleApp1
{ 
   
        public class ImageClass
        {
            
            public int test { get; set; }
           
            public int BackgroundWidth { get; set; }
            public int BackgroundHeight { get; set; }
            public int SourceWidth { get; set; }
            public int SourceHeight { get; set; }
            public int backgroundAlpha { get; set; }
            public int sourceAlpha { get; set; }
            public byte[] red { get; set; }

            public byte[] green { get; set; }

            public byte[] blue { get; set; }

            public float time { get; set; }
            const int size = 16;
        [DllImport("C:/Users/48573/source/repos/UI app/x64/Debug/C_PROC.dll")]
        static extern void LightenC(IntPtr tab, byte ratio);
            [DllImport("C:/Users/48573/Downloads/Brightness-20201012T104727Z-001/Brightness/x64/Debug/C_PROC.dll")]
            static extern void DimC(IntPtr tab, byte ratio);

            [DllImport("ASM_PROC.dll")]
            static extern void LightenASM(IntPtr tab, byte ratio);
            [DllImport("ASM_PROC.dll")]
            static extern void DimASM(IntPtr tab, byte ratio);
        [DllImport("C:/Users/48573/source/repos/UI app/x64/Debug/C_PROC.dll")]
        static extern void BlendingC(IntPtr tab, byte ratio);


            public void Brighten(int ratio, bool isAsm, int cores)
            {
                // dane dzielone są na 16 bajtowe wektory, rozmiar tablic - ilosc pikseli / 16 (size = 16)
                int arraySize = 15;
                IntPtr[] redArray = new IntPtr[arraySize];
                IntPtr[] greenArray = new IntPtr[arraySize];
                IntPtr[] blueArray = new IntPtr[arraySize];

                // funkcja alokujące pamięć tablic

                AllocateArrays(redArray, greenArray, blueArray, arraySize);
                if (ratio >= 0)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    if (isAsm)
                    {
                        Parallel.For(0, arraySize, new ParallelOptions { MaxDegreeOfParallelism = cores }, i =>
                        {
                            LightenASM(redArray[i], (byte)ratio);
                            LightenASM(greenArray[i], (byte)ratio);
                            LightenASM(blueArray[i], (byte)ratio);
                            BlendingC(blueArray[i], (byte)ratio);

                        });
                    }
                    else
                    {
                        Parallel.For(0, arraySize, new ParallelOptions { MaxDegreeOfParallelism = cores }, i =>
                        {
                            LightenC(redArray[i], (byte)ratio);
                            LightenC(greenArray[i], (byte)ratio);
                            LightenC(blueArray[i], (byte)ratio);
                            BlendingC(blueArray[i], (byte)ratio);
                        });
                    }
                    stopwatch.Stop();
                }
                else if (ratio < 0)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    if (isAsm)
                    {
                        Parallel.For(0, arraySize, new ParallelOptions { MaxDegreeOfParallelism = cores }, i =>
                        {
                            DimASM(redArray[i], (byte)Math.Abs(ratio));
                            DimASM(greenArray[i], (byte)Math.Abs(ratio));
                            DimASM(blueArray[i], (byte)Math.Abs(ratio));
                        });
                    }
                    else
                    {
                        Parallel.For(0, arraySize, new ParallelOptions { MaxDegreeOfParallelism = cores }, i =>
                        {
                            DimC(redArray[i], (byte)Math.Abs(ratio));
                            DimC(greenArray[i], (byte)Math.Abs(ratio));
                            DimC(blueArray[i], (byte)Math.Abs(ratio));
                        });
                    }
                    stopwatch.Stop();
                }



                // Przypisanie nowych wartosci
                AssignNewValues(redArray, greenArray, blueArray, arraySize, ratio);

                //zwolnienie pamieci
                for (int i = 0; i < arraySize; i++)
                {
                    Marshal.FreeHGlobal(redArray[i]);
                    Marshal.FreeHGlobal(greenArray[i]);
                    Marshal.FreeHGlobal(blueArray[i]);
                }
            }

            private void AllocateArrays(IntPtr[] redArray, IntPtr[] greenArray, IntPtr[] blueArray, int arraySize)
            {
                int begin = 0;

                for (int i = 0; i < arraySize; i++)
                {
                    // każda z tablic musi przechowywać 16 pikseli = bajt * 16
                    redArray[i] = Marshal.AllocHGlobal(sizeof(byte) * size);
                    greenArray[i] = Marshal.AllocHGlobal(sizeof(byte) * size);
                    blueArray[i] = Marshal.AllocHGlobal(sizeof(byte) * size);

                    // kopiowanie wartości r g b do nowych tablic 

                    begin += size;
                }
            }

            // funkcja poprawiająca wartości po sumowaniu (sprawdza czy przekroczyły zakres)
            private void AssignNewValues(IntPtr[] redArray, IntPtr[] greenArray, IntPtr[] blueArray, int arraySize, int ratio)
            {
                byte[] r = new byte[arraySize * size];
                byte[] g = new byte[arraySize * size];
                byte[] b = new byte[arraySize * size];
                int counter = 0;
                bool pos = ratio > 0 ? true : false;
                byte rb;
                byte gb;
                byte bb;
                for (int i = 0; i < arraySize; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        //odczytanie nowych wartosci r g b piksela
                        rb = Marshal.ReadByte(redArray[i] + j);
                        gb = Marshal.ReadByte(greenArray[i] + j);
                        bb = Marshal.ReadByte(blueArray[i] + j);
                        // przypisanie do r g b
                        r[counter] = rb;
                        g[counter] = gb;
                        b[counter] = bb;
                        counter++;
                    }
                }
                // przypisanie nowych wartości
                red = r;
                green = g;
                blue = b;
            }
        
    }
}