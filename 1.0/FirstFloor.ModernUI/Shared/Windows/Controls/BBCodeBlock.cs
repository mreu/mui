namespace FirstFloor.ModernUI.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Navigation;
    using FirstFloor.ModernUI.Windows.Controls.BBCode;
    using FirstFloor.ModernUI.Windows.Navigation;

    /// <summary>
    /// A lighweight control for displaying small amounts of rich formatted BBCode content.
    /// </summary>
    [ContentProperty("BBCode")]
    public class BBCodeBlock
        : TextBlock
    {
        /// <summary>
        /// Identifies the BBCode dependency property.
        /// </summary>
        private static readonly DependencyProperty BBCodeProperty = DependencyProperty.Register("BBCode", typeof(string), typeof(BBCodeBlock), new PropertyMetadata(OnBBCodeChanged));
        /// <summary>
        /// Identifies the LinkNavigator dependency property.
        /// </summary>
        private static readonly DependencyProperty LinkNavigatorProperty = DependencyProperty.Register("LinkNavigator", typeof(ILinkNavigator), typeof(BBCodeBlock), new PropertyMetadata(new DefaultLinkNavigator(), OnLinkNavigatorChanged));

        /// <summary>
        /// The dirty flag.
        /// </summary>
        private bool dirty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BBCodeBlock"/> class.
        /// </summary>
        public BBCodeBlock()
        {
            // ensures the implicit BBCodeBlock style is used
            DefaultStyleKey = typeof(BBCodeBlock);

            AddHandler(FrameworkContentElement.LoadedEvent, new RoutedEventHandler(OnLoaded));
            AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigate));
        }

        /// <summary>
        /// Raises the BB code changed event.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void OnBBCodeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((BBCodeBlock)o).UpdateDirty();
        }

        /// <summary>
        /// Raises the link navigator changed event.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        /// <exception cref="ArgumentNullException">LinkNavigator</exception>
        private static void OnLinkNavigatorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                // null values disallowed
                throw new ArgumentNullException("LinkNavigator");
            }

            ((BBCodeBlock)o).UpdateDirty();
        }

        /// <summary>
        /// Raises the loaded event.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLoaded(object o, EventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Update dirty flag.
        /// </summary>
        private void UpdateDirty()
        {
            dirty = true;
            Update();
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            if (!IsLoaded || !dirty)
            {
                return;
            }

            var bbcode = BBCode;

            Inlines.Clear();

            if (!string.IsNullOrWhiteSpace(bbcode))
            {
                Inline inline;
                try
                {
                    var parser = new BBCodeParser(bbcode, this)
                    {
                        Commands = LinkNavigator.Commands
                    };
                    inline = parser.Parse();
                }
                catch (Exception)
                {
                    // parsing failed, display BBCode value as-is
                    inline = new Run { Text = bbcode };
                }

                Inlines.Add(inline);
            }

            dirty = false;
        }

        /// <summary>
        /// Raises the request navigate event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The request navigate event arguments.</param>
        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                // perform navigation using the link navigator
                LinkNavigator.Navigate(e.Uri, this, e.Target);
            }
            catch (Exception error)
            {
                // display navigation failures
                ModernDialog.ShowMessage(error.Message, ModernUI.Resources.NavigationFailed, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Gets or sets the BB code.
        /// </summary>
        /// <value>The BB code.</value>
        public string BBCode
        {
            get { return (string)GetValue(BBCodeProperty); }
            set { SetValue(BBCodeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the link navigator.
        /// </summary>
        /// <value>The link navigator.</value>
        public ILinkNavigator LinkNavigator
        {
            get { return (ILinkNavigator)GetValue(LinkNavigatorProperty); }
            set { SetValue(LinkNavigatorProperty, value); }
        }
    }
}
