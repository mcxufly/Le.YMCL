using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Threading;
using Newtonsoft.Json;
using LeYMCL.Main.Public;
using LeYMCL.Main.Public.Classes;
using LeYMCL.Main.Public.Controls.TaskManage;

namespace LeYMCL.Main;

public partial class TaskCenterWindow : Window
{
    public WindowTitleBarStyle titleBarStyle;

    public TaskCenterWindow()
    {
        InitializeComponent();
        Hide();

        PropertyChanged += (s, e) =>
        {
            if (titleBarStyle == WindowTitleBarStyle.Ymcl && e.Property.Name == nameof(WindowState))
                switch (WindowState)
                {
                    case WindowState.Normal:
                        Root.Margin = new Thickness(0);
                        break;
                    case WindowState.Maximized:
                        Root.Margin = new Thickness(20);
                        break;
                }
        };
        UpdateTaskNumber();
    }

    public async void UpdateTaskNumber()
    {
        await Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(100);
                Dispatcher.UIThread.Invoke(() =>
                {
                    var tasks = TaskContainer.Children;
                    var index = 1;
                    foreach (var item in tasks)
                    {
                        var task = item as PageTaskEntry;
                        task.TaskNumber.Text = $"#{index}";
                        index++;
                    }
                });
            }
        });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        Hide();
        base.OnLoaded(e);
        Hide();
        SystemDecorations = SystemDecorations.Full;

        var setting = Const.Data.Setting;
        titleBarStyle = setting.WindowTitleBarStyle;
        switch (setting.WindowTitleBarStyle)
        {
            case WindowTitleBarStyle.Unset:
            case WindowTitleBarStyle.System:
                TitleBar.IsVisible = false;
                Root.CornerRadius = new CornerRadius(0, 0, 8, 8);
                TaskContainer.Margin = new Thickness(10);
                ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
                ExtendClientAreaToDecorationsHint = false;
                break;
            case WindowTitleBarStyle.Ymcl:
                TitleBar.IsVisible = true;
                Root.CornerRadius = new CornerRadius(8);
                WindowState = WindowState.Maximized;
                WindowState = WindowState.Normal;
                ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
                ExtendClientAreaToDecorationsHint = true;
                if (setting.CustomBackGround == CustomBackGroundWay.Image)
                {
                    TitleBar.Margin = new Thickness(0, 0, 0, 10);
                }

                break;
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
        base.OnClosing(e);
    }
}