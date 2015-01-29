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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Thread(delegate()
            {
                Orders sellOrders = new Orders(true, new Point(2222, 88));
                Orders buyOrders = new Orders(false, new Point(2222, 524));
                Random rnd = new Random();
                while (true)
                {
                    Notifier.I.Notify("Marketeering", "Nouvelle phase commencée");
                    sellOrders.updateSales();
                    buyOrders.updateSales();
                    System.Threading.Thread.Sleep(60000 * 5 + rnd.Next(1, 60000));
                }
                //
                //MessageBox.Show("Ceci est un message qui devrais arrivé avant la notification");
            }).Start();
        }
    }
}
