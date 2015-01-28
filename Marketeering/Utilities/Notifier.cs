using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marketeering.Utilities
{
    //une classe permettant d'envoyer une notification à un service Externet (en l'occurence Pushbullet)
    class Notifier
    {
        private static Notifier instance;

        private List<JObject> devices;

        private string key;

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

        private Notifier(){
            //on va aller chercher les informations nécessaire à la bonne config
            readConfiguration();

            fetchDevicesList();
        }

        private void readConfiguration()
        {
            key = ConfigurationManager.AppSettings["PushbulletKey"];
        }

        private void fetchDevicesList()
        {
            devices = new List<JObject>();         
            WebRequest request = WebRequest.Create("https://api.pushbullet.com/v2/devices");
            request.Credentials = new NetworkCredential(key, "");
            WebResponse response = request.GetResponse();            
            var dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            JObject master = JObject.Parse(responseFromServer);
            var tempDevices = master["devices"];
            foreach (var device in tempDevices.Where(d => (bool)d["pushable"] == true))
            {
                devices.Add((JObject)device);
                //MessageBox.Show(device.ToString());
            }


        }

        public void Notify(string title, string text){
            //l'envoie d'une notification démarre toujours sur un autre thread parce qu'on ne veut pas bloquer le reste de l'application

            new Thread(delegate()
            {
                WebRequest request = WebRequest.Create("https://api.pushbullet.com/v2/pushes");
                request.Credentials = new NetworkCredential(key, "");
                request.ContentType = "application/json; charset=UTF-8";
                request.Method = "POST";



                string postData = "{\"type\": \"note\", \"title\": \"" + title + "\", \"body\": \"" + text + "\"}";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                //on créé et envoie le push

                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                JObject master = JObject.Parse(responseFromServer);
                string iden = master["iden"].ToString();
                
                //on attends quelque temps
                System.Threading.Thread.Sleep(60000);

                //ensuite on demande de supprimer
                WebRequest deleteReq = WebRequest.Create("https://api.pushbullet.com/v2/pushes/" + iden);
                deleteReq.Credentials = new NetworkCredential(key, "");
                deleteReq.Method = "DELETE";
                WebResponse deleteResponse = deleteReq.GetResponse();
                dataStream = deleteResponse.GetResponseStream();
                StreamReader deleteReader = new StreamReader(dataStream);
                string deleteResponseFromServer = reader.ReadToEnd();
                //MessageBox.Show("Deleting " + iden + ", Response: " + deleteResponseFromServer);
                
            }).Start();

        }
    }
}
