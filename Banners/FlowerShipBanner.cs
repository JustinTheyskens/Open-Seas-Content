using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class FlowerShipBanner : ShipBanner
    {
        public override ShipBannerDeed GetDeed { get { return new FlowerShipBannerDeed(); } }

        [Constructable]
        public FlowerShipBanner(BaseShip ship)
            : base(ship)
        {

        }

        public FlowerShipBanner(Serial serial) : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

    }

    public class FlowerShipBannerDeed : ShipBannerDeed
    {
        public override ShipBannerType BannerType { get { return ShipBannerType.Flower; } }

        [Constructable]
        public FlowerShipBannerDeed()
        {
            Name = "Flower Ship Banner";
        }

        public FlowerShipBannerDeed(Serial serial)
            : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
