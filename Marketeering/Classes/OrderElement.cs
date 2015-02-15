using Marketeering.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MouseKeyboardActivityMonitor.Demo.Classes
{
    class OrderElement
    {
        public bool isSellOrder;
        public double orderAmmount;
        public List<ObjectOtherOrders> latestExportedData;
        public string orderId;

        public OrderElement(double currentAmmount, bool isBuyOrder)
        {
            this.isSellOrder = !isBuyOrder;
            this.orderAmmount = currentAmmount;
        }

        public double getUpdatedPrice(List<ObjectOtherOrders> latestExportedData)
        {
            double proposedPrice = 0;
            //on va ordonner les données importés pour avoir seulement ceux de la station (Jita)
            ObjectOtherOrders lowestSellPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == false).OrderBy(i => i.price).First();
            ObjectOtherOrders highestBuyPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == true).OrderByDescending(i => i.price).First();
            if(isSellOrder){
                //on regarde la différence entre les deux prix pour ne pas se faire avoir
                if (lowestSellPrice.price < orderAmmount)
                {
                    proposedPrice = highestBuyPrice.price - 0.01;
                }


                //si la différence est plus grande que 5%, on ne le fait pas
                if (Math.Abs(orderAmmount - proposedPrice) / orderAmmount > 0.05)
                {
                    proposedPrice = 0;
                }

            }else{
                ObjectOtherOrders highestPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == true).OrderByDescending(i => i.price).First();
                //on regarde la différence entre les deux prix pour ne pas se faire avoir
                if (highestPrice.price > orderAmmount)
                {
                    //Notifier.I.Notify()
                    proposedPrice = highestPrice.price + 0.01;
                }

                //on regarde si le profit < 10%
                if (((lowestSellPrice.price - highestBuyPrice.price) / highestBuyPrice.price) < 0.1)
                {
                    proposedPrice = 0;
                }
            }
            return proposedPrice;
        }

        public void addOrderData(List<ObjectOtherOrders> data){
            this.latestExportedData = data;
        }
    }
}
