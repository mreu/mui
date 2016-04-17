namespace FirstFloor.ModernUI.Presentation
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    using FirstFloor.ModernUI.Windows.Navigation;

    /// <summary>
    /// Manages the theme, font size and accent colors for a Modern UI application.
    /// </summary>
    public class AppearanceManager
        : NotifyPropertyChanged
    {
        /// <summary>
        /// The location of the dark theme resource dictionary.
        /// </summary>
        public static readonly Uri DarkThemeSource = new Uri("/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml", UriKind.Relative);
        /// <summary>
        /// The location of the light theme resource dictionary.
        /// </summary>
        public static readonly Uri LightThemeSource = new Uri("/FirstFloor.ModernUI;component/Assets/ModernUI.Light.xaml", UriKind.Relative);

        /// <summary>
        /// The resource key for the accent color.
        /// </summary>
        public const string KeyAccentColor = "AccentColor";

        /// <summary>
        /// The resource key for the accent brush.
        /// </summary>
        public const string KeyAccent = "Accent";

        /// <summary>
        /// The resource key for the default font size.
        /// </summary>
        public const string KeyDefaultFontSize = "DefaultFontSize";

        /// <summary>
        /// The resource key for the fixed font size.
        /// </summary>
        public const string KeyFixedFontSize = "FixedFontSize";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppearanceManager"/> class.
        /// </summary>
        private AppearanceManager()
        {
            DarkThemeCommand = new RelayCommand(x => ThemeSource = DarkThemeSource, x => !DarkThemeSource.Equals(ThemeSource));
            LightThemeCommand = new RelayCommand(x => ThemeSource = LightThemeSource, x => !LightThemeSource.Equals(ThemeSource));
            SetThemeCommand = new RelayCommand(
                x =>
                    {
                        var uri = NavigationHelper.ToUri(x);
                        if (uri != null)
                        {
                            ThemeSource = uri;
                        }
                    }, x => x is Uri || x is string);
            LargeFontSizeCommand = new RelayCommand(x => FontSize = FontSize.Large);
            SmallFontSizeCommand = new RelayCommand(x => FontSize = FontSize.Small);
            AccentColorCommand = new RelayCommand(
                x =>
                    {
                        if (x is Color)
                        {
                            AccentColor = (Color)x;
                        }
                        else
                        {
                            // parse color from string
                            var str = x as string;
                            if (str != null)
                            {
                                // ReSharper disable once PossibleNullReferenceException
                                AccentColor = (Color)ColorConverter.ConvertFromString(str);
                            }
                        }
                    }, x => x is Color || x is string);
        }

        /// <summary>
        /// Get theme dictionary.
        /// </summary>
        /// <returns>The <see cref="ResourceDictionary"/>.</returns>
        private ResourceDictionary GetThemeDictionary()
        {
            // determine the current theme by looking at the app resources and return the first dictionary having the resource key 'WindowBackground' defined.
            return (from dict in Application.Current.Resources.MergedDictionaries
                    where dict.Contains("WindowBackground")
                    select dict).FirstOrDefault();
        }

        /// <summary>
        /// Get theme source.
        /// </summary>
        /// <returns>The <see cref="Uri"/>.</returns>
        private Uri GetThemeSource()
        {
            var dict = GetThemeDictionary();
            return dict?.Source;
        }

        /// <summary>
        /// Set theme source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="useThemeAccentColor">The useThemeAccentColor.</param>
        /// <exception cref="ArgumentNullException">If source is null.</exception>
        private void SetThemeSource(Uri source, bool useThemeAccentColor)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var oldThemeDict = GetThemeDictionary();
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var themeDict = new ResourceDictionary { Source = source };

            // if theme defines an accent color, use it
            var accentColor = themeDict[KeyAccentColor] as Color?;
            if (accentColor.HasValue)
            {
                // remove from the theme dictionary and apply globally if useThemeAccentColor is true
                themeDict.Remove(KeyAccentColor);

                if (useThemeAccentColor)
                {
                    ApplyAccentColor(accentColor.Value);
                }
            }

            // add new before removing old theme to avoid dynamicresource not found warnings
            dictionaries.Add(themeDict);

            // remove old theme
            if (oldThemeDict != null)
            {
                dictionaries.Remove(oldThemeDict);
            }

            OnPropertyChanged("ThemeSource");
        }

        /// <summary>
        /// Apply the accent color.
        /// </summary>
        /// <param name="accentColor">The accentColor.</param>
        private void ApplyAccentColor(Color accentColor)
        {
            // set accent color and brush resources
            Application.Current.Resources[KeyAccentColor] = accentColor;
            Application.Current.Resources[KeyAccent] = new SolidColorBrush(accentColor);
        }

        /// <summary>
        /// Get the font size.
        /// </summary>
        /// <returns>The <see cref="FontSize"/>.</returns>
        private FontSize GetFontSize()
        {
            var defaultFontSize = Application.Current.Resources[KeyDefaultFontSize] as double?;

            if (defaultFontSize.HasValue)
            {
                return defaultFontSize.Value <= 12.1 ? FontSize.Small : FontSize.Large;
            }

            // default large
            return FontSize.Large;
        }

        /// <summary>
        /// Set font size.
        /// </summary>
        /// <param name="fontSize">The fontSize.</param>
        private void SetFontSize(FontSize fontSize)
        {
            if (GetFontSize() == fontSize)
            {
                return;
            }

            Application.Current.Resources[KeyDefaultFontSize] = fontSize == FontSize.Small ? 12D : 13D;
            Application.Current.Resources[KeyFixedFontSize] = fontSize == FontSize.Small ? 10.667D : 13.333D;

            OnPropertyChanged("FontSize");
        }

        /// <summary>
        /// Get accent color.
        /// </summary>
        /// <returns>The <see cref="Color"/>.</returns>
        private Color GetAccentColor()
        {
            var accentColor = Application.Current.Resources[KeyAccentColor] as Color?;

            if (accentColor.HasValue)
            {
                return accentColor.Value;
            }

            // default color: teal
            return Color.FromArgb(0xff, 0x1b, 0xa1, 0xe2);
        }

        /// <summary>
        /// Set accent color.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetAccentColor(Color value)
        {
            ApplyAccentColor(value);

            // re-apply theme to ensure brushes referencing AccentColor are updated
            var themeSource = GetThemeSource();
            if (themeSource != null)
            {
                SetThemeSource(themeSource, false);
            }

            OnPropertyChanged("AccentColor");
        }

        /// <summary>
        /// Gets the current <see cref="AppearanceManager"/> instance.
        /// </summary>
        public static AppearanceManager Current { get; } = new AppearanceManager();

        /// <summary>
        /// Gets the command that sets the dark theme.
        /// </summary>
        public ICommand DarkThemeCommand { get; private set; }
        /// <summary>
        /// Gets the command that sets the light color theme.
        /// </summary>
        public ICommand LightThemeCommand { get; private set; }
        /// <summary>
        /// Gets the command that sets a custom theme.
        /// </summary>
        public ICommand SetThemeCommand { get; private set; }
        /// <summary>
        /// Gets the command that sets the large font size.
        /// </summary>
        public ICommand LargeFontSizeCommand { get; private set; }
        /// <summary>
        /// Gets the command that sets the small font size.
        /// </summary>
        public ICommand SmallFontSizeCommand { get; private set; }
        /// <summary>
        /// Gets the command that sets the accent color.
        /// </summary>
        public ICommand AccentColorCommand { get; private set; }

        /// <summary>
        /// Gets or sets the current theme source.
        /// </summary>
        public Uri ThemeSource
        {
            get { return GetThemeSource(); }
            set { SetThemeSource(value, true); }
        }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public FontSize FontSize
        {
            get { return GetFontSize(); }
            set { SetFontSize(value); }
        }

        /// <summary>
        /// Gets or sets the accent color.
        /// </summary>
        public Color AccentColor
        {
            get { return GetAccentColor(); }
            set { SetAccentColor(value); }
        }
    }
}
