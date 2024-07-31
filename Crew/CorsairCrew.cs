using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a pirate corpse")]
    public class CorsairCrew : BaseCreature
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
        public CorsairCrew()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 2, .2, .4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            Name = "a corsair crewman";

            SetSkill(SkillName.Tactics, 55.0, 70.0);
            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Anatomy, 60.0, 85.0);
            SetSkill(SkillName.Healing, 60.0, 80.0);
            SetSkill(SkillName.Archery, 100.0, 120.0);
            SetSkill(SkillName.DetectHidden, 40.0, 45.0);

            int hue = GetRandomHue();

            SetWearable(new Boots(), 2413);
            SetWearable(new FancyShirt());
            SetWearable(new LongPants(), 1109);
            SetWearable(new RingmailArms(), 2213);
            SetWearable(new SkullCap(), hue);
            SetWearable(new BodySash(), hue);
            SetWearable(new Cutlass());
            SetWearable(new Buckler());

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

            Fame = 8000;
            Karma = -8000;

            if (IsSoulboundEnemies)
                IsSoulbound = true;
        }

        public int GetRandomHue()
        {
            switch (Utility.Random(5))
            {
                default:
                case 0:
                    return Utility.RandomBlueHue();
                case 1:
                    return Utility.RandomGreenHue();
                case 2:
                    return Utility.RandomRedHue();
                case 3:
                    return Utility.RandomYellowHue();
                case 4:
                    return Utility.RandomNeutralHue();
            }
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

        public CorsairCrew(Serial serial)
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
