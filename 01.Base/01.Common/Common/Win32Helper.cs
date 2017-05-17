using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Win32帮助类 
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary> 
    public class Win32Helper
    {
        /// <summary>
        /// 
        /// </summary>
        public const Int32 GWL_STYLE = -16;
        /// <summary>
        /// 
        /// </summary>
        public const Int32 SW_MAXIMIZE = 3;
        /// <summary>
        /// 
        /// </summary>
        public const Int32 SWP_FRAMECHANGED = 0x0020;
        /// <summary>
        /// 
        /// </summary>
        public const Int32 SWP_NOMOVE = 0x0002;
        /// <summary>
        /// 
        /// </summary>
        public const Int32 SWP_NOSIZE = 0x0001;
        /// <summary>
        /// 
        /// </summary>
        public const Int32 SWP_NOZORDER = 0x0004;
        /// <summary>
        /// 
        /// </summary>
        public const Int32 WS_BORDER = (Int32)0x00800000L;
        /// <summary>
        /// 
        /// </summary>
        public const Int32 WS_THICKFRAME = (Int32)0x00040000L;
        /// <summary>
        /// 
        /// </summary>
        public IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern Int32 GetWindowLong(IntPtr hWnd,
            Int32 nIndex
        );

        /// <summary>
        /// 设置鼠标的坐标 
        /// </summary>
        /// <param name="X"> 横坐标 </param>
        /// <param name="Y"> 纵坐标 </param>
        /// <returns></returns>

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWndChild"></param>
        /// <param name="hWndNewParent"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild,
            IntPtr hWndNewParent
        );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern Int32 SetWindowLong(IntPtr hWnd,
            Int32 nIndex,
            Int32 dwNewLong
        );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern Int32 SetWindowPos(IntPtr hWnd,
            IntPtr hWndInsertAfter,
            Int32 X,
            Int32 Y,
            Int32 cx,
            Int32 cy,
            UInt32 uFlags
        );
    }
}