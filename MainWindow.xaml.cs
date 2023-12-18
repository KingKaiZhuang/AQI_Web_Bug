using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AQI_Web_Bug
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string url = "https://data.moenv.gov.tw/api/v2/aqx_p_432?api_key=e8dd42e6-9b8b-43f8-991e-b3dee723a52d&limit=1000&sort=ImportDate%20desc&format=JSON";
        AQIdata aqiData = new AQIdata();
        List<Field> fields = new List<Field>();
        List<Record> records = new List<Record>();
        List<Record> selectedRecords = new List<Record>();
        public MainWindow()
        {
            InitializeComponent();
            UrlTextBox.Text = url;
        }

        private async void fetchButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTextBox.Text = "抓取資料...";

            string jsontext = await FetchContentAsync(url);
            ContentTextBox.Text = jsontext;
            aqiData = JsonSerializer.Deserialize<AQIdata>(jsontext);
            fields = aqiData.fields.ToList(); // 欄位名稱
            records = aqiData.records.ToList(); // 內容
            selectedRecords = records;
            statusTextBlock.Text = $"總共有 {records.Count} 筆資料";
            // 接著要output到畫面上
            DisplayAQIData();
        }

        private void DisplayAQIData()
        {
            RecordDataGrid.ItemsSource = records;
            DataWrapPanel.Children.Clear();

            foreach (var field in fields)
            {
                var propertyInfo = typeof(Record).GetProperty(field.id);
                if (propertyInfo != null )
                {
                    string value = propertyInfo.GetValue(records[0]) as string;
                    if (double.TryParse(value, out double v))
                    {
                        CheckBox cb = new CheckBox
                        {
                            Content = field.info.label,
                            Tag = field.id,
                            Margin = new Thickness(3),
                            Width = 120,
                            FontSize = 14,
                            FontWeight = FontWeights.Bold
                        };
                        DataWrapPanel.Children.Add(cb);
                    }
                }

            }
        }

        private async Task<string> FetchContentAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(200);
                try
                {
                    return await client.GetStringAsync(url);
                } catch(TaskCanceledException) {
                    MessageBox.Show("請求超時或被取消");
                    throw;
                } catch(Exception ex)
                {
                    MessageBox.Show($"讀取數據時發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }

        private void RecordDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
