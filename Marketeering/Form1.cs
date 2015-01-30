using Marketeering.Utilities;
using MouseKeyboardActivityMonitor.Demo;
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
        Thread botThread;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            botThread = new Thread(delegate()
            {
                Orders Orders = new Orders(new Point(2222, 524), new Point(2222, 88));
                Random rnd = new Random();
                while (true)
                {
                    Notifier.I.Notify("Marketeering", "Nouvelle phase commencée");
                    Orders.updateSales();
                    System.Threading.Thread.Sleep(60000 * 5 + rnd.Next(1, 60000));
                }
                //
                //MessageBox.Show("Ceci est un message qui devrais arrivé avant la notification");
            });

            botThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            botThread.Abort();
        }
    }
}
