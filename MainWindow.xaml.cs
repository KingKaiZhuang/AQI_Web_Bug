﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string defaultURL = "https://data.moenv.gov.tw/api/v2/aqx_p_432?api_key=e8dd42e6-9b8b-43f8-991e-b3dee723a52d&limit=1000&sort=ImportDate%20desc&format=JSON";
        AQIdata aqiData = new AQIdata();
        public MainWindow()
        {
            InitializeComponent();
            UrlTextBox.Text = defaultURL;
        }

        private async void fetchButton_Click(object sender, RoutedEventArgs e)
        {
            string url = UrlTextBox.Text;
            ContentTextBox.Text = "抓取資料...";

            string data = await FetchContentAsync(url);
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
    }
}
