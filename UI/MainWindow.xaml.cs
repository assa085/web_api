using System.Windows;
using System.Windows.Controls;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string WaitingResponse = "Ожидание ответа...";
        SenderRequests _senderRequests;
        public MainWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;


            _senderRequests = new SenderRequests("http://localhost:5000");
        }

        private async void ButtonGetApi_Click(object sender, RoutedEventArgs e)
        {
            var data = "/status";
            await SendRequestAsync(data, ButtonGetApi);
        }
        
        private async void ButtonGetSQLVersion_Click(object sender, RoutedEventArgs e)
        {
            var data = "/version";
            await SendRequestAsync(data, ButtonGetSQLVersion);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TB.Text);
        }

        async Task UpdateUiWithRequestAsync(string data)
        {
            TB.Text = WaitingResponse;
            TB.Text = await _senderRequests.GetRequest(data);
        }
        async Task SendRequestAsync(string data,Button button)
        {
            button.IsEnabled = false;
            await UpdateUiWithRequestAsync(data);
            button.IsEnabled = true;
        }
        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {   

            if (sender is System.Windows.Controls.Primitives.ToggleButton btn)
            {
                btn.IsEnabled = false;
                try
                {
                    bool isConnected = await _senderRequests.GetStatus();
                    string path = "/connect";

                    await UpdateUiWithRequestAsync(path);

                    btn.IsChecked = await _senderRequests.GetStatus();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    btn.IsEnabled = true;
                }
               
            }
        }

    }
}