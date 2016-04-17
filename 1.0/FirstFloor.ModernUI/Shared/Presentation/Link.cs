namespace FirstFloor.ModernUI.Presentation
{
    using System;

    /// <summary>
    /// Represents a displayable link.
    /// </summary>
    public class Link
        : Displayable
    {
        /// <summary>
        /// The source uri.
        /// </summary>
        private Uri source;

        /// <summary>
        /// Gets or sets the source uri.
        /// </summary>
        /// <value>The source.</value>
        public Uri Source
        {
            get
            {
                return source;
            }

            set
            {
                if (source != value)
                {
                    source = value;
                    OnPropertyChanged("Source");
                }
            }
        }

        /// <summary>
        /// Gets or sets an arbitrary object value that can be used to store custom information about this element.
        /// </summary>
        public object Tag { get; set; }
    }
}
