using System;
using System.Collections.Generic;
using Server.Items;
using Server.Multis;
using Server.Regions;

namespace Server.Mobiles
{
    public class NewSBShipwright : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo;
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public NewSBShipwright(Mobile m)
        {
            if (m != null)
            {
                m_BuyInfo = new InternalBuyInfo(m);
            }
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return m_SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(Mobile m)
            {
                Add(new GenericBuyInfo("Fishing Boat", typeof(FishingBoatDeed), 2500, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041205", typeof(SmallShipDeed), 10177, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041206", typeof(SmallDragonShipDeed), 10177, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041207", typeof(MediumShipDeed), 11552, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041208", typeof(MediumDragonShipDeed), 11552, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041209", typeof(LargeShipDeed), 12927, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041210", typeof(LargeDragonShipDeed), 12927, 20, 0x14F2, 0));

                Add(new GenericBuyInfo(typeof(Spyglass), 3, 20, 0x14F5, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                //You technically CAN sell them back, *BUT* the vendors do not carry enough money to buy with
                Add(typeof(Spyglass), 1);
            }
        }
    }
}
