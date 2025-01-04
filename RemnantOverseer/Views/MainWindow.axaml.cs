using Avalonia.Controls;
using RemnantOverseer.ViewModels;

namespace RemnantOverseer.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        // https://docs.avaloniaui.net/docs/guides/implementation-guides/ide-support
        // Prevent the previewer's DataContext from being set when the application is run.
        if (Design.IsDesignMode)
        {
            // This can be before or after InitializeComponent.
            var settingsService = new Services.SettingsService();
            Design.SetDataContext(this, new MainWindowViewModel(settingsService, new Services.SaveDataService(settingsService)));
        }
        InitializeComponent();
    }
}