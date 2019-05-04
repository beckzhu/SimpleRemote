using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Modes
{
    public class DbItemRemoteLink
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PrivateKey { get; set; }
        public string Description { get; set; }
        public bool IsExpander1 { get; set; }
        public bool IsExpander2 { get; set; }
        public double ExternalWindowWidth { get; set; }
        public double ExternalWindowHeight{ get; set; }
        public bool ExternalIsMaximize { get; set; }
    }
}
