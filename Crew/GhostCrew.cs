using Server;
using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class GhostCrew : BaseCreature
    {
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ClickTitle { get { return false; } }

        [Constructable]
        public GhostCrew() : base(AIType.AI_Archer, FightMode.Closest, 10, 4, 0.2, 0.4)
        {
            Name = "a ghostly crewman";

            Body = Utility.RandomDouble() > 0.75 ? 26 : 0x190;
            Hue = 2448;
            //SpellHue = 2100;

            SetHits(2000);

            if (Body == 26) // Mage
            {
                SetStr(140, 160);
                SetDex(130, 150);
                SetInt(170, 190);
                SetDamage(4, 14);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 30, 40);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 20, 30);
                SetResistance(ResistanceType.Poison, 30, 40);
                SetResistance(ResistanceType.Energy, 30, 40);

                ChangeAIType(AIType.AI_Mage);

                SetSkill(SkillName.Wrestling, 45.0, 55.0);
                SetSkill(SkillName.Tactics, 50.0, 60.0);
                SetSkill(SkillName.MagicResist, 65.0, 75.0);
                SetSkill(SkillName.Magery, 60.0, 70.0);
                SetSkill(SkillName.EvalInt, 60.0, 75.0);
                SetSkill(SkillName.Meditation, 70.0, 90.0);
                SetSkill(SkillName.Focus, 80.0, 100.0);
            }
            else
            {
                SetStr(150, 200);
                SetDex(90, 110);
                SetInt(70, 100);
                SetDamage(5, 9);

                SetDamageType(ResistanceType.Physical, 75);
                SetDamageType(ResistanceType.Cold, 25);

                SetResistance(ResistanceType.Physical, 20, 30);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 15, 25);
                SetResistance(ResistanceType.Poison, 15, 25);
                SetResistance(ResistanceType.Energy, 20, 30);

                SetSkill(SkillName.Archery, 60.0, 90.0);
                SetSkill(SkillName.Tactics, 70.0, 85.0);
                SetSkill(SkillName.MagicResist, 70.0, 85.0);

                int hue = 2448;
                SetWearable(new Shirt(), hue);
                SetWearable(new LongPants(), hue);
                SetWearable(new Bandana(), hue);
                SetWearable(new Boots(), hue);
                SetWearable(new Crossbow(), hue);

                AddItem(new Bolt(10));
            }

            SetSkill(SkillName.DetectHidden, 40.0, 45.0);

            Fame = 8000;
            Karma = -8000;

            if (IsSoulboundEnemies)
                IsSoulbound = true;
        }


        public override int TreasureMapLevel { get { return 3; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosUltraRich, 2);
        }

        public override void DisplayPaperdollTo(Mobile to)
        {
            // Do nothing
        }

        public GhostCrew(Serial serial)
            : base(serial)
        {
        }

        public override bool OnBeforeDeath()
        {
            Body = 15;

            return base.OnBeforeDeath();
        }


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
