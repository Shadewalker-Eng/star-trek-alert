using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics;

namespace StarTrekAlert;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AppWindow.Resize(new SizeInt32(515, 565));
        var titleBar = AppWindow.TitleBar;
        if (ExtractIconEx(Environment.ProcessPath!, 0, nint.Zero, out windowIcon, 1) == 1 && windowIcon != nint.Zero)
        {
            AppWindow.SetIcon(Microsoft.UI.Win32Interop.GetIconIdFromIcon(windowIcon));
            titleBar.IconShowOptions = IconShowOptions.ShowIconAndSystemMenu;
            Closed += (_, _) => DestroyIcon(windowIcon);
        }
        var background = Windows.UI.Color.FromArgb(255, 0, 0, 0);
        titleBar.BackgroundColor = background;
        titleBar.InactiveBackgroundColor = background;
        titleBar.ButtonBackgroundColor = background;
        titleBar.ButtonInactiveBackgroundColor = background;
        titleBar.ForegroundColor = Microsoft.UI.Colors.White;
        titleBar.ButtonForegroundColor = Microsoft.UI.Colors.White;
        titleBar.InactiveForegroundColor = Microsoft.UI.Colors.LightGray;
        titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.LightGray;
        SelectAlert(GreenButton);
    }

    private Button? activeButton;
    private nint windowIcon;

    [DllImport("shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode)]
    private static extern uint ExtractIconEx(string file, int index, nint largeIcon, out nint smallIcon, uint count);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(nint icon);

    private void AlertClicked(object sender, RoutedEventArgs args) => SelectAlert((Button)sender);

    private void SelectAlert(Button button)
    {
        if (activeButton == button) return;
        if (activeButton is not null)
        {
            activeButton.ClearValue(Control.BackgroundProperty);
            activeButton.ClearValue(Control.ForegroundProperty);
        }
        var accent = ((SolidColorBrush)button.BorderBrush).Color;
        var dim = Color.FromArgb(255, (byte)(accent.R / 6), (byte)(accent.G / 6), (byte)(accent.B / 6));
        var illuminated = MakeRail(accent, dim);
        HeaderRail.Background = illuminated;
        UpperRail.Background = illuminated;
        LowerRail.Background = illuminated;
        HeaderSegment.Background = new SolidColorBrush(dim);
        MiddleRail.Background = new SolidColorBrush(accent);
        DisplayFrame.BorderBrush = new SolidColorBrush(dim);
        button.Background = new SolidColorBrush(accent);
        button.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);
        activeButton = button;
        string file = button == GreenButton ? "Green.gif"
            : button == YellowButton ? "Yellow.gif"
            : "Red.gif";

        if (AlertImage.Source is BitmapImage previous)
        {
            previous.Stop();
        }

        string label = AutomationProperties.GetName(button);
        StatusText.Text = (string)button.Content;
        StatusText.Foreground = button.BorderBrush;
        AutomationProperties.SetName(AlertImage, label);
        Title = $"Star Trek Alert | {label}";
        using var gif = typeof(MainWindow).Assembly
            .GetManifestResourceStream($"StarTrekAlert.Assets.{file}")!;
        using var source = gif.AsRandomAccessStream();
        var bitmap = new BitmapImage { AutoPlay = true };
        bitmap.SetSource(source);
        AlertImage.Source = bitmap;
    }

    private static LinearGradientBrush MakeRail(Color accent, Color dim)
    {
        var rail = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 0)
        };
        rail.GradientStops.Add(new GradientStop { Offset = 0, Color = dim });
        rail.GradientStops.Add(new GradientStop { Offset = 0.55, Color = accent });
        rail.GradientStops.Add(new GradientStop { Offset = 0.7, Color = accent });
        rail.GradientStops.Add(new GradientStop { Offset = 1, Color = dim });
        return rail;
    }
}






