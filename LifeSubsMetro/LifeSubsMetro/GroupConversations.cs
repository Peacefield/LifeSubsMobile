﻿using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace LifeSubsMetro
{
    public partial class GroupConversations : MetroForm
    {
        string path = @"C:\audiotest";
        MainMenu mm;
        IMClient ic;
        string ownIpAddress;
        MicLevelListener mll;
        string currentListener;
        Listener listener1 = null;
        Listener listener2 = null;
        Settings settings;

        public GroupConversations(MainMenu mm)
        {
            settings = new Settings();
            this.mm = mm;
            InitializeComponent();

            //listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);


            ownIpAddress = getOwnIp();
            ipLabel.Text = ownIpAddress;
            friendIpTextBox.Text = ownIpAddress;
            ownPort.Text = "11000";
            otherPort.Text = "11001";
            //populating listView:

            Font font = new System.Drawing.Font("Impact", 15);
            dataGridOutput.DefaultCellStyle.Font = new Font(font, FontStyle.Regular);
            dataGridOutput.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font(dataGridOutput.DefaultCellStyle.Font.ToString(), 50);


            //populating listView:
            //    addMessage("FOTO", "BERICHT", Color.Orange);
            //    addMessage("FOTO", "TEST", Color.Blue);
            //addMessage("FOTO", "NOG EEN TESTNOG EEN TESTNOG EEN TESTNOG EEN TESTNOG EEN TEST", Color.Yellow);
            //addMessage("FOTO", "BERICHT", Color.Orange);
            //addMessage("FOTO", "TEST", Color.Blue);
            //addMessage("FOTO", "NOG EEN TESTNOG EEN TESTNOG EEN TESTNOG EEN TESTNOG EEN TEST", Color.Yellow);

            //addMessage("Ik verstuur ook zelf iets", Color.Red);
        }

        public void addMessage(string msg, Color c)
        {
            DataGridViewRow dr = new DataGridViewRow();

            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            dr.Cells.Add(cell1);
            
            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Style.BackColor = c;
            cell2.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            cell2.Value = msg;
            dr.Cells.Add(cell2);

            dataGridOutput.Rows.Add(dr);
        }

        private string getOwnIp()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            Console.WriteLine(host);

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";

        }



        public void addMessage(string sender, string msg, Color c)
        {
            DataGridViewRow dr = new DataGridViewRow();

            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = sender;
            dr.Cells.Add(cell1);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Style.BackColor = c;
            cell2.Value = msg;
            dr.Cells.Add(cell2);

            dr.Height = 50;

            dataGridOutput.Rows.Add(dr);
        }

        private void GroupConversations_FormClosed(object sender, FormClosedEventArgs e)
        {
            mm.Visible = true;
        }

        private void sendTile_Click(object sender, EventArgs e)
        {
                addMessage(tbInput.Text, Color.PowderBlue);
                try
                {
                    ic.SendMessage("127.0.0.1",tbInput.Text);
                }
                catch (Exception excx)
                {
                    Console.WriteLine(excx);
                }
                
        }
                

        private void startBtn_Click(object sender, EventArgs e)
        {

            try
            {
                ic = new IMClient();
                ic.SetupConn();
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx);
            }


        }

        private void startGroupListenerBtn_Click(object sender, EventArgs e)
        {
            mll = new MicLevelListener(this);
            mll.listenToStream();
            int deviceNumber = settings.microphone;

            //Initiate recording
            currentListener = "listener1";
            listener1 = new Listener(deviceNumber, currentListener, this);
            listener1.startRecording();
            //listener1 = new Listener(deviceNumber, currentListener, this);
            //listener1.startRecording();
        }

        #region Directory Handling
        private void createDir()
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists) Directory.CreateDirectory(path);
        }

        private void deleteDir()
        {
            bool folderExists = Directory.Exists(path);
            if (folderExists) Directory.Delete(path, true);
        }
        #endregion
        
    }
}
