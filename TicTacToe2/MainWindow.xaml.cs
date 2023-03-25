using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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

namespace TicTacToe2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient _tcpClient;
        private char sym;
        private Button[] _buttons = new Button[9];
        public MainWindow(char sym, TcpClient tcpClient)
        {
            InitializeComponent();

            _tcpClient = tcpClient;
            this.sym = sym;
            _buttons[0] = bt1;
            _buttons[1] = bt2;
            _buttons[2] = bt3;
            _buttons[3] = bt4;
            _buttons[4] = bt5;
            _buttons[5] = bt6;
            _buttons[6] = bt7;
            _buttons[7] = bt8;
            _buttons[8] = bt9;
            for(int i = 0; i < 9; i++)
            {
                _buttons[i].Content = ' ';
            }
            if (sym == 'O')
            {
                LockOrUnlockButtons(false);
                Task.Run(async () =>
                {
                    await ReceiveMessageAsync();
                });
                LockOrUnlockButtons(true);
            }
        }

        private void LockOrUnlockButtons(bool TorF)
        {
            foreach(var button in _buttons)
            {
                button.IsEnabled = TorF;
            }
        }

        private async void Bt_ClickAsync(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Content = sym;
            Message message = new Message()
            {
                BtName = ((Button)sender).Name,
                Sym = sym
            };
            WinCheck();
            try
            {
                await SendMessageAsync(message);
                LockOrUnlockButtons(false);
                await ReceiveMessageAsync();
                LockOrUnlockButtons(true);
            }
            catch(Exception ex)
            {

            }
        }

        private void WinCheck()
        {
            var a = new char[3, 3];
            for(int i = 0, k = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++, k++)
                {
                    a[i, j] = (char)_buttons[k].Content;
                }
            }
            if ('O' == a[0, 0] && 'O' == a[1, 1] && 'O' == a[2, 2] || 'O' == a[0, 2] && 'O' == a[1, 1] && 'O' == a[2, 0] || 'O' == a[0, 0] && 'O' == a[1, 0] && 'O' == a[2, 0] || 'O' == a[0, 1] && 'O' == a[1, 1] && 'O' == a[2, 1] || 'O' == a[0, 2] && 'O' == a[1, 2] && 'O' == a[2, 2] || 'O' == a[0, 0] && 'O' == a[0, 1] && 'O' == a[0, 2] || 'O' == a[1, 0] && 'O' == a[1, 1] && 'O' == a[1, 2] || 'O' == a[2, 0] && 'O' == a[2, 1] && 'O' == a[2, 2])
            {
                MessageBox.Show("O is winner!");
                this.Close();
                return;
            }
            else if ('X' == a[0, 0] && 'X' == a[1, 1] && 'X' == a[2, 2] || 'X' == a[0, 2] && 'X' == a[1, 1] && 'X' == a[2, 0] || 'X' == a[0, 0] && 'X' == a[1, 0] && 'X' == a[2, 0] || 'X' == a[0, 1] && 'X' == a[1, 1] && 'X' == a[2, 1] || 'X' == a[0, 2] && 'X' == a[1, 2] && 'X' == a[2, 2] || 'X' == a[0, 0] && 'X' == a[0, 1] && 'X' == a[0, 2] || 'X' == a[1, 0] && 'X' == a[1, 1] && 'X' == a[1, 2] || 'X' == a[2, 0] && 'X' == a[2, 1] && 'X' == a[2, 2])
            {
                MessageBox.Show("X is winner!");
                this.Close();
                return;
            }
            else if (_buttons.Where(x=>x.Content == ' '.ToString()).FirstOrDefault() != null)
            {
                MessageBox.Show("Draw");
                this.Close();
                return;
            }
        }

        private async Task SendMessageAsync(Message message)
        {
            string ResponseMessage = JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(ResponseMessage);

            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            await _tcpClient.Client.SendToAsync(buffer, SocketFlags.None, remoteEndPoint);
        }

        private async Task ReceiveMessageAsync()
        {
            var localIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            byte[] buffer = new byte[256];

            IPEndPoint endIP = new IPEndPoint(IPAddress.Any, 0);
            var result = await _tcpClient.Client.ReceiveFromAsync(buffer, SocketFlags.None, endIP);

            var message = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);

            var currentButton = JsonSerializer.Deserialize<Message>(message);

            Dispatcher.Invoke(()=>_buttons.Where(x => x.Name == currentButton.BtName).First().Content = currentButton.Sym);
        }
    }
}
