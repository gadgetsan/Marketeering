using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AutoItX3Lib;

namespace MouseKeyboardActivityMonitor.Demo.Classes
{
    class Automation
    {
        private static Automation instance;

        private AutoItX3 autoIT = new AutoItX3();

        public double lagMultiplicator;

        public static Automation I
        {
            get
            {
                if (instance == null)
                {
                    instance = new Automation();
                    instance.init();
                }
                return instance;
            }
        }

        private void init()
        {
            autoIT.Opt("SendKeyDelay", 250);
        }

        public void mouseClick(Point location){

            waitABitLonger();
            autoIT.MouseClick("primary", location.X, location.Y);
            waitABitLonger();
        }

        public void waitALittle()
        {
            System.Threading.Thread.Sleep((int)(250 * lagMultiplicator));
        }
        public void waitABitLonger()
        {
            System.Threading.Thread.Sleep((int)(500 * lagMultiplicator));
        }

        public void waitAWhile()
        {
            System.Threading.Thread.Sleep((int)(1000 * lagMultiplicator));
        }

        public void waitALongTime()
        {
            System.Threading.Thread.Sleep((int)(2000 * lagMultiplicator));
        }

        public void Send(string text)
        {
            waitALittle();
            autoIT.Send(text);
        }

        public void Copy()
        {
            waitALittle();
            autoIT.Send("^c");
        }

        public void EmptyClipboard()
        {
            waitABitLonger();
            autoIT.ClipPut("0");
            return;
        }

        public string GetClipboard()
        {
            waitAWhile();
            return autoIT.ClipGet();
        }

        public void rightMouseClick(Point location)
        {
            waitABitLonger();
            autoIT.MouseClick("secondary", location.X, location.Y);
            waitABitLonger();
        }

        public void mouseMove(Point location){
            autoIT.MouseMove(location.X, location.Y);
        }
    }
}
