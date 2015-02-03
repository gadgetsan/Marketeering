using Marketeering.Utilities;
using MouseKeyboardActivityMonitor.Demo;
using MouseKeyboardActivityMonitor.Demo.Classes;
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
                Automation.I.lagMultiplicator = 1.3;
                Orders Orders = new Orders(new Point(310, 518), new Point(310, 88));
                Random rnd = new Random();
                while (true)
                {
                    DateTime startTime = DateTime.Now;
                    int milisecsDeltaTime = 60000 * 5 + rnd.Next(1, 120000);

                    DateTime nextTime = startTime.AddMilliseconds(milisecsDeltaTime);
                    Notifier.I.Notify("Marketeering", "Phase commencée à " + startTime.ToShortTimeString() + ", Prochaine phase à " + nextTime.ToShortTimeString(), milisecsDeltaTime);
                    Orders.updateSales();

                    DateTime newCurrentTime = DateTime.Now;

                    double timeLeftToWait = (nextTime - newCurrentTime).TotalMilliseconds; ;

                    //MessageBox.Show("on attend " + timeLeftToWait + " millisecondes entre "+ newCurrentTime.ToShortTimeString() + " et " + nextTime.ToShortTimeString());
                    if (timeLeftToWait > 0)
                    {
                        System.Threading.Thread.Sleep((int)timeLeftToWait);
                    }
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

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Lol, u dumb m8?");
        }
    }
}
