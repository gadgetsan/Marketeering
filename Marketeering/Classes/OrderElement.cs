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
            this.latestExportedData = latestExportedData;
            double proposedPrice = 0;
            //on va ordonner les données importés pour avoir seulement ceux de la station (Jita)
            if(isSellOrder){
                ObjectOtherOrders lowestSellPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == false).OrderBy(i => i.price).First();
                //on regarde la différence entre les deux prix pour ne pas se faire avoir
                if (lowestSellPrice.price < orderAmmount)
                {
                    //si la différence est plus grande que 5%, on ne le fait pas
                    if (Math.Abs(orderAmmount - proposedPrice) / orderAmmount > 0.05)
                    {
                        proposedPrice = 0;
                    }else{
                        proposedPrice = lowestSellPrice.price - 0.01;
                    }
                }else if(latestExportedData.Where(i => i.jumps == 0 && i.bid == false).Count() > 1){
                    //sinon si on est le plus bas, on va se raprocher du second plus bas
                    ObjectOtherOrders secondLowestSellPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == false).OrderBy(i => i.price).ToArray()[1];
                    if (secondLowestSellPrice.price - orderAmmount > 0.02)
                    {
                        proposedPrice = secondLowestSellPrice.price - 0.01;
                    }
                }else{                        
                    proposedPrice = 0;
                }

            }
            else
            {
                ObjectOtherOrders highestBuyPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == true).OrderByDescending(i => i.price).First();

                //si on n'est pas le plus haut
                if (highestBuyPrice.price > orderAmmount)
                {
                    //on regarde si cet item est en vente aussi
                    if (latestExportedData.Where(i => i.jumps == 0 && i.bid == false).Count() > 0)
                    {
                        ObjectOtherOrders lowestSellPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == false).OrderBy(i => i.price).First();
                        //on regarde si le profit < 10% (ET cet item est en vente)
                        if (((lowestSellPrice.price - highestBuyPrice.price) / highestBuyPrice.price) < 0.1)
                        {
                            proposedPrice = 0;
                        }
                        else
                        {
                            proposedPrice = highestBuyPrice.price + 0.01;
                        }
                    }
                    else
                    {
                        //si il n'est pas en vente, on suis le marché
                        proposedPrice = highestBuyPrice.price + 0.01;
                    }
                }
                else if (latestExportedData.Where(i => i.jumps == 0 && i.bid == true).Count() > 1)
                {
                    //sinon, on essai de se rapprocher du 2ieme
                    ObjectOtherOrders secondHighestBuyPrice = latestExportedData.Where(i => i.jumps == 0 && i.bid == true).OrderByDescending(i => i.price).ToArray()[1];
                    if (orderAmmount - secondHighestBuyPrice.price > 0.02)
                    {
                        proposedPrice = secondHighestBuyPrice.price + 0.01;
                    }
                }else{
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
