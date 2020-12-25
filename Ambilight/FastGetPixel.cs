using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ambilight
{
    public class FastGetPixel
    {
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll")]
        private static extern int BitBlt(
          IntPtr hdcDest, // handle to destination DC
          int nXDest,  // x-coord of destination upper-left corner
          int nYDest,  // y-coord of destination upper-left corner
          int nWidth,  // width of destination rectangle
          int nHeight, // height of destination rectangle
          IntPtr hdcSrc,  // handle to source DC
          int nXSrc,   // x-coordinate of source upper-left corner
          int nYSrc,   // y-coordinate of source upper-left corner
          UInt32 dwRop    // raster operation code
        );
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hDC, int x, int y);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        private const Int32 SRCCOPY = 13369376;
        private IntPtr hWnd;
        private Bitmap innerbitmap;
        private bool screenmade = false;
        private BitmapData bmpData;
        private int byteLen = 4;
        private byte[] bitmapBuffer;
        private Color col = Color.Empty;
        public FastGetPixel()
        {
            hWnd = GetDesktopWindow();
        }

        public FastGetPixel(IntPtr hWndsource)
        {
            hWnd = hWndsource;
        }

        public IntPtr Handle
        {
            get { return hWnd; }
            set { try { innerbitmap.UnlockBits(bmpData); } catch { } try { innerbitmap.Dispose(); } catch { } bitmapBuffer = null; screenmade = false; hWnd = value; }
        }

        public Color GetLockedPixel(int Xpos, int Ypos)
        {
            try
            {
                int index = bmpData.Stride * Ypos + Xpos * byteLen;
                col = Color.FromArgb(bitmapBuffer[index + 2], bitmapBuffer[index + 1], bitmapBuffer[index + 0]);
            }
            catch { col = Color.Empty; }
            return col;
        }

        public bool LockWindowImage(PixelFormat PF)
        {
            try
            {
                if (!screenmade)
                {
                    RECT rectal = new RECT();
                    GetWindowRect(new HandleRef(null, hWnd), out rectal);
                    innerbitmap = new Bitmap(rectal.Right, rectal.Bottom);
                    Graphics loGraphics = Graphics.FromImage(innerbitmap);
                    IntPtr lnDst = loGraphics.GetHdc();
                    IntPtr hDC = GetWindowDC(hWnd);
                    BitBlt(lnDst, 0, 0, rectal.Right - rectal.Left, rectal.Bottom - rectal.Top, hDC, 0, 0, SRCCOPY);
                    loGraphics.ReleaseHdc(lnDst);
                    loGraphics.Flush();
                    loGraphics.Dispose();
                    ReleaseDC(hWnd, hDC);
                    bmpData = innerbitmap.LockBits(new Rectangle(0, 0, innerbitmap.Width, innerbitmap.Height), ImageLockMode.ReadOnly, PF);
                    bitmapBuffer = new byte[bmpData.Stride * bmpData.Height];
                    Marshal.Copy(bmpData.Scan0, bitmapBuffer, 0, bitmapBuffer.Length);
                    byteLen = bmpData.Stride / bmpData.Width;
                    screenmade = true;
                }
                else { return false; }
            }

            catch { return false; }
            return true;
        }

        public bool LockWindowImage()
        {
            try
            {
                if (!screenmade)
                {
                    RECT rectal = new RECT();
                    GetWindowRect(new HandleRef(null, hWnd), out rectal);
                    innerbitmap = new Bitmap(rectal.Right, rectal.Bottom);
                    Graphics loGraphics = Graphics.FromImage(innerbitmap);
                    IntPtr lnDst = loGraphics.GetHdc();
                    IntPtr hDC = GetWindowDC(hWnd);
                    BitBlt(lnDst, 0, 0, rectal.Right - rectal.Left, rectal.Bottom - rectal.Top, hDC, 0, 0, SRCCOPY);
                    loGraphics.ReleaseHdc(lnDst);
                    loGraphics.Flush();
                    loGraphics.Dispose();
                    ReleaseDC(hWnd, hDC);
                    bmpData = innerbitmap.LockBits(new Rectangle(0, 0, innerbitmap.Width, innerbitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    bitmapBuffer = new byte[bmpData.Stride * bmpData.Height];
                    Marshal.Copy(bmpData.Scan0, bitmapBuffer, 0, bitmapBuffer.Length);
                    byteLen = 4;
                    screenmade = true;
                }
                else { return false; }
            }
            catch { return false; }
            return true;
        }

        public bool LockWindowImage(int Left, int Top, int Width, int Height, PixelFormat PF)
        {
            try
            {
                if (!screenmade)
                {
                    innerbitmap = new Bitmap(Width, Height);
                    Graphics loGraphics = Graphics.FromImage(innerbitmap);
                    IntPtr lnDst = loGraphics.GetHdc();
                    IntPtr hDC = GetWindowDC(hWnd);
                    BitBlt(lnDst, 0, 0, Width, Height, hDC, Left, Top, SRCCOPY);
                    loGraphics.ReleaseHdc(lnDst);
                    loGraphics.Flush();
                    loGraphics.Dispose();
                    ReleaseDC(hWnd, hDC);
                    bmpData = innerbitmap.LockBits(new Rectangle(0, 0, innerbitmap.Width, innerbitmap.Height), ImageLockMode.ReadOnly, PF);
                    bitmapBuffer = new byte[bmpData.Stride * bmpData.Height];
                    Marshal.Copy(bmpData.Scan0, bitmapBuffer, 0, bitmapBuffer.Length);
                    byteLen = bmpData.Stride / bmpData.Width;
                    screenmade = true;
                }
                else { return false; }
            }
            catch { return false; }
            return true;
        }
        public void Clear() { try { innerbitmap.UnlockBits(bmpData); } catch { } try { innerbitmap.Dispose(); } catch { } bitmapBuffer = null; screenmade = false; }
    }
}
