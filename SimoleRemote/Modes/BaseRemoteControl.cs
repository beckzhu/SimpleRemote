using SimpleRemote.Core;
using SimpleRemote.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SimpleRemote.Modes
{
    public enum LogonError
    {
        /// <summary>
        /// 用户被拒绝访问
        /// </summary>
        ACCESSDENIED,
        /// <summary>
        /// 用户名和密码不正确
        /// </summary>
        ERRORUSERPASSWRD,
        /// <summary>
        /// 密码已过期
        /// </summary>
        PASSWORDEXPIRED,
        /// <summary>
        /// 用户被锁定
        /// </summary>
        LOCKUSER,
        /// <summary>
        /// 其他错误
        /// </summary>
        OTHER
    }

    public delegate void Closed();
    public delegate bool FullScreen(bool state);

    public delegate void EventConnected();
    public delegate void EventFatalError(string title, string errorText);
    public delegate void EventNonfatal(string title, string errorText);
    public delegate void MouseMoveEvent(int x, int y);

    public class BaseRemoteControl:UserControl
    {
        public BaseRemoteControl(ContentControl userControl)
        {
            ParentControl = userControl;
        }
        /// <summary>
        /// 连接远程服务器
        /// </summary>
        public virtual void Connect(DbItemRemoteLink linkSettings, DbItemSetting lastSetting)
        {
            
        }
        public virtual void GoFullScreen(bool state)
        {

        }
        /// <summary>
        /// 释放所有的资源
        /// </summary>
        public virtual void Release()
        {

        }
        /// <summary>
        /// 父级UserControl
        /// </summary>
        public ContentControl ParentControl;
        /// <summary>
        /// 远程连接成功回调函数
        /// </summary>
        public EventConnected OnConnected;
        /// <summary>
        /// 在客户端控件遇到非致命错误时调用
        /// </summary>
        public EventNonfatal OnNonfatal;
        /// <summary>
        /// 在客户端控件遇到致命错误时调用
        /// </summary>
        public EventFatalError OnFatalError;
        /// <summary>
        /// 关闭
        /// </summary>
        public Closed Closed;
        /// <summary>
        /// 全屏  成功返回True  失败返回false
        /// </summary>
        public FullScreen FullScreen;
        /// <summary>
        /// 鼠标移动
        /// </summary>
        public MouseMoveEvent MouseMoveProc;
    }
}
