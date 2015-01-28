using Marketeering.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marketeering
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Notifier.I.Notify("Notification Vraiment Awesome", "Cette Notification est envoyée automagiquement avec Pushbullet");
            //MessageBox.Show("Ceci est un message qui devrais arrivé avant la notification");
        }
    }
}
