using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Counter
{
    public partial class CounterPage : ContentPage
    {
        private int value;
        private string counterName;

        public CounterPage(string name, int initialValue = 0)
        {
            InitializeComponent();

            counterName = name;
            value = initialValue;

            CounterLabel.Text = value.ToString();
            Title = counterName;
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            value++;
            CounterLabel.Text = value.ToString();
            await UpdateCounterValue();
        }

        private async void SubstractBtn_Clicked(object sender, EventArgs e)
        {
            value--;
            CounterLabel.Text = value.ToString();
            await UpdateCounterValue();
        }

        private async void ReturnBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async Task UpdateCounterValue()
        {
            try
            {
                string path = Path.Combine(FileSystem.AppDataDirectory, "counters.txt");

                if (!File.Exists(path))
                    return;

                var lines = await File.ReadAllLinesAsync(path);

                for (int i = 0; i < lines.Length; i++)
                {
                    var parts = lines[i].Split(';');

                    if (parts.Length == 2 && parts[0] == counterName)
                    {
                        lines[i] = $"{counterName};{value}";
                        break;
                    }
                }

                await File.WriteAllLinesAsync(path, lines);
            }
            catch (Exception ex)
            {
                await DisplayAlert("B³¹d zapisu", ex.Message, "OK");
            }
        }
    }
}
