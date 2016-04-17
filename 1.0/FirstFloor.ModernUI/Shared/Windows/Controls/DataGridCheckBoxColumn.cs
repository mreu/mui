namespace FirstFloor.ModernUI.Windows.Controls
{
    using System.Windows;

    /// <summary>
    /// A DataGrid checkbox column using default Modern UI element styles.
    /// </summary>
    public class DataGridCheckBoxColumn
        : System.Windows.Controls.DataGridCheckBoxColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCheckBoxColumn"/> class.
        /// </summary>
        public DataGridCheckBoxColumn()
        {
            ElementStyle = Application.Current.Resources["DataGridCheckBoxStyle"] as Style;
            EditingElementStyle = Application.Current.Resources["DataGridEditingCheckBoxStyle"] as Style;
        }
    }
}
