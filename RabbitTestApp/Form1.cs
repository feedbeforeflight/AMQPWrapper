using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;

namespace RabbitTestApp
{
    public partial class MainForm : Form
    {
        private IConnection mRabbitConnection;

        public IConnection RabbitConnection
        {
            get {
                return GetRabbitConnection();
            }
        }


        private IConnection GetRabbitConnection()
        {
            if (mRabbitConnection != null)
            {
                return mRabbitConnection;
            };

            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "test",
                Password = "test",
                HostName = "192.168.1.13",
                VirtualHost = "/",
                Protocol = Protocols.DefaultProtocol,
                Port = AmqpTcpEndpoint.UseDefaultPort
        };
            mRabbitConnection = factory.CreateConnection();
            return mRabbitConnection;
        }

        private void CloseConnections()
        {
            if (mRabbitConnection != null)
            {
                mRabbitConnection.Close();
                mRabbitConnection = null;
            }
        }
        
        public MainForm()
        {
            InitializeComponent();
        }

        ~MainForm()
        {
            CloseConnections();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string messageBuffer = textMessageToSend.Text;

            if (messageBuffer != "")
            {
                IModel channel = RabbitConnection.CreateModel();

                byte[] messageBytes = Encoding.UTF8.GetBytes(messageBuffer);

                channel.BasicPublish("test", "testRK", null, messageBytes);
            }
        }

        private void buttonReceive_Click(object sender, EventArgs e)
        {
            string messageBuffer = "";

            textMessageReceived.Text = messageBuffer;
        }

    }
 
}
