﻿using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YMCL.Main.Public.Classes;
using YMCL.Main.Public.Langs;
using FileInfo = YMCL.Main.Public.Classes.FileInfo;
using Path = System.IO.Path;

namespace YMCL.Main.Public
{
    public class Method
    {
        public static bool isPrimaryButtonClick = false;
        public static void SetAccentColor(Color color)
        {
            Application.Current.Resources["SystemAccentColor"] = color;
            Application.Current.Resources["SystemAccentColorLight1"] = ColorVariant(color, 0.15f);
            Application.Current.Resources["SystemAccentColorLight2"] = ColorVariant(color, 0.30f);
            Application.Current.Resources["SystemAccentColorLight3"] = ColorVariant(color, 0.45f);
            Application.Current.Resources["SystemAccentColorDark1"] = ColorVariant(color, -0.15f);
            Application.Current.Resources["SystemAccentColorDark2"] = ColorVariant(color, -0.30f);
            Application.Current.Resources["SystemAccentColorDark3"] = ColorVariant(color, -0.45f);
        }
        public static Color ColorVariant(Color color, float percent)
        {
            // 确保百分比在-1到1之间  
            percent = Math.Max(-1f, Math.Min(1f, percent));

            // 计算调整后的RGB值  
            float adjust = 1f + percent; // 亮化是1+percent，暗化是1+(negative percent)，即小于1  
            int r = (int)Math.Round(color.R * adjust);
            int g = (int)Math.Round(color.G * adjust);
            int b = (int)Math.Round(color.B * adjust);

            // 确保RGB值在有效范围内  
            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));

            // 创建一个新的颜色（保持Alpha通道不变）  
            return Color.FromArgb(color.A, (byte)r, (byte)g, (byte)b);
        }
        public static void ToggleTheme(Theme theme)
        {
            if (theme == Theme.Light)
            {
                var rd = (AvaloniaXamlLoader.Load(new Uri("avares://YMCL.Main/Public/Styles/LightTheme.axaml")) as ResourceDictionary)!;
                Application.Current!.Resources.MergedDictionaries.Add(rd);
                Application.Current.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;
            }
            else if (theme == Theme.Dark)
            {
                var rd = (AvaloniaXamlLoader.Load(new Uri("avares://YMCL.Main/Public/Styles/DarkTheme.axaml")) as ResourceDictionary)!;
                Application.Current!.Resources.MergedDictionaries.Add(rd);
                Application.Current.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
            }
        }
        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Create();
            }
        }
        public static void MarginAnimation((double, double, double, double) original, (double, double, double, double) target, TimeSpan time, Control control, bool visibility = false)
        {
            var (ol, ot, or, ob) = original;
            var (tl, tt, tr, tb) = target;

            if (control != null && control.Transitions != null)
            {
                control.Transitions.Clear();
                control.Margin = new Thickness(ol, ot, or, ob);
                control.Transitions.Add(new ThicknessTransition
                {
                    Duration = time,
                    Easing = new SineEaseInOut(),
                    Property = Avalonia.Layout.Layoutable.MarginProperty
                });
                if (visibility)
                {
                    control.IsVisible = true;
                }
                control.Margin = new Thickness(tl, tt, tr, tb);
            }
        }
        public static void Toast(string msg, NotificationType type = NotificationType.Information, bool time = true, string title = "Yu Minecraft Launcher")
        {
            var showTitle = Const.AppTitle;
            if (!string.IsNullOrEmpty(title))
            {
                showTitle = title;
            }
            if (time)
            {
                showTitle += $" - {DateTime.Now.ToString("HH:mm:ss")}";
            }
            Const.notification.Show(new Notification(showTitle, msg, type));
        }
        public static async Task<List<FolderInfo>> OpenFolderPicker(TopLevel topLevel = null, FolderPickerOpenOptions options = null)
        {
            isPrimaryButtonClick = false;
            var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.OpenFileWay == OpenFileWay.FileSelectWindow)
            {
                if (options != null && topLevel != null)
                {
                    var storageProvider = topLevel!.StorageProvider;
                    var result = await storageProvider.OpenFolderPickerAsync(options);
                    var list = new List<FolderInfo>();
                    result.ToList().ForEach(item =>
                    {
                        list.Add(new FolderInfo() { Name = item.Name, Path = item.Path.LocalPath });
                    });
                    return list;
                }
                else
                {
                    new Exception("ParameterIsNull");
                    return null;
                }
            }
            else
            {
                var textBox = new TextBox() { FontFamily = (FontFamily)Application.Current.Resources["Font"], TextWrapping = TextWrapping.Wrap };
                ContentDialog dialog = new()
                {
                    FontFamily = (FontFamily)Application.Current.Resources["Font"],
                    Title = MainLang.InputFolderPath,
                    PrimaryButtonText = MainLang.Ok,
                    CloseButtonText = MainLang.Cancel,
                    DefaultButton = ContentDialogButton.Primary,
                    Content = textBox
                };
                dialog.PrimaryButtonClick += (_, _) =>
                {
                    isPrimaryButtonClick = true;
                };
                var result = await dialog.ShowAsync();
                var path = textBox.Text;
                if (!Directory.Exists(path) && isPrimaryButtonClick)
                {
                    Toast(MainLang.FolderNotExist, NotificationType.Error);
                    return null;
                }
                var folder = Path.GetFileName(path);
                var list = new List<FolderInfo>() { new FolderInfo() { Name = folder, Path = path } };
                return list;
            }
        }
        public static async Task<List<FileInfo>> OpenFilePicker(TopLevel topLevel = null, FilePickerOpenOptions options = null)
        {
            isPrimaryButtonClick = false;
            var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.OpenFileWay == OpenFileWay.FileSelectWindow)
            {
                if (options != null && topLevel != null)
                {
                    var storageProvider = topLevel!.StorageProvider;
                    var result = await storageProvider.OpenFilePickerAsync(options);
                    var list = new List<FileInfo>();
                    result.ToList().ForEach(item =>
                    {
                        var path = item.Path.LocalPath;
                        var fullName = Path.GetFileName(path);
                        var name = Path.GetFileNameWithoutExtension(fullName);
                        var extension = Path.GetExtension(fullName);
                        list.Add(new FileInfo() { Name = name, Path = path, FullName = fullName, Extension = extension });
                    });
                    return list;
                }
                else
                {
                    new Exception("ParameterIsNull");
                    return null;
                }
            }
            else
            {
                var textBox = new TextBox() { FontFamily = (FontFamily)Application.Current.Resources["Font"], TextWrapping = TextWrapping.Wrap };
                ContentDialog dialog = new()
                {
                    FontFamily = (FontFamily)Application.Current.Resources["Font"],
                    Title = MainLang.InputFilePath,
                    PrimaryButtonText = MainLang.Ok,
                    CloseButtonText = MainLang.Cancel,
                    DefaultButton = ContentDialogButton.Primary,
                    Content = textBox
                };
                dialog.PrimaryButtonClick += (_, _) =>
                {
                    isPrimaryButtonClick = true;
                };
                var result = await dialog.ShowAsync();
                var path = textBox.Text;
                if (!File.Exists(path) && isPrimaryButtonClick)
                {
                    Toast(MainLang.FileNotExist, NotificationType.Error);
                    return null;
                }
                var fullName = Path.GetFileName(path);
                var name = Path.GetFileNameWithoutExtension(fullName);
                var extension = Path.GetExtension(fullName);
                var list = new List<FileInfo>() { new FileInfo() { Name = name, Path = path, FullName = fullName, Extension = extension } };
                return list;
            }
        }
    }
}
