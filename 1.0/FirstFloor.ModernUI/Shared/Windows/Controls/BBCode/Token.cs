namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    using System.Globalization;

    /// <summary>
    /// Represents a single token.
    /// </summary>
    internal class Token
    {
        /// <summary>
        /// Represents the token that marks the end of the input.
        /// </summary>
        /// <remarks>The token is immutable.</remarks>
        public static readonly Token End = new Token(string.Empty, Lexer.TokenEnd);

        /// <summary>
        /// The value (readonly).
        /// </summary>
        private readonly string value;

        /// <summary>
        /// The token type (readonly).
        /// </summary>
        private readonly int tokenType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="tokenType">Type of the token.</param>
        public Token(string value, int tokenType)
        {
            this.value = value;
            this.tokenType = tokenType;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value => value;

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type.</value>
        public int TokenType => tokenType;

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", tokenType, value);
        }
    }
}
