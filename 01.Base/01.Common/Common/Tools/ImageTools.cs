using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 图像处理工具扩展
    /// </summary>
    public static class ImageTools
    {
        /// <summary>
        /// 将二进制流转换成图片 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapImage ByteArrayToBitmapImage(this byte[] byteArray)
        {
            System.Windows.Media.Imaging.BitmapImage bmp = null;

            try
            {
                bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new System.IO.MemoryStream(byteArray);
                bmp.EndInit();
                bmp.Freeze();
            }
            catch
            {
                bmp = null;
            }

            return bmp;
        }

        /// <returns></returns>
        /// <summary>
        /// 从Bitmap转换成BitmapSource 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapSource ChangeBitmapToBitmapSource(this System.Drawing.Bitmap bmp)
        {
            System.Windows.Media.Imaging.BitmapSource returnSource;
            try
            {
                IntPtr hBitmap = bmp.GetHbitmap();
                returnSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                if (!Win32Helper.DeleteObject(hBitmap))//记得要进行内存释放。否则会有内存不足的报错。
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }

        /// <summary>
        /// 从bitmap转换成ImageSource 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static System.Windows.Media.ImageSource ChangeBitmapToImageSource(this System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            System.Windows.Media.ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            if (!Win32Helper.DeleteObject(hBitmap))//记得要进行内存释放。否则会有内存不足的报错。
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static System.Windows.Media.ImageSource ChangeIconToImageSource(this System.Drawing.Icon icon)
        {
            IntPtr hBitmap = icon.Handle;
            System.Windows.Media.ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
            hBitmap,
            System.Windows.Int32Rect.Empty,
            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            if (!Win32Helper.DeleteObject(hBitmap))//记得要进行内存释放。否则会有内存不足的报错。
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return imageSource;
        }

        /// <summary>
        /// 将图片转换成二进制 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this System.Drawing.Image image)
        {
            byte[] bValue = null;
            try
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    bValue = ms.ToArray();
                }
            }
            catch
            {
                bValue = null;
            }

            return bValue;
        }

    }
}
