using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using OneC.ExternalComponents;

[Guid("7166906E-EAC8-43D1-9B32-4FFE8FDAEEC3")]
[ProgId("AddIn.AMQPWrapper")]
public class AMQPWrapper : ExtComponentBase
{
    private int MyNumber;
    IConnection connection;
    IModel model;

    public AMQPWrapper()
    {
        ComponentName = "AMQPWrapper";
        //MessageBox.Show("constructor", "constructor procedure", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //InitEvent += new InitEventHandler(Initialization);
    }

    ~AMQPWrapper()
    {
        Disconnect();
    }

    [Export1c]
    public void DoSomething(string msg)
    {
        //            MessageBox.Show(msg, "Cообщение компоненты RabbitAdapter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        int num = 0;
        TimerCallback tm = new TimerCallback(Count);
        // создаем таймер
        System.Threading.Timer timer = new System.Threading.Timer(tm, num, 0, 2000);

        MessageBox.Show(msg, "Cообщение компоненты RabbitAdapter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    public void Count(object obj)
    {
        Async.ExternalEvent("AddIn.AMQPWrapper", "SampleEvent", "Hi from external component! " + MyNumber.ToString());
        MyNumber++;
    }

    [Export1c]
    public bool Connect(string host, string port, string login, string password, string vhost)
    {
        //MessageBox.Show("connect method call", "Cообщение компоненты RabbitAdapter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        connection = GetRabbitConnection(host, port, login, password, vhost);

        return true;
    }

    [Export1c]
    public void Disconnect()
    {
        if (model != null)
        {
            model.Close();
            model = null;
        }
        if (connection != null)
        {
            connection.Close();
            connection = null;
        }
    }

    [Export1c]
    public bool Send(string exchangeName, string queueName, string routingKey, string Message)
    {
        SendMessage(exchangeName, routingKey, Message);

        return true;
    }

    [Export1c]
    public string Receive(string exchangeName, string queueName, string routingKey)
    {
        return ReceiveIndividualMessage(exchangeName, queueName, routingKey);
    }

    #region "rabbit interface"

    private IConnection GetRabbitConnection(string host, string port, string login, string password, string vhost)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            UserName = login,
            Password = password,
            VirtualHost = vhost,
            HostName = host + port
        };
        IConnection conn = factory.CreateConnection();
        return conn;
    }

    private void GetRabbitChannel()
    {
        if (model == null)
        {
            model = connection.CreateModel();
        }
//        model.ExchangeDeclare(exchangeName, ExchangeType.Direct);
//        model.QueueDeclare(queueName, false, false, false, null);
//        model.QueueBind(queueName, exchangeName, routingKey, null);
    }

    private void DeclareExchange(string exchangeName)
    {
        if (model != null)
        {
            model.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        }
    }

    private void BindQueue(string exchangeName, string queueName, string routingKey)
    {
        if (model != null)
        {
            model.QueueDeclare(queueName, false, false, false, null);
            model.QueueBind(queueName, exchangeName, routingKey, null);
        }
    }

    private void SendMessage(string exchangeName, string routingKey, string Message)
    {
        GetRabbitChannel();
        DeclareExchange(exchangeName);
        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(Message);
        model.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
    }

    private string ReceiveIndividualMessage(string exchangeName, string queueName, string routingKey)
    {
        string originalMessage = "";
        GetRabbitChannel();
        BindQueue(exchangeName, queueName, routingKey);
        BasicGetResult result = model.BasicGet(queueName, true);
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

    #endregion
}
