using System;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    public static class Extensions
    {
        public static T Invoke<T>( this DispatcherObject dispatcherObject,  Func<T> func)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                return func();
            }
            else
            {
                return (T)dispatcherObject.Dispatcher.Invoke(new Func<T>(func));
            }
        }

        public static void Invoke( this DispatcherObject dispatcherObject,  Action invokeAction)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }
            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }
            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                invokeAction();
            }
            else
            {
                dispatcherObject.Dispatcher.Invoke(invokeAction);
            }
        }

        /// <summary> 
        ///   Executes the specified action asynchronously with the DispatcherPriority.Background on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object where the action runs.</param>
        /// <param name="invokeAction">An action that takes no parameters.</param>
        /// <param name="priority">The dispatcher priority.</param> 
        public static void BeginInvoke( this DispatcherObject dispatcherObject,  Action invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }
            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }
            dispatcherObject.Dispatcher.BeginInvoke(priority, invokeAction);
        }
    }
}