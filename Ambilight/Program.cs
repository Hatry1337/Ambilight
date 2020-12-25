using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using MSIRGB;

namespace Ambilight
{
    class Program
    {
        static void Main(string[] args)
        {
            Lighting light;
            try
            {
                light = new Lighting(false);
                FastGetPixel fgp = new FastGetPixel();
                Color last = Color.FromRgb(0, 0, 0);
                while (true){
                    fgp.LockWindowImage(0, 0, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    int d = 0;
                    for (int j = 0; j < System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size.Height; j+=20)
                    {
                        for (int i = 0; i < System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size.Width; i+=20)
                        {
                            System.Drawing.Color c1 = fgp.GetLockedPixel(i, j);
                            r += c1.R;
                            g += c1.G;
                            b += c1.B;
                            d++;
                        }
                    }
                    fgp.Clear();
                    r /= d;
                    g /= d;
                    b /= d;

                    for (float i = 0; i < 1f; i += 0.03f)
                    {

                        int mixR = (int)(last.R * (1f - i) + r * i);
                        int mixG = (int)(last.G * (1f - i) + g * i);
                        int mixB = (int)(last.B * (1f - i) + b * i);

                        light.BatchBegin();

                        for (byte j = 1; j < 9; j++)
                        {
                            light.SetColour(j, Color.FromRgb((byte)(mixR/ 0x11), (byte)(mixG/ 0x11), (byte)(mixB/ 0x11)));
                        }

                        light.BatchEnd();
                    }
                    last = Color.FromRgb((byte)r, (byte)g, (byte)b);
                    //Thread.Sleep(10);
                }
            }
            catch (Lighting.Exception exc)
            {
                if (exc.GetErrorCode() == Lighting.ErrorCode.MotherboardModelNotSupported)
                {
                    Console.WriteLine("Your motherboard is not on the list of supported motherboards. " +
                                        "Attempting to use this program may cause irreversible damage to your hardware and/or personal data. " +
                                        "Are you sure you want to continue?");
                }
                else if (exc.GetErrorCode() == Lighting.ErrorCode.MotherboardVendorNotSupported)
                {
                    Console.WriteLine("Your motherboard's vendor was not detected to be MSI. MSIRGB only supports MSI motherboards. " +
                        "To avoid damage to your hardware, MSIRGB will shutdown. " + Environment.NewLine + Environment.NewLine +
                        "If your motherboard's vendor is MSI, " + "" +
                        "please report this problem on the issue tracker at: https://github.com/ixjf/MSIRGB");
                }
                else if (exc.GetErrorCode() == Lighting.ErrorCode.DriverLoadFailed)
                {
                    Console.WriteLine("Failed to load driver. This could be either due to some program interfering with MSIRGB's driver, " +
                                    "or it could be a bug. Please report this on the issue tracker at: https://github.com/ixjf/MSIRGB");
                }
                else if (exc.GetErrorCode() == Lighting.ErrorCode.LoadFailed)
                {
                    Console.WriteLine("Failed to load. Please report this on the issue tracker at: https://github.com/ixjf/MSIRGB");
                }
                Console.ReadKey();
            }
        }
    }
}
