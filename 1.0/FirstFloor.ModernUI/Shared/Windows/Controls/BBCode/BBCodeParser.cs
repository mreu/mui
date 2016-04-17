namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using FirstFloor.ModernUI.Windows.Navigation;

    /// <summary>
    /// Represents the BBCode parser.
    /// </summary>
    internal class BBCodeParser
        : Parser<Span>
    {
        // supporting a basic set of BBCode tags
        /// <summary>
        /// The tag bold (const). Value: "b".
        /// </summary>
        private const string TagBold = "b";

        /// <summary>
        /// The tag color (const). Value: "color".
        /// </summary>
        private const string TagColor = "color";

        /// <summary>
        /// The tag italic (const). Value: "i".
        /// </summary>
        private const string TagItalic = "i";

        /// <summary>
        /// The tag size (const). Value: "size".
        /// </summary>
        private const string TagSize = "size";

        /// <summary>
        /// The tag underline (const). Value: "u".
        /// </summary>
        private const string TagUnderline = "u";

        /// <summary>
        /// The tag url (const). Value: "url".
        /// </summary>
        private const string TagUrl = "url";

        /// <summary>
        /// The parse context class.
        /// </summary>
        public class ParseContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ParseContext"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public ParseContext(Span parent)
            {
                Parent = parent;
            }

            /// <summary>
            /// Sets the parent.
            /// </summary>
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public Span Parent { private get; set; }

            /// <summary>
            /// Sets the font size.
            /// </summary>
            public double? FontSize { private get; set; }

            /// <summary>
            /// Sets the font weight.
            /// </summary>
            public FontWeight? FontWeight { private get; set; }

            /// <summary>
            /// Sets the font style.
            /// </summary>
            public FontStyle? FontStyle { private get; set; }

            /// <summary>
            /// Sets the foreground.
            /// </summary>
            public Brush Foreground { private get; set; }

            /// <summary>
            /// Sets the text decorations.
            /// </summary>
            public TextDecorationCollection TextDecorations { private get; set; }

            /// <summary>
            /// Gets or sets the navigate uri.
            /// </summary>
            public string NavigateUri { get; set; }

            /// <summary>
            /// Creates a run reflecting the current context settings.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns>The <see cref="Run"/>.</returns>
            public Run CreateRun(string text)
            {
                var run = new Run { Text = text };
                if (FontSize.HasValue)
                {
                    run.FontSize = FontSize.Value;
                }

                if (FontWeight.HasValue)
                {
                    run.FontWeight = FontWeight.Value;
                }

                if (FontStyle.HasValue)
                {
                    run.FontStyle = FontStyle.Value;
                }

                if (Foreground != null)
                {
                    run.Foreground = Foreground;
                }

                run.TextDecorations = TextDecorations;

                return run;
            }
        }

        /// <summary>
        /// The source.
        /// </summary>
        private readonly FrameworkElement source;

        /// <summary>
        /// Initializes a new instance of the <see cref="BBCodeParser"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="source">The framework source element this parser operates in.</param>
        public BBCodeParser(string value, FrameworkElement source)
            : base(new BBCodeLexer(value))
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
        }

        /// <summary>
        /// Gets or sets the available navigable commands.
        /// </summary>
        public CommandDictionary Commands { get; set; }

        /// <summary>
        /// Parse tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="start">The start.</param>
        /// <param name="context">The context.</param>
        private void ParseTag(string tag, bool start, ParseContext context)
        {
            if (tag == TagBold)
            {
                context.FontWeight = null;
                if (start)
                {
                    context.FontWeight = FontWeights.Bold;
                }
            }
            else if (tag == TagColor)
            {
                if (start)
                {
                    var token = LA(1);
                    if (token.TokenType == BBCodeLexer.TokenAttribute)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        var color = (Color)ColorConverter.ConvertFromString(token.Value);
                        context.Foreground = new SolidColorBrush(color);

                        Consume();
                    }
                }
                else
                {
                    context.Foreground = null;
                }
            }
            else if (tag == TagItalic)
            {
                if (start)
                {
                    context.FontStyle = FontStyles.Italic;
                }
                else
                {
                    context.FontStyle = null;
                }
            }
            else if (tag == TagSize)
            {
                if (start)
                {
                    var token = LA(1);
                    if (token.TokenType == BBCodeLexer.TokenAttribute)
                    {
                        context.FontSize = Convert.ToDouble(token.Value);

                        Consume();
                    }
                }
                else
                {
                    context.FontSize = null;
                }
            }
            else if (tag == TagUnderline)
            {
                context.TextDecorations = start ? TextDecorations.Underline : null;
            }
            else if (tag == TagUrl)
            {
                if (start)
                {
                    var token = LA(1);
                    if (token.TokenType == BBCodeLexer.TokenAttribute)
                    {
                        context.NavigateUri = token.Value;
                        Consume();
                    }
                }
                else
                {
                    context.NavigateUri = null;
                }
            }
        }

        /// <summary>
        /// Parse.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <exception cref="ParseException">Thrown if unexpected token.</exception>
        /// <exception cref="ParseException">Thrown if unknown token.</exception>
        private void Parse(Span span)
        {
            var context = new ParseContext(span);

            while (true)
            {
                var token = LA(1);
                Consume();

                if (token.TokenType == BBCodeLexer.TokenStartTag)
                {
                    ParseTag(token.Value, true, context);
                }
                else if (token.TokenType == BBCodeLexer.TokenEndTag)
                {
                    ParseTag(token.Value, false, context);
                }
                else if (token.TokenType == BBCodeLexer.TokenText)
                {
                    var parent = span;
                    Uri uri;
                    string parameter;
                    string targetName;

                    // parse uri value for optional parameter and/or target, eg [url=cmd://foo|parameter|target]
                    if (NavigationHelper.TryParseUriWithParameters(context.NavigateUri, out uri, out parameter, out targetName))
                    {
                        var link = new Hyperlink();

                        // assign ICommand instance if available, otherwise set NavigateUri
                        ICommand command;
                        if (Commands != null && Commands.TryGetValue(uri, out command))
                        {
                            link.Command = command;
                            link.CommandParameter = parameter;
                            if (targetName != null)
                            {
                                link.CommandTarget = source.FindName(targetName) as IInputElement;
                            }
                        }
                        else
                        {
                            link.NavigateUri = uri;
                            link.TargetName = parameter;
                        }

                        parent = link;
                        span.Inlines.Add(parent);
                    }

                    var run = context.CreateRun(token.Value);
                    parent.Inlines.Add(run);
                }
                else if (token.TokenType == BBCodeLexer.TokenLineBreak)
                {
                    span.Inlines.Add(new LineBreak());
                }
                else if (token.TokenType == BBCodeLexer.TokenAttribute)
                {
                    throw new ParseException(Resources.UnexpectedToken);
                }
                else if (token.TokenType == Lexer.TokenEnd)
                {
                    break;
                }
                else
                {
                    throw new ParseException(Resources.UnknownTokenType);
                }
            }
        }

        /// <summary>
        /// Parses the text and returns a Span containing the parsed result.
        /// </summary>
        /// <returns>The <see cref="Span"/>.</returns>
        public override Span Parse()
        {
            var span = new Span();

            Parse(span);

            return span;
        }
    }
}
