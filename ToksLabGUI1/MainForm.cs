using System;
using System.Windows.Forms;
using System.Drawing;
using TOKSClasses;

namespace ToksLabGUI1
{
    public partial class MainForm : Form
    {

        private ComPortPair ComPortPair = new ComPortPair();
        private ByteStuffingChat StuffingChat = new ByteStuffingChat();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.MaximumSize = new Size(713, 557);
            this.MinimumSize = new Size(713, 557);
            outputTextBox.ReadOnly = true;
            statusTextBox.ReadOnly = true;
            
            //try
            //{
            //    ComPortPair.initPorts();
            //    statusTextBox.AppendText(this.ComPortPair.getStatus());
            //}
            //catch (Exception ex)
            //{
            //    statusTextBox.AppendText(ex.Message);
            //}
        }

        private void inputTextBox_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                statusTextBox.Text = string.Empty;
                try
                {
                    StuffingChat.toByteStuffMessage(inputTextBox.Text);
                }catch(Exception ex)
                {
                    inputTextBox.AppendText(ex.Message);
                }
                String temp = StuffingChat.bufferedMessage;
                if (temp != "\r\n" && temp != null)
                {
                    if(temp[0] == ByteStuffingChat.flagCode)
                    {
                        statusTextBox.SelectionColor = Color.BlueViolet;
                        statusTextBox.AppendText(Convert.ToString(temp[0]));
                        statusTextBox.SelectionColor = Color.Black;
                    }
                    for (int i = 1; i < temp.Length; i++)
                    {
                        if ((temp[i] == ByteStuffingChat.escSymbol && temp[i + 1] == ByteStuffingChat.flagCode) || 
                            (temp[i] == ByteStuffingChat.escSymbol && temp[i + 1] == ByteStuffingChat.escCode))
                        {
                            statusTextBox.SelectionColor = Color.Red;
                            statusTextBox.AppendText(Convert.ToString(temp[i]));
                            statusTextBox.AppendText(Convert.ToString(temp[i + 1]));
                            statusTextBox.SelectionColor = Color.Black;
                            i++;
                        }
                        else statusTextBox.AppendText(Convert.ToString(temp[i]));
                    }
                    outputTextBox.AppendText(StuffingChat.toDeByteStuffMessage() + "\r\n");
                }
                inputTextBox.Text = string.Empty;
            }
        }
    }
}
