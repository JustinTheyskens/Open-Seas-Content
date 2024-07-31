using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a pirate corpse")]
    public class PrivateerCrew : BaseCreature
    {
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ClickTitle {  get { return false; } }

        public override WeaponAbility GetWeaponAbility()
        {
            Item weapon = FindItemOnLayer(Layer.TwoHanded);

            if (weapon == null)
                return null;

            if (weapon is BaseWeapon)
            {
                if (Utility.RandomBool())
                    return ((BaseWeapon)weapon).PrimaryAbility;
                else
                    return ((BaseWeapon)weapon).SecondaryAbility;
            }
            return null;
        }

        [Constructable]
        public PrivateerCrew()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 2, .2, .4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            Name = "a privateer crewman";

            SetStr(120, 170);
            SetDex(90, 110);
            SetInt(70, 100);

            SetDamage(8, 11);
            SetHits(120, 160);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.Tactics, 50.5, 65.5);
            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Anatomy, 55.0, 75.0);
            SetSkill(SkillName.Healing, 50.0, 70.0);
            SetSkill(SkillName.Archery, 90.0, 110.0);
            SetSkill(SkillName.DetectHidden, 40.0, 45.0);

            int hue = Utility.RandomNeutralHue();

            SetWearable(new Boots());
            SetWearable(new Shirt());
            SetWearable(new LongPants(), hue);
            SetWearable(new SkullCap(), hue);
            SetWearable(new BodySash(), hue);
            SetWearable(new Cutlass());

            switch (Utility.Random(4))
            {
                default:
                case 0: SetWearable(new Crossbow());
                    PackItem(new Bolt(Utility.RandomMinMax(5, 15))); break;
                case 1: SetWearable(new HeavyCrossbow());
                    PackItem(new Bolt(Utility.RandomMinMax(5, 15))); break;
                case 2: SetWearable(new Bow());
                    PackItem(new Arrow(Utility.RandomMinMax(5, 15))); break;
                case 3: SetWearable(new CompositeBow());
                    PackItem(new Arrow(Utility.RandomMinMax(5, 15))); break;
            }

            Fame = 7000;
            Karma = -7000;

            if (IsSoulboundEnemies)
                IsSoulbound = true;
        }

        public override int TreasureMapLevel { get { return 3; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
        }

        public override void DisplayPaperdollTo(Mobile to)
        {
            // Do nothing
        }

        public PrivateerCrew(Serial serial)
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
