using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ApparelScoreViewer;
using LudeonTK;
using UnityEngine;
using Verse;

namespace MeleeSkillPanel
{

    public static class Main
    {
        public static Tuple<List<Verb>, List<TableDataGetter<Verb>>> ListMeleeVerbs(Pawn p)
        {
            List<Verb> allMeleeVerbs = p.meleeVerbs.GetUpdatedAvailableVerbsList(false)
                .Select(x => x.verb).ToList();
            float highestWeight = 0.0f;
            foreach (Verb v in allMeleeVerbs)
            {
                float num = VerbUtility.InitialVerbWeight(v, p);
                if ((double)num > highestWeight)
                    highestWeight = num;
            }

            float totalSelectionWeight = 0.0f;
            foreach (Verb verb in allMeleeVerbs)
                totalSelectionWeight += VerbUtility.FinalSelectionWeight(verb, p, allMeleeVerbs, highestWeight);
            allMeleeVerbs.SortBy<Verb, float>(x => -VerbUtility.InitialVerbWeight(x, p));
            List<TableDataGetter<Verb>> tableDataGetterList = new List<TableDataGetter<Verb>>()
            {
                new TableDataGetter<Verb>( I18Constant.Verb.Translate(),
                    v => v.ToString().Split(new[] { '/' }, StringSplitOptions.None)[1].TrimEnd(')')),
                new TableDataGetter<Verb>(I18Constant.Source.Translate(), v =>
                {
                    if (v.HediffSource != null)
                        return v.HediffSource.Label;
                    return v.tool != null ? v.tool.label : "";
                }),
                new TableDataGetter<Verb>(I18Constant.Damage.Translate(),
                    v => v.verbProps.AdjustedMeleeDamageAmount(v, p)),
                new TableDataGetter<Verb>(I18Constant.Cooldown.Translate(), 
                    v => v.verbProps.AdjustedCooldown(v, p).ToString(CultureInfo.InvariantCulture) + "s"),
                new TableDataGetter<Verb>( I18Constant.DPS.Translate(), v => VerbUtility.DPS(v, p)),
                new TableDataGetter<Verb>(I18Constant.ArmorPenetration.Translate() ,
                    v => v.verbProps.AdjustedArmorPenetration(v, p)),
                new TableDataGetter<Verb>(I18Constant.Hediff.Translate() ,v =>
                {
                    string str = "";
                    if (v.verbProps.meleeDamageDef != null && !v.verbProps.meleeDamageDef.additionalHediffs
                            .NullOrEmpty())
                    {
                        foreach (DamageDefAdditionalHediff additionalHediff in v.verbProps.meleeDamageDef
                                     .additionalHediffs)
                            str = $"{str}{additionalHediff.hediff.label} ";
                    }

                    return str;
                }),
                new TableDataGetter<Verb>(  I18Constant.Weight.Translate(), v => VerbUtility.InitialVerbWeight(v, p)),
                new TableDataGetter<Verb>(I18Constant.Category.Translate(), v =>
                {
                    switch (v.GetSelectionCategory(p, highestWeight))
                    {
                        case VerbSelectionCategory.Best:
                            return I18Constant.Category_Best.Translate().Colorize(Color.green);
                        case VerbSelectionCategory.Worst:
                            return I18Constant.Category_Worst.Translate().Colorize(Color.grey);
                        default:
                            return I18Constant.Category_Mid.Translate();
                    }
                }),
                new TableDataGetter<Verb>(I18Constant.Sel.Translate() + " %",
                    v => GetSelectionPercent(v).ToStringPercent("F2"))
            };

            float GetSelectionPercent(Verb v)
            {
                return VerbUtility.FinalSelectionWeight(v, p, allMeleeVerbs, highestWeight) / totalSelectionWeight;
            }

            return Tuple.Create(allMeleeVerbs, tableDataGetterList);
        }
    }
}