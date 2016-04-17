namespace FirstFloor.ModernUI.Win32
{
    /// <summary>
    /// The monitor dpi type enum.
    /// </summary>
    internal enum MonitorDpiType
    {
        /// <summary>
        /// The EffectiveDpi = 0.
        /// </summary>
        EffectiveDpi = 0,

        /// <summary>
        /// The AngularDpi = 1.
        /// </summary>
        AngularDpi = 1,

        /// <summary>
        /// The RawDpi = 2.
        /// </summary>
        RawDpi = 2,

        /// <summary>
        /// The Default = EffectiveDpi.
        /// </summary>
        Default = EffectiveDpi,
    }
}
