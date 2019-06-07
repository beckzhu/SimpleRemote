using System;

namespace SimpleRemote.Controls.Dragablz.Referenceless
{
    internal sealed class DefaultDisposable : IDisposable
    {
        public static readonly DefaultDisposable Instance = new DefaultDisposable();

        static DefaultDisposable()
        {
        }

        private DefaultDisposable()
        {
        }

        public void Dispose()
        {
        }
    }
}
