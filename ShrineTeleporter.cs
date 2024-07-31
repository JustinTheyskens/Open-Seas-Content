using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Items
{
    public class SeaShrineTeleporter : Item
    {
        public Point3D Destination;
        private BaseSeaShrine Shrine;

        [Constructable]
        public SeaShrineTeleporter(BaseSeaShrine shrine, int id)
            : base(id)
        {
            Shrine = shrine;
            Name = "Ruins";
            Movable = false;
        }

        public SeaShrineTeleporter(Serial serial)
            : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

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
                Point3D p;
                if (Destination != Point3D.Zero)
                    p = Destination;
                else
                    p = new Point3D(X, Y, Z + 3);

                BaseCreature.TeleportPets(from, p, Map);
                from.Location = p;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
