using Avalonia.Controls;
using RemnantOverseer.ViewModels;

namespace RemnantOverseer.Views;

public partial class WorldView : UserControl
{
    public WorldView()
    {
        if (Design.IsDesignMode)
        {
            // This can be before or after InitializeComponent.
            var settingsService = new Services.SettingsService();
            Design.SetDataContext(this, new WorldViewModel(new Services.SaveDataService(settingsService)));
        }
        InitializeComponent();
    }
}
