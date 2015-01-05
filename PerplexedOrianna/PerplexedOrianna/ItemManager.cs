using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;  
namespace PerplexedOrianna
{
    public class ItemManager
    {
        private static Obj_AI_Hero Player = ObjectManager.Player;
        public static List<Item> Items;
        public static void Initialize()
        {
            Items = new List<Item>();
            //Offensive
            Items.Add(new Item("DFG", "Deathfire Grasp", 3128, ItemType.Offensive, 750));
            //Defensive
            Items.Add(new Item("Seraphs", "Seraph's Embrace", 3040, ItemType.Defensive));
            Items.Add(new Item("Zhonyas", "Zhonya's Hourglass", 3157, ItemType.Defensive));

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            double incomingDmg = 0;
            if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy && args.Target.Type == GameObjectType.obj_AI_Hero && (args.Target.IsMe || args.Target.IsAlly))
            {
                Obj_AI_Hero attacker = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == sender.NetworkId);
                Obj_AI_Hero attacked = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == args.Target.NetworkId);

                SpellDataInst spellData = attacker.Spellbook.Spells.FirstOrDefault(hero => args.SData.Name.Contains(hero.SData.Name));
                SpellSlot spellSlot = spellData == null ? SpellSlot.Unknown : spellData.Slot;

                if (spellSlot == SpellSlot.Q || spellSlot == SpellSlot.W || spellSlot == SpellSlot.E || spellSlot == SpellSlot.R)
                    incomingDmg = Damage.GetSpellDamage(attacker, attacked, spellSlot);
            }
            else if (sender.Type == GameObjectType.obj_AI_Turret && sender.IsEnemy && args.Target.Type == GameObjectType.obj_AI_Hero && (args.Target.IsMe || args.Target.IsAlly))
                incomingDmg = sender.BaseAttackDamage;
            if (incomingDmg > 0)
            {
                try
                {
                    Obj_AI_Hero attacked = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == args.Target.NetworkId);
                    if (attacked.IsMe)
                    {
                        UseDefensiveItemsIfInDanger(incomingDmg);
                        SpellManager.UseHealIfInDanger(incomingDmg);
                        SpellManager.ShieldAlly(Player);
                    }
                    else if(attacked.IsAlly)
                        SpellManager.ShieldAlly(attacked);
                }
                catch (Exception ex)
                {
                    Game.PrintChat("Exception: " + ex.Message);
                }
            }
        }

        public static void UseDefensiveItemsIfInDanger(double incomingDmg)
        {
            foreach (var item in ItemManager.Items)
            {
                if (item.Type == ItemType.Defensive)
                    item.UseIfInDanger(incomingDmg);
            }
        }

        public static void UseOffensiveItems()
        {
            foreach (var item in ItemManager.Items)
            {
                if (item.Type == ItemType.Offensive)
                {
                    var target = TargetSelector.GetTarget(item.Range, TargetSelector.DamageType.Magical);
                    item.Use(target);
                }
            }
        }
    }

    public class Item
    {
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
        public ItemType Type { get; set; }
        public float Range { get; set; }
        public bool ShouldUse { get { return Config.ShouldUseItem(this.ShortName); } }
        public int UseOnPercent { get { return Config.UseOnPercent(this.ShortName); } }
        public Item(string shortName, string name, int id, ItemType type, float range = 0)
        {
            this.ShortName = shortName;
            this.Name = name;
            this.ID = id;
            this.Type = type;
            this.Range = range;
        }

        public void Use(Obj_AI_Hero target = null)
        {
            if (this.ShouldUse)
            {
                if (Items.CanUseItem(this.ID))
                    Items.UseItem(this.ID, target);
            }
        }

        public void UseIfInDanger(double incomingDmg)
        {
            Obj_AI_Hero Player = ObjectManager.Player;
            if (this.ShouldUse)
            {
                int healthToUse = (int)(Player.MaxHealth / 100) * this.UseOnPercent;
                if ((Player.Health - incomingDmg) <= healthToUse)
                    Use(Player);
            }
        }
    }

    public enum ItemType
    {
        Offensive,
        Defensive
    }
}
