using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls.Notifications;
using MinecraftLaunch.Classes.Models.Game;
using LeYMCL.Main.Public.Classes;
using LeYMCL.Main.Views.Crash;
using LeYMCL.Main.Views.DeskLyric;
using LeYMCL.Main.Views.Initialize;
using LeYMCL.Main.Views.Main;

namespace LeYMCL.Main.Public;

public abstract class Const
{
    public abstract class Data
    {
        public static Setting Setting { get; set; } = null;
        public static JavaEntry AutoJava { get; set; } = new() { JavaPath = "Auto", JavaVersion = "All" };
        public static List<UrlImageDataListEntry> UrlImageDataList { get; set; } = [];
        public static List<NewsDataListEntry> NewsDataList { get; set; } = [];
        public static Platform Platform { get; set; }
        public static string TranslateToken { get; set; }
        public static CrashWindow LastCrashInfoWindow { get; set; } = null;
    }

    public abstract class String
    {
        public static string AzureClientId { get; } = "c16d4d68-7751-4a8a-a2ff-d1b46688f428";
        public static string AppTitle { get; } = "Le Yu Minecraft Launcher";

        public static string UserDataRootPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Legetle.LeYMCL");

        public static string SettingDataPath { get; } = Path.Combine(UserDataRootPath, "Legetle.LeYMCL.Setting.json");

        public static string MinecraftFolderDataPath { get; } =
            Path.Combine(UserDataRootPath, "Legetle.LeYMCL.MinecraftFolder.json");

        public static string JavaDataPath { get; } = Path.Combine(UserDataRootPath, "Legetle.LeYMCL.Java.json");
        public static string AppPathDataPath { get; } = Path.Combine(UserDataRootPath, "Legetle.LeYMCL.AppPath.txt");
        public static string PlayerDataPath { get; } = Path.Combine(UserDataRootPath, "Legetle.LeYMCL.Player.json");

        public static string CustomHomePageXamlDataPath { get; } =
            Path.Combine(UserDataRootPath, "Legetle.LeYMCL.CustomHomePageXaml.xaml");

        public static string AccountDataPath { get; } = Path.Combine(UserDataRootPath, "Legetle.LeYMCL.Account.json");
        public static string PluginDataPath { get; } = Path.Combine(UserDataRootPath, "Legetle.LeYMCL.Plugin.json");
        public static string PluginFolderPath { get; } = Path.Combine(UserDataRootPath, "Plugin");
        public static string TempFolderPath { get; } = Path.Combine(UserDataRootPath, "Temp");
        public static string UpdateFolderPath { get; } = Path.Combine(UserDataRootPath, "Update");
        public static string VersionSettingFileName { get; } = "Legetle.LeYMCLSetting.Version";

        public static string UpdateApiUrl { get; } =
            "";

        public static string MusicApiUrl { get; set; } = "";
        public static string CurseForgeApiKey { get; } = "$2a$10$ndSPnOpYqH3DRmLTWJTf5Ofm7lz9uYoTGvhSj0OjJWJ8WdO4ZTsr.";
    }

    public abstract class Window
    {
        public static InitializeWindow initialize = new();
        public static MainWindow main;
        public static DeskLyric deskLyric = new();
    }

    public abstract class Notification
    {
        public static WindowNotificationManager main { get; set; }
        public static WindowNotificationManager initialize { get; set; }
    }
}