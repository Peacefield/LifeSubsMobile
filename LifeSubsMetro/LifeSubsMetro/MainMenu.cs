﻿using MetroFramework;
using MetroFramework.Forms;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace LifeSubsMetro
{
    public partial class MainMenu : MetroForm
    {
        ApiHandler apiHandler = new ApiHandler();
        public MainMenu()
        {
            InitializeComponent();
        }

        #region Tile click eventhandlers
        /// <summary>
        /// Starts subtitle mode
        /// If no valid michrophone was selected the user gets prompted that no mic has been found
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tileSubtitle_Click(object sender, EventArgs e)
        {
            showTiles();
            Settings settings = new Settings();
            if (settings.microphone == -1)
            {
                MetroMessageBox.Show(this, "Microfoon niet gevonden", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Subtitle subtitle = new Subtitle(this);
            subtitle.Show();
            this.Visible = false;
        }

        /// <summary>
        /// Close application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Show joinRoom menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tileJoinRoom_Click(object sender, EventArgs e)
        {
            showTiles();
            this.tileJoinRoom.Visible = false;
        }

        /// <summary>
        /// Show CreateRoom menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tileCreateRoom_Click(object sender, EventArgs e)
        {
            showTiles();
            tileCreateRoom.Visible = false;
        }

        /// <summary>
        /// Close all submenus 
        /// </summary>
        private void showTiles()
        {
            foreach (Control c in this.Controls)
            {
                if (c.GetType() == typeof(MetroFramework.Controls.MetroTile))
                {
                    c.Visible = true;
                }
            }
        }

        /// <summary>
        /// Start groupconversation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinRoomButton_Click(object sender, EventArgs e)
        {
            //this.tileJoinRoom.Visible = true;

            apiHandler.joinRoom(roomNameBox.Text, toMD5(passwordBox.Text), getOwnIp(), usernameTB.Text, this);
        }

        /// <summary>
        /// Start settingsmenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tileSettings_Click(object sender, EventArgs e)
        {
            showTiles();
            new SettingsMenu(this).ShowDialog();
        }

        #endregion

        #region hoverEvents
        private void tileSubtitle_MouseEnter(object sender, EventArgs e)
        {
            this.tileSubtitle.Style = MetroFramework.MetroColorStyle.Lime;
        }

        private void tileSubtitle_MouseLeave(object sender, EventArgs e)
        {
            this.tileSubtitle.Style = MetroFramework.MetroColorStyle.Green;
        }

        private void tileSettings_MouseEnter(object sender, EventArgs e)
        {
            this.tileSettings.Style = MetroFramework.MetroColorStyle.Lime;
        }

        private void tileSettings_MouseLeave(object sender, EventArgs e)
        {
            this.tileSettings.Style = MetroFramework.MetroColorStyle.Green;
        }

        private void tileCreateRoom_MouseEnter(object sender, EventArgs e)
        {
            this.tileCreateRoom.Style = MetroFramework.MetroColorStyle.Lime;
        }

        private void tileCreateRoom_MouseLeave(object sender, EventArgs e)
        {
            this.tileCreateRoom.Style = MetroFramework.MetroColorStyle.Green;
        }

        private void tileJoinRoom_MouseEnter(object sender, EventArgs e)
        {
            this.tileJoinRoom.Style = MetroFramework.MetroColorStyle.Lime;
        }

        private void tileJoinRoom_MouseLeave(object sender, EventArgs e)
        {
            this.tileJoinRoom.Style = MetroFramework.MetroColorStyle.Green;
        }

        private void tileExit_MouseEnter(object sender, EventArgs e)
        {
            this.tileExit.Style = MetroFramework.MetroColorStyle.Lime;
        }

        private void tileExit_MouseLeave(object sender, EventArgs e)
        {
            this.tileExit.Style = MetroFramework.MetroColorStyle.Green;
        }

        #endregion

        private string toMD5(string pass)
        {
            // byte array representation of that string
            byte[] encodedPassword = new UTF8Encoding().GetBytes(pass);

            // need MD5 to calculate the hash
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            // string representation (similar to UNIX format)
            string encoded = BitConverter.ToString(hash)
                // without dashes
               .Replace("-", string.Empty)
                // make lowercase
               .ToLower();

            // encoded contains the hash you are wanting

            return encoded;
        }

        private void addRoomBtn_Click(object sender, EventArgs e)
        {
            string roomName = roomNameCreateTextbox.Text;
            string roomPass = roomPassCreateRoomTextbox.Text;
            string roomUsername = usernameCreateRoomTextbox.Text;
            string roomPassMD5 = toMD5(roomPass);
            string ownerIp = getOwnIp();

            apiHandler.createRoom(roomName, ownerIp, roomPass, roomPassMD5, roomUsername, this);
            //tileCreateRoom.Visible = true;
        }

        private string getOwnIp()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            Console.WriteLine(host);

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        private void MainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}