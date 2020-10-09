using System;
using System.IO.Ports;
using System.Threading;

namespace TOKSClasses
{
    public class ComPortPair
    {
        public SerialPort port { get; private set; }
        public SerialPort port2 { get; private set; }
        public bool isInited { get; private set; } = false;
        public string[] portNames { get; private set; }
        public string bufferedMessage { get; private set; }
        private void getPortNames()
        {
            portNames = SerialPort.GetPortNames();
            if (portNames.Length <= 3)
                throw new Exception("Cannot init ports.");
        }
        public void initPorts()
        {
            port = new SerialPort();
            port2 = new SerialPort();

            try
            {
                // настройки порта
                getPortNames();
                port.PortName = portNames[2];
                port.BaudRate = 115200;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Encoding = System.Text.Encoding.Default;
                port.Open();

                port2.PortName = portNames[3];
                port2.BaudRate = 115200;
                port2.DataBits = 8;
                port2.Parity = Parity.None;
                port2.StopBits = StopBits.One;
                port2.Encoding = System.Text.Encoding.Default;
                port2.Open();

                port2.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);

                isInited = true;
            }
            catch (Exception e)
            {
                throw new Exception("Cannot init ports.");
            }
        }
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            byte[] data = new byte[port.BytesToRead];
            if (port.IsOpen)
            {
                port.Read(data, 0, data.Length);
                this.bufferedMessage = System.Text.Encoding.Default.GetString(data);
            }
            else
            {
                throw new Exception("Port pair is not opened");
            }
        }
        public string getStatus()
        {
            if (isInited == true)
            {
                return port.PortName + " Is inited" + "\t\t" + port2.PortName + " Is inited" + " \r\n" +
                  "Baudrate: " + port.BaudRate + "\t\t" + "Baudrate: " + port2.BaudRate + "\r\n" +
                  "Databits: " + port.DataBits + "\t\t" + "Databits: " + port2.DataBits + "\r\n" +
                  "Paritycheck: " + port.Parity + "\t\t" + "Paritycheck: " + port2.Parity + "\r\n" +
                  "Stopbits: " + port.StopBits + "\t\t" + "Stopbits: " + port2.StopBits;
            }
            else return null;
        }
        public void sendMessage(string data)
        {
            if (isInited == false)
                throw new Exception("Port pair is not inited");
            this.port.RtsEnable = true;
            this.port.Write(data.ToCharArray(), 0, data.Length);
            Thread.Sleep(100);
            this.port.RtsEnable = false;
        }
    }

    public class ByteStuffingChat
    {
        public string bufferedMessage { get; private set; }

        public const char escSymbol = 'x';
        public const char flagCode = (char)106;
        public const char escCode = 'e';

        public void toByteStuffMessage(string msg)
        {
            for(int i =0; i < msg.Length; i++)
            {
                if (msg[i] == escSymbol) 
                {
                    msg = msg.Insert(i + 1, Convert.ToString(escCode));
                    i += 1;
                }
                if(msg[i] == flagCode)
                {
                    msg = msg.Insert(i, Convert.ToString(escSymbol));
                    i += 1;
                }
            }
            msg = msg.Insert(0, Convert.ToString(flagCode));
            bufferedMessage = msg;
        }

        public string toDeByteStuffMessage()
        {
            string clearMsg = "";
            for(int i = 1; i < bufferedMessage.Length; i++)
            {
                if (bufferedMessage[i] == escSymbol && bufferedMessage[i + 1] == flagCode)
                {
                    clearMsg += bufferedMessage[i + 1];
                    i++;
                    continue;
                }
                if (bufferedMessage[i] == escSymbol && bufferedMessage[i + 1] == escCode)
                {
                    clearMsg += bufferedMessage[i];
                    i++;
                    continue;
                }
                clearMsg += bufferedMessage[i];
            }
            return clearMsg;
        }
    }
}
