using System;
using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using UnityEngine;
using Verse;

namespace MeleeSkillPanel
{
    public class MeleeVerbsWindow : Window
    {
        private readonly List<Verb> _verbs;
        private readonly List<TableDataGetter<Verb>> _columns;

        public MeleeVerbsWindow(List<Verb> allMeleeVerbs, List<TableDataGetter<Verb>> tableDataGetterList)
        {
            _verbs = allMeleeVerbs;
            _columns = tableDataGetterList;
            doCloseX = true;
            preventCameraMotion = true;
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            resizeable = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            Vector2 screenSize = new Vector2(UI.screenWidth, UI.screenHeight);
            float x = (screenSize.x - InitialSize.x) / 2f;
            float y = (screenSize.y - InitialSize.y) / 2f;
            this.windowRect = new Rect(x, y, InitialSize.x, InitialSize.y);
        }

        public override Vector2 InitialSize =>
            new Vector2(Math.Min(UI.screenWidth * 0.7f, 1300f), UI.screenHeight * 0.5f);

        private Vector2 scrollPosition = Vector2.zero; // 需在类中加这个字段记录滚动状态

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Gap(4f);

            float tableHeight = 25 + (_verbs.Count * 25); // 粗略估算表格的高度（可调整）

            // 显示可滚动区域
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(inRect.width),
                GUILayout.Height(inRect.height - listing.CurHeight));

            // 绘制表头
            GUILayout.BeginHorizontal();
            foreach (var col in _columns)
            {
                GUILayout.Label(col.label, GUILayout.Width(100f));
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(4f);

            // 绘制每行数据
            foreach (var verb in _verbs)
            {
                GUILayout.BeginHorizontal();
                foreach (var col in _columns)
                {
                    GUILayout.Label(col.getter(verb), GUILayout.Width(100f));
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            listing.End();
        }
    }
}