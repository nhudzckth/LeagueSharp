using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace PerplexedEzreal
{
    class Config
    {
        public static Menu Settings = new Menu("Perplexed Ezreal", "menu", true);
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Initialize()
        {
            //Orbwalker
            Settings.AddSubMenu(new Menu("Orbwalker", "orbMenu"));
            Orbwalker = new Orbwalking.Orbwalker(Settings.SubMenu("orbMenu"));
            //Target Selector
            Settings.AddSubMenu(new Menu("Target Selector", "ts"));
            TargetSelector.AddToMenu(Settings.SubMenu("ts"));
            //Combo
            Settings.AddSubMenu(new Menu("Combo", "menuCombo"));
            Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboQ", "Q").SetValue<bool>(true));
            Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboW", "W").SetValue<bool>(true));
            //Harass
            Settings.AddSubMenu(new Menu("Harass", "menuHarass"));
            Settings.SubMenu("menuHarass").AddItem(new MenuItem( "harassQ", "Q").SetValue<bool>(true));
            Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassW", "W").SetValue<bool>(true));
            //Auto
            Settings.AddSubMenu(new Menu("Auto", "menuAuto"));
            Settings.SubMenu("menuAuto").AddSubMenu(new Menu("Settings", "autoSettings"));
            foreach(Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValid && hero.IsEnemy))
                Settings.SubMenu("menuAuto").SubMenu("autoSettings").AddItem(new MenuItem("auto" + hero.ChampionName, hero.ChampionName).SetValue<bool>(true));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoQ", "Q").SetValue<bool>(true));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoW", "W").SetValue<bool>(false));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("manaR", "Save Mana For R").SetValue<bool>(true));
            //Kill Steal
            Settings.AddSubMenu(new Menu("Kill Steal", "menuKS"));
            Settings.SubMenu("menuKS").AddItem(new MenuItem("ks", "Kill Steal With R").SetValue<bool>(true));
            Settings.SubMenu("menuKS").AddItem(new MenuItem("ksRange", "KS Range").SetValue<Slider>(new Slider(1000, 1000, 5000)));
            //Drawing
            Settings.AddSubMenu(new Menu("Drawing", "menuDrawing"));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawW", "Draw W Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawR", "Draw R Range").SetValue(new Circle(true, Color.Yellow)));
            //Other
            Settings.AddItem(new MenuItem("recallBlock", "Recall Block").SetValue<bool>(true));
            Settings.AddItem(new MenuItem("usePackets", "Use Packets").SetValue<bool>(true));
            //Finish
            Settings.AddToMainMenu();
        }

        public static bool ComboQ { get { return Settings.Item("comboQ").GetValue<bool>(); } }
        public static bool ComboW { get { return Settings.Item("comboW").GetValue<bool>(); } }

        public static bool HarassQ { get { return Settings.Item("harassQ").GetValue<bool>(); } }
        public static bool HarassW { get { return Settings.Item("harassW").GetValue<bool>(); } }

        public static bool KillSteal { get { return Settings.Item("ks").GetValue<bool>(); } }
        public static int KillSteal_Range { get { return Settings.Item("ksRange").GetValue<Slider>().Value; } }

        public static bool ShouldAuto(string championName)
        {
            return Settings.Item("auto" + championName).GetValue<bool>();
        }
        public static bool AutoQ { get { return Settings.Item("autoQ").GetValue<bool>(); } }
        public static bool AutoW { get { return Settings.Item("autoW").GetValue<bool>(); } }
        public static bool ManaR { get { return Settings.Item("manaR").GetValue<bool>(); } }

        public static bool DrawQ { get { return Settings.Item("drawQ").GetValue<Circle>().Active; } }
        public static bool DrawW { get { return Settings.Item("drawW").GetValue<Circle>().Active; } }
        public static bool DrawR { get { return Settings.Item("drawR").GetValue<Circle>().Active; } }

        public static bool RecallBlock { get { return Settings.Item("recallBlock").GetValue<bool>(); } }
        public static bool UsePackets { get { return Settings.Item("usePackets").GetValue<bool>(); } }
    }
}
