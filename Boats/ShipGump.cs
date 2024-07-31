using System;
using Server.Accounting;
using System.Diagnostics;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    internal class ShipGump : Gump
    {
        private BaseBoat Boat;
        private PlayerMobile Player;

        public ShipGump(BaseBoat boat, PlayerMobile player)
            : this(boat, player, 0)
        {
        }

        public ShipGump(BaseBoat boat, PlayerMobile player, int confirm)
            : base(250, 150)
        {
            Boat = boat;
            Player = player;
            AddGumpLayout(confirm);
        }

        public void AddGumpLayout(int confirm)
        {
            AddBackground(0, 30, 400, 330, 0x6DB);
            
            if (Boat != null)
            {
                string shipName;
                if (Boat.ShipName == null)
                    shipName = "Unnamed";
                else
                    shipName = Boat.ShipName;

                AddHtml(20, 40, 190, 16, FormatGump("Ship Name: ", "#FFFF00"), false, false);
                AddHtml(100, 40, 190, 16, FormatGump(shipName, "#32CD32"), false, false);

                if (Boat is BaseShip)
                {
                    BaseShip ship = (BaseShip)Boat;
                    if (ship != null)
                    {
                        AddHtml(20, 60, 190, 16, FormatGump("Ship Type: ", "#FFFF00"), false, false);
                        AddHtml(100, 60, 190, 16, FormatGump(ship.GetTypeName(), "#F0F8FF"), false, false);

                        AddHtml(220, 40, 190, 16, FormatGump("Total Copper:", "#FFFF00"), false, false);

                        int amount = 0;
                        Item copper = Player.Backpack.FindItemByType(typeof(Copper));
                        if (copper != null)
                            amount = copper.Amount;

                        AddHtml(220, 60, 190, 16, FormatGump(amount.ToString(), "#B87333"), false, false);

                        int cannons;
                        if (ship.Cannons != null)
                            cannons = ship.Cannons.Count;
                        else
                            cannons = 0;

                        AddHtml(20, 80, 190, 16, FormatGump("Cannons: ", "#FFFF00"), false, false); 
                        AddHtml(95, 80, 190, 16, FormatGump(string.Format(" {0} / {1}", cannons, ship.CurrentMaxCannons), "#F0F8FF"), false, false); // change starting to current

                        AddHtml(20, 100, 190, 16, FormatGump("Cannon Slots: ", "#FFFF00"), false, false); 
                        AddHtml(110, 100, 190, 16, FormatGump(string.Format(" {0} / {1}", ship.CurrentMaxCannons, ship.MaxCannons), "#F0F8FF"), false, false); // change starting to current
                        AddImage(350, 0, 0x589);
                        AddItem(377, 7, 0x14F7); //anchor

                        AddButton(20, 130, 0x99C, 0x99D, 1, GumpButtonType.Reply, 0);
                        AddHtml(100, 130, 190, 16, FormatGump("750", "#B87333"), false, false);

                        for (int i = 0; i < ship.MaxCannons; i++)
                        {
                            AddItem(20 + (i * 45), 130, 0xA7CD); // cannons
                        }

                        if (ship.CurrentMaxCannons < ship.MaxCannons)
                        {
                            for (int j = ship.CurrentMaxCannons; j < ship.MaxCannons; ++j) // change starting to current
                            {
                                AddImage(35 + (j * 45), 155, 0x82C); //lock
                            }
                        }

                        AddHtml(20, 210, 190, 16, FormatGump("Reinforced Hull: ", "#F0F8FF"), false, false);
                        AddItem(40, 235, 0x3E9D);

                        if (!ship.UpgradedHull)
                        {
                            AddImage(50, 255, 0x82C); // lock
                            AddHtml(20, 300, 190, 16, FormatGump("1000", "#B87333"), false, false);
                            AddButton(20, 320, 0x99C, 0x99D, 2, GumpButtonType.Reply, 0);
                        }

                        AddHtml(160, 210, 190, 16, FormatGump("Upgraded Hold: ", "#F0F8FF"), false, false);
                        AddItem(165, 230, 0x3E96);
                        if (!ship.UpgradedHold)
                        {
                            AddImage(175, 260, 0x82C); // lock
                            AddHtml(170, 300, 190, 16, FormatGump("500", "#B87333"), false, false);
                            AddButton(170, 320, 0x99C, 0x99D, 3, GumpButtonType.Reply, 0);
                        }

                        AddHtml(300, 210, 190, 16, FormatGump("Sail Banners: ", "#F0F8FF"), false, false);
                        AddItem(305, 230, 0x4C28); // painting
                        if (!ship.BannerUpgrade)
                        {
                            AddImage(315, 260, 0x82C); // lock
                            AddHtml(310, 300, 190, 16, FormatGump("250", "#B87333"), false, false);
                            AddButton(310, 320, 0x99C, 0x99D, 4, GumpButtonType.Reply, 0);
                        }
                    }

                    switch(confirm)
                    {
                        case 1:
                            AddBackground(100, 110, 250, 150, 0x6DB);
                            AddHtml(130, 120, 190, 80, FormatGump("Are you sure you wish to add a cannon slot to your ship?", "#F0F8FF"), false, false);

                            AddButton(150, 200, 0xFB7, 0xFB9, 5, GumpButtonType.Reply, 0);

                            AddButton(250, 200, 0xFB4, 0xFB6, 6, GumpButtonType.Reply, 0);
                            break;
                        case 2:
                            AddBackground(100, 110, 250, 150, 0x6DB);
                            AddHtml(130, 120, 190, 80, FormatGump("Are you sure you wish to reinforce your ship's hull?", "#F0F8FF"), false, false);

                            AddButton(150, 200, 0xFB7, 0xFB9, 7, GumpButtonType.Reply, 0);

                            AddButton(250, 200, 0xFB4, 0xFB6, 6, GumpButtonType.Reply, 0);
                            break;
                        case 3:
                            AddBackground(100, 110, 250, 150, 0x6DB);
                            AddHtml(130, 120, 190, 80, FormatGump("Are you sure you wish to upgrade your ship's hold?", "#F0F8FF"), false, false);

                            AddButton(150, 200, 0xFB7, 0xFB9, 8, GumpButtonType.Reply, 0);

                            AddButton(250, 200, 0xFB4, 0xFB6, 6, GumpButtonType.Reply, 0);
                            break;
                        case 4:
                            AddBackground(100, 110, 250, 150, 0x6DB);
                            AddHtml(130, 120, 190, 80, FormatGump("Are you sure you wish to add sail banners to your ship?", "#F0F8FF"), false, false);

                            AddButton(150, 200, 0xFB7, 0xFB9, 9, GumpButtonType.Reply, 0);

                            AddButton(250, 200, 0xFB4, 0xFB6, 6, GumpButtonType.Reply, 0);
                            break;
                        case 10:
                            AddBackground(100, 110, 250, 150, 0x6DB);
                            AddHtml(130, 120, 190, 80, FormatGump("You do not have enough Copper to purchase that upgrade.", "#F0F8FF"), false, false);

                            AddButton(200, 200, 0xFB7, 0xFB9, 6, GumpButtonType.Reply, 0);
                            break;

                    }
                }
            }
        }

        public string FormatGump(string val, string color)
        {
            if (color == null)
                return String.Format("<div align=left>{0}</div>", val);
            else
                return String.Format("<BASEFONT COLOR={1}><dic align=left>{0}</div>", val, color);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int button = info.ButtonID;
            if (Boat is BaseShip)
            {
                BaseShip ship = (BaseShip)Boat;
                if (ship == null)
                    return;

                Item copper = Player.Backpack.FindItemByType(typeof(Copper));
                if (copper == null)
                    Player.SendGump(new ShipGump(Boat, Player, 10));

                int amount = 0;
                if (copper != null)
                    amount = copper.Amount;

                int price = 0;
                if (button == 1 || button == 5)
                    price = 750;
                else if (button == 2 || button == 7)
                    price = 1000;
                else if (button == 3 || button == 8)
                    price = 500;
                else if (button == 4 || button == 9)
                    price = 250;

                switch (button)
                {
                    case 0:
                        Player.CloseGump(typeof(ShipGump));
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4: // add buttons
                        if (amount >= price)
                        {
                            Player.SendGump(new ShipGump(Boat, Player, info.ButtonID));
                        }
                        else
                            Player.SendGump(new ShipGump(Boat, Player, 10)); // not enough copper :(
                        break;
                    case 5: // confirm cannon slot
                        if (amount >= price)
                        {
                            ship.CurrentMaxCannons += 1;
                            
                            if (copper != null)
                            {
                                if (amount > price)
                                    copper.Amount -= price;
                                else if (amount == price)
                                    copper.Delete();
                            }
                            Player.SendGump(new ShipGump(Boat, Player, 0));
                            Player.PlaySound(0x2E6);
                        }
                        else
                            Player.SendGump(new ShipGump(Boat, Player, 10));
                        break;
                    case 6: // cancel
                        Player.SendGump(new ShipGump(Boat, Player, 0));
                        break;
                    case 7: // confirm upgrade hull
                        if (amount >= price)
                        {
                            ship.UpgradedHull = true;

                            if (copper != null)
                            {
                                if (amount > price)
                                    copper.Amount -= price;
                                else if (amount == price)
                                    copper.Delete();
                            }
                            Player.SendGump(new ShipGump(Boat, Player, 0));
                            Player.PlaySound(0x2E6);
                        }
                        else
                            Player.SendGump(new ShipGump(Boat, Player, 10));
                        break;
                    case 8: // confirm upgrade hold
                        if (amount >= price)
                        {
                            ship.UpgradedHold = true;

                            if (copper != null)
                            {
                                if (amount > price)
                                    copper.Amount -= price;
                                else if (amount == price)
                                    copper.Delete();
                            }
                            Player.SendGump(new ShipGump(Boat, Player, 0));
                            Player.PlaySound(0x2E6);
                        }
                        else
                            Player.SendGump(new ShipGump(Boat, Player, 10));
                        break;
                    case 9: // confirm add sail banners
                        if (amount >= price)
                        {
                            ship.BannerUpgrade = true;

                            if (copper != null)
                            {
                                if (amount > price)
                                    copper.Amount -= price;
                                else if (amount == price)
                                    copper.Delete();
                            }
                            Player.SendGump(new ShipGump(Boat, Player, 0));
                            Player.PlaySound(0x2E6);
                        }
                        else
                            Player.SendGump(new ShipGump(Boat, Player, 10));
                        break;
                }

            }
        }
    }
}
