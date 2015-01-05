using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace PerplexedOrianna
{
    public static class Extensions
    {
        public static bool HasBall(this Obj_AI_Hero hero)
        {
            return Utility.HasBuff(hero, "OrianaGhostSelf") || Utility.HasBuff(hero, "orianaghost");
        }

        public static bool ComboKillable(this Obj_AI_Hero target)
        {
            return DamageCalc.GetComboDamage(target) >= target.Health;
        }
    }
}
