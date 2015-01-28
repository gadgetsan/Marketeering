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
        public string elementName;
        public List<ObjectOtherOrders> latestExportedData;

        public OrderElement(double currentAmmount, bool isSellOrder)
        {
            this.isSellOrder = isSellOrder;
            this.orderAmmount = currentAmmount;
        }

        public double getUpdatedPrice()
        {
            double proposedPrice = 0;
            //on va ordonner les données importés pour avoir seulement ceux de la station (Jita)
            if(isSellOrder){
                ObjectOtherOrders lowestPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == false).OrderByDescending(i => i.price).First();
                //on regarde la différence entre les deux prix pour ne pas se faire avoir
                if (lowestPrice.price < orderAmmount)
                {
                    proposedPrice = lowestPrice.price - 0.01;
                }
            }else{
                ObjectOtherOrders highestPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == true).OrderByDescending(i => i.price).First();
                //on regarde la différence entre les deux prix pour ne pas se faire avoir
                if (highestPrice.price > orderAmmount)
                {
                    proposedPrice = highestPrice.price + 0.01;
                }
            }
            //si la différence est plus grande que 5%, on ne le fait pas
            if (Math.Abs(orderAmmount - proposedPrice) / orderAmmount > 0.05)
            {
                proposedPrice = 0;
            }
            return proposedPrice;
        }

        public void addOrderData(List<ObjectOtherOrders> data){
            this.latestExportedData = data;
        }
    }
}
