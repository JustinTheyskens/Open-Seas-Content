using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Items
{
    public interface INewShipCannon : IEntity
    {
        int Hits { get; set; }
        int Range { get; }
        AmmunitionType AmmoType { get; set; }
        BaseShip Ship { get; set; }
        DamageLevel DamageState { get; set; }
        Direction Facing { get; }
        ShipPosition Position { get; set; }
        NewShipCannonDeed GetDeed { get; }
        bool CanLight { get; }

        Direction GetFacing();
        void OnDamage(int damage, Mobile shooter);
        void LightFuse(Mobile from);
        void Shoot(object cannoneer);
        void DoAreaMessage(int cliloc, int range, Mobile from);
    }

    public class SmallShipCannon : NewShipCannon
    {
        public override int Range { get { return 10; } }
        public override NewShipCannonDeed GetDeed { get { return new SmallCannonDeed(); } }
        public override CannonPower Power { get { return CannonPower.Light; } }

        public SmallShipCannon(BaseShip ship) : base(ship)
        {
            Name = "Ship Cannon";
        }

        public SmallShipCannon(Serial serial) : base(serial) { }

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

    public class LargeShipCannon : NewShipCannon
    {
        public override int Range { get { return 10; } }
        public override NewShipCannonDeed GetDeed { get { return new LargeCannonDeed(); } }
        public override CannonPower Power { get { return CannonPower.Heavy; } }
        public LargeShipCannon(BaseShip ship) : base(ship)
        {
            Name = "Ship Cannon";
        }

        public LargeShipCannon(Serial serial) : base(serial) { }

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

    public class PowerfulShipCannon : NewShipCannon
    {
        public override int Range { get { return 10; } }
        public override NewShipCannonDeed GetDeed { get { return new PowerfulCannonDeed(); } }
        public override CannonPower Power { get { return CannonPower.Heavy; } }
        public PowerfulShipCannon(BaseShip ship) : base(ship)
        {
            Name = "Ship Cannon";
            Hue = 2406;
        }

        public PowerfulShipCannon(Serial serial) : base(serial) { }

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
