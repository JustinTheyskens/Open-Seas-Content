using Server.Items;
using Server.Misc;
using System;

namespace Server.Mobiles
{
    [CorpseName("a pirate corpse")]
    public class VikingCrew : BaseCreature
    {
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ClickTitle {  get { return false; } }

        public bool IsMage;

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
        public VikingCrew()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 2, .2, .4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            Name = "a viking crewman";

            if (Utility.RandomDouble() > 0.66)
                IsMage = true;
            else
                IsMage = false;

            Utility.AssignRandomFacialHair(this);
            SetStr(220, 270);

            if (IsMage)
            {
                ChangeAIType(AIType.AI_Mage);

                SetDex(70, 100);
                SetInt(130, 150);

                SetDamage(8, 11);
                SetHits(220, 260);
                //SpellHue = 1153;

                SetSkill(SkillName.Tactics, 50.5, 65.5);
                SetSkill(SkillName.MagicResist, 50.0, 70.0);
                SetSkill(SkillName.Magery, 65.0, 85.0);
                SetSkill(SkillName.EvalInt, 60.0, 80.0);
                SetSkill(SkillName.Wrestling, 90.0, 110.0);
                SetSkill(SkillName.Alchemy, 40.0, 45.0);

                if (Utility.RandomBool())
                    SetWearable(new NorseHelm());
                else
                    SetWearable(new DeerMask());
            }
            else
            {
                SetDex(120, 130);
                SetInt(70, 100);

                SetDamage(12, 21);
                SetHits(320, 360);

                SetSkill(SkillName.Tactics, 50.5, 65.5);
                SetSkill(SkillName.MagicResist, 50.0, 70.0);
                SetSkill(SkillName.Anatomy, 55.0, 75.0);
                SetSkill(SkillName.Healing, 50.0, 70.0);
                SetSkill(SkillName.Archery, 90.0, 110.0);
                SetSkill(SkillName.Swords, 90.0, 110.0);
                SetSkill(SkillName.DetectHidden, 40.0, 45.0);

                AddItem(new Crossbow());

                if (Utility.RandomBool())
                    SetWearable(new NorseHelm());
                else
                    SetWearable(new BearMask());
            }

            SetDamageType(ResistanceType.Physical, 100);
            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            int hue = Utility.RandomList(1816, 1817, 1818, 1826, 1827, 1828, 1835, 1836, 1844, 1845);
            SetWearable(new LeatherChest(), hue);
            SetWearable(new LeatherArms(), hue);
            SetWearable(new LeatherLegs(), hue);
            SetWearable(new LeatherGloves(), hue);
            SetWearable(new LeatherGorget(), hue);
            SetWearable(new Boots());
            

            Fame = 7500;
            Karma = -7050;

            if (Backpack == null)
            {
                Backpack pack = new Backpack();
                pack.Movable = false;
                AddItem(pack);
            }

            Backpack.DropItem(new VikingSword());

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

        private DateTime SwapTime;
        public void SwapWeapon()
        {
            if (Backpack == null)
                return;

            SwapTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);

            if (Combatant is Mobile && InRange(Combatant.Location, 3) && InLOS(Combatant) && FindItemOnLayer(Layer.TwoHanded) is Crossbow && SwapTime > DateTime.UtcNow)
            {
                Item crossbow = FindItemOnLayer(Layer.TwoHanded);
                Backpack.DropItem(crossbow);

                Item sword = Backpack.FindItemByType(typeof(VikingSword));

                if (sword != null)
                {
                    ChangeAIType(AIType.AI_Melee);
                    AddItem(sword);
                }
                    

            }
            else if (Combatant is Mobile && !InRange(Combatant.Location, 3) && InRange(Combatant.Location, 10) && InLOS(Combatant) && FindItemOnLayer(Layer.OneHanded) is VikingSword && SwapTime > DateTime.UtcNow)
            {
                Item sword = FindItemOnLayer(Layer.OneHanded);
                Backpack.DropItem(sword);

                Item crossbow = Backpack.FindItemByType(typeof(Crossbow));

                if (crossbow != null)
                {
                    ChangeAIType(AIType.AI_Archer);
                    AddItem(crossbow);
                }
                    


            }

        }

        public override void OnThink()
        {
            base.OnThink();

            SwapWeapon();
        }

        public VikingCrew(Serial serial)
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
