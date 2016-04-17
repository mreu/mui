namespace FirstFloor.ModernUI.Windows.Controls
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using FirstFloor.ModernUI.Presentation;

    /// <summary>
    /// Represents a control that contains multiple pages that share the same space on screen.
    /// </summary>
    public class ModernTab
        : Control
    {
        /// <summary>
        /// Identifies the ContentLoader dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentLoaderProperty = DependencyProperty.Register("ContentLoader", typeof(IContentLoader), typeof(ModernTab), new PropertyMetadata(new DefaultContentLoader()));
        /// <summary>
        /// Identifies the Layout dependency property.
        /// </summary>
        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register("Layout", typeof(TabLayout), typeof(ModernTab), new PropertyMetadata(TabLayout.Tab));
        /// <summary>
        /// Identifies the ListWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ListWidthProperty = DependencyProperty.Register("ListWidth", typeof(GridLength), typeof(ModernTab), new PropertyMetadata(new GridLength(170)));
        /// <summary>
        /// Identifies the Links dependency property.
        /// </summary>
        public static readonly DependencyProperty LinksProperty = DependencyProperty.Register("Links", typeof(LinkCollection), typeof(ModernTab), new PropertyMetadata(OnLinksChanged));
        /// <summary>
        /// Identifies the SelectedSource dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSourceProperty = DependencyProperty.Register("SelectedSource", typeof(Uri), typeof(ModernTab), new PropertyMetadata(OnSelectedSourceChanged));

        /// <summary>
        /// Occurs when the selected source has changed.
        /// </summary>
        public event EventHandler<SourceEventArgs> SelectedSourceChanged;

        /// <summary>
        /// The link list.
        /// </summary>
        private ListBox linkList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernTab"/> class.
        /// </summary>
        public ModernTab()
        {
            DefaultStyleKey = typeof(ModernTab);

            // create a default links collection
            SetCurrentValue(LinksProperty, new LinkCollection());
        }

        /// <summary>
        /// Raises the links changed event.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void OnLinksChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernTab)o).UpdateSelection();
        }

        /// <summary>
        /// Raises the selected source changed event.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void OnSelectedSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernTab)o).OnSelectedSourceChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        /// <summary>
        /// Raises the selected source changed event.
        /// </summary>
        /// <param name="oldValue">The oldValue.</param>
        /// <param name="newValue">The newValue.</param>
        private void OnSelectedSourceChanged(Uri oldValue, Uri newValue)
        {
            UpdateSelection();

            // raise SelectedSourceChanged event
            var handler = SelectedSourceChanged;
            handler?.Invoke(this, new SourceEventArgs(newValue));
        }

        /// <summary>
        /// Update selection.
        /// </summary>
        private void UpdateSelection()
        {
            if (linkList == null || Links == null)
            {
                return;
            }

            // sync list selection with current source
            linkList.SelectedItem = Links.FirstOrDefault(l => l.Source == SelectedSource);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (linkList != null)
            {
                linkList.SelectionChanged -= OnLinkListSelectionChanged;
            }

            linkList = GetTemplateChild("LinkList") as ListBox;
            if (linkList != null)
            {
                linkList.SelectionChanged += OnLinkListSelectionChanged;
            }

            UpdateSelection();
        }

        /// <summary>
        /// Raises the link list selection changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The selection changed event arguments.</param>
        private void OnLinkListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var link = linkList.SelectedItem as Link;
            if (link != null && link.Source != SelectedSource)
            {
                SetCurrentValue(SelectedSourceProperty, link.Source);
            }
        }

        /// <summary>
        /// Gets or sets the content loader.
        /// </summary>
        public IContentLoader ContentLoader
        {
            get { return (IContentLoader)GetValue(ContentLoaderProperty); }
            set { SetValue(ContentLoaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating how the tab should be rendered.
        /// </summary>
        public TabLayout Layout
        {
            get { return (TabLayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        /// <summary>
        /// Gets or sets the collection of links that define the available content in this tab.
        /// </summary>
        public LinkCollection Links
        {
            get { return (LinkCollection)GetValue(LinksProperty); }
            set { SetValue(LinksProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the list when Layout is set to List.
        /// </summary>
        /// <value>
        /// The width of the list.
        /// </value>
        public GridLength ListWidth
        {
            get { return (GridLength)GetValue(ListWidthProperty); }
            set { SetValue(ListWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the source URI of the selected link.
        /// </summary>
        /// <value>The source URI of the selected link.</value>
        public Uri SelectedSource
        {
            get { return (Uri)GetValue(SelectedSourceProperty); }
            set { SetValue(SelectedSourceProperty, value); }
        }
    }
}
