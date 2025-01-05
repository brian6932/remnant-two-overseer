using Avalonia.Controls;
using RemnantOverseer.ViewModels;

namespace RemnantOverseer.Views;

public partial class CharacterSelectView : UserControl
{
    public CharacterSelectView()
    {
        if (Design.IsDesignMode)
        {
            // This can be before or after InitializeComponent.
            var settingsService = new Services.SettingsService();
            Design.SetDataContext(this, new CharacterSelectViewModel(new Services.SaveDataService(settingsService)));
        }
        InitializeComponent();
    }
}