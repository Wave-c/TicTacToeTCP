using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TicTacToe2
{
    /// <summary>
    /// Логика взаимодействия для Lobby.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        public LobbyWindow()
        {
            InitializeComponent();
        }

        private char sym;

        private async void IHostButton_Click(object sender, RoutedEventArgs e)
        {
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            tcpListener.Start();

            var tcpClient = await tcpListener.AcceptTcpClientAsync();

            RadioButton_IsChecked();
            var window = new MainWindow(sym, tcpClient);
            window.Show();
            this.Close();
        }

        private async void IClientButtonClick(object sender, RoutedEventArgs e)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(IPAddress.Parse("127.0.0.1"), 8888);

            RadioButton_IsChecked();
            var window = new MainWindow(sym, tcpClient);
            window.Show();
            this.Close();
        }

        private void RadioButton_IsChecked()
        {
            if((bool)_rbR.IsChecked)
            {
                int r = new Random().Next(0, 2);
                sym = r == 1 ? 'X' : 'O';
            }
            else if((bool)_rbX.IsChecked)
            {
                sym = 'X';
            }
            else
            {
                sym = 'O';
            }
        }
    }
}
