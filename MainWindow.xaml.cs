using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.ComponentModel;

namespace WpfApp2
{
    
    public partial class MainWindow : Window
    {
        private TcpClient client;
        public StreamReader reader;
        public StreamWriter writer;
        public string receive;
        public string your_nickname;
        public String message;
        BackgroundWorker myGetWorker = new BackgroundWorker();
        BackgroundWorker mySendWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            
            myGetWorker.DoWork += new DoWorkEventHandler(backgroundWorkerGet_DoWork);
            mySendWorker.DoWork += new DoWorkEventHandler(backgroundWorkerSend_DoWork);

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress address in localIP)
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    yourip.Text = address.ToString();
                }
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Status.Text = "Waiting";
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(Port.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;

            myGetWorker.RunWorkerAsync();
            mySendWorker.WorkerSupportsCancellation = true;

        }

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (Message.Text != "")
            {
                message = Message.Text;
                mySendWorker.RunWorkerAsync();

            }
            Message.Text = "";
            Chat_Window.ScrollToEnd();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(IP.Text), int.Parse(Port.Text));
            Status.Text = "Waiting";
            try
            {
                client.Connect(IP_End);
                if (client.Connected)
                {
                    Status.Text = "Connected";
                    Chat_Window.AppendText("Connected" + "\n");
                    writer = new StreamWriter(client.GetStream());
                    reader = new StreamReader(client.GetStream());
                    writer.AutoFlush = true;

                    myGetWorker.RunWorkerAsync();
                    mySendWorker.WorkerSupportsCancellation = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                Status.Text = "Error";
            }

        }

        private void IP_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Port_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        
        private void backgroundWorkerGet_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                   
                    receive = reader.ReadLine();
                   // receive = your_nickname + ": " + receive;
             
                    this.Message.Dispatcher.Invoke((delegate () { Chat_Window.AppendText(receive + "\n"); }));
                    receive = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private void backgroundWorkerSend_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                message = your_nickname + ": " + message;
                writer.WriteLine(message);
                this.Message.Dispatcher.Invoke((delegate () { Chat_Window.AppendText(message + "\n"); }));

            }
            else
            {
                MessageBox.Show("Send failed");
            
            }
            mySendWorker.CancelAsync();
        }

        private void Set_nickname_Click(object sender, RoutedEventArgs e)
        {
            your_nickname = Nick.Text;
        }
    }
}
