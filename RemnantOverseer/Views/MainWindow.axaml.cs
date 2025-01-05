using Avalonia.Controls;
using RemnantOverseer.Utilities;
using RemnantOverseer.ViewModels;
using System;

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
        Win32Properties.AddWndProcHookCallback(this, WndProcHook);
    }

    private nint WndProcHook(nint hWnd, uint msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == NativeMethods.RO_WM_SHOWME)
        {
            ShowMe();
            handled = true;
        }
        return nint.Zero;
    }

    private void ShowMe()
    {
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }
        bool top = Topmost;
        Topmost = true;
        Topmost = top;
    }
}