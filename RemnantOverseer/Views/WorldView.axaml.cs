using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using RemnantOverseer.ViewModels;
using System;

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

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext as WorldViewModel is null) throw new Exception("DataContext is still empty");
        ((WorldViewModel)DataContext).OnViewLoaded();
    }

    // Flyout can only be shown by explicitly calling it
    private void FiltersButton_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (sender is Control control)
        {
            FlyoutBase.ShowAttachedFlyout(control);
        }
    }
}
