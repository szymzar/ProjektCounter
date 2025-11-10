using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Counter
{
    public partial class MainPage : ContentPage
    {
        private List<(string Name, int Value)> counters = new();

        public MainPage()
        {
            InitializeComponent();
            _ = LoadCountersFromFile();
        }

        private async void AddCounterButton_Clicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Nowy licznik", "Podaj nazwę licznika:", "OK", "Anuluj");

            if (string.IsNullOrWhiteSpace(name))
                return;

            counters.Add((name, 0));

            await SaveCountersToFile();
            ListCounter();

            await Navigation.PushAsync(new CounterPage(name, 0));
        }

        private void ListCounter()
        {
            CountersHeader.Children.Clear();

            foreach (var c in counters)
            {
                var button = new Button
                {
                    Text = c.Name,   
                    CornerRadius = 8,
                    BackgroundColor = Colors.LightGray,
                    FontSize = 14
                };

                button.Clicked += async (s, e) =>
                {
                    await Navigation.PushAsync(new CounterPage(c.Name, c.Value));
                };

                CountersHeader.Children.Add(button);
            }
        }

        private async Task SaveCountersToFile()
        {
            try
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, "counters.txt");

                var lines = counters.Select(c => $"{c.Name};{c.Value}");

                await File.WriteAllLinesAsync(path, lines);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd zapisu", ex.Message, "OK");
            }
        }

        private async Task LoadCountersFromFile()
        {
            try
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, "counters.txt");

                if (!File.Exists(path))
                    return;

                var lines = await File.ReadAllLinesAsync(path);

                counters.Clear();

                foreach (var line in lines)
                {
                    var parts = line.Split(';');

                    if (parts.Length == 2 && int.TryParse(parts[1], out int value))
                        counters.Add((parts[0], value));
                }

                ListCounter();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd wczytywania", ex.Message, "OK");
            }
        }
    }
}
