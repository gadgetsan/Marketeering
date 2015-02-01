using MouseKeyboardActivityMonitor.Demo.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Windows.Forms;
using Marketeering.Utilities;

namespace MouseKeyboardActivityMonitor.Demo
{
    class Orders
    {

        int ROW_HEIGHT = 20;
        Point BuyStartPoint;
        Point SelltartPoint;
        Point ExportToFileLocation = new Point(1023, 1028);
        Point ExportLocation = new Point(264, 1027);
        Point MyOrdersLocation = new Point(358, 57);
        int displacement = 0;
        bool isSellOrders;
        List<int> allSoldIndexes = new List<int>();

        List<OrderElement> orders = new List<OrderElement>();

        public Orders(Point buyOrdersStart, Point sellOrdersStart)
        {

            this.BuyStartPoint = buyOrdersStart;
            this.SelltartPoint = sellOrdersStart;

        }

        public void setupCoordinates()
        {
            //en fait ce qu'on fait ici c'est qu'on setup les emplacement des différents items
        }

        public void fetchMyOrders()
        {
            //on va commencer par exporter les données des orders en cliquant sur le bouton
            Automation.I.mouseClick(ExportLocation);
            Automation.I.waitAWhile();

            //ensuite on importe les commandes
            var directory = new DirectoryInfo("C:\\Users\\Stéphane\\Documents\\EVE\\logs\\Marketlogs");
            var myOrdersFile = directory.GetFiles()
                         .OrderByDescending(f => f.LastWriteTime)
                         .First();
            TextFieldParser parser = new TextFieldParser(myOrdersFile.FullName);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            int index = 0;
            orders = new List<OrderElement>();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (index != 0)
                {
                    OrderElement toAdd = new OrderElement(double.Parse(fields[10], CultureInfo.InvariantCulture), bool.Parse(fields[9]));
                    toAdd.orderId = fields[0];
                    orders.Add(toAdd);
                }
                index++;
            }
            parser.Close();
            File.Delete(myOrdersFile.FullName);

        }

        public void updateSales()
        {
            clearFolder();
            fetchMyOrders();

            updateSaleType("sell");
            updateSaleType("buy");
        }

        public void updateSaleType(string type){
            int index = 0;
            List<ObjectOtherOrders> buyOrderData = fetchOrderData(type, index);
            while (buyOrderData != null && buyOrderData.Count > 0)
            {
                //on va aller voir à laquelle de nos commandes ça correspond
                OrderElement myOrderForThisItem = orders.Where(o => (buyOrderData.Where(bod => bod.orderId == o.orderId && bod.bid == (type == "buy")).Count()) > 0).First();
                double newPrice = myOrderForThisItem.getUpdatedPrice(buyOrderData);

                if (newPrice > 0)
                {
                    //MessageBox.Show("Présent Montant: "+ myOrderForThisItem.orderAmmount.ToString() + " Montant: " + newPrice.ToString());
                    modifyOrderSpecificElement(type, index, newPrice, myOrderForThisItem.orderAmmount);
                }
                index++;
                Automation.I.waitAWhile();
                buyOrderData = fetchOrderData(type, index);
            }
            //on 
            return;
        }

        public void modifyOrderSpecificElement(string orderType, int element, double newOrderPrice, double verificationPrice)
        {

            Point startingPoing;
            if(orderType == "buy"){
                startingPoing = BuyStartPoint;
            }else{

                startingPoing = SelltartPoint;
            }

            Automation.I.mouseClick(new Point(startingPoing.X + 20, startingPoing.Y + element * ROW_HEIGHT + ROW_HEIGHT / 2));
            Automation.I.rightMouseClick(new Point(startingPoing.X + 5, startingPoing.Y + element * ROW_HEIGHT + ROW_HEIGHT / 2));
            Automation.I.mouseClick(new Point(startingPoing.X + 25, startingPoing.Y + element * ROW_HEIGHT + ROW_HEIGHT / 2 + 5));

            //on va vérifier si le montant est vraiment celui que l'on pense
            Automation.I.Copy();
            Automation.I.waitALittle();
            string clip = Automation.I.GetClipboard();

            Double clipDouble = 0.0;

            try
            {
                clipDouble = Double.Parse(clip);
            }
            catch (Exception e)
            {

            }

            if (Math.Abs(clipDouble - verificationPrice) > 0.01)
            {
                //Notifier.I.SendMessage("ERREUR Marketeering", "Cette valeur (" + clip + ") n'est pas celle à laquelle on s'attendais: " + verificationPrice);
                //MessageBox.Show("Cette valeur (" + clip + ") n'est pas celle à laquelle on s'attendais: " + verificationPrice);
                Automation.I.Send("{Tab}");
                Automation.I.Send("{Tab}");
                Automation.I.Send("{Enter}");
                return;
            }


            Automation.I.Send(newOrderPrice.ToString().Replace(',', '.'));
            Automation.I.waitALittle();
            Automation.I.Send("{Tab}");
            Automation.I.Send("{Enter}");
            Automation.I.Send("{Tab}");
            Automation.I.Send("{Enter}");
            Automation.I.waitALittle();
        }

        public void clearFolder()
        {
            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo("C:\\Users\\Stéphane\\Documents\\EVE\\logs\\Marketlogs");

            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }
        }

        public List<ObjectOtherOrders> fetchOrderData(string orderType, int element)
        {
            Point startingPoing;
            if(orderType == "buy"){
                startingPoing = BuyStartPoint;
            }else{

                startingPoing = SelltartPoint;
            }

            //Automation.I.mouseClick(new Point(startingPoing.X + 10, startingPoing.Y + element * ROW_HEIGHT + ROW_HEIGHT / 2));
            Automation.I.waitALittle();
            Automation.I.rightMouseClick(new Point(startingPoing.X + 10, startingPoing.Y + element * ROW_HEIGHT + ROW_HEIGHT / 2));
            Automation.I.waitALittle();
            Automation.I.mouseClick(new Point(startingPoing.X + 40, startingPoing.Y + element * ROW_HEIGHT + ROW_HEIGHT / 2 + 60));
            Automation.I.waitALittle();
            Automation.I.mouseClick(ExportToFileLocation);
            Automation.I.waitAWhile();
            Automation.I.mouseClick(MyOrdersLocation);
            Automation.I.waitALongTime();

            //on va aller lire le ficher exporter
            var directory = new DirectoryInfo("C:\\Users\\Stéphane\\Documents\\EVE\\logs\\Marketlogs");
            try
            {
                var myFile = directory.GetFiles()
                             .OrderByDescending(f => f.LastWriteTime)
                             .First();
                //si le fichier est vieux, on retourne null
                if(myFile.LastWriteTime < DateTime.Now.AddSeconds((int)(-10 * Automation.I.lagMultiplicator))){
                    return null;
                }
                TextFieldParser parser = new TextFieldParser(myFile.FullName);
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                int index = 0;
                List<ObjectOtherOrders> toSend = new List<ObjectOtherOrders>();
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (index != 0)
                    {
                        var otherOrderToAdd = new ObjectOtherOrders();
                        otherOrderToAdd.price = double.Parse(fields[0], CultureInfo.InvariantCulture);
                        otherOrderToAdd.volumeRemaining = (int)double.Parse(fields[1], CultureInfo.InvariantCulture);
                        otherOrderToAdd.typeId = int.Parse(fields[2]);
                        otherOrderToAdd.range = int.Parse(fields[3]);
                        otherOrderToAdd.orderId = fields[4];
                        otherOrderToAdd.totalVolume = int.Parse(fields[5]);
                        otherOrderToAdd.minimumVolume = int.Parse(fields[6]);
                        otherOrderToAdd.bid = bool.Parse(fields[7]);
                        otherOrderToAdd.issueDate = DateTime.Parse(fields[8]);
                        otherOrderToAdd.duration = int.Parse(fields[9]);
                        otherOrderToAdd.stationId = int.Parse(fields[10]);
                        otherOrderToAdd.regionId = int.Parse(fields[11]);
                        otherOrderToAdd.solarSystemId = int.Parse(fields[12]);
                        otherOrderToAdd.jumps = int.Parse(fields[13]);
                        toSend.Add(otherOrderToAdd);
                    }
                    index++;
                }
                parser.Close();
                File.Delete(myFile.FullName);

                return toSend;
            }
            catch(Exception e)
            {
                //si on n'as pas pu exporter parce que cet élément n'existe pas...
                return null;
            }

        }

    }


}

