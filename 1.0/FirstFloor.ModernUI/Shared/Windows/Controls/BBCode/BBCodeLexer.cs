namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    /// <summary>
    /// The BBCode lexer.
    /// </summary>
    internal class BBCodeLexer
        : Lexer
    {
        /// <summary>
        /// The quote chars (readonly). Value: { '\'', '"' }.
        /// </summary>
        private static readonly char[] QuoteChars = { '\'', '"' };

        /// <summary>
        /// The whitespace chars (readonly). Value: { ' ', '\t' }.
        /// </summary>
        private static readonly char[] WhitespaceChars = { ' ', '\t' };

        /// <summary>
        /// The newline chars (readonly). Value: { '\r', '\n' }.
        /// </summary>
        private static readonly char[] NewlineChars = { '\r', '\n' };

        /// <summary>
        /// Start tag
        /// </summary>
        public const int TokenStartTag = 0;
        /// <summary>
        /// End tag
        /// </summary>
        public const int TokenEndTag = 1;
        /// <summary>
        /// Attribute
        /// </summary>
        public const int TokenAttribute = 2;
        /// <summary>
        /// Text
        /// </summary>
        public const int TokenText = 3;
        /// <summary>
        /// Line break
        /// </summary>
        public const int TokenLineBreak = 4;

        /// <summary>
        /// Normal state
        /// </summary>
        public const int StateNormal = 0;
        /// <summary>
        /// Tag state
        /// </summary>
        public const int StateTag = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="BBCodeLexer"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public BBCodeLexer(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Is tag name char.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool IsTagNameChar()
        {
            return IsInRange('A', 'Z') || IsInRange('a', 'z') || IsInRange(new[] { '*' });
        }

        /// <summary>
        /// Open tag.
        /// </summary>
        /// <returns>The <see cref="Token"/>.</returns>
        private Token OpenTag()
        {
            Match('[');
            Mark();
            while (IsTagNameChar())
            {
                Consume();
            }

            return new Token(GetMark(), TokenStartTag);
        }

        /// <summary>
        /// Close tag.
        /// </summary>
        /// <returns>The <see cref="Token"/>.</returns>
        private Token CloseTag()
        {
            Match('[');
            Match('/');

            Mark();
            while (IsTagNameChar())
            {
                Consume();
            }

            var token = new Token(GetMark(), TokenEndTag);
            Match(']');

            return token;
        }

        /// <summary>
        /// The newline.
        /// </summary>
        /// <returns>The <see cref="Token"/>.</returns>
        private Token Newline()
        {
            Match('\r', 0, 1);
            Match('\n');

            return new Token(string.Empty, TokenLineBreak);
        }

        /// <summary>
        /// The text.
        /// </summary>
        /// <returns>The <see cref="Token"/>.</returns>
        private Token Text()
        {
            Mark();
            while (LA(1) != '[' && LA(1) != char.MaxValue && !IsInRange(NewlineChars))
            {
                Consume();
            }

            return new Token(GetMark(), TokenText);
        }

        /// <summary>
        /// The attribute.
        /// </summary>
        /// <returns>The <see cref="Token"/>.</returns>
        private Token Attribute()
        {
            Match('=');
            while (IsInRange(WhitespaceChars))
            {
                Consume();
            }

            Token token;

            if (IsInRange(QuoteChars))
            {
                Consume();
                Mark();
                while (!IsInRange(QuoteChars))
                {
                    Consume();
                }

                token = new Token(GetMark(), TokenAttribute);
                Consume();
            }
            else
            {
                Mark();
                while (!IsInRange(WhitespaceChars) && LA(1) != ']' && LA(1) != char.MaxValue)
                {
                    Consume();
                }

                token = new Token(GetMark(), TokenAttribute);
            }

            while (IsInRange(WhitespaceChars))
            {
                Consume();
            }

            return token;
        }

        /// <summary>
        /// Gets the default state of the lexer.
        /// </summary>
        /// <value>The state of the default.</value>
        protected override int DefaultState => StateNormal;

        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns>The <see cref="Token"/>.</returns>
        public override Token NextToken()
        {
            if (LA(1) == char.MaxValue)
            {
                return Token.End;
            }

            if (State == StateNormal)
            {
                if (LA(1) == '[')
                {
                    if (LA(2) == '/')
                    {
                        return CloseTag();
                    }

                    var token = OpenTag();
                    PushState(StateTag);
                    return token;
                }

                if (IsInRange(NewlineChars))
                {
                    return Newline();
                }

                return Text();
            }

            if (State == StateTag)
            {
                if (LA(1) == ']')
                {
                    Consume();
                    PopState();
                    return NextToken();
                }

                return Attribute();
            }

            throw new ParseException("Invalid state");
        }
    }
}
