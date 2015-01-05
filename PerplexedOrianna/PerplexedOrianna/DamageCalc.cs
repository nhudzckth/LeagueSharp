using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace PerplexedOrianna
{
    class DamageCalc
    {
        private static Obj_AI_Hero Player = ObjectManager.Player;

        public static float GetComboDamage(Obj_AI_Hero target)
        {
            double dmg = Player.GetAutoAttackDamage(target);

            if (SpellManager.Q.IsReady())
                dmg += Player.GetSpellDamage(target, SpellSlot.Q);
            if (SpellManager.W.IsReady())
                dmg += Player.GetSpellDamage(target, SpellSlot.W);
            if (SpellManager.E.IsReady())
                dmg += Player.GetSpellDamage(target, SpellSlot.E);
            if (SpellManager.R.IsReady())
                dmg += GetUltDamage(target);

            if (Items.CanUseItem(ItemManager.Items.First(item => item.ShortName == "DFG").ID))
                dmg += dmg * 1.2;

            return (float)dmg;
        }

        public static double GetUltDamage(Obj_AI_Hero target)
        {
            if (SpellManager.R.IsReady())
                return Player.GetSpellDamage(target, SpellSlot.R);
            else
                return 0;
        }

        public static float GetDrawDamage(Obj_AI_Hero target)
        {
            double dmg = Config.DrawAADmg ? Player.GetAutoAttackDamage(target) : 0;

            if (SpellManager.Q.IsReady() && Config.DrawQDmg)
                dmg += Player.GetSpellDamage(target, SpellSlot.Q);
            if (SpellManager.W.IsReady() && Config.DrawWDmg)
                dmg += Player.GetSpellDamage(target, SpellSlot.W);;
            if (SpellManager.R.IsReady() && Config.DrawRDmg)
                dmg += GetUltDamage(target);

            return (float)dmg;
        }
    }
}
