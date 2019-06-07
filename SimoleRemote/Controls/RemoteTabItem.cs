using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SimpleRemote.Controls
{
    public class RemoteTabItem : TabItem
    {
        public delegate void EventClosed(object sender);

        public EventClosed Closed { get; set; }
    }
}
