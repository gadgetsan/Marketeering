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
        public string typeId;
        public string charId;
        public string charName;
        public string regionId;
        public string regionName;
        public string stationId;
        public string stationName;
        public int range;
        public int volumeEntered;
        public double volumeRemaining;
        public DateTime issueDate;
        public String orderState;
        public int minVolume;
        public string accountId;
        public int duration;
        public bool isCorp;
        public string solarSystemId;
        public string solarSystemName;
        public double escrow; 

        public OrderElement(double currentAmmount, bool isBuyOrder)
        {
            this.isSellOrder = !isBuyOrder;
            this.orderAmmount = currentAmmount;
        }

        public double getUpdatedPrice(List<ObjectOtherOrders> latestExportedData)
        {
            double proposedPrice = 0;
            //on va ordonner les données importés pour avoir seulement ceux de la station (Jita)
            if(isSellOrder){
                ObjectOtherOrders lowestPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == false).OrderBy(i => i.price).First();
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
                    //Notifier.I.Notify()
                    proposedPrice = highestPrice.price + 0.01;
                }
            }
            //si la différence est plus grande que 5%, on ne le fait pas
            if ((Math.Abs(orderAmmount - proposedPrice) / orderAmmount > 0.20 && isSellOrder)  || (Math.Abs(orderAmmount - proposedPrice) / orderAmmount > 0.05 && !isSellOrder))
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
