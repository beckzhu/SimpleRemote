using MemoryModules;
using SimpleRemote.Core;
using SimpleRemote.Modes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Native
{
    public class Putty: IDisposable
    {
        public struct Settings
        {
            public string fontname;
            public int fontsize;
            public int curtype;//光标类型 0=块 1=下划线 2=垂直线
            public string linecodepage;
            public int backspaceisdelete;//0=Control-H 1=Control-?(127)
            public int mouseisxterm;//0=混合模式 1=xterm 2=windows
            public PuttyColor puttycolor;//配色信息
            public int rxvthomeend;//0=标准 1=rxvt
            public int functionkeys;//0=esc 1=linux 2=xterm 3=vt400 4=vt100 5=sco
            public bool cjkambigwide;//将不确定的字符处理为CJK字符
            public bool capslockcyr;//Caps Lock大写按键用于Cyrillic切换
            public bool crimplieslf;//在每个CR字符后面加上LF
            public bool lfimpliescr;//在每个LF字符后面加上CR
        };
        public struct PuttyColor
        {
            public PuttyColor(DbPuttyColor color)
            {
                if (color != null)
                {
                    Colour0 = color.Colour0;
                    Colour1 = color.Colour1;
                    Colour2 = color.Colour2;
                    Colour3 = color.Colour3;
                    Colour4 = color.Colour4;
                    Colour5 = color.Colour5;
                    Colour6 = color.Colour6;
                    Colour7 = color.Colour7;
                    Colour8 = color.Colour8;
                    Colour9 = color.Colour9;
                    Colour10 = color.Colour10;
                    Colour11 = color.Colour11;
                    Colour12 = color.Colour12;
                    Colour13 = color.Colour13;
                    Colour14 = color.Colour14;
                    Colour15 = color.Colour15;
                    Colour16 = color.Colour16;
                    Colour17 = color.Colour17;
                    Colour18 = color.Colour18;
                    Colour19 = color.Colour19;
                    Colour20 = color.Colour20;
                    Colour21 = color.Colour21;
                }
                else
                {
                    Colour0 = 12303291;
                    Colour1 = 16777215;
                    Colour2 = 0;
                    Colour3 = 5592405;
                    Colour4 = 0;
                    Colour5 = 65280;
                    Colour6 = 0;
                    Colour7 = 5592405;
                    Colour8 = 187;
                    Colour9 = 5592575;
                    Colour10 = 47872;
                    Colour11 = 5635925;
                    Colour12 = 48059;
                    Colour13 = 5636095;
                    Colour14 = 12255232;
                    Colour15 = 16733525;
                    Colour16 = 12255419;
                    Colour17 = 16733695;
                    Colour18 = 12303104;
                    Colour19 = 16777045;
                    Colour20 = 12303291;
                    Colour21 = 16777215;

                }
            }
            public int Colour0;
            public int Colour1;
            public int Colour2;
            public int Colour3;
            public int Colour4;
            public int Colour5;
            public int Colour6;
            public int Colour7;
            public int Colour8;
            public int Colour9;
            public int Colour10;
            public int Colour11;
            public int Colour12;
            public int Colour13;
            public int Colour14;
            public int Colour15;
            public int Colour16;
            public int Colour17;
            public int Colour18;
            public int Colour19;
            public int Colour20;
            public int Colour21;
        }

        //创建Putty控件,请使用线程创建，该函数要等待WM_QUIT消息才会返回，成功返回True，失败返回0,调用Putty_GetError()获取详细信息
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate bool Putty_Create(IntPtr hWnd, string cmdline, int left, int top, int width, int height, Settings settings);
        //显示窗口
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void Putty_Show();
        //获取错误信息
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr Putty_GetError();
        //初始化Putty控件，以免在线程中创建的时候初始化失败
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate bool Putty_Init();
        //设置回调函数
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void Putty_SetCallback(ErrorEvent errorevent, ConnectedEvent connectedevent, SecurityAlertEvent securityalertevent,
                                               VerifyHostKeyEvent verifyhostkeyevent, StoreHostKeyEvent storehostkeyevent, KeyDownEvent keydownevent,
                                               MouseMoveEvent mousemoveevent, DebugEvent debugevent);
        //退出putty
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void Putty_Exit();
        //设置putty控件的位置
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void Putty_Move(int x, int y, int width, int height);
        //设置putty控件的句柄
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr Putty_GetHwnd();

        //发生错误时调用
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void ErrorEvent(int level, IntPtr title, IntPtr text, int lParam);
        //安全验证时调用
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate int SecurityAlertEvent(IntPtr text);
        //成功连接时调用
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void ConnectedEvent();
        //验证主机密钥时调用
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate int VerifyHostKeyEvent(string key, IntPtr otherstr, ref int readlen);
        //写入主机密钥时调用
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void StoreHostKeyEvent(string key, IntPtr otherstr, int readlen);
        //键盘按下
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void KeyDownEvent(int wParam, int lParam);
        //鼠标移动
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void MouseMoveEvent(int wParam, int lParam);
        //调试输出
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void DebugEvent(IntPtr text);


        private MemoryModule _memoryModule;
        public Putty()
        {
            string libFileName = Environment.Is64BitProcess ? "SimpleRemote.Lib.putty64.dll.Compress" : "SimpleRemote.Lib.putty.dll.Compress";
            _memoryModule = MemoryModule.Create(CommonServices.GetCompressResBytes(libFileName));
            Init = _memoryModule.GetProcDelegate<Putty_Init>("Putty_Init");
            Create = _memoryModule.GetProcDelegate<Putty_Create>("Putty_Create");
            GetError = _memoryModule.GetProcDelegate<Putty_GetError>("Putty_GetError");
            SetCallback = _memoryModule.GetProcDelegate<Putty_SetCallback>("Putty_SetCallback");
            Move = _memoryModule.GetProcDelegate<Putty_Move>("Putty_Move");
            GetHwnd = _memoryModule.GetProcDelegate<Putty_GetHwnd>("Putty_GetHwnd");
            Exit = _memoryModule.GetProcDelegate<Putty_Exit>("Putty_Exit");
            Show = _memoryModule.GetProcDelegate<Putty_Show>("Putty_Show");
        }
        public Putty_Init Init { get; internal set; }
        public Putty_Create Create { get; internal set; }
        public Putty_Show Show { get; internal set; }
        public Putty_GetError GetError { get; internal set; }
        public Putty_SetCallback SetCallback { get; internal set; }
        public Putty_Move Move { get; internal set; }
        public Putty_GetHwnd GetHwnd { get; internal set; }
        public Putty_Exit Exit { get; internal set; }

        public void Dispose()
        {
            _memoryModule.FreeLibrary();
        }
    }
}
