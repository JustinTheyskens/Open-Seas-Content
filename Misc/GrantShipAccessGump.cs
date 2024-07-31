using System;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    public class GrantShipAccessGump : Gump
    {
        public static readonly int NAHue = 0x5EF7;
        public static readonly int PassengerHue = 0x1CFF;
        public static readonly int CrewHue = 0x1FE7;
        public static readonly int OfficerHue = 0x7FE7;
        public static readonly int DenyHue = 0x7CE7;
        public static readonly int CaptainHue = 0x7DE7;
        public static readonly int LabelColor = 0x7FFF;
        public static readonly int NoHue = 0x3DEF;

        private Mobile m_Player;
        private BaseShip m_Ship;
        private ShipSecurityEntry m_Entry;

        public GrantShipAccessGump(Mobile player, BaseShip ship)
            : base(100, 100)
        {
            player.CloseGump(typeof(GrantShipAccessGump));

            m_Player = player;
            m_Ship = ship;
            m_Entry = ship.SecurityEntry;

            AddPage(0);

            AddBackground(0, 0, 320, 385, 0xA3C);
            AddHtmlLocalized(10, 10, 300, 18, 1149724, 0x7FEF, false, false); //<CENTER>Passenger and Crew Manifest</CENTER>

            string shipName = "unnamed ship";
            if (ship.ShipName != null && ship.ShipName != string.Empty && ship.ShipName != "")
                shipName = ship.ShipName;

            AddHtmlLocalized(10, 38, 75, 18, 1149761, LabelColor, false, false); //Ship:
            AddLabel(80, 38, 0x53, shipName);

            AddHtmlLocalized(10, 56, 75, 18, 1149762, LabelColor, false, false); //Owner:
            AddLabel(80, 56, 0x53, ship.Owner != null ? ship.Owner.Name : "Unknown");

            if (m_Entry == null)
            {
                m_Entry = new ShipSecurityEntry(m_Ship);
                m_Ship.SecurityEntry = m_Entry;
            }

            bool isAccessed = m_Entry.Manifest.ContainsKey(player);
            bool inGuild = m_Entry.IsInGuild(player);
            bool inParty = m_Entry.IsInParty(player);
            bool isPublic = m_Entry.IsPublic;
            SecurityLevel level = m_Entry.GetEffectiveLevel(player);

            //Player Info
            AddHtmlLocalized(10, 79, 125, 18, 1149763, LabelColor, false, false); // Player: 
            AddLabel(140, 79, 0x30, player.Name);

            AddHtmlLocalized(10, 97, 125, 18, 1149768, LabelColor, false, false); // Effective Level:
            AddHtmlLocalized(140, 97, 160, 18, GetLevel(level), GetHue(level), false, false);

            //Default Info
            int cliloc = isPublic ? 1149756 : 1149757;
            int hue = isPublic ? CrewHue : NoHue;

            AddHtmlLocalized(10, 120, 125, 18, 1149731, LabelColor, false, false); // Public Access:
            AddHtmlLocalized(140, 120, 50, 18, cliloc, hue, false, false); // Yes/No

            if (isPublic)
                AddHtmlLocalized(200, 120, 100, 18, GetLevel(m_Entry.DefaultPublicAccess), GetHue(m_Entry.DefaultPublicAccess), false, false);

            cliloc = inParty ? 1149756 : 1149757;
            hue = inParty ? CrewHue : NoHue;

            AddHtmlLocalized(10, 138, 125, 18, 1149769, LabelColor, false, false); // Is Party Member:
            AddHtmlLocalized(140, 138, 50, 18, cliloc, hue, false, false);

            if (inParty)
                AddHtmlLocalized(200, 138, 50, 18, GetLevel(m_Entry.DefaultPartyAccess), GetHue(m_Entry.DefaultPartyAccess), false, false);

            cliloc = inGuild ? 1149756 : 1149757;
            hue = inGuild ? CrewHue : NoHue;

            AddHtmlLocalized(10, 156, 125, 18, 1149770, LabelColor, false, false); // Is Guild Member
            AddHtmlLocalized(140, 156, 50, 18, cliloc, hue, false, false);

            if (inGuild)
                AddHtmlLocalized(200, 156, 50, 18, GetLevel(m_Entry.DefaultGuildAccess), GetHue(m_Entry.DefaultGuildAccess), false, false);

            AddHtmlLocalized(10, 179, 300, 18, 1149747, LabelColor, false, false); // Access List Status:

            if (level == SecurityLevel.NA)
            {
                AddImage(65, 197, 0xFA6);
                AddHtmlLocalized(100, 199, 200, 18, 1149775, NoHue, false, false); // NOT IN ACCESS LIST
            }
            else
            {
                AddButton(65, 197, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(100, 199, 200, 18, 1149776, NoHue, false, false); // REMOVE FROM LIST
            }

            if (level == SecurityLevel.Denied)
            {
                AddImage(65, 215, 0xFA6);
            }
            else
            {
                AddButton(65, 215, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 217, 100, 18, 1149726, level == SecurityLevel.Denied ? GetHue(level) : LabelColor, false, false); // DENY ACCESS

            if (level == SecurityLevel.Passenger)
            {
                AddImage(65, 233, 0xFA6);
            }
            else
            {
                AddButton(65, 233, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 235, 100, 18, 1149727, level == SecurityLevel.Passenger ? GetHue(level) : LabelColor, false, false); // PASSENGER

            if (level == SecurityLevel.Crewman)
            {
                AddImage(65, 251, 0xFA6);
            }
            else
            {
                AddButton(65, 251, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 253, 100, 18, 1149728, level == SecurityLevel.Crewman ? GetHue(level) : LabelColor, false, false); // CREW

            if (level == SecurityLevel.Officer)
            {
                AddImage(65, 269, 0xFA6);
            }
            else
            {
                AddButton(65, 269, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 271, 100, 18, 1149729, level == SecurityLevel.Officer ? GetHue(level) : LabelColor, false, false); // OFFICER

            if (level == SecurityLevel.Captain)
            {
                AddImage(65, 287, 0xFA6);
            }
            else
            {
                AddButton(65, 287, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 289, 100, 18, 1149730, level == SecurityLevel.Captain ? GetHue(level) : LabelColor, false, false); // CAPTAIN

            AddButton(10, 355, 0xFA5, 0xFA7, 7, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 357, 100, 18, 1149777, LabelColor, false, false); // MAIN MENU

            AddButton(160, 355, 0xFA5, 0xFA, 8, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 357, 100, 18, 1149734, LabelColor, false, false); // ACCESS LIST
        }

        public int GetHue(SecurityLevel level)
        {
            switch (level)
            {
                case SecurityLevel.Captain: return CaptainHue;
                case SecurityLevel.Officer: return OfficerHue;
                case SecurityLevel.Crewman: return CrewHue;
                case SecurityLevel.Passenger: return PassengerHue;
                case SecurityLevel.NA: return NAHue;
                case SecurityLevel.Denied: return DenyHue;
                default: return LabelColor;
            }
        }

        public static int GetLevel(SecurityLevel level)
        {
            switch (level)
            {
                default:
                case SecurityLevel.Denied: return 1149726;
                case SecurityLevel.Passenger: return 1149727;
                case SecurityLevel.Crewman: return 1149728;
                case SecurityLevel.Officer: return 1149729;
                case SecurityLevel.Captain: return 1149730;
                case SecurityLevel.NA: return 1149725;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null)
                return;

            switch (info.ButtonID)
            {
                case 0: return;
                case 1: // REMOVE FROM LIST
                    m_Entry.RemoveFromAccessList(m_Player);
                    from.SendGump(new ShipAccessListGump(m_Player, m_Ship));
                    return;
                case 2: // DENY ACCESS
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Denied);
                    break;
                case 3: // PASSENGER
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Passenger);
                    break;
                case 4: // CREW
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Crewman);
                    break;
                case 5: // OFFICER
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Officer);
                    break;
                case 6: // CAPTAIN	
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Captain);
                    break;
                case 7: // MAIN MENU
                    from.SendGump(new NewShipSecurityGump(from, m_Ship));
                    return;
                case 8: // ACCESS LIST
                    from.SendGump(new ShipAccessListGump(m_Player, m_Ship));
                    return;
            }

            from.SendGump(new GrantShipAccessGump(m_Player, m_Ship));
        }
    }
}
