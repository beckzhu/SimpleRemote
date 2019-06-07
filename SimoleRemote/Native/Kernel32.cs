using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRemote.Native
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll")]

        public static extern void CopyMemory(ref MSTSCLib._RemotableHandle dest, ref IntPtr src, int count);
    }
}
