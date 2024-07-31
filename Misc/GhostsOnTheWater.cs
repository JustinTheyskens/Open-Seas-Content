using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Commands;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public static class GhostsOnTheWater
    {
        private static Point3D[] _Locations = EscapeStone.Locations;

        public static void GetPlayer(Mobile player)
        {
            WaitTimer timer = new WaitTimer(player);
            timer.Start();
            player.PrivateOverheadMessage(MessageType.Regular, 0, true, "You've died on the open seas. If you are not resurrected, your spirit will be transported to safe location.", player.NetState);
        }

        public static void MovePlayer(Mobile player)
        {
            if (player.Alive)
                return;

            int random = Utility.RandomMinMax(0, _Locations.Length - 1);
            Point3D p = _Locations[random];
            player.MoveToWorld(p, Map.Felucca);
            player.SendMessage("Your spirit has been transported to city where you may seek out a healer.");
        }
    }

    public class WaitTimer : Timer
    {
        private Mobile Player;
        private DateTime End;
        public WaitTimer(Mobile player)
            : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
        {
            Player = player;
            End = DateTime.Now + TimeSpan.FromMinutes(1);
        }

        protected override void OnTick()
        {
            if (Player == null || Player.Alive)
                Stop();

            if (End <= DateTime.UtcNow)
            {
                GhostsOnTheWater.MovePlayer(Player);
                Stop();
            }
        }
    }
}
