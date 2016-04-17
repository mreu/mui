namespace FirstFloor.ModernUI.Windows.Navigation
{
    using System.Windows.Input;

    /// <summary>
    /// The routed link commands.
    /// </summary>
    public static class LinkCommands
    {
        /// <summary>
        /// Gets the navigate link routed command.
        /// </summary>
        public static RoutedUICommand NavigateLink { get; } = new RoutedUICommand(Resources.NavigateLink, nameof(NavigateLink), typeof(LinkCommands));
    }
}
