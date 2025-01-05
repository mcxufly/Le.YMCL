using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Newtonsoft.Json;
using LeYMCL.Main.Public;
using LeYMCL.Main.Public.Classes;
using LeYMCL.Main.Public.Langs;
using LeYMCL.Main.Views.Crash;
using LeYMCL.Main.Views.Main;

namespace LeYMCL.Main;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Const.Window.initialize;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UIThread.UnhandledException += UIThread_UnhandledException;
        }

        base.OnFrameworkInitializationCompleted();

        Current.Resources["Opacity"] = 1.0;
    }

    private void UIThread_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Console.WriteLine(e.Exception);
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
        try
        {
            var win = new CrashWindow(e.Exception.ToString())
            {
                Topmost = true
            };
            // win.Hide();
            // win.ShowDialog(Const.Window.main);
        }
        finally
        {
            e.Handled = true;
        }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Console.WriteLine(e);
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
        try
        {
            var win = new CrashWindow(e.ToString()!)
            {
                Topmost = true
            };
            // win.Hide();
            // win.ShowDialog(Const.Window.main);
        }
        catch
        {
            // ignored
        }
    }
}