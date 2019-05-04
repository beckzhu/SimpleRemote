using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Modes
{
    public interface IRemoteControl
    {
        void Remove();
        void Jump();
        void Open(DbItemRemoteLink linkSettings, DbItemSetting itemSetting, bool jump);
        event EventHandler OnRemove;
        bool FullScreen(bool state);
    }
}
