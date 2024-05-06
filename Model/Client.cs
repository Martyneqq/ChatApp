using System;
using System.Net;
using System.Net.Sockets;
using System.Printing;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

public class Client
{
    private Socket socket;
    private IPEndPoint ep;
    private byte[] bufferRecv;
    private ArraySegment<byte> bufferRecvSegment;
    private TextBox messageTextBox;

    public Client(IPAddress address, int port, TextBox textBox)
    {
        messageTextBox = textBox;
        bufferRecv = new byte[4096];
        bufferRecvSegment = new(bufferRecv);
        ep = new IPEndPoint(address, port);
        socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
    }
    public void Start()
    {
        _ = Task.Run(async () =>
        {
            SocketReceiveMessageFromResult res;
            while (true)
            {
                res = await socket.ReceiveMessageFromAsync(bufferRecvSegment, SocketFlags.None, ep);
                string message = Encoding.UTF8.GetString(bufferRecv, 0, res.ReceivedBytes);
                messageTextBox.Dispatcher.Invoke(() => messageTextBox.AppendText($"{message}\n"));
            }
        });
    }
    public async Task Send(byte[] data)
    {
        var s = new ArraySegment<byte>(data);
        await socket.SendToAsync(s, SocketFlags.None, ep);
    }
}
