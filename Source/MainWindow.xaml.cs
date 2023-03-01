using Source.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace Source;

public partial class MainWindow : Window
{
    ObservableCollection<Car>Cars { get; set; }
    CancellationTokenSource token;
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

    }
    private void StartBtn_Click(object sender, RoutedEventArgs e)
    {
        token = new();
        if(TgButton.IsChecked == true) 
            MultiCars(token);
        else
            SingleCars(token);
    }

    private void SingleCars(CancellationTokenSource token)
    {
        CarsList.Items.Clear();
    }

    private void MultiCars(CancellationTokenSource token)
    {
        CarsList.Items.Clear();

    }
}
