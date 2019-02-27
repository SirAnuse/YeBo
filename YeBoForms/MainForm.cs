using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using YeBo;

namespace YeBoForms
{
    public partial class MainForm : Form
    {
        public static Log Log;

        public MainForm()
        {
            InitializeComponent();
            Main.SetOutputBox(outputBox);
            Main.SetOutputForm(this);
            Main.Initialize();
            Log = new Log("MainForm");
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            Main.QueuedCommands.Enqueue(inputBox.Text);
            inputBox.Clear();

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void outputBox_TextChanged(object sender, EventArgs e)
        {
            outputBox.SelectionStart = outputBox.Text.Length;
            outputBox.ScrollToCaret();
        }

        // Stop the background threads when the form is closed, so the program actually shuts down.
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Main.StopThreads();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            outputBox.Width = Width - 18;
            outputBox.Height = Height - 68;
            var loc = inputBox.Location;
            loc.Y = outputBox.Height + 2;
            inputBox.Location = loc;
            inputBox.Width = Width - 18;
        }
    }
}
