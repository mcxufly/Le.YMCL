using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using LeYMCL.Main.Public.Classes;

namespace LeYMCL.Main.Public.Controls;

public partial class UrlImage : UserControl
{
    public static readonly StyledProperty<string> UrlProperty =
        AvaloniaProperty.Register<TitleBar, string>(nameof(Url),
            "null");

    public UrlImage()
    {
        InitializeComponent();
        _ = LoadImgAsync();
    }

    public string Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    public async Task LoadImgAsync()
    {
        var index = 0;
        while (Url is "null" or "Url" && index < 20)
        {
            await Task.Delay(500);
            index++;
        }

        var entry =
            Const.Data.UrlImageDataList.Find(UrlImageDataListEntry => UrlImageDataListEntry.Url == Url);
        if (entry == null)
        {
            var bitmap = await Method.Value.LoadImageFromUrlAsync(Url);
            if (bitmap != null)
            {
                Img.Source = bitmap;
                Const.Data.UrlImageDataList.Add(new UrlImageDataListEntry { Url = Url, Bitmap = bitmap });
            }
        }
        else
        {
            Img.Source = entry.Bitmap;
        }
    }
}