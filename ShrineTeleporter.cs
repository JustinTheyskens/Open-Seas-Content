using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class SeaShrineTeleporter : Item
    {
        private BaseSeaShrine Shrine;
        private InternalTimer Timer;

        [Constructable]
        public SeaShrineTeleporter(BaseSeaShrine shrine, int id)
            : this(shrine, id, 0)
        {
        }

        [Constructable]
        public SeaShrineTeleporter(BaseSeaShrine shrine, int id, int hue)
            : base(id)
        {
            Name = "Mystical Symbol";
            Shrine = shrine;
            Hue = hue;
            Movable = false;
            Timer = new InternalTimer(this);
            Timer.Start();
        }

        public SeaShrineTeleporter(Serial serial)
            : base(serial) { }



        public override void OnDoubleClickDead(Mobile from)
        {
            OnDoubleClick(from);
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (Shrine == null)
                return;

            if (BaseBoat.IsDriving(from))
            {
                from.SendLocalizedMessage(1116610); // You can't do that while piloting a ship!
                return;
            }

            if (from.InRange(GetWorldLocation(), 8) && !Shrine.Contains(from) && !from.InRange(Shrine.GetWorldLocation(), 3))
            {
                Point3D p = new Point3D(X, Y, Z + 3);

                BaseCreature.TeleportPets(from, p, Map);
                from.Location = p;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(Shrine);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Timer = new InternalTimer(this);
            Timer.Start();

            if (version > 0)
                Shrine = reader.ReadItem() as BaseSeaShrine;
        }

        private class InternalTimer : Timer
        {
            private Item Tele;
            public InternalTimer(Item tele)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                Tele = tele;
            }

            protected override void OnTick()
            {
                if (Tele == null || Tele.Deleted)
                {
                    Stop();
                    return;
                }

                Effects.SendLocationEffect(Tele.Location, Tele.Map, 0x376A, 30);
            }
        }
    }
}
