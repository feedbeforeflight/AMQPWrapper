using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;

namespace RabbitAdapter
{
    [Guid("DE08E197-26FA-4A0B-8542-08E7D336BC9E")]

    internal interface IRabbitAdapter

    {

        [DispId(1)]

        void message(string msg);

        [DispId(2)]

        void InitializeRabbit();

        [DispId(3)]

        void SendToExchange(string ExchangeName, string Message);
               
    }



    [Guid("80B67123-1C81-45F1-9B98-B233C0C6DFBE"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]

    public interface IRabbitAdapterEvents

    {

    }



    [Guid("BEB7F94E-522F-488C-9927-8395AFDF51AC"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IRabbitAdapterEvents))]

    public class AMQPWrapper : IRabbitAdapter

    {

        string exchangeName = "test";
        string queueName = "test";
        string routingKey = "test";

        IConnection connection;
        
        public AMQPWrapper()
        {
            
        }
        
        public void message(string msg)
        {
            MessageBox.Show(msg, "Cообщение компоненты RabbitAdapter!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public void InitializeRabbit()
        {
        
            connection = GetRabbitConnection();

        }

        public void DeinitializeRabbit(){
        }

        public void SendToExchange(string ExchangeName, string Message)
        {



        }

        public string GetFromQueue()
        {
            return "";
        }
        
        private IConnection GetRabbitConnection()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost"
            };
            IConnection conn = factory.CreateConnection();
            return conn;
        }        

        private IModel GetRabbitChannel(string exchangeName, string queueName, string routingKey)
        {
            IModel model = connection.CreateModel();
            model.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            model.QueueDeclare(queueName, false, false, false, null);
            model.QueueBind(queueName, exchangeName, routingKey, null);
            return model;
        }

        private void SendMessage()
        {
            IModel model = GetRabbitChannel(exchangeName, queueName, routingKey);
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello, world!");
            model.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
        }

        private string ReceiveIndividualMessage()
        {
            string originalMessage = "";
            IModel model = GetRabbitChannel(exchangeName, queueName, routingKey);
            BasicGetResult result = model.BasicGet(queueName, false);
            if (result == null)
            {
                // В настоящее время нет доступных сообщений.
            }
            else
            {
                byte[] body = result.Body;
                originalMessage = Encoding.UTF8.GetString(body);
            }
            return originalMessage;
        }
    }
}
