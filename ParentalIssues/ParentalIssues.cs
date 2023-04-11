using HarmonyLib;
using NeosModLoader;
using System;
using System.Reflection;
using System.Collections.Generic;
using FrooxEngine;
using BaseX;
using System.Reflection.Emit;

namespace ParentalIssues
{
    public class ParentalIssues : NeosMod
    {
        public override string Name => "ParentalIssues";
        public override string Author => "art0007i";
        public override string Version => "1.1.0";
        public override string Link => "https://github.com/art0007i/ParentalIssues/";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("me.art0007i.ParentalIssues");
            harmony.PatchAll();

        }
        [HarmonyPatch]
        class ParentalIssuesPatch
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                var func = AccessTools.Method(typeof(SceneInspector), "OnInsertParentPressed");
                yield return func; 
                func = AccessTools.Method(typeof(SceneInspector), "OnAddChildPressed");
                yield return func;
                func = AccessTools.Method(typeof(SlotPositioning), nameof(SlotPositioning.CreatePivotAtCenter),
                    new Type[] {
                        typeof(Slot),
                        typeof(BoundingBox).MakeByRefType(),
                        typeof(bool)
                });
                yield return func;
            }
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
            {
                foreach (var code in codes)
                {
                    if(code.operand is string s && s.StartsWith(" - "))
                    {
                        yield return new CodeInstruction(OpCodes.Call, typeof(ParentalIssues).GetMethod(nameof(ProcessString)));
                        code.operand = s.Remove(0, 2);
                    }
                    yield return code;
                }
            }
        }
        public static string ProcessString(string str)
        {
            if (str.ToLower().EndsWith("s"))
            {
                return str + "'";
            }
            return str + "'s";
        }
    }
}