using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace SimpleRemote.DllLibrary
{
    public static class User32
    {
        public delegate bool EnumWindowFunc(IntPtr hwnd, IntPtr lParam);
        public delegate int WindowFunc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, int wMsgFilterMin, int wMsgFilterMax);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, int wMsgFilterMin, int wMsgFilterMax, int wRemoveMsg);
        [DllImport("User32.dll")]
        public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);
        [DllImport("User32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("User32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("User32.dll",EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLong64(IntPtr hwnd, int nIndex, WindowFunc dwNewLong);
        [DllImport("User32.dll", EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLong(IntPtr hwnd, int nIndex, WindowFunc dwNewLong);
        public const int GWL_EXSTYLE = -20;
        [DllImport("User32.dll")]
        public static extern int CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("User32.dll")]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowFunc lpEnumFunc, IntPtr lParam);
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr Hwnd);
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr Hwnd, bool nCmdShow);
        [DllImport("User32.dll")]
        public static extern bool UpdateWindow(IntPtr Hwnd);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassNameW(IntPtr hWnd, string lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();
        /// <summary>
        /// 根据进程位数 选择SetWindowLong  WinApi调用
        /// </summary>
        public static IntPtr SetWindowLongPtr(IntPtr hwnd, int nIndex, WindowFunc dwNewLong)
        {
            if (Environment.Is64BitProcess) return SetWindowLong64(hwnd, nIndex, dwNewLong);
            return SetWindowLong(hwnd, nIndex, dwNewLong);
        }
    }
}
