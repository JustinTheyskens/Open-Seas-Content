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
    public class SeaFortTeleporter : Item
    {
        private Point3D _Destination;
        private SeaFortController _Controller;

        [CommandProperty(AccessLevel.GameMaster)]
        public SeaFortController Controller
        {
            get { return _Controller; }
            set { _Controller = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }

        private BaseSeaFort Fort;

        public SeaFortTeleporter(BaseSeaFort fort)
            : base(0x07C9)
        {
            Fort = fort;
            Name = "Entrance";
            Movable = false;
            Visible = false;

            Timer.DelayCall(TimeSpan.FromSeconds(0.2), () =>
            {
                Direction d = GetDirectionTo(new Point2D(fort.X, fort.Y));
                if (d == Direction.North || d == Direction.South)
                    ItemID = 0x07CD;
            });
        }

        public Direction GetDirectionTo(int x, int y)
        {
            int dx = X - x;
            int dy = Y - y;

            int rx = (dx - dy) * 44;
            int ry = (dx + dy) * 44;

            int ax = Math.Abs(rx);
            int ay = Math.Abs(ry);

            Direction ret;

            if (((ay >> 1) - ax) >= 0)
            {
                ret = (ry > 0) ? Direction.Up : Direction.Down;
            }
            else if (((ax >> 1) - ay) >= 0)
            {
                ret = (rx > 0) ? Direction.Left : Direction.Right;
            }
            else if (rx >= 0 && ry >= 0)
            {
                ret = Direction.West;
            }
            else if (rx >= 0 && ry < 0)
            {
                ret = Direction.South;
            }
            else if (rx < 0 && ry < 0)
            {
                ret = Direction.East;
            }
            else
            {
                ret = Direction.North;
            }

            return ret;
        }

        public Direction GetDirectionTo(Point2D p)
        {
            return GetDirectionTo(p.X, p.Y);
        }

        public SeaFortTeleporter(Serial serial)
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
            if (Fort == null)
                return;

            if (BaseBoat.IsDriving(from))
            {
                from.SendLocalizedMessage(1116610); // You can't do that while piloting a ship!
                return;
            }

            if (from.InRange(GetWorldLocation(), 8) && !Fort.Contains(from) && (Controller != null && !from.InRange(Controller, 5)))
            {
                //from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502502); // That is locked but your godly powers allow access

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
