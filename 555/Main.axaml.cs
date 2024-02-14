using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace _555;

public partial class Main : Window
{
    public Main()
    {
        InitializeComponent();
    }

    private void OpenTobaForm(object? sender, RoutedEventArgs e)
    {
        Toba TobaForm = new Toba();
        TobaForm.Show();
    }

    private void OpenKlientForm(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OpenRabForm(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OpenRepForm(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OpenZakazForm(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}