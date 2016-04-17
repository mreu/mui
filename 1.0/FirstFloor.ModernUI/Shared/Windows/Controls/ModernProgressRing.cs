namespace FirstFloor.ModernUI.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a control that indicates that an operation is ongoing.
    /// </summary>
    [TemplateVisualState(GroupName = GroupActiveStates, Name = StateInactive)]
    [TemplateVisualState(GroupName = GroupActiveStates, Name = StateActive)]
    public class ModernProgressRing
        : Control
    {
        /// <summary>
        /// The group active states (const). Value: "ActiveStates".
        /// </summary>
        private const string GroupActiveStates = "ActiveStates";

        /// <summary>
        /// The state inactive (const). Value: "Inactive".
        /// </summary>
        private const string StateInactive = "Inactive";

        /// <summary>
        /// The state active (const). Value: "Active".
        /// </summary>
        private const string StateActive = "Active";

        /// <summary>
        /// Identifies the IsActive property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(ModernProgressRing), new PropertyMetadata(false, OnIsActiveChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernProgressRing"/> class.
        /// </summary>
        public ModernProgressRing()
        {
            DefaultStyleKey = typeof(ModernProgressRing);
        }

        /// <summary>
        /// Goto current state.
        /// </summary>
        /// <param name="animate">The animate.</param>
        private void GotoCurrentState(bool animate)
        {
            var state = IsActive ? StateActive : StateInactive;

            VisualStateManager.GoToState(this, state, animate);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GotoCurrentState(false);
        }

        /// <summary>
        /// Raises the is active changed event.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ModernProgressRing)o).GotoCurrentState(true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ModernProgressRing"/> is showing progress.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
    }
}
