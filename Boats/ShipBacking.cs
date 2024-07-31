using System;
using Server;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using System.Collections.Generic;
using Ultima;

namespace Server.Items
{
    public class ShipBacking : TillerMan
    {
        public override bool Babbles { get { return false; } }
        public BaseBoat Boat { get; private set; }

        public ShipBacking(BaseBoat boat)
            : base(0x3EAC)
        {
            Boat = boat;
            Movable = false;
        }

        public ShipBacking(Serial serial)
            : base(serial)
        {
        }

        public override void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.South: ItemID = 0x3EAC; break;
                case Direction.North: ItemID = 0x3EAB; break;
                case Direction.West: ItemID = 0x3E61; break;
                case Direction.East: ItemID = 0x3E83; break; 
            }
        }

        public override void Say(int number)
        {
            //PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public override void Say(int number, string args)
        {
            //PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version

            writer.Write(Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

        }
    }
}
