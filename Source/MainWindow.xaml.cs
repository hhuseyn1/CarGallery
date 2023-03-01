using Source.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Source;

public partial class MainWindow : Window
{
    ObservableCollection<Car> Cars { get; set; }
    CancellationTokenSource token;
    DispatcherTimer timer = new DispatcherTimer();
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        timer.Interval = new TimeSpan(0, 0, 1);
    }
    private void StartBtn_Click(object sender, RoutedEventArgs e)
    {
        timer.Start();
        token = new();
        if (TgButton.IsChecked == true)
            MultiCars(token.Token);
        else
            SingleCars(token.Token);
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        token.Cancel();
    }
    private void SingleCars(CancellationToken token)
    {
        CarsList.Items.Clear();
        new Thread(() =>
        {
            var directory = new DirectoryInfo(@"..\..\..\FakeDatas");
            foreach (var file in directory.GetFiles())
            {

                if (file.Extension == ".json")
                {
                    var text = File.ReadAllText(file.FullName);

                    var carlist = JsonSerializer.Deserialize<List<Car>>(text);

                    if (carlist is not null)
                        foreach (var car in carlist)
                        {
                            if (token.IsCancellationRequested)
                            {
                                Dispatcher.Invoke(() => { Timertxtbox.Text = timer.Interval.ToString(); });
                                break;
                            }

                            Dispatcher.Invoke(() => Cars?.Add(car));
                            Dispatcher.Invoke(() => { Timertxtbox.Text = timer.Interval.ToString(); });
                            Thread.Sleep(100);
                        }
                }
            }
            Dispatcher.Invoke(() => { Timertxtbox.Text = timer.Interval.ToString(); });
        }).Start();
    }

    private void MultiCars(CancellationToken token)
    {
        var sync = new object();
        var dirInfo = new DirectoryInfo(@"..\..\..\FakeDatas");
        CarsList.Items.Clear();
        foreach (var file in dirInfo.GetFiles())
        {
            if (file.Extension == ".json")
            {
                ThreadPool.QueueUserWorkItem(s =>
                {
                    var text = File.ReadAllText(file.FullName);
                    var carsList = JsonSerializer.Deserialize<List<Car>>(text);
                    if (carsList is not null)
                    {
                        foreach (var car in carsList)
                        {
                            if (token.IsCancellationRequested)
                            {
                                Dispatcher.Invoke(() => { Timertxtbox.Text = timer.Interval.ToString(); });
                                break;
                            }
                            lock (sync)
                                Dispatcher.Invoke(() => Cars?.Add(car));

                            Dispatcher.Invoke(() => { Timertxtbox.Text = timer.Interval.ToString(); });
                            Thread.Sleep(100);
                        }
                    }
                });
            }

        }
    }
}
