namespace MahApps.Metro.Controls.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// 用于操作打开的ProgressDialog的类.
    /// </summary>
    public class ProgressDialogController
    {
        private ProgressDialog WrappedDialog { get; }

        private Func<Task> CloseCallback { get; }

        /// <summary>
        /// 关闭对话框时,引发此事件.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// 当用户按下取消按钮时会引发此事件
        /// </summary>
        public event EventHandler Canceled;
        /// <summary>
        /// 当用户按下确定按钮时会引发此事件
        /// </summary>
        public event EventHandler Determine;

        /// <summary>
        /// 获取是否已按下“取消”按钮.
        /// </summary>        
        public bool IsCanceled { get; private set; }
        /// <summary>
        /// 获取是否已按下“确定”按钮.
        /// </summary>        
        public bool IsDetermine { get; private set; }

        /// <summary>
        /// 获取包装的ProgressDialog是否已打开.
        /// </summary>        
        public bool IsOpen { get; private set; }

        internal ProgressDialogController(ProgressDialog dialog, Func<Task> closeCallBack)
        {
            this.WrappedDialog = dialog;
            this.CloseCallback = closeCallBack;

            this.IsOpen = dialog.IsVisible;

            this.WrappedDialog.Invoke(() => { this.WrappedDialog.PART_NegativeButton.Click += this.PART_NegativeButton_Click; });
            this.WrappedDialog.Invoke(() => { this.WrappedDialog.PART_DetermineButton.Click += this.PART_DetermineButton_Click; });

            dialog.CancellationToken.Register(() => { this.PART_NegativeButton_Click(null, new RoutedEventArgs()); });
            dialog.CancellationToken.Register(() => { this.PART_DetermineButton_Click(null, new RoutedEventArgs()); });
        }

        private void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            Action action = () =>
                {
                    this.IsCanceled = true;
                    this.Canceled?.Invoke(this, EventArgs.Empty);
                    this.WrappedDialog.PART_NegativeButton.IsEnabled = false;
                };
            this.WrappedDialog.Invoke(action);
        }

        private void PART_DetermineButton_Click(object sender, RoutedEventArgs e)
        {
            Action action = () =>
            {
                this.IsDetermine = true;
                this.Determine?.Invoke(this, EventArgs.Empty);
                this.WrappedDialog.PART_DetermineButton.IsEnabled = false;
            };
            this.WrappedDialog.Invoke(action);
        }

        /// <summary>
        /// 将ProgressBar的IsIndeterminate设置为true。 要将其设置为false，请调用SetProgress.
        /// </summary>
        public void SetIndeterminate()
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.SetIndeterminate());
        }

        /// <summary>
        /// 设置“取消”按钮是否可见.
        /// </summary>
        /// <param name="value"></param>
        public void SetCancelable(bool value)
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.IsCancelable = value);
        }
        /// <summary>
        /// 设置“确定”按钮是否可见.
        /// </summary>
        /// <param name="value"></param>
        public void SetDetermine(bool value)
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.IsDetermine = value);
        }

        /// <summary>
        /// 设置对话框的进度条值并将IsIndeterminate设置为false.
        /// </summary>
        /// <param name="value">The percentage to set as the value.</param>
        public void SetProgress(double value)
        {
            Action action = () =>
                {
                    if (value < this.WrappedDialog.Minimum || value > this.WrappedDialog.Maximum)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }
                    this.WrappedDialog.ProgressValue = value;
                };
            this.WrappedDialog.Invoke(action);
        }

        /// <summary>
        ///  获取/设置进度Value属性的最小限制
        /// </summary>
        public double Minimum
        {
            get { return this.WrappedDialog.Invoke(() => this.WrappedDialog.Minimum); }
            set { this.WrappedDialog.Invoke(() => this.WrappedDialog.Minimum = value); }
        }

        /// <summary>
        ///  获取/设置进度Value属性的最大限制
        /// </summary>
        public double Maximum
        {
            get { return this.WrappedDialog.Invoke(() => this.WrappedDialog.Maximum); }
            set { this.WrappedDialog.Invoke(() => this.WrappedDialog.Maximum = value); }
        }

        /// <summary>
        /// 设置对话框的消息内容.
        /// </summary>
        /// <param name="message">The message to be set.</param>
        public void SetMessage(string message)
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.Message = message);
        }

        /// <summary>
        /// 设置对话框的标题.
        /// </summary>
        /// <param name="title">The title to be set.</param>
        public void SetTitle(string title)
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.Title = title);
        }

        /// <summary>
        /// 设置对话框的进度条画笔
        /// </summary>
        /// <param name="brush">The brush to use for the progress bar's foreground</param>
        public void SetProgressBarForegroundBrush(Brush brush)
        {
            this.WrappedDialog.Invoke(() => this.WrappedDialog.ProgressBarForeground = brush);
        }

        /// <summary>
        /// 开始关闭ProgressDialog的操作.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        public Task CloseAsync()
        {
            Action action = () =>
                {
                    if (!this.WrappedDialog.IsVisible)
                    {
                        throw new InvalidOperationException("Dialog isn't visible to close");
                    }
                    this.WrappedDialog.Dispatcher.VerifyAccess();
                    this.WrappedDialog.PART_NegativeButton.Click -= this.PART_NegativeButton_Click;
                };

            this.WrappedDialog.Invoke(action);

            return this.CloseCallback()
                       .ContinueWith(_ => this.WrappedDialog.Invoke(() =>
                                                                        {
                                                                            this.IsOpen = false;
                                                                            this.Closed?.Invoke(this, EventArgs.Empty);
                                                                        }));
        }
    }
}