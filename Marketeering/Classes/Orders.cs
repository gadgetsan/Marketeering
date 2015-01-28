using MouseKeyboardActivityMonitor.Demo.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;

namespace MouseKeyboardActivityMonitor.Demo
{
    class Orders
    {

        int ROW_HEIGHT = 20;
        int SCROLL_DELTA = 38;
        Point WindowTop = new Point(1295, 262);
        Point StartPoint = new Point(2226, 310);
        Point ExportToFileLocation = new Point(2941, 1066);
        Point MyOrdersLocation = new Point(2266, 0056);
        int displacement = 0;
        bool isSellOrders;

        List<OrderElement> orders = new List<OrderElement>();

        public Orders(bool isSellOrders)
        {
            this.isSellOrders = isSellOrders;

            setupCoordinates();

            fetchOrdersList();
        }

        public void setupCoordinates()
        {
            //en fait ce qu'on fait ici c'est qu'on setup les emplacement des différents items
        }

        public void fetchOrdersList()
        {
            double lastOrderCurrentPrice = 0;
            int index = 0;
            double orderAmmount = getOrderAmmountForSpecificElement(index);
            orders.Add(new OrderElement(orderAmmount, isSellOrders));

            //on va aller lire tout les montants de vente
            while (orderAmmount != lastOrderCurrentPrice && orderAmmount != 0.0 && index < 10)
            {
                index++;
                lastOrderCurrentPrice = orderAmmount;
                orderAmmount = getOrderAmmountForSpecificElement(index);
                if (orderAmmount != lastOrderCurrentPrice && orderAmmount > 0)
                {
                    orders.Add(new OrderElement(orderAmmount, isSellOrders));
                }
            }
        }

        public void updateSales()
        {
            for (int i = 0; i < orders.Count(); i++)
                updateSale(i);
        }

        public void updateSale(int element)
        {
            //on va commencer par aller voir combien on met comme montant
            exportOrdersForSpecificElement(element);
            double newPrice = orders.ElementAt(element).getUpdatedPrice();
            //MessageBox.Show("nouveau prix proposé: " + newPrice);
            if (newPrice > 0)
            {
                modifyOrderSpecificElement(element);
                Automation.I.Send(newPrice.ToString().Replace(',', '.'));
                System.Threading.Thread.Sleep(500);
                Automation.I.Send("{Tab}");
                //System.Threading.Thread.Sleep(60000);
                Automation.I.Send("{Enter}");
                orders.ElementAt(element).orderAmmount = newPrice;
            }
            //exportOrdersForSpecificElement(element);
        }

        public int CalculateElementPosition(int element)
        {
            int relativeElementLocation = ROW_HEIGHT/2 + ROW_HEIGHT*element;
            //MessageBox.Show("currentClick.Y: " + currentClick.Y + ", MAX: " + (this.WINDOW.Y + this.START_POINT.Y));
            //on va aller voir si ce clique serait à l'extérieur de la liste, si oui on va scroll down
            while (relativeElementLocation < displacement || relativeElementLocation > displacement + WindowTop.Y)
            {
                //le cas ou cet élément est en haut
                if (relativeElementLocation < displacement)
                {
                    //si il est en haut, on réduit le déplacement
                    reduceDisplacement();
                }
                else if (relativeElementLocation > displacement + WindowTop.Y)
                {
                    augmentDisplacement();
                }
            }

            return relativeElementLocation - displacement;
        }

        public void reduceDisplacement()
        {
            Automation.I.mouseClick(new Point(StartPoint.X + WindowTop.X - 5, StartPoint.Y + 5));
            displacement -= SCROLL_DELTA;
        }

        public void augmentDisplacement()
        {
            Automation.I.mouseClick(new Point(StartPoint.X + WindowTop.X - 5, StartPoint.Y + WindowTop.Y - 5));
            displacement += SCROLL_DELTA;
        }

        public void leftClickSpecificElement(int element)
        {
            //Automation.mouseMove(START_POINT);
            int locationInWindow = CalculateElementPosition(element);
            Automation.I.mouseClick(new Point(StartPoint.X + 5, StartPoint.Y + locationInWindow));
        }

        public void modifyOrderSpecificElement(int element)
        {
            int locationInWindow = CalculateElementPosition(element);
            Automation.I.mouseClick(new Point(StartPoint.X + 5, StartPoint.Y + locationInWindow));
            Automation.I.rightMouseClick(new Point(StartPoint.X + 5, StartPoint.Y + locationInWindow));
            Automation.I.mouseClick(new Point(StartPoint.X + 25, StartPoint.Y + locationInWindow + 5));
        }
        public void exportMarketForSpecificElement(int element)
        {
            int locationInWindow = CalculateElementPosition(element);
            Automation.I.mouseClick(new Point(StartPoint.X + 5, StartPoint.Y + locationInWindow));
            Automation.I.rightMouseClick(new Point(StartPoint.X + 5, StartPoint.Y + locationInWindow));
            Automation.I.mouseClick(new Point(StartPoint.X + 25, StartPoint.Y + locationInWindow + 60));
            Automation.I.mouseClick(ExportToFileLocation);
            Automation.I.mouseClick(MyOrdersLocation);
        }

        public void leftClickOnEachElements(){

            Point currentClick = new Point(this.StartPoint.X + 10, this.StartPoint.Y + ROW_HEIGHT / 2);
            Automation.I.mouseClick(currentClick);
            while (currentClick.Y < (this.WindowTop.Y+this.StartPoint.Y))
            {
                Automation.I.mouseMove(new Point(100, 100));
                currentClick.Y += this.ROW_HEIGHT;
                //MessageBox.Show("X: " + currentClick.X + ", Y: " + currentClick.Y);
                Automation.I.mouseClick(currentClick);
            }
        }

        public void exportOrdersForSpecificElement(int element)
        {
            exportMarketForSpecificElement(element);
            //return;
            //TODO: Export to file
            var directory = new DirectoryInfo("C:\\Users\\Stéphane\\Documents\\EVE\\logs\\Marketlogs");
            var myFile = directory.GetFiles()
                         .OrderByDescending(f => f.LastWriteTime)
                         .First();
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
                //Process row
                foreach (string field in fields)
                {
                    //MessageBox.Show("field: " + field);
                    //TODO: Process field
                }
            }
            orders.ElementAt(element).addOrderData(toSend);
            parser.Close();
        }

        public double getOrderAmmountForSpecificElement(int element)
        {
            modifyOrderSpecificElement(element);

            System.Threading.Thread.Sleep(1000);
            Automation.I.Copy();
            System.Threading.Thread.Sleep(1000);

            //MessageBox.Show("cb: " + Clipboard.GetText());
            string cbString = Automation.I.GetClipboard();
            double toReturn = 0;
            try
            {
                toReturn = double.Parse(cbString);
            }
            catch
            {
                return 0;
            }

            Automation.I.Send("{Tab 2}");
            Automation.I.Send("{Enter}");
            return toReturn;
        }

    }


}

