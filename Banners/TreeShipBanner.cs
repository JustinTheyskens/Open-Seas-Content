using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class TreeShipBanner : ShipBanner
    {
        public override ShipBannerDeed GetDeed { get { return new TreeShipBannerDeed(); } }

        [Constructable]
        public TreeShipBanner(BaseShip ship)
            : base(ship)
        {

        }

        public TreeShipBanner(Serial serial) : base(serial)
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

    public class TreeShipBannerDeed : ShipBannerDeed
    {
        public override ShipBannerType BannerType { get { return ShipBannerType.Tree; } }

        [Constructable]
        public TreeShipBannerDeed()
        {
            Name = "Tree Ship Banner";
        }

        public TreeShipBannerDeed(Serial serial)
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
