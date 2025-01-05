using Avalonia.Controls;

namespace LeYMCL.Main.Public.Controls;

public partial class ModFileView : UserControl
{
    public ModFileView(string header)
    {
        InitializeComponent();
        Expander.Header = header;
        
    }
}