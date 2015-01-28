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
                }
                return instance;
            }
        }

        public void mouseClick(Point location){

            autoIT.MouseClick("primary", location.X, location.Y, 1);
        }

        public void Send(string text)
        {
            autoIT.Send(text);
        }

        public void Copy()
        {
            autoIT.Send("^c");
        }

        public string GetClipboard()
        {
            return autoIT.ClipGet();
        }

        public void rightMouseClick(Point location)
        {
            autoIT.MouseClick("secondary", location.X, location.Y, 1);
        }

        public void mouseMove(Point location){
            autoIT.MouseMove(location.X, location.Y);
        }
    }
}
