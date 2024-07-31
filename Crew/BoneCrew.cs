using Server;
using System;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a skeletal corpse")]
    public class BoneCrew : BaseCreature
    {
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ClickTitle { get { return false; } }

        [Constructable]
        public BoneCrew() : base(AIType.AI_Archer, FightMode.Closest, 10, 4, 0.2, 0.4)
        {
            Name = "a skeletal crewman";

            Body = Utility.RandomDouble() > 0.75 ? 0x32 : 229;
            //SpellHue = 2100;

            SetHits(2000);

            if (Body == 0x32) // Mage
            {
                Hue = 33;
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
                SetSkill(SkillName.MagicResist, 55.0, 65.0);
                SetSkill(SkillName.Magery, 60.0, 70.0);
                SetSkill(SkillName.EvalInt, 40.0, 55.0);
                SetSkill(SkillName.Meditation, 50.0, 80.0);
            }
            else
            {
                SetStr(150, 200);
                SetDex(90, 110);
                SetInt(70, 100);
                SetDamage(5, 7);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 20, 30);
                SetResistance(ResistanceType.Fire, 20, 30);
                SetResistance(ResistanceType.Cold, 20, 30);
                SetResistance(ResistanceType.Poison, 20, 30);
                SetResistance(ResistanceType.Energy, 20, 30);

                SetSkill(SkillName.Archery, 50.0, 70.0);
                SetSkill(SkillName.Tactics, 70.0, 85.0);
                SetSkill(SkillName.MagicResist, 70.0, 85.0);

                SetWearable(new Bow());

                AddItem(new Arrow(10));
            }

            SetSkill(SkillName.DetectHidden, 40.0, 45.0);

            Fame = 7000;
            Karma = -7000;

            if (IsSoulboundEnemies)
                IsSoulbound = true;
        }


        public override int TreasureMapLevel { get { return 2; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosRich, 2);
        }

        public BoneCrew(Serial serial)
            : base(serial)
        {
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
