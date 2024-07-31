using Server;
using System;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class VikingShipCaptain : ShipEncounterCaptain
    {
        private DateTime m_NextTalk;
        private int m_PirateName;
        private int m_Adjective;
        private int m_Noun;

        public int PirateName { get { return m_PirateName; } }
        public int Adjective { get { return m_Adjective; } }
        public int Noun { get { return m_Noun; } }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool Commandable { get { return false; } }

        #region Bounty Quest
        private ProfessionalBountyQuest m_Quest;
        private bool m_IsCaught;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsCaught { get { return m_IsCaught; } set { m_IsCaught = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ProfessionalBountyQuest Quest { get { return m_Quest; } set { m_Quest = value; } }
        #endregion

        [Constructable]
        public VikingShipCaptain()
            : this(null)
        {
        }

        public VikingShipCaptain(BaseShip galleon)
            : base(galleon, AIType.AI_Melee, FightMode.Weakest, 25, 1, .2, .4)
        {
            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                Utility.AssignRandomHair(this);
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                Utility.AssignRandomFacialHair(this);

                if (Utility.RandomBool())
                    AddItem(new LongHair(Utility.RandomHairHue()));
            }

            SpeechHue = Utility.RandomDyedHue();
            Title = "the Viking captain";
            Hue = Race.RandomSkinHue();

            SetStr(500, 750);
            SetDex(125, 175);
            SetInt(61, 75);

            SetHits(5000, 6000);

            SetDamage(18, 30);

            SetSkill(SkillName.Fencing, 85.9, 105.5);
            SetSkill(SkillName.Macing, 85.9, 105.5);
            SetSkill(SkillName.MagicResist, 85.9, 105.5);
            SetSkill(SkillName.Swords, 85.9, 105.5);
            SetSkill(SkillName.Tactics, 85.9, 105.5);
            SetSkill(SkillName.Wrestling, 85.9, 105.5);
            SetSkill(SkillName.Anatomy, 85.9, 105.5);


            int hue = Utility.RandomList(1816, 1817, 1818, 1826, 1827, 1828, 1835, 1836, 1844, 1845);
            SetWearable(new LeatherArms(), hue);
            SetWearable(new LeatherLegs(), hue);
            SetWearable(new LeatherGloves(), hue);
            SetWearable(new LeatherGorget(), hue);
            SetWearable(new Boots());
            SetWearable(new NorseHelm());

            if (Female)
                SetWearable(new FemalePlateChest());
            else
                SetWearable(new LeatherChest(), hue);

            Fame = 25000;
            Karma = -25000;

            switch(Utility.Random(4))
            {
                case 0:
                    SetWearable(new Axe());
                    break;
                case 1:
                    SetWearable(new DoubleAxe());
                    break;
                case 2:
                    SetWearable(new BattleAxe());
                    break;
                case 3:
                    SetWearable(new VikingSword());
                    break;
            }

            if (IsSoulboundEnemies)
                IsSoulbound = true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 3);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (!IsCaught || m_NextTalk > DateTime.UtcNow)
                return;

            IPooledEnumerable eable = this.GetMobilesInRange(7);
            foreach (Mobile mob in eable)
            {
                if (mob is PlayerMobile)
                {
                    OnTalk();
                    break;
                }
            }
            eable.Free();
        }

        public void OnTalk()
        {
            Say(Utility.RandomMinMax(1149701, 1149720));
            m_NextTalk = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        }

        #region Quest Stuff
        public bool TryBound(Mobile from, BaseQuest quest)
        {
            if (from == null || Ship == null || !Ship.Contains(this) || quest == null)
                return false;

            if (m_IsCaught)
            {
                from.SendMessage("That viking is already bound to a ship!");
                return false;
            }

            Combatant = null;
            Warmode = false;
            m_IsCaught = true;
            return true;
        }

        public void OnBound(ProfessionalBountyQuest quest)
        {
            if (quest == null || quest.Pole == null)
                return;

            BindingPole pole = quest.Pole;

            int x = pole.X;
            int y = pole.Y;

            while (x == pole.X && y == pole.Y)
            {
                x = Utility.RandomMinMax(pole.X - 1, pole.X + 1);
                y = Utility.RandomMinMax(pole.Y - 1, pole.Y + 1);
            }

            Frozen = true;

            Item toDisarm = FindItemOnLayer(Layer.OneHanded);
            if (toDisarm == null || !toDisarm.Movable)
                toDisarm = FindItemOnLayer(Layer.TwoHanded);

            if (toDisarm != null)
            {
                if (Backpack != null)
                    Backpack.DropItem(toDisarm);
                else
                    toDisarm.Delete();
            }

            m_Quest = quest;

            if (quest != null && quest.Galleon != null)
                quest.Galleon.CapturedCaptain = this;

            Timer.DelayCall(TimeSpan.FromSeconds(2.5), new TimerStateCallback(MoveCaptainToShip), new object[] { x, y, pole });
        }

        private void MoveCaptainToShip(object obj)
        {
            object[] objs = (object[])obj;
            int x = (int)objs[0];
            int y = (int)objs[1];
            Item pole = objs[2] as Item;

            if (pole != null)
                MoveToWorld(new Point3D(x, y, pole.Z), pole.Map);

            Blessed = true;
            Title = "[Captured Captain]";
        }

        public override bool OnBeforeDeath()
        {
            List<PlayerMobile> hasQuest = new List<PlayerMobile>();
            List<DamageStore> rights = GetLootingRights();
            for (int i = 0; i < rights.Count; i++)
            {
                if (!rights[i].m_HasRight)
                    continue;

                Mobile mob = rights[i].m_Mobile;

                //if they have the quest and looting rights, give them a certificate
                if (mob is PlayerMobile && mob.NetState != null && QuestHelper.GetQuest((PlayerMobile)mob, typeof(ProfessionalBountyQuest)) != null)
                    hasQuest.Add((PlayerMobile)mob);
            }

            if (hasQuest.Count > 0)
            {
                PlayerMobile questee = hasQuest[Utility.Random(hasQuest.Count)];
                BaseQuest q = QuestHelper.GetQuest(questee, typeof(ProfessionalBountyQuest));

                if (q != null && q is ProfessionalBountyQuest)
                {
                    ((ProfessionalBountyQuest)q).OnPirateDeath(this);
                    questee.AddToBackpack(new DeathCertificate(this));
                }
            }

            if (m_IsCaught && m_Quest != null)
            {
                for (int i = 0; i < m_Quest.Objectives.Count; i++)
                {
                    if (m_Quest.Objectives[i] is BountyQuestObjective && ((BountyQuestObjective)m_Quest.Objectives[i]).CapturedCaptain == this)
                    {
                        ((BountyQuestObjective)m_Quest.Objectives[i]).CapturedCaptain = null;
                        ((BountyQuestObjective)m_Quest.Objectives[i]).Captured = false;
                        m_Quest = null;
                    }
                }
            }

            return base.OnBeforeDeath();
        }
        #endregion

        public VikingShipCaptain(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(m_IsCaught);
            writer.Write(m_Adjective);
            writer.Write(m_Noun);
            writer.Write(m_PirateName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsCaught = reader.ReadBool();
            m_Adjective = reader.ReadInt();
            m_Noun = reader.ReadInt();
            m_PirateName = reader.ReadInt();

            if (IsCaught)
                Frozen = true;

            m_NextTalk = DateTime.UtcNow;
        }
    }
}
