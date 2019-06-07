using System;

namespace SimpleRemote.Controls.Dragablz.Referenceless
{
    internal interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
