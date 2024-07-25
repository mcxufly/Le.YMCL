using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using MinecraftLaunch.Classes.Models.Game;
using NAudio.Wave;
using Newtonsoft.Json;
using YMCL.Main.Public;
using YMCL.Main.Public.Classes;
using YMCL.Main.Public.Langs;
using YMCL.Main.Views.Main.Pages.Download;
using YMCL.Main.Views.Main.Pages.Launch;
using YMCL.Main.Views.Main.Pages.More;
using YMCL.Main.Views.Main.Pages.Music;
using YMCL.Main.Views.Main.Pages.Setting;
using static YMCL.Main.Public.Classes.PlaySongListViewItemEntry;
using FileInfo = YMCL.Main.Public.Classes.FileInfo;

namespace YMCL.Main.Views.Main;

public partial class MainWindow : Window
{
    public readonly DownloadPage downloadPage = new();
    public readonly LaunchPage launchPage = new();
    public readonly MorePage morePage = new();
    public readonly MusicPage musicPage = new();
    public readonly SettingPage settingPage = new();
    private bool _isOpeningTaskCenter;
    public WindowTitleBarStyle titleBarStyle;

    public MainWindow()
    {
        InitializeComponent();
        EventBinding();
    }

    private void EventBinding()
    {
        Activated += (_, _) =>
        {
            if (settingPage.PluginSettingNavControl.IsSelected) settingPage.pluginSettingPage.ReloadPluginListUi();
        };
        TitleText.PointerPressed += (s, e) => { BeginMoveDrag(e); };
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
        AddHandler(DragDrop.DragLeaveEvent, (_, e_) => { DragTip.IsOpen = false; }, RoutingStrategies.Bubble);
        AddHandler(DragDrop.DragEnterEvent, (_, _) => { DragTip.IsOpen = true; }, RoutingStrategies.Bubble);
        AddHandler(DragDrop.DropEvent, (s, e) => { HandleDrop(s!, e); }, RoutingStrategies.Bubble);
        Nav.SelectionChanged += async (s, e) =>
        {
            switch (((NavigationViewItem)((NavigationView)s!).SelectedItem!).Tag)
            {
                case "Launch":
                    if (!_isOpeningTaskCenter)
                    {
                        launchPage.Root.IsVisible = false;
                        FrameView.Content = launchPage;
                    }

                    _isOpeningTaskCenter = false;
                    break;
                case "Setting":
                    settingPage.Root.IsVisible = false;
                    FrameView.Content = settingPage;
                    break;
                case "Download":
                    downloadPage.Root.IsVisible = false;
                    FrameView.Content = downloadPage;
                    break;
                case "Music":
                    musicPage.Root.IsVisible = false;
                    FrameView.Content = musicPage;
                    break;
                case "More":
                    morePage.Root.IsVisible = false;
                    FrameView.Content = morePage;
                    break;
                case "Task":
                    Const.Window.taskCenter.Show();
                    Const.Window.taskCenter.Activate();
                    _isOpeningTaskCenter = true;
                    Nav.SelectedItem = Nav.MenuItems[0];
                    FrameView.Content = settingPage;
                    FrameView.Content = launchPage;
                    break;
            }
        };
    }

    public void LoadWindow()
    {
        SystemDecorations = SystemDecorations.Full;

        var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath));
        FrameView.Content = launchPage;
        titleBarStyle = setting.WindowTitleBarStyle;
        switch (setting.WindowTitleBarStyle)
        {
            case WindowTitleBarStyle.System:
                TitleBar.IsVisible = false;
                TitleRoot.IsVisible = false;
                Root.CornerRadius = new CornerRadius(0, 0, 8, 8);
                ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
                ExtendClientAreaToDecorationsHint = false;
                break;
            case WindowTitleBarStyle.Ymcl:
                TitleBar.IsVisible = true;
                TitleRoot.IsVisible = true;
                Root.CornerRadius = new CornerRadius(8);
                WindowState = WindowState.Maximized;
                WindowState = WindowState.Normal;
                ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
                ExtendClientAreaToDecorationsHint = true;
                break;
        }

        Const.Window.initialize.Hide();
        Const.Window.main.Show();
        Const.Window.initialize.Close();

        if (setting.CustomHomePage == CustomHomePageWay.Local)
            try
            {
                var c = (Control)AvaloniaRuntimeXamlLoader.Load(File.ReadAllText(Const.CustomHomePageXamlDataPath));
                launchPage.CustomPageRoot.Child = c;
            }
            catch (Exception ex)
            {
                Method.Ui.ShowLongException(MainLang.CustomHomePageSourceCodeError, ex);
            }

        downloadPage.curseForgeFetcherPage.SearchModFromCurseForge();
    }

    public async void HandleDrop(object sender, DragEventArgs e)
    {
        DragTip.IsOpen = false;
        if (!e.Data.GetDataFormats().Contains(DataFormats.Files) || null == e.Data.GetFiles()) return;
        var items = e.Data.GetFiles()!.ToList();
        var files = new List<FileInfo>();
        items.ForEach(item =>
        {
            var path = item.TryGetLocalPath();
            if (Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);
                var files1 = dirInfo.GetFiles();
                foreach (var file in files1) files.Add(Method.IO.GetFileInfoFromPath(file.FullName));
            }
            else if (File.Exists(path))
            {
                files.Add(Method.IO.GetFileInfoFromPath(path));
            }
        });

        if (files.Count == 0) return;
        var jarFile = new List<FileInfo>();
        var zipFile = new List<FileInfo>();
        var audioFile = new List<FileInfo>();
        files.ForEach(file =>
        {
            switch (file.Extension)
            {
                case ".jar":
                    jarFile.Add(file);
                    break;
                case ".zip":
                    zipFile.Add(file);
                    break;
                case ".mp3":
                case ".ogg":
                case ".flac":
                case ".wav":
                    audioFile.Add(file);
                    break;
                default:
                    Method.Ui.Toast($"{MainLang.UnsupportedFileType} - {file.Extension}\n{file.Path}",
                        type: NotificationType.Error);
                    break;
            }
        });
        if (jarFile.Count > 0)
        {
            var entry = launchPage.VersionListView.SelectedItem as GameEntry;
            if (entry.Type == "BedRock")
            {
                Method.Ui.Toast(MainLang.UnableToAddModsForBedrockEdition, type: NotificationType.Error);
                return;
            }

            if (null == entry)
            {
                Method.Ui.Toast(MainLang.NoChooseGameOrCannotFindGame, type: NotificationType.Error);
                return;
            }

            Nav.SelectedItem = Nav.MenuItems[0];
            var text = string.Empty;
            jarFile.ForEach(jar => { text += $"{jar.FullName}\n"; });
            var result = await Method.Ui.ShowDialogAsync(MainLang.AddTheFollowingFilesAsModsToTheCurrentVersion + "?",
                text, b_cancel: MainLang.Cancel, b_primary: MainLang.Ok);
            if (result == ContentDialogResult.Primary)
            {
                Method.IO.TryCreateFolder(Path.Combine(Path.GetDirectoryName(entry.JarPath)!, "mods"));
                jarFile.ForEach(jar =>
                {
                    File.Copy(jar.Path, Path.Combine(Path.GetDirectoryName(entry.JarPath)!, "mods", jar.FullName),
                        true);
                });
                Method.Ui.Toast(MainLang.SuccessAdd, type: NotificationType.Success);
            }
        }

        if (zipFile.Count > 0)
        {
            Nav.SelectedItem = Nav.MenuItems[0];
            var text = string.Empty;
            zipFile.ForEach(zip => { text += $"{zip.FullName}\n"; });
            var result = await Method.Ui.ShowDialogAsync(
                MainLang.InstallTheFollowingFilesAsAnIntegrationPackageCurseforgeFormat + "?", text,
                b_cancel: MainLang.Cancel, b_primary: MainLang.Ok);
            if (result == ContentDialogResult.Primary)
                foreach (var file in zipFile)
                {
                    var importResult = await Method.Mc.ImportModPack(file.Path);
                    if (!importResult)
                        Method.Ui.Toast($"{MainLang.ImportFailed}: {file.FullName}", type: NotificationType.Error);
                    else
                        Method.Ui.Toast($"{MainLang.ImportSuccess}: {file.FullName}", type: NotificationType.Success);
                }
        }

        if (audioFile.Count > 0)
        {
            Nav.SelectedItem = Nav.MenuItems[3];
            foreach (var file in audioFile)
                using (var reader = new MediaFoundationReader(file.Path))
                {
                    var time = Method.Value.MsToTime(reader.TotalTime.TotalMilliseconds);
                    var song = new PlaySongListViewItemEntry
                    {
                        DisplayDuration = time,
                        Duration = reader.TotalTime.TotalMilliseconds,
                        Img = null,
                        SongName = file.Name,
                        Authors = file.Extension.TrimStart('.'),
                        Path = file.Path,
                        Type = PlaySongListViewItemEntryType.Local
                    };
                    musicPage.playSongList.Add(song);
                    musicPage.PlayListView.Items.Add(song);
                }

            File.WriteAllText(Const.PlayerDataPath,
                JsonConvert.SerializeObject(musicPage.playSongList, Formatting.Indented));
            musicPage.PlayListView.SelectedIndex = musicPage.PlayListView.Items.Count - 1;
        }
    }
}