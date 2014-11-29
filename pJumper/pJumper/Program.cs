using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace pJumper
{
    /*Before anyone bashes me, I'd like you to know that I'm knew to the L# API, so a lot of the stuff I'm writing has been inspired by other assemblies.
     * I'm an experienced C# programmer, I'm just trying to get familiar with the L# API and how it works.
     * So if you feel that a piece of code looks familiar to you, then I apologize, I'm only trying :D*/
    class Program
    {
        static List<string> validChamps = new List<string>(new string[] { "LeeSin", "Katarina", "Jax" });
        static Spell jumpSpell;
        static Obj_AI_Hero Player;
        static Menu Menu;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (!validChamps.Contains(Player.ChampionName))
                return;
            ChooseSpell();
            LoadMenu();
        }

        static void ChooseSpell()
        {
            //Choose spell to ward jump based on champion chosen.
            switch (Player.ChampionName)
            {
                case "LeeSin":
                    jumpSpell = new Spell(SpellSlot.W, 700);
                    break;
                case "Katarina":
                    jumpSpell = new Spell(SpellSlot.E, 700);
                    break;
                case "Jax":
                    jumpSpell = new Spell(SpellSlot.Q, 700);
                    break;
            }
        }

        static void LoadMenu()
        {
            Menu = new Menu("pJumper", "pJumper", true);
            Menu.AddItem(new MenuItem("wJump", "Ward Jump").SetValue(new KeyBind(71, KeyBindType.Press)));
            Menu.AddToMainMenu();
            Game.OnGameUpdate += Game_OnGameUpdate;

            Game.PrintChat("pJumper Loaded!");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Menu.Item("wJump").GetValue<KeyBind>().Active)
            {
                Vector3 cursorPos = Game.CursorPos;
                Player.IssueOrder(GameObjectOrder.MoveTo, cursorPos);
                Vector3 myPos = Player.ServerPosition;
                if (CanJump())
                {
                    //Credits to andreluis034 for this.
                    var jumpTo = ObjectManager.Get<Obj_AI_Base>().OrderBy(obj => obj.Distance(myPos)).FirstOrDefault(obj => obj.IsAlly && !obj.IsMe && (!(obj.Name.IndexOf("turret", StringComparison.InvariantCultureIgnoreCase) >= 0) && Vector3.DistanceSquared(cursorPos, obj.ServerPosition) <= 150 * 150));
                    //We found a minion/ward/ally to jump to.
                    if (jumpTo != null)
                    {
                        jumpSpell.CastOnUnit(jumpTo);
                        return;
                    }
                    //No minion, ward or ally, so we'll place a ward if we can.
                    if (CanWard())
                    {
                        var wardSlot = Items.GetWardSlot();
                        wardSlot.UseItem(cursorPos);
                    }
                }
            }

        }

        static bool CanJump()
        {
            if (Player.ChampionName == "LeeSin")
                return jumpSpell.IsReady() && Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne";
            else
                return jumpSpell.IsReady();
        }

        static bool CanWard()
        {
            var wardSlot = Items.GetWardSlot();
            if (wardSlot == null || wardSlot.Stacks == 0)
                return false;
            return true;
        }
    }
}
