namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a token buffer.
    /// </summary>
    internal class TokenBuffer
    {
        /// <summary>
        /// The tokens (readonly). Value: new List&lt;Token&gt;().
        /// </summary>
        private readonly List<Token> tokens = new List<Token>();

        /// <summary>
        /// The position.
        /// </summary>
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBuffer"/> class.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        public TokenBuffer(Lexer lexer)
        {
            if (lexer == null)
            {
                throw new ArgumentNullException(nameof(lexer));
            }

            Token token;
            do
            {
                token = lexer.NextToken();
                tokens.Add(token);
            }
            while (token.TokenType != Lexer.TokenEnd);
        }

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of tokens to look ahead.</param>
        /// <returns>The <see cref="Token"/>.</returns>
        public Token LA(int count)
        {
            var index = position + count - 1;
            if (index < tokens.Count)
            {
                return tokens[index];
            }

            return Token.End;
        }

        /// <summary>
        /// Consumes the next token.
        /// </summary>
        public void Consume()
        {
            position++;
        }
    }
}
