using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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
            Design.SetDataContext(this, new CharacterSelectViewModel(new Services.SaveDataService(settingsService), settingsService));
        }
        InitializeComponent();
    }

    //public void StackPanel_Tapped(object sender, TappedEventArgs args)
    //{
    //    (DataContext as CharacterSelectViewModel).CharacterSelectedCommand.Execute((sender as Control).DataContext);
    //}
}