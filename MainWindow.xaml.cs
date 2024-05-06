using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatApp
{
    public enum ServerConfig
    {
        Port = 5004
    }
    public partial class MainWindow : Window
    {
        private string pattern = @"\S";
        private IPAddress ip = IPAddress.Loopback;
        int port = (int)ServerConfig.Port;

        private Client client;
        private Server server;
        public MainWindow()
        {
            InitializeComponent();
            InitializeServerAndClient();

            txt1.Focus();
        }
        private void InitializeServerAndClient()
        {
            client = new Client(ip, port, tb1);
            server = new Server(ip, port, tb1, txt2, lb2);

            client.Start();
            server.Start();
        }

        private void Users_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt1.Text))
                txt1.Background = (ImageBrush)FindResource("watermark");
            else
                txt1.Background = null;
        }

        private async void btn1_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(txt1.Text, pattern) && Regex.IsMatch(txt2.Text, pattern))
            {
                try
                {
                    string messageToSend = $"[{txt2.Text}][{DateTime.Now.ToString("H:mm:ss")}]: " + txt1.Text;
                    await client.Send(Encoding.UTF8.GetBytes(messageToSend));
                    txt1.Clear();
                    txt1.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void sendMessage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    btn1_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void exitApp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void lb2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txt2_TextChanged(object sender, TextChangedEventArgs e)
        {
            //lb2.Items.Add(txt2.Text);
        }

        private void lb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"IP: {ip}");
            sb.AppendLine($"Client port: {port}");
            sb.AppendLine("Connected clients:");
            foreach (var client in server.connectedClients)
            {
                sb.AppendLine($"{client}");
            }

            string text = sb.ToString();
            string caption = "Debug";
            MessageBox.Show(text, caption);
        }

        private void tb1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void lb2_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}