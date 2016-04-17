namespace FirstFloor.ModernUI.Windows.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using FirstFloor.ModernUI.Presentation;

    /// <summary>
    /// Represents a Modern UI styled dialog window.
    /// </summary>
    public class ModernDialog
        : DpiAwareWindow
    {
        /// <summary>
        /// Identifies the BackgroundContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(object), typeof(ModernDialog));
        /// <summary>
        /// Identifies the Buttons dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons", typeof(IEnumerable<Button>), typeof(ModernDialog));

        /// <summary>
        /// The ok button.
        /// </summary>
        private Button okButton;

        /// <summary>
        /// The cancel button.
        /// </summary>
        private Button cancelButton;

        /// <summary>
        /// The yes button.
        /// </summary>
        private Button yesButton;

        /// <summary>
        /// The no button.
        /// </summary>
        private Button noButton;

        /// <summary>
        /// The close button.
        /// </summary>
        private Button closeButton;

        /// <summary>
        /// The message box result.
        /// </summary>
        private MessageBoxResult messageBoxResult = MessageBoxResult.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModernDialog"/> class.
        /// </summary>
        public ModernDialog()
        {
            DefaultStyleKey = typeof(ModernDialog);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            CloseCommand = new RelayCommand(x =>
            {
                var result = x as MessageBoxResult?;
                if (result.HasValue)
                {
                    messageBoxResult = result.Value;

                    // sets the Window.DialogResult as well
                    if (result.Value == MessageBoxResult.OK || result.Value == MessageBoxResult.Yes)
                    {
                        DialogResult = true;
                    }
                    else if (result.Value == MessageBoxResult.Cancel || result.Value == MessageBoxResult.No)
                    {
                        DialogResult = false;
                    }
                    else
                    {
                        DialogResult = null;
                    }
                }

                Close();
            });

            Buttons = new[] { CloseButton };

            // set the default owner to the app main window (if possible)
            if (Application.Current != null && !Equals(Application.Current.MainWindow, this))
            {
                Owner = Application.Current.MainWindow;
            }
        }

        /// <summary>
        /// Create close dialog button.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="isDefault">The isDefault.</param>
        /// <param name="isCancel">The isCancel.</param>
        /// <param name="result">The result.</param>
        /// <returns>The <see cref="Button"/>.</returns>
        private Button CreateCloseDialogButton(string content, bool isDefault, bool isCancel, MessageBoxResult result)
        {
            return new Button
            {
                Content = content,
                Command = CloseCommand,
                CommandParameter = result,
                IsDefault = isDefault,
                IsCancel = isCancel,
                MinHeight = 21,
                MinWidth = 65,
                Margin = new Thickness(4, 0, 0, 0)
            };
        }

        /// <summary>
        /// Gets the close window command.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Gets the Ok button.
        /// </summary>
        public Button OkButton => okButton ?? (okButton = CreateCloseDialogButton(ModernUI.Resources.Ok, true, false, MessageBoxResult.OK));

        /// <summary>
        /// Gets the Cancel button.
        /// </summary>
        public Button CancelButton => cancelButton ?? (cancelButton = CreateCloseDialogButton(ModernUI.Resources.Cancel, false, true, MessageBoxResult.Cancel));

        /// <summary>
        /// Gets the Yes button.
        /// </summary>
        public Button YesButton => yesButton ?? (yesButton = CreateCloseDialogButton(ModernUI.Resources.Yes, true, false, MessageBoxResult.Yes));

        /// <summary>
        /// Gets the No button.
        /// </summary>
        public Button NoButton => noButton ?? (noButton = CreateCloseDialogButton(ModernUI.Resources.No, false, true, MessageBoxResult.No));

        /// <summary>
        /// Gets the Close button.
        /// </summary>
        public Button CloseButton => closeButton ?? (closeButton = CreateCloseDialogButton(ModernUI.Resources.Close, true, false, MessageBoxResult.None));

        /// <summary>
        /// Gets or sets the background content of this window instance.
        /// </summary>
        public object BackgroundContent
        {
            get { return GetValue(BackgroundContentProperty); }
            set { SetValue(BackgroundContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the dialog buttons.
        /// </summary>
        public IEnumerable<Button> Buttons
        {
            get { return (IEnumerable<Button>)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        /// <summary>
        /// Gets the message box result.
        /// </summary>
        /// <value>
        /// The message box result.
        /// </value>
        public MessageBoxResult MessageBoxResult => messageBoxResult;

        /// <summary>
        /// Displays a messagebox.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="title">The title.</param>
        /// <param name="button">The button.</param>
        /// <param name="owner">The window owning the messagebox. The messagebox will be located at the center of the owner.</param>
        /// <returns>The <see cref="MessageBoxResult"/>.</returns>
        public static MessageBoxResult ShowMessage(string text, string title, MessageBoxButton button, Window owner = null)
        {
            var dlg = new ModernDialog
            {
                Title = title,
                Content = new BBCodeBlock { BBCode = text, Margin = new Thickness(0, 0, 0, 8) },
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = 480,
                MaxWidth = 640,
            };
            if (owner != null)
            {
                dlg.Owner = owner;
            }

            dlg.Buttons = GetButtons(dlg, button);
            dlg.ShowDialog();
            return dlg.messageBoxResult;
        }

        /// <summary>
        /// Get buttons.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="button">The button.</param>
        /// <returns>The <see cref="T:IEnumerable{Button}"/>.</returns>
        private static IEnumerable<Button> GetButtons(ModernDialog owner, MessageBoxButton button)
        {
            if (button == MessageBoxButton.OK)
            {
                yield return owner.OkButton;
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                yield return owner.OkButton;
                yield return owner.CancelButton;
            }
            else if (button == MessageBoxButton.YesNo)
            {
                yield return owner.YesButton;
                yield return owner.NoButton;
            }
            else if (button == MessageBoxButton.YesNoCancel)
            {
                yield return owner.YesButton;
                yield return owner.NoButton;
                yield return owner.CancelButton;
            }
        }
    }
}
