using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ApparelScoreViewer;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

namespace MeleeSkillPanel
{
    [StaticConstructorOnStartup]
    public static class PawnRightClickMenuPatcher
    {
        static PawnRightClickMenuPatcher()
        {
            Harmony harmony = new Harmony(ModConstant.ModId);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Pawn), "GetFloatMenuOptions")]
        public static class FloatMenuMakerMap_ChoicesAtFor_Patch
        {
            public static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> values, Pawn selPawn)
            {
                // 先返回所有原始选项
                foreach (var option in values)
                {
                    yield return option;
                }

                // 添加自定义选项
                if (selPawn != null && selPawn.IsColonist)
                {
                    yield return new FloatMenuOption( I18Constant.MenuTitle.Translate(),
                        () =>
                        {
                            var listMeleeVerbs = Main.ListMeleeVerbs(selPawn);
                            Find.WindowStack.Add(
                                new MeleeVerbsWindow(listMeleeVerbs.Item1, listMeleeVerbs.Item2));
                        });
                }
            }
        }
    }

}