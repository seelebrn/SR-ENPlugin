using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;
using Sirenix.Serialization;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using Il2CppSystem;
using BepInEx.IL2CPP.Utils.Collections;
using System.Collections.Generic;
using System.Drawing;
using HappyTall;
using static Il2CppSystem.Globalization.TimeSpanFormat;
using System.Security.AccessControl;
using System;
using Il2CppSystem.Reflection;

namespace TranslationENMOD
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource mod = new ManualLogSource("EN"); // The source name is shown in BepInEx log
        public static System.Collections.Generic.List<string> contents = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> processed = new System.Collections.Generic.List<string>();
        public static string[] forbidden = new string[]
        {
        "animations",
        "audios",
        "begin",
        "bgothers",
        "fight",
        "flsl",
        "font1",
        "font2",
        "fonts"
        };
        public static Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.FieldInfo> fields = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.FieldInfo>();


        public override void Load()
        {

            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(BepInEx.Paths.PluginPath, "Dump"));

            /*foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }*/
            AddComponent<mbmb>();
            BepInEx.Logging.Logger.Sources.Add(mod);


            Log.LogInfo("Hello");
            var harmony = new Harmony("Cadenza.GAME.ENMOD");
            Harmony.CreateAndPatchAll(System.Reflection.Assembly.GetExecutingAssembly(), null);


        }

        internal static class MyPluginInfo
        {
            public const string PLUGIN_GUID = "Example.Plugin";
            public const string PLUGIN_NAME = "My first plugin";
            public const string PLUGIN_VERSION = "1.0.0";
        }
    }
    class mbmb : MonoBehaviour
    {
        public mbmb(System.IntPtr handle) : base(handle) { }
        private void Update()
        {

        }
    }


    /* [HarmonyPatch(typeof(ResourceControl), "GetAssetNowProgress")]
     static class Patch00
     {
         static void Postfix(ResourceControl __instance)
         {


             foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, AssetBundle> ab in __instance.ABS)
             {
                 var bundle = ab.Value;
                 var objects = bundle.LoadAllAssets<UnityEngine.Object>();
                 Plugin.mod.LogInfo("Bundle name = " + bundle.name);
                 if (!Plugin.processed.Contains(bundle.name))
                 {
                     if (!Plugin.forbidden.Contains(bundle.name))
                     {
                         foreach (var obj in objects)
                         {
                             System.Collections.Generic.List<string> strings = new System.Collections.Generic.List<string>();
                             System.Collections.Generic.List<string> dialogs = new System.Collections.Generic.List<string>();

                             //Debug.Log("Object Type  = " + obj.GetIl2CppType().Name);
                             if (UnhollowerRuntimeLib.Il2CppType.Of<ScriptableObject>().IsAssignableFrom(obj.GetIl2CppType()))
                             {
                                 if (obj.GetIl2CppType().Name != "DialogUnitFSM")
                                 {
                                     try
                                     { 
                                     //Plugin.mod.LogInfo("        ScriptableObject = " + obj.GetIl2CppType().Name);
                                     string result = UnityEngine.JsonUtility.ToJson(obj);

                                     //byte[] serializedData = SerializationUtility.SerializeValue(obj.GetIl2CppType(), DataFormat.JSON);
                                     //string result = System.Text.Encoding.UTF8.GetString(serializedData);
                                     //if (obj.GetIl2CppType().Name == "AwardGroup") { Plugin.mod.LogInfo("SerializedData = " + result); }
                                     var pattern = "\"([^\"]*)\"";
                                     MatchCollection matchCollection = Regex.Matches(result, pattern);
                                     foreach (var match in matchCollection)
                                     {
                                         if (match != null && Helpers.IsChinese(match.ToString()) && !strings.Contains(match.ToString()))
                                         {
                                             result = Regex.Replace(result, match.ToString(), "\"" + "000" + "\"");
                                             strings.Add(match.ToString().Replace("\"", ""));

                                         }

                                     }
                                      }
                                     catch
                                     {

                                     }
                                 }
                                 if (obj.GetIl2CppType().Name == "DialogUnitFSM")
                                 {
                                     DialogUnitFSM du = obj.Cast<DialogUnitFSM>();

                                         var pattern = "\"([^\"]*)\"";

                                     MatchCollection matchCollection = Regex.Matches(Regex.Unescape(du._serializedGraph.ToString()), pattern);
                                     //Plugin.mod.LogInfo("duserializedgraph = " + Regex.Unescape(du._serializedGraph.ToString()));
                                         foreach (var match in matchCollection)
                                         {
                                             if (match != null && Helpers.IsChinese(match.ToString()) && !strings.Contains(match.ToString()))
                                             {
                                                 dialogs.Add(match.ToString().Replace("\"", ""));
                                                 du._serializedGraph = du._serializedGraph.Replace(match.ToString(), "aaa");
                                             }

                                         }

                                 }
                             }*/

    /*if (strings.Count > 0)
    {
        var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", obj.GetIl2CppType().Name + ".txt");
        if (File.Exists(path))
        {
            Plugin.contents = File.ReadAllLines(path).ToList();
        }
        else
        {
            Plugin.contents = new System.Collections.Generic.List<string>();
        }

        using (StreamWriter sw = new StreamWriter(path, append: true))
        {

            foreach (string str in strings.ToArray().Distinct())
            {
                if (!Plugin.contents.Contains(str))
                {
                    sw.Write(str + System.Environment.NewLine);
                }
            }
        }
        if (dialogs.Count > 0 && bundle.name == "dataasset_dialog")
    {
        var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", obj.GetIl2CppType().Name + ".txt");
        if (File.Exists(path))
        {
            Plugin.contents = File.ReadAllLines(path).ToList();
        }
        else
        {
            Plugin.contents = new System.Collections.Generic.List<string>();
        }

        using (StreamWriter sw = new StreamWriter(path, append: true))
        {

            foreach (string str in dialogs.ToArray().Distinct())
            {
                if (!Plugin.contents.Contains(str))
                {
                    sw.Write(str + System.Environment.NewLine);
                }
            }
        }
    }


}

}
Plugin.processed.Add(bundle.name);
}
}
}

}*/
    [HarmonyPatch]
    static class Patch_CostItemData
    {
        static System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            return AccessTools.GetDeclaredMethods(typeof(CostItemData)).Distinct();
        }

        static void Postfix()
        {
            foreach (var mb in TargetMethods())
            {
                foreach (System.Reflection.FieldInfo fi in TargetMethods().First().DeclaringType.GetFields())

                    if (fi.GetUnderlyingType().Name == "String")
                    {
                        var obj = System.Activator.CreateInstance(fi.DeclaringType);
                        Plugin.mod.LogInfo("Name = " + fi.Name + " + Value = " + fi.GetValue(obj));
                    }
            }
        }
    }

    public static class Helpers
    {
        public static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static bool IsChinese(string s)
        {
            return cjkCharRegex.IsMatch(s);
        }
    }
}
