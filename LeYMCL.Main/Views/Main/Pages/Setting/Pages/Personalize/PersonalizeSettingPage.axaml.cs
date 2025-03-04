using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using Newtonsoft.Json;
using LeYMCL.Main.Public;
using LeYMCL.Main.Public.Langs;

namespace LeYMCL.Main.Views.Main.Pages.Setting.Pages.Personalize;

public partial class PersonalizeSettingPage : UserControl
{
    private bool _firstLoad = true;

    public PersonalizeSettingPage()
    {
        InitializeComponent();
        ControlProperty();
        BindingEvent();
    }

    private void BindingEvent()
    {
        Application.Current.ActualThemeVariantChanged += async (_, _) =>
        {
            Method.Ui.UpdateTheme();
            await Task.Delay(20);
            Method.Ui.SetWindowBackGroundImg();
        };


        Loaded += async (s, e) =>
        {
            Method.Ui.PageLoadAnimation((0, 50, 0, -50), (0, 0, 0, 0), TimeSpan.FromSeconds(0.30), Root, true);
            if (_firstLoad)
            {
                _firstLoad = false;
                var setting = Const.Data.Setting;
                WindowTitleBarStyleComboBox.SelectedIndex =
                    setting.WindowTitleBarStyle == WindowTitleBarStyle.System ? 0 : 1;
            }

            await Task.Delay(20);
            ColorPicker.Width = ColorPickerRoot.Bounds.Width - 2 * 6.5 - ColorPickerLabel.Bounds.Width - 30;
            LyricColorPicker.Width = LyricRoot.Bounds.Width - 2 * 6.5 - LyricColorPickerLabel.Bounds.Width - 30;
        };
        SizeChanged += (s, e) =>
        {
            try
            {
                ColorPicker.Width = ColorPickerRoot.Bounds.Width - 2 * 6.5 - ColorPickerLabel.Bounds.Width - 30;
                LyricColorPicker.Width = LyricRoot.Bounds.Width - 2 * 6.5 - LyricColorPickerLabel.Bounds.Width - 30;
            }
            catch
            {
            }
        };
        CustomHomePageComboBox.SelectionChanged += (s, e) =>
        {
            var setting = Const.Data.Setting;
            if (CustomHomePageComboBox.SelectedIndex == 2)
            {
                CustomHomePageComboBox.SelectedIndex = 0;
                Method.Ui.Toast(MainLang.NoSupportNow);
                return;
            }

            EditCustomHomePageBtn.IsVisible = CustomHomePageComboBox.SelectedIndex == 1 ? true : false;
            if (CustomHomePageComboBox.SelectedIndex != (int)setting.CustomHomePage)
            {
                setting.CustomHomePage = (CustomHomePageWay)CustomHomePageComboBox.SelectedIndex;
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
                Method.Ui.RestartApp();
            }
        };
        CustomBackGroundImgComboBox.SelectionChanged += (s, e) =>
        {
            var setting = Const.Data.Setting;
            if (setting.CustomBackGround == CustomBackGroundWay.AcrylicBlur && false &&
                CustomBackGroundImgComboBox.SelectedIndex == 0)
            {
                setting.CustomBackGround = CustomBackGroundWay.Default;
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
                Method.Ui.RestartApp();
            }

            EditCustomBackGroundImgBtn.IsVisible = CustomBackGroundImgComboBox.SelectedIndex == 1;
            if (CustomBackGroundImgComboBox.SelectedIndex == (int)setting.CustomBackGround) return;
            setting.CustomBackGround = (CustomBackGroundWay)CustomBackGroundImgComboBox.SelectedIndex;
            File.WriteAllText(Const.String.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            Method.Ui.SetWindowBackGroundImg();
        };
        LyricAlignComboBox.SelectionChanged += (s, e) =>
        {
            var setting = Const.Data.Setting;
            Const.Window.deskLyric.LyricText.TextAlignment = (TextAlignment)LyricAlignComboBox.SelectedIndex;
            if (setting.DeskLyricAlignment == (TextAlignment)LyricAlignComboBox.SelectedIndex) return;
            setting.DeskLyricAlignment = (TextAlignment)LyricAlignComboBox.SelectedIndex;
            File.WriteAllText(Const.String.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        };
        ColorPicker.ColorChanged += (s, e) =>
        {
            var color = ColorPicker.Color;
            var setting = Const.Data.Setting;
            if (setting.AccentColor != color)
            {
                setting.AccentColor = color;
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
            }

            Method.Ui.SetAccentColor(color);
        };
        LyricColorPicker.ColorChanged += (s, e) =>
        {
            var color = LyricColorPicker.Color;
            var setting = Const.Data.Setting;
            if (setting.DeskLyricColor != color)
            {
                setting.DeskLyricColor = color;
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
            }

            Const.Window.deskLyric.LyricText.Foreground = new SolidColorBrush(color);
        };
        EditCustomHomePageBtn.Click += (s, e) =>
        {
            var launcher = TopLevel.GetTopLevel(this).Launcher;
            launcher.LaunchFileInfoAsync(new FileInfo(Const.String.CustomHomePageXamlDataPath));
        };
        DisplayIndependentTaskWindowSwitch.Click += (s, e) =>
        {
            Const.Data.Setting.EnableDisplayIndependentTaskWindow =
                DisplayIndependentTaskWindowSwitch.IsChecked == true;
            File.WriteAllText(Const.String.SettingDataPath,
                JsonConvert.SerializeObject(Const.Data.Setting, Formatting.Indented));
        };
        LyricSizeSlider.ValueChanged += (s, e) =>
        {
            LyricSizeSliderText.Text = Math.Round(LyricSizeSlider.Value).ToString();
            var color = LyricColorPicker.Color;
            var setting = Const.Data.Setting;
            if (setting.DeskLyricSize != Math.Round(LyricSizeSlider.Value))
            {
                setting.DeskLyricSize = Math.Round(LyricSizeSlider.Value);
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
            }

            Const.Window.deskLyric.LyricText.Transitions = null;
            Const.Window.deskLyric.LyricText.FontSize = Math.Round(LyricSizeSlider.Value);
            Const.Window.deskLyric.Height = setting.DeskLyricSize + 20;
        };
        WindowTitleBarStyleComboBox.SelectionChanged += async (_, _) =>
        {
            var setting = Const.Data.Setting;
            if ((WindowTitleBarStyleComboBox.SelectedIndex == 0 &&
                 setting.WindowTitleBarStyle == WindowTitleBarStyle.System) ||
                (WindowTitleBarStyleComboBox.SelectedIndex == 1 &&
                 setting.WindowTitleBarStyle == WindowTitleBarStyle.Ymcl)) return;
            setting.WindowTitleBarStyle = WindowTitleBarStyleComboBox.SelectedIndex == 0
                ? WindowTitleBarStyle.System
                : WindowTitleBarStyle.Ymcl;
            File.WriteAllText(Const.String.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            switch (setting.WindowTitleBarStyle)
            {
                case WindowTitleBarStyle.Unset:
                    await Task.Delay(350);
                    Const.Window.main.TitleBar.IsVisible = false;
                    Const.Window.main.TitleRoot.IsVisible = false;
                    Const.Window.main.Root.CornerRadius = new CornerRadius(0, 0, 8, 8);
                    Const.Window.main.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
                    Const.Window.main.ExtendClientAreaToDecorationsHint = false;
                    var comboBox = new ComboBox
                    {
                        FontFamily = (FontFamily)Application.Current.Resources["Font"],
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };
                    comboBox.Items.Add("System");
                    comboBox.Items.Add("Ymcl");
                    comboBox.SelectedIndex = 0;
                    ContentDialog dialog = new()
                    {
                        FontFamily = (FontFamily)Application.Current.Resources["Font"],
                        Title = MainLang.WindowTitleBarStyle,
                        PrimaryButtonText = MainLang.Ok,
                        DefaultButton = ContentDialogButton.Primary,
                        Content = comboBox
                    };
                    comboBox.SelectionChanged += (_, _) =>
                    {
                        if (comboBox.SelectedIndex == 0)
                        {
                            Const.Window.main.TitleBar.IsVisible = false;
                            Const.Window.main.TitleRoot.IsVisible = false;
                            Const.Window.main.Root.CornerRadius = new CornerRadius(0, 0, 8, 8);
                            Const.Window.main.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
                            Const.Window.main.ExtendClientAreaToDecorationsHint = false;
                        }
                        else
                        {
                            Const.Window.main.TitleBar.IsVisible = true;
                            Const.Window.main.TitleRoot.IsVisible = true;
                            Const.Window.main.Root.CornerRadius = new CornerRadius(8);
                            Const.Window.main.WindowState = WindowState.Maximized;
                            Const.Window.main.WindowState = WindowState.Normal;
                            Const.Window.main.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
                            Const.Window.main.ExtendClientAreaToDecorationsHint = true;
                        }
                    };
                    dialog.PrimaryButtonClick += (_, _) =>
                    {
                        setting.WindowTitleBarStyle = comboBox.SelectedIndex == 0
                            ? WindowTitleBarStyle.System
                            : WindowTitleBarStyle.Ymcl;
                        File.WriteAllText(Const.String.SettingDataPath,
                            JsonConvert.SerializeObject(setting, Formatting.Indented));
                    };
                    await dialog.ShowAsync(Const.Window.main);
                    break;
                case WindowTitleBarStyle.System:
                    Const.Window.main.TitleBar.IsVisible = false;
                    Const.Window.main.TitleRoot.IsVisible = false;
                    Const.Window.main.Root.CornerRadius = new CornerRadius(0, 0, 8, 8);
                    Const.Window.main.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.Default;
                    Const.Window.main.ExtendClientAreaToDecorationsHint = false;
                    break;
                case WindowTitleBarStyle.Ymcl:
                    Const.Window.main.TitleBar.IsVisible = true;
                    Const.Window.main.TitleRoot.IsVisible = true;
                    Const.Window.main.Root.CornerRadius = new CornerRadius(8);
                    Const.Window.main.WindowState = WindowState.Maximized;
                    Const.Window.main.WindowState = WindowState.Normal;
                    Const.Window.main.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
                    Const.Window.main.ExtendClientAreaToDecorationsHint = true;
                    break;
            }
        };
        OpenFileWayComboBox.SelectionChanged += (s, e) =>
        {
            var setting = Const.Data.Setting;
            setting.OpenFileWay = (OpenFileWay)OpenFileWayComboBox.SelectedIndex;
            File.WriteAllText(Const.String.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        };
        LanguageComboBox.SelectionChanged += (s, e) =>
        {
            var setting = Const.Data.Setting;
            if (LanguageComboBox.SelectedItem.ToString().Split(' ')[0] != setting.Language)
            {
                setting.Language = LanguageComboBox.SelectedItem.ToString().Split(' ')[0];
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
                Method.Ui.RestartApp();
            }
        };
        ThemeComboBox.SelectionChanged += (_, _) =>
        {
            var setting = Const.Data.Setting;
            if (ThemeComboBox.SelectedIndex != (int)setting.Theme)
            {
                setting.Theme = (Theme)ThemeComboBox.SelectedIndex;
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
                if (setting.Theme == Public.Theme.Light)
                    Method.Ui.ToggleTheme(Public.Theme.Light);
                else if (setting.Theme == Public.Theme.Dark)
                    Method.Ui.ToggleTheme(Public.Theme.Dark);
                else if (setting.Theme == Public.Theme.System)
                    Method.Ui.ToggleTheme(Public.Theme.System);
            }

            // Method.Ui.RestartApp();
        };
        LauncherVisibilityComboBox.SelectionChanged += (_, _) =>
        {
            var setting = Const.Data.Setting;
            if (LauncherVisibilityComboBox.SelectedIndex != (int)setting.LauncherVisibility)
            {
                setting.LauncherVisibility = (LauncherVisibility)LauncherVisibilityComboBox.SelectedIndex;
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
            }
        };
        EditCustomBackGroundImgBtn.Click += async (_, _) =>
        {
            var path = (await Method.IO.OpenFilePicker(TopLevel.GetTopLevel(this),
                    new FilePickerOpenOptions()
                        { Title = MainLang.SelectImgFile, FileTypeFilter = new[] { FilePickerFileTypes.ImageAll } },
                    MainLang.SelectImgFile))
                .FirstOrDefault();

            if (path != null)
            {
                var base64 = Method.Value.BytesToBase64(File.ReadAllBytes(path.Path));

                var setting = Const.Data.Setting;
                setting.WindowBackGroundImgData = base64;
                File.WriteAllText(Const.String.SettingDataPath,
                    JsonConvert.SerializeObject(setting, Formatting.Indented));
            }

            Method.Ui.SetWindowBackGroundImg();
        };
    }

    private void ControlProperty()
    {
        var setting = Const.Data.Setting;
        OpenFileWayComboBox.SelectedIndex = (int)setting.OpenFileWay;
        var langs = new List<string>
        {
            $"zh-CN {MainLang.zh_CN}", $"zh-Hant {MainLang.zh_Hant}", $"en-US {MainLang.en_US}",
            $"ja-JP {MainLang.ja_JP}", $"ru-RU {MainLang.ru_RU}"
        };
        langs.ForEach(lang => { LanguageComboBox.Items.Add(lang); });
        langs.ForEach(lang =>
        {
            var arr = lang.Split(' ');
            if (arr[0] == setting.Language) LanguageComboBox.SelectedItem = lang;
        });
        if (setting.Language == null || setting.Language == string.Empty)
        {
            LanguageComboBox.SelectedItem = "zh-CN";
            setting.Language = "zh-CN";
            File.WriteAllText(Const.String.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        ThemeComboBox.SelectedIndex = (int)setting.Theme;
        LauncherVisibilityComboBox.SelectedIndex = (int)setting.LauncherVisibility;
        CustomHomePageComboBox.SelectedIndex = (int)setting.CustomHomePage;
        LyricAlignComboBox.SelectedIndex = (int)setting.DeskLyricAlignment;
        EditCustomHomePageBtn.IsVisible = CustomHomePageComboBox.SelectedIndex == 1 ? true : false;
        ColorPicker.Color = setting.AccentColor;
        LyricColorPicker.Color = setting.DeskLyricColor;
        LyricSizeSlider.Value = setting.DeskLyricSize;
        LyricSizeSliderText.Text = Math.Round(setting.DeskLyricSize).ToString();
        CustomBackGroundImgComboBox.SelectedIndex = (int)setting.CustomBackGround;
        EditCustomBackGroundImgBtn.IsVisible = CustomBackGroundImgComboBox.SelectedIndex == 1;
    }
}