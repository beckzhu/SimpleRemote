using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public partial class ProgressDialog : BaseMetroDialog
    {
        internal ProgressDialog()
            : this(null)
        {
        }

        internal ProgressDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal ProgressDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            this.InitializeComponent();
        }

        protected override void OnLoaded()
        {
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;
            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.SetResourceReference(ProgressBarForegroundProperty, this.DialogSettings.ColorScheme == MetroDialogColorScheme.Theme ? "AccentColorBrush" : "BlackBrush");
        }

        public static readonly DependencyProperty ProgressBarForegroundProperty = DependencyProperty.Register("ProgressBarForeground", typeof(Brush), typeof(ProgressDialog), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ProgressDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty IsCancelableProperty = DependencyProperty.Register("IsCancelable", typeof(bool), typeof(ProgressDialog), new PropertyMetadata(default(bool), (s, e) => 
                                                                        {
                                                                            ((ProgressDialog)s).PART_NegativeButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
                                                                            ((ProgressDialog)s).PART_NegativeButton.Width = (bool)e.NewValue ? 80 : 0;
                                                                        }));
        public static readonly DependencyProperty IsDetermineProperty = DependencyProperty.Register("IsDetermine", typeof(bool), typeof(ProgressDialog), new PropertyMetadata(default(bool), (s, e) =>
                                                                        {
                                                                            ((ProgressDialog)s).PART_DetermineButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
                                                                            ((ProgressDialog)s).PART_DetermineButton.Width = (bool)e.NewValue ? 80 : 0;
                                                                        }));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(ProgressDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(ProgressDialog), new PropertyMetadata("Define"));

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        public bool IsCancelable
        {
            get { return (bool)this.GetValue(IsCancelableProperty); }
            set { this.SetValue(IsCancelableProperty, value); }
        }
        public bool IsDetermine
        {
            get { return (bool)this.GetValue(IsDetermineProperty); }
            set { this.SetValue(IsDetermineProperty, value); }
        }


        public string NegativeButtonText
        {
            get { return (string)this.GetValue(NegativeButtonTextProperty); }
            set { this.SetValue(NegativeButtonTextProperty, value); }
        }

        public string AffirmativeButtonText
        {
            get { return (string)this.GetValue(AffirmativeButtonTextProperty); }
            set { this.SetValue(AffirmativeButtonTextProperty, value); }
        }

        public Brush ProgressBarForeground
        {
            get { return (Brush)this.GetValue(ProgressBarForegroundProperty); }
            set { this.SetValue(ProgressBarForegroundProperty, value); }
        }

        internal CancellationToken CancellationToken => this.DialogSettings.CancellationToken;

        internal double Minimum
        {
            get { return this.PART_ProgressBar.Minimum; }
            set { this.PART_ProgressBar.Minimum = value; }
        }

        internal double Maximum
        {
            get { return this.PART_ProgressBar.Maximum; }
            set { this.PART_ProgressBar.Maximum = value; }
        }

        internal double ProgressValue
        {
            get { return this.PART_ProgressBar.Value; }
            set
            {
                this.PART_ProgressBar.IsIndeterminate = false;
                this.PART_ProgressBar.Value = value;
                this.PART_ProgressBar.ApplyTemplate();
            }
        }

        internal void SetIndeterminate()
        {
            this.PART_ProgressBar.IsIndeterminate = true;
        }
    }
}