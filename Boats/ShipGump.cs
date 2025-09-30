using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    internal class ShipGump : Gump
    {
        private readonly BaseBoat Boat;
        private readonly PlayerMobile Player;

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
            Closable = true;    // Allows the gump to be closed
            Disposable = true;  // Automatically disposes of the gump when closed
            Dragable = true;    // Allows the gump to be dragged
            Resizable = false;  // Prevents resizing of the gump
        }

        public void AddGumpLayout(int confirm)
        {
            AddBackground(0, 30, 400, 330, 6644);

            if (Boat != null)
            {
                string shipName;
                if (Boat.ShipName == null)
                {
                    shipName = "Unnamed";
                }
                else
                {
                    shipName = Boat.ShipName;
                }

                AddHtml(20, 40, 190, 16, FormatGump("Ship Name: ", "#FFFF00"), false, false);
                AddHtml(100, 40, 190, 16, FormatGump(shipName, "#32CD32"), false, false);

                if (Boat is BaseShip ship)
                {
                    if (ship != null)
                    {
                        AddHtml(20, 60, 190, 16, FormatGump("Ship Type: ", "#FFFF00"), false, false);
                        AddHtml(100, 60, 190, 16, FormatGump(ship.GetTypeName(), "#F0F8FF"), false, false);

                        AddHtml(220, 40, 190, 16, FormatGump("Total Copper:", "#FFFF00"), false, false);

                        var amount = 0;
                        var copper = Player.Backpack.FindItemByType(typeof(Copper));
                        if (copper != null)
                        {
                            amount = copper.Amount;
                        }

                        AddHtml(220, 60, 190, 16, FormatGump(amount.ToString(), "#B87333"), false, false);

                        int cannons;
                        if (ship.Cannons != null)
                        {
                            cannons = ship.Cannons.Count;
                        }
                        else
                        {
                            cannons = 0;
                        }

                        AddHtml(20, 80, 190, 16, FormatGump("Cannons: ", "#FFFF00"), false, false);
                        AddHtml(95, 80, 190, 16, FormatGump(string.Format(" {0} / {1}", cannons, ship.CurrentMaxCannons), "#F0F8FF"), false, false); // change starting to current

                        AddHtml(20, 100, 190, 16, FormatGump("Cannon Slots: ", "#FFFF00"), false, false);
                        AddHtml(110, 100, 190, 16, FormatGump(string.Format(" {0} / {1}", ship.CurrentMaxCannons, ship.MaxCannons), "#F0F8FF"), false, false); // change starting to current
                        AddImage(350, 0, 0x589);
                        AddItem(377, 7, 0x14F7); //anchor

                        AddButton(20, 130, 30083, -1, 1, GumpButtonType.Reply, 0);
                        AddHtml(100, 130, 190, 16, FormatGump("750", "#B87333"), false, false);

                        for (var i = 0; i < ship.MaxCannons; i++)
                        {
                            AddItem(20 + (i * 45), 130, 0xA7CD); // cannons
                        }

                        if (ship.CurrentMaxCannons < ship.MaxCannons)
                        {
                            for (var j = ship.CurrentMaxCannons; j < ship.MaxCannons; ++j) // change starting to current
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
                            AddButton(20, 320, 30083, -1, 2, GumpButtonType.Reply, 0);
                        }

                        AddHtml(160, 210, 190, 16, FormatGump("Upgraded Hold: ", "#F0F8FF"), false, false);
                        AddItem(165, 230, 0x3E96);
                        if (!ship.UpgradedHold)
                        {
                            AddImage(175, 260, 0x82C); // lock
                            AddHtml(170, 300, 190, 16, FormatGump("500", "#B87333"), false, false);
                            AddButton(170, 320, 30083, -1, 3, GumpButtonType.Reply, 0);
                        }

                        AddHtml(300, 210, 190, 16, FormatGump("Sail Banners: ", "#F0F8FF"), false, false);
                        AddItem(305, 230, 0x4C28); // painting
                        if (!ship.BannerUpgrade)
                        {
                            AddImage(315, 260, 0x82C); // lock
                            AddHtml(310, 300, 190, 16, FormatGump("250", "#B87333"), false, false);
                            AddButton(310, 320, 30083, -1, 4, GumpButtonType.Reply, 0);
                        }
                    }

                    switch (confirm)
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
            {
                return string.Format("<div align=left>{0}</div>", val);
            }
            else
            {
                return string.Format("<BASEFONT COLOR={1}><dic align=left>{0}</div>", val, color);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var button = info.ButtonID;
            if (button == 0)
            {
                return;
            }
            if (Boat is BaseShip ship)
            {
                if (ship == null)
                {
                    return;
                }

                var copper = Player.Backpack.FindItemByType(typeof(Copper));

                // Fix: Handle case where player has no copper
                if (copper == null)
                {
                    _ = Player.SendGump(new ShipGump(Boat, Player, 10));
                    return; // Exit the method to prevent further actions
                }

                var amount = copper.Amount;
                var price = 0;

                if (button == 1 || button == 5)
                {
                    price = 750;
                }
                else if (button == 2 || button == 7)
                {
                    price = 1000;
                }
                else if (button == 3 || button == 8)
                {
                    price = 500;
                }
                else if (button == 4 || button == 9)
                {
                    price = 250;
                }

                switch (button)
                {
                    case 0:
                    _ = Player.CloseGump(typeof(ShipGump));
                    break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    if (amount >= price)
                    {
                        _ = Player.SendGump(new ShipGump(Boat, Player, info.ButtonID));
                    }
                    else
                    {
                        _ = Player.SendGump(new ShipGump(Boat, Player, 10));
                    }

                    break;
                    case 5:
                    if (amount >= price)
                    {
                        ship.CurrentMaxCannons += 1;
                        if (copper.Amount > price)
                        {
                            copper.Amount -= price;
                        }
                        else if (copper.Amount == price)
                        {
                            copper.Delete();
                        }

                        _ = Player.SendGump(new ShipGump(Boat, Player, 0));
                        Player.PlaySound(0x2E6);
                    }
                    else
                    {
                        _ = Player.SendGump(new ShipGump(Boat, Player, 10));
                    }

                    break;
                    case 6:
                    _ = Player.SendGump(new ShipGump(Boat, Player, 0));
                    break;
                    case 7:
                    if (amount >= price)
                    {
                        ship.UpgradedHull = true;
                        if (copper.Amount > price)
                        {
                            copper.Amount -= price;
                        }
                        else if (copper.Amount == price)
                        {
                            copper.Delete();
                        }

                        _ = Player.SendGump(new ShipGump(Boat, Player, 0));
                        Player.PlaySound(0x2E6);
                    }
                    else
                    {
                        _ = Player.SendGump(new ShipGump(Boat, Player, 10));
                    }

                    break;
                    case 8:
                    if (amount >= price)
                    {
                        ship.UpgradeHold();
                        if (copper.Amount > price)
                        {
                            copper.Amount -= price;
                        }
                        else if (copper.Amount == price)
                        {
                            copper.Delete();
                        }

                        _ = Player.SendGump(new ShipGump(Boat, Player, 0));
                        Player.PlaySound(0x2E6);
                    }
                    else
                    {
                        _ = Player.SendGump(new ShipGump(Boat, Player, 10));
                    }

                    break;
                    case 9:
                    if (amount >= price)
                    {
                        ship.BannerUpgrade = true;
                        if (copper.Amount > price)
                        {
                            copper.Amount -= price;
                        }
                        else if (copper.Amount == price)
                        {
                            copper.Delete();
                        }

                        _ = Player.SendGump(new ShipGump(Boat, Player, 0));
                        Player.PlaySound(0x2E6);
                    }
                    else
                    {
                        _ = Player.SendGump(new ShipGump(Boat, Player, 10));
                    }

                    break;
                }
            }
        }
    }
}
