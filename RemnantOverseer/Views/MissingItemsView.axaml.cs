using Avalonia.Controls;
using Avalonia.Interactivity;
using RemnantOverseer.ViewModels;
using System;

namespace RemnantOverseer.Views;

public partial class MissingItemsView : UserControl
{
    public MissingItemsView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext as MissingItemsViewModel is null) throw new Exception("DataContext is still empty");
        ((MissingItemsViewModel)DataContext).OnViewLoaded();
    }
}