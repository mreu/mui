namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    using System;

    /// <summary>
    /// Provides basic parse functionality.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    internal abstract class Parser<TResult>
    {
        /// <summary>
        /// The buffer (readonly).
        /// </summary>
        private readonly TokenBuffer buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser{TResult}"/> class.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        protected Parser(Lexer lexer)
        {
            if (lexer == null)
            {
                throw new ArgumentNullException(nameof(lexer));
            }

            buffer = new TokenBuffer(lexer);
        }

        /// <summary>
        /// Performs a token look-ahead
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>The <see cref="Token"/>.</returns>
        protected Token LA(int count)
        {
            return buffer.LA(count);
        }

        /// <summary>
        /// Consumes the next token.
        /// </summary>
        protected void Consume()
        {
            buffer.Consume();
        }

        /// <summary>
        /// Determines whether the current token is in specified range.
        /// </summary>
        /// <param name="tokenTypes">The token types.</param>
        /// <returns>
        /// <c>true</c> if current token is in specified range; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsInRange(params int[] tokenTypes)
        {
            if (tokenTypes == null)
            {
                return false;
            }

            var token = LA(1);
            for (var i = 0; i < tokenTypes.Length; i++)
            {
                if (token.TokenType == tokenTypes[i])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Matches the specified token type.
        /// </summary>
        /// <param name="tokenType">Type of the token.</param>
        protected void Match(int tokenType)
        {
            if (LA(1).TokenType == tokenType)
            {
                Consume();
            }
            else
            {
                throw new ParseException("Token mismatch");
            }
        }

        /// <summary>
        /// Does not matches the specified token type
        /// </summary>
        /// <param name="tokenType">Type of the token.</param>
        protected void MatchNot(int tokenType)
        {
            if (LA(1).TokenType != tokenType)
            {
                Consume();
            }
            else
            {
                throw new ParseException("Token mismatch");
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="tokenTypes">The token types.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void MatchRange(int[] tokenTypes, int minOccurs, int maxOccurs)
        {
            var i = 0;
            while (IsInRange(tokenTypes))
            {
                Consume();
                i++;
            }

            if (i < minOccurs || i > maxOccurs)
            {
                throw new ParseException("Invalid number of tokens");
            }
        }

        /// <summary>
        /// Parses the text and returns an object of type TResult.
        /// </summary>
        /// <returns>The TResult.</returns>
        public abstract TResult Parse();
    }
}
