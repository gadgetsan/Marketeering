using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Marketeering.Utilities
{
    //une classe permettant d'envoyer une notification à un service Externet (en l'occurence Pushbullet)
    class Notifier
    {
        private static Notifier instance;

        public static Notifier I
        {
            get
            {
                if (instance == null)
                {
                    instance = new Notifier();
                }
                return instance;
            }
        }

        public void Notify(string title, string text){
            WebRequest request = WebRequest.Create("https://api.pushbullet.com/v2/users/me");
            request.Credentials = new NetworkCredential("", "");

        }
    }
}
