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

            System.Threading.Thread.Sleep(500);
            autoIT.MouseClick("primary", location.X, location.Y, 1);
        }

        public void Send(string text)
        {
            System.Threading.Thread.Sleep(100);
            autoIT.Send(text);
        }

        public void Copy()
        {
            System.Threading.Thread.Sleep(100);
            autoIT.Send("^c");
        }

        public string GetClipboard()
        {
            System.Threading.Thread.Sleep(500);
            return autoIT.ClipGet();
        }

        public void rightMouseClick(Point location)
        {
            System.Threading.Thread.Sleep(500);
            autoIT.MouseClick("secondary", location.X, location.Y, 1);
        }

        public void mouseMove(Point location){
            autoIT.MouseMove(location.X, location.Y);
        }
    }
}
