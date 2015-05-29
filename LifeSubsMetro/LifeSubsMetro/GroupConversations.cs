﻿using MetroFramework.Forms;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace LifeSubsMetro
{
    public partial class GroupConversations : MetroForm
    {
        public string userId { get; set; }
        public string roomId { get; set; }
        public DateTime time { get; set; }

        string path = @"C:\audiotest";
        MainMenu mm;

        Thread th;
        Thread th2;

        MicLevelListener mll;
        string currentListener;
        Listener listener1 = null;
        Settings settings;

        MessageHandler mh;

        public GroupConversations(MainMenu mm)
        {
            this.mm = mm;
            InitializeComponent();

            setStyle();
            mh = new MessageHandler(this);

            //TODO: Get userId and roomId from database
            userId = "2";
            roomId = "1";
        }

        #region sendMessage
        /// <summary>
        /// Place your own message into the datagridview dataGridOutput as a new row
        /// </summary>
        /// <param name="msg"></param>
        public void sendMessage(string msg)
        {
            //sendRequest(msg);

            DataGridViewRow dr = new DataGridViewRow();

            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Style.BackColor = Color.PowderBlue;
            dr.Cells.Add(cell1);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Style.BackColor = Color.PowderBlue;
            cell2.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            cell2.Value = msg;
            dr.Cells.Add(cell2);

            if (this.dataGridOutput.InvokeRequired)
            {
                try
                {
                    this.dataGridOutput.Invoke((MethodInvoker)delegate { dataGridOutput.Rows.Add(dr); dataGridOutput.CurrentCell = dataGridOutput.Rows[dr.Index+1].Cells[0]; });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                dataGridOutput.Rows.Add(dr);
                dataGridOutput.CurrentCell = dataGridOutput.Rows[dr.Index+1].Cells[0]; 
                tbInput.Text = "";
            }
            
        }
        
        public void sendRequest(string msg)
        {
            time = DateTime.Now;

            string path = "http://lifesubs.windesheim.nl/api/addMessage.php?func=addMessage&room=" + roomId + "&sender=" + userId + "&text=" + msg + "&time=" + time;

            Console.WriteLine("request started: " + path);
            return;
            string result;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                Stream stream = request.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(stream);

                result = sr.ReadToEnd();

                sr.Close();
                stream.Close();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        if ((int)response.StatusCode == 500)
                        {
                            Console.WriteLine("HTTP Status Code: " + (int)response.StatusCode);
                            result = "500";
                        }
                        else
                        {
                            Console.WriteLine("HTTP Status Code: " + (int)response.StatusCode);
                            result = "";
                        }
                    }
                    else
                    {
                        // no http status code available
                        Console.WriteLine("ProtocolError: " + ex.Status);
                        result = "";
                    }
                }
                else
                {
                    // no http status code available
                    Console.WriteLine(ex.Status.ToString());
                    result = "";
                }
            }
        }

        /// <summary>
        /// KeyDown event handler.
        /// Sends the text on "Enter"-press if tbInput containts text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (tbInput.Text != "")
                    sendMessage(tbInput.Text);
            }
        }
        /// <summary>
        /// Click event handler.
        /// Sends your message if tbInput containts text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendTile_Click(object sender, EventArgs e)
        {
            if (tbInput.Text == "") return;

            sendMessage(tbInput.Text);
        }

        #endregion

        /// <summary>
        /// Place a received message into the datagridview dataGridOutput as a new row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public void receiveMessage(string sender, string msg)
        {
            DataGridViewRow dr = new DataGridViewRow();

            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = sender;
            dr.Cells.Add(cell1);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cell2.Value = msg;
            dr.Cells.Add(cell2);

            if (this.dataGridOutput.InvokeRequired)
            {
                try
                {
                    this.dataGridOutput.Invoke((MethodInvoker)delegate { dataGridOutput.Rows.Add(dr); dataGridOutput.CurrentCell = dataGridOutput.Rows[dr.Index].Cells[0]; });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
                
        #region set properties from other thread

        public void setListenButton(Boolean show)
        {
            try
            {
                if (this.startGroupListenerBtn.InvokeRequired)
                {
                    try
                    {
                        if (show == false)
                        {
                            this.startGroupListenerBtn.Invoke((MethodInvoker)delegate { startGroupListenerBtn.Visible = true; });
                        }
                        if (show == true)
                        {
                            this.startGroupListenerBtn.Invoke((MethodInvoker)delegate { startGroupListenerBtn.Visible = false; });
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Knop niet kunnen vinden");
            }
        }

        public void setVolumeMeter(int amp)
        {
            amp = amp + 50;
            try
            {
                if (this.volumemeterGrp.InvokeRequired)
                {
                    try
                    {
                        this.volumemeterGrp.Invoke((MethodInvoker)delegate { this.volumemeterGrp.Value = amp; });
                        Console.WriteLine("AMP = " + amp);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("VolumeMeter niet kunnen vinden");
            }

        }

        public void setCanSendPanel(Boolean sent)
        {
            try
            {
                if (this.canSendPanelGrp.InvokeRequired)
                {
                    try
                    {
                        if (sent == false)
                        {
                            this.canSendPanelGrp.Invoke((MethodInvoker)delegate { canSendPanelGrp.Visible = true; });
                        }
                        if (sent == true)
                        {
                            this.canSendPanelGrp.Invoke((MethodInvoker)delegate { canSendPanelGrp.Visible = false; });
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Knop niet kunnen vinden");
            }

        }
        #endregion

        #region Speech-to-text listener
        private void canSendPanelGrp_VisibleChanged(object sender, EventArgs e)
        {
            if (canSendPanelGrp.Visible == true)
            {
                Console.WriteLine("VERSTUUR");
                send();
                canSendPanelGrp.Visible = false;
                volumemeterGrp.Visible = false;
                startGroupListenerBtn.Visible = true;
                th.Abort();
            }
        }

        public void send()
        {
            if (listener1 == null) return;
            Console.WriteLine("listener1 currently recording");
            //Stop listener
            Console.WriteLine("Stop listener1");
            listener1.stop();

            th = new Thread(listener1.request);
            th.Start();
            while (!th.IsAlive) ;
            Thread.Sleep(1);
        }

        /// <summary>
        /// Starts a listenerloop that stops when this button is pressed and ended when this button is pressed again.
        /// TODO: Implement above ^^^
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startGroupListenerBtn_Click(object sender, EventArgs e)
        {
            startGroupListenerBtn.Visible = false;
            volumemeterGrp.Visible = true;
            createDir();
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
        #endregion

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

        /// <summary>
        /// Event handler for when the settings picturebox is clicked.
        /// Opens the Settingsmenu in a Dialog window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsPB_Click(object sender, EventArgs e)
        {
            SettingsMenu sm = new SettingsMenu(this);
            sm.ShowDialog();
        }

        /// <summary>
        /// Event handler for when this form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupConversations_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { deleteDir(); }
            catch (Exception direx) { Console.WriteLine(direx.Message); }

            mh.stopTimer();
            mm.Visible = true;
        }

        /// <summary>
        /// Set the datagridview and textbox tot the user-specified or default style
        /// </summary>
        public void setStyle()
        {
            settings = new Settings();
            Font font = new System.Drawing.Font(settings.font, settings.fontsize);
            dataGridOutput.DefaultCellStyle.Font = font;
            dataGridOutput.DefaultCellStyle.BackColor = settings.bgColor;
            dataGridOutput.DefaultCellStyle.ForeColor = settings.subColor;
            dataGridOutput.DefaultCellStyle.SelectionBackColor = settings.bgColor;
            dataGridOutput.DefaultCellStyle.SelectionForeColor = settings.subColor;
            dataGridOutput.BackgroundColor = settings.bgColor;

            tbInput.BackColor = settings.bgColor;
            tbInput.ForeColor = settings.subColor;

            //TODO: Complete listener functionality and change microphone of requestlistener and miclevellistener here
        }
    }
}
