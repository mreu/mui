namespace FirstFloor.ModernUI.Windows.Controls.BBCode
{
    using System;

    /// <summary>
    /// Represents a character buffer.
    /// </summary>
    internal class CharBuffer
    {
        /// <summary>
        /// The value (readonly).
        /// </summary>
        private readonly string value;

        /// <summary>
        /// The position.
        /// </summary>
        private int position;

        /// <summary>
        /// The mark.
        /// </summary>
        private int mark;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharBuffer"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public CharBuffer(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.value = value;
        }

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of character to look ahead.</param>
        /// <returns>The <see cref="char"/>.</returns>
        public char LA(int count)
        {
            var index = position + count - 1;
            if (index < value.Length)
            {
                return value[index];
            }

            return char.MaxValue;
        }

        /// <summary>
        /// Marks the current position.
        /// </summary>
        public void Mark()
        {
            mark = position;
        }

        /// <summary>
        /// Gets the mark.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetMark()
        {
            if (mark < position)
            {
                return value.Substring(mark, position - mark);
            }

            return string.Empty;
        }

        /// <summary>
        /// Consumes the next character.
        /// </summary>
        public void Consume()
        {
            position++;
        }
    }
}
