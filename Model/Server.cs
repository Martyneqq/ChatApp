using ChatApp;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Windows.Controls;

public class Server
{
    private Socket socket;
    private IPEndPoint ep;
    private byte[] bufferRecv;
    private ArraySegment<byte> bufferRecvSegment;
    private TextBox messageTextBox;
    private TextBox userTextBox;
    private ListBox userListBox;
    public List<IPEndPoint> connectedClients;
    public Server(IPAddress address, int port, TextBox msgTextBox, TextBox usrTextBox, ListBox listBox)
    {
        messageTextBox = msgTextBox;
        userTextBox = usrTextBox;
        userListBox = listBox;
        bufferRecv = new byte[4096];
        bufferRecvSegment = new(bufferRecv);
        ep = new IPEndPoint(address, port);
        socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
        socket.Bind(ep);

        connectedClients = new List<IPEndPoint>();
    }
    public void Start()
    {
        Task.Run(async () =>
        {
            SocketReceiveMessageFromResult res;
            while (true)
            {
                res = await socket.ReceiveMessageFromAsync(bufferRecvSegment, SocketFlags.None, ep);
                string message = Encoding.UTF8.GetString(bufferRecv, 0, res.ReceivedBytes);
                messageTextBox.Dispatcher.Invoke(() => messageTextBox.AppendText($"{message}\n"));

                if (res.RemoteEndPoint != null)
                {
                    if (!connectedClients.Contains(res.RemoteEndPoint))
                    {
                        connectedClients.Add((IPEndPoint)res.RemoteEndPoint);
                        UpdateUserList();
                    }
                }
                else
                {
                    messageTextBox.Dispatcher.Invoke(() => messageTextBox.AppendText("Received message from null remote endpoint.\n"));
                }

                await SendTo(res.RemoteEndPoint, Encoding.UTF8.GetBytes(message));
            }
        });
    }

    public async Task SendTo(EndPoint rec, byte[] data)
    {
        var s = new ArraySegment<byte>(data);
        await socket.SendToAsync(s, SocketFlags.None, rec);
    }
    private void UpdateUserList()
    {
        userListBox.Dispatcher.Invoke(() =>
        {
            userListBox.Items.Clear();
            foreach (var client in connectedClients)
            {
                userListBox.Items.Add($"{client.ToString()} ({userTextBox.Text})");
            }
        });
    }
}
