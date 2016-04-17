namespace FirstFloor.ModernUI.Win32
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The RECT struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
        /// <summary>
        /// The left.
        /// </summary>
        public int left;

        /// <summary>
        /// The top.
        /// </summary>
        public int top;

        /// <summary>
        /// The right.
        /// </summary>
        public int right;

        /// <summary>
        /// The bottom.
        /// </summary>
        public int bottom;
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
    }
}
