using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Xml.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;
using Sirenix.Serialization;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using BepInEx.IL2CPP.Utils.Collections;
using System.Collections.Generic;
using System.Drawing;
using HappyTall;
using static Il2CppSystem.Globalization.TimeSpanFormat;
using System.Security.AccessControl;
using System;
using Il2CppSystem.Reflection;
using Sirenix.Serialization.Utilities;
using static ServerMail;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Il2CppSystem;
using UnhollowerBaseLib.Runtime.VersionSpecific.AssemblyName;
using ParadoxNotion;
using Il2CppSystem.Linq;
using ParadoxNotion.Serialization.FullSerializer;
using UnhollowerBaseLib;
using Il2CppSystem.Linq.Expressions;
using TranslationENMOD;

namespace UVExtractor.Il2Cpp
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource log = new ManualLogSource("EN"); // The source name is shown in BepInEx log
        public static System.Collections.Generic.List<string> contents = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> processed = new System.Collections.Generic.List<string>();
        public static string UVPTranslatorPath = Path.Combine(BepInEx.Paths.PluginPath, "Dump");
        public static string sourceDir = BepInEx.Paths.PluginPath;
        public static string fileDir = Application.dataPath;
        public static List<string> meshes = new List<string>();
        public static System.Collections.Generic.List<string> texts = new List<string>();
        public static System.Collections.Generic.List<string> textcontent = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> meshescontent = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> items = new System.Collections.Generic.List<string>();
        public static string[] forbidden = new string[]
        {
        "animations",
        "audios",
        "font1",
        "font2",
        "fonts"
        };
        public static Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.FieldInfo> fields = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.FieldInfo>();


        public override void Load()
        {
            BepInEx.Logging.Logger.Sources.Add(log);
            var Config = File.ReadAllLines(Path.Combine(BepInEx.Paths.PluginPath, "Redump.txt"));
            if(Config.Contains("Dump=false"))
            {
                Plugin.log.LogInfo("Switch Dump set to false, not dumping anything");
            }
            if (Config.Contains("Dump=true"))
            {

                Plugin.log.LogInfo("Switch Dump set to true, dupming everything to plugins\\Dump\\");
                CleanFiles();
                string assetPath = $"{Paths.GameRootPath}\\灵墟_Data\\StreamingAssets\\Test";
                foreach (string assetFile in Directory.GetFiles(assetPath))
                {
                    try
                    {
                        if (!assetFile.Contains("manifest") && !assetFile.Equals("pc")) 
                    {
                        var ab = AssetBundle.LoadFromFile(assetFile);
                        var objects = ab.LoadAllAssets<UnityEngine.Object>();
                        Plugin.log.LogInfo("Bundle name = " + ab.name);
                        if (!Plugin.processed.Contains(ab.name))
                        {
                            if (!Plugin.forbidden.Contains(ab.name))
                            {
                                foreach (var obj in objects)
                                {
                                    
                                        System.Collections.Generic.List<string> strings = new System.Collections.Generic.List<string>();
                                        System.Collections.Generic.List<string> dialogs = new System.Collections.Generic.List<string>();

                                        //Plugin.log.LogInfo("Object Type  = " + obj.GetIl2CppType().Name);
                                        //Plugin.log.LogInfo("Object Name  = " + obj.name);
                                        if (UnhollowerRuntimeLib.Il2CppType.Of<ScriptableObject>().IsAssignableFrom(obj.GetIl2CppType()))
                                        {
                                            if (obj.GetIl2CppType().Name != "DialogUnitFSM")
                                            {

                                                //Plugin.mod.LogInfo("        ScriptableObject = " + obj.GetIl2CppType().Name);
                                                string result = UnityEngine.JsonUtility.ToJson(obj);

                                                //byte[] serializedData = SerializationUtility.SerializeValue(obj.GetIl2CppType(), DataFormat.JSON);
                                                //string result = System.Text.Encoding.UTF8.GetString(serializedData);
                                                //if (obj.GetIl2CppType().Name == "AwardGroup") { Plugin.mod.LogInfo("SerializedData = " + result); }
                                                //Plugin.log.LogInfo("Name = " + obj.name + " // Type = " + obj.GetIl2CppType().Name);
                                                if(result.Contains("\\u"))
                                                {
                                                    Plugin.log.LogInfo("Here ! " + result);
                                                }
                                                JObject p = JObject.Parse(result);

                                                strings = utils.PopulateTexts(p);


                                            }
                                            if (obj.GetIl2CppType().Name == "DialogUnitFSM")
                                            {
                                                DialogUnitFSM du = obj.Cast<DialogUnitFSM>();
                                                var pattern = "\"([^\"]*)\"";
                                                MatchCollection matchCollection = Regex.Matches(Regex.Unescape(du._serializedGraph.ToString()), pattern);
                                                //Plugin.log.LogInfo("duserializedgraph = " + Regex.Unescape(du._serializedGraph.ToString()));
                                                foreach (var match in matchCollection)
                                                {
                                                    if (match != null && Helpers.IsChinese(match.ToString()) && !dialogs.Contains(match.ToString()))
                                                    {
                                                        dialogs.Add(match.ToString().Replace("\"", ""));
                                                        //Plugin.log.LogInfo("Dialog String = " + match.ToString().Replace("\"", ""));
                                                    }
                                                }
                                            }
                                        }

                                        System.Collections.Generic.List<GameObject> gameObjects = ab.LoadAllAssets<GameObject>().ToList();
                                        foreach (GameObject go in gameObjects)
                                        {

                                            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
                                            TextMeshProUGUI[] meshComponents = go.GetComponentsInChildren<TextMeshProUGUI>(true);
                                            UnityEngine.UI.Text[] textComponents = go.GetComponentsInChildren<UnityEngine.UI.Text>(true);
                                            meshComponents.AddRangeToArray(go.GetComponents<TextMeshProUGUI>());
                                            meshComponents.AddRangeToArray(go.GetComponentsInParent<TextMeshProUGUI>(true));
                                            textComponents.AddRangeToArray(go.GetComponents<UnityEngine.UI.Text>());
                                            textComponents.AddRangeToArray(go.GetComponentsInParent<UnityEngine.UI.Text>(true));
                                            foreach (var x in meshComponents)
                                            {
                                                if (!Plugin.meshescontent.Contains(x.text) && Helpers.IsChinese(x.text) && x.text != "") { Plugin.meshescontent.Add(Helpers.CustomEscape(x.text)); }
                                            }
                                            foreach (var x in textComponents)
                                            {
                                                if (!Plugin.textcontent.Contains(x.text) && Helpers.IsChinese(x.text) && x.text != "") { Plugin.textcontent.Add(Helpers.CustomEscape(x.text)); }
                                            }
                                        }


                                        if (strings.Count > 0)
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

                                                        sw.Write(Helpers.CustomEscape(str) + System.Environment.NewLine);
                                                    }
                                                }
                                            }


                                        }


                                        if (dialogs.Count > 0 && ab.name == "dataasset_dialog")
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
                                                        if (str.StartsWith("【"))
                                                        {
                                                            Regex.Match(str, "【([^】]*)】");
                                                            var stro0 = str.Replace("【", "sr:\"(.*)[").Replace("】", "](.*)") + "(.*)\"";
                                                            sw.Write("##" + Helpers.CustomEscape(stro0) + System.Environment.NewLine);
                                                        }
                                                        if (!str.StartsWith("【"))
                                                        {
                                                             
                                                        sw.Write(Helpers.CustomEscape(str) + System.Environment.NewLine);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }


                                }

                            }
                            Plugin.processed.Add(ab.name);
                        }
                    }
                    catch
                    {

                    }
                }
                WriteMasterDump();
                WriteTMPUIText();
                Log.LogInfo("DONE EXPORTING EVERYTHING ! ");
            }
            var dir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "Test")).GetFiles();
            foreach (var x in dir)
            {
                if(!x.FullName.Contains("resS") && !x.FullName.Contains("manifest"))
                { 
            Plugin.log.LogInfo("File = " + x.FullName);
                    //Dump.ExtractFile(x.FullName, Application.dataPath);
                    Unwrite();
                }
            }
            //AddComponent<mbmb>();






            var harmony = new Harmony("Cadenza.GAME.ENMOD");
            Harmony.CreateAndPatchAll(System.Reflection.Assembly.GetExecutingAssembly(), null);


        }
        static void CleanFiles()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(BepInEx.Paths.PluginPath, "Dump"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            System.IO.DirectoryInfo di2 = new DirectoryInfo(Path.Combine(BepInEx.Paths.PluginPath, "Dump", "XUnity"));

            foreach (FileInfo file in di2.GetFiles())
            {
                file.Delete();
            }
        }
        public static void WriteTMPUIText()
        {
            if (textcontent.Count > 0)
            {
                var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "XUnity", "UITEXT.txt");

                using (StreamWriter sw = new StreamWriter(path, append: true))
                {
                    foreach (string str in textcontent.ToArray().Distinct())
                    {

                        sw.Write(str.Replace("=", "\\=") + System.Environment.NewLine);

                    }
                }
            }
            if (meshescontent.Count > 0)
            {
                var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "XUnity", "TMP.txt");

                using (StreamWriter sw = new StreamWriter(path, append: true))
                {
                    foreach (string str in meshescontent.ToArray().Distinct())
                    {

                        sw.Write(str.Replace("=", "\\=") + System.Environment.NewLine);

                    }
                }
            }
        }

        static void WriteMasterDump()
        {
            if (File.Exists(Path.Combine(BepInEx.Paths.PluginPath, "Dump", "XUnity", "MasterDump.txt"))) { File.Delete(Path.Combine(BepInEx.Paths.PluginPath, "Dump", "XUnity", "MasterDump.txt")); }
            var liststrings = new List<string>();
            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(BepInEx.Paths.PluginPath, "Dump"));
            foreach (FileInfo file in di.GetFiles())
            {
                var content = File.ReadAllLines(file.FullName).ToList();
                foreach (string s in content)
                {
                    var pattern = @"^.*{\d+}.*$";

                    MatchCollection matchCollection = Regex.Matches(s, pattern);
                    if (matchCollection.Count == 0 && s.Contains("=") && !s.StartsWith("##"))
                    {
                        liststrings.Add(s.Replace("=", "\\="));
                    }
                    if (matchCollection.Count == 0 && !s.Contains("=") && !s.StartsWith("##"))
                    {
                        liststrings.Add(s);
                    }

                    if (matchCollection.Count > 0 && !s.StartsWith("##"))
                    {
                        for (int i = 0; i < matchCollection.Count; i++)
                        {

                            string str = Regex.Replace(s, "{\\d+}", "(.*)");
                            string str2 = "sr:\"(.*)" + str + "(.*)";
                            var pattern2 = "\\(.*\\)";
                            if (s.Contains("="))
                            {
                                liststrings.Add(str2.Replace("=", "\\=") + "=");
                            }
                            else
                            {
                                liststrings.Add(str2);
                            }


                        }
                    }
                    if(s.StartsWith("##"))
                    {
                        liststrings.Add(s.Replace("##", "").Replace("=", "\\="));
                    }


                }




            }
            using (StreamWriter sw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "Dump", "XUnity", "MasterDump.txt"), append: true))
            {
                foreach (string str in liststrings.Distinct())
                {

                    sw.Write(str + System.Environment.NewLine);
                }
            }
        }

        internal static class MyPluginInfo
        {
            public const string PLUGIN_GUID = "Example.Plugin";
            public const string PLUGIN_NAME = "My first plugin";
            public const string PLUGIN_VERSION = "1.0.0";
        }




        public static void Write()
        {
            string assetPath0 = $"{Paths.GameRootPath}\\灵墟_Data";
            string assetPath = $"{Paths.GameRootPath}\\灵墟_Data\\StreamingAssets";
            foreach (string assetFile in Directory.GetFiles(assetPath))
            {

                byte key = 0x40;
                byte[] byteArr = File.ReadAllBytes(assetFile);
                for (int i = 0; i < byteArr.Length; i++)
                {
                    byteArr[i] = (byte)(byteArr[i] ^ key);
                }

                File.WriteAllBytes(Path.Combine(Paths.GameRootPath, "灵墟_Data", "StreamingAssets", "Test", Path.GetFileName(assetFile)), byteArr);

            }
            
        }
        public static void Unwrite()
        {
            string assetPath0 = $"{Paths.GameRootPath}\\灵墟_Data";
            string assetPath = $"{Paths.GameRootPath}\\灵墟_Data\\StreamingAssets\\Test";
            foreach (string assetFile in Directory.GetFiles(assetPath))
            {

                byte key = 0x40;
                byte[] byteArr = File.ReadAllBytes(assetFile);
                for (int i = 0; i < byteArr.Length; i++)
                {
                    byteArr[i] = (byte)(System.Math.Pow(byteArr[i], (1/key)));
                }

                File.WriteAllBytes(Path.Combine(Paths.GameRootPath, "灵墟_Data", "StreamingAssets", "Test", "Test2", Path.GetFileName(assetFile)), byteArr);

            }

        }
        public static void AssetsDump()
        {

           
        }
    }
    class mbmb : MonoBehaviour
    {
        public mbmb(System.IntPtr handle) : base(handle) { }
        private void Update()
        {

        foreach(var x in UnhollowerRuntimeLib.Il2CppType.Of<Common>().GetFields())
            {
                if (x.FieldType.Name == "String[]")
                {
                    //Plugin.log.LogInfo(x.Name);
                    var a = x.GetValue(null).Cast<Il2CppStringArray>;
                    foreach (var str in a.Invoke())
                    {
                        if (str != null)
                        {
                            if (Helpers.IsChinese(str))
                            {
                                Plugin.items.Add(str);
                            }
                        }
                    
                    }
                }

            }
           
        if(Input.GetKeyDown(KeyCode.F1))
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(BepInEx.Paths.PluginPath, "Common.txt")))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(BepInEx.Paths.PluginPath, "Common.txt"));
                }
                foreach (string str in Plugin.items.Distinct())
                {

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(BepInEx.Paths.PluginPath, "Common.txt"), append: true))
                    {
                        sw.Write(str + System.Environment.NewLine);
                    }
                    //Plugin.log.LogInfo("String = " + str + Environment.NewLine);
                }
            }
        }
    }

    /*[HarmonyPatch]

    class Patch_StringUtils
    {
        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();
            foreach (var mi in typeof(StringUtils).GetMethods())
            {
                if (mi.Name != "Equals" && mi.ReturnType == typeof(string))
                {
                    Plugin.log.LogInfo(mi.Name);
                    yield return AccessTools.Method(typeof(StringUtils), mi.Name);
                }
            }
            yield return AccessTools.Method(typeof(StringUtils), "Equals", new System.Type[] { typeof(Il2CppSystem.Object) });
            yield return AccessTools.Method(typeof(StringUtils), "Equals", new System.Type[] { typeof(Il2CppSystem.Object), typeof(Il2CppSystem.Object) });

        }
        static void Postfix(ref string __result)
        {
            Plugin.log.LogInfo("Patch_StringUtils : " + __result.ToString());
        }
    }*/

    //ParadoxNotion.Serialization.JSONSerializer), "ShowData"
    //JsonTool.ToObject()
    //UnhollowerBaseLib.IL2CPP), "ManagedStringToIl2Cpp
    //StringL all
    //ParadoxNotion.Serialization.FullSerializer.fsData), "CreateDictionary" // Pas grand chose a priori, à voir
    //UnhollowerRuntimeLib.IL2CPP.ManagedStringToIl2Cpp() returns all the strings as they are displayed on screen.
    //ParadoxNotion.Serialization.FullSerializer.fsJsonParser _input
    [HarmonyPatch]

    class Patch_Util
    {

        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();


            yield return AccessTools.Method(typeof(Util), "ToChinese");
            yield return AccessTools.Method(typeof(Util), "StringToUnicode");
            yield return AccessTools.Method(typeof(Util), "UnicodeToString");
            //yield return AccessTools.Method(typeof(Util), "ToString");
            yield return AccessTools.Method(typeof(Util), "GetWWWStreamFile");
            yield return AccessTools.Method(typeof(Util), "ConvertSidText");
            yield return AccessTools.Method(typeof(Util), "MakeConditionStrColor");
        }
            static void Postfix(Util __instance, ref string __result)
        {

            Plugin.log.LogInfo("Patch_Util : " + Regex.Unescape(__result));

            //Plugin.log.LogInfo("a = " + a);
        }
    }

    [HarmonyPatch]

    class Patch_Util2
    {

        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();


            yield return AccessTools.GetDeclaredMethods(typeof(Util)).Where(x => x.Name == "PropertySet").Where(x => !x.IsGenericMethod).First();
        }

        static void Postfix(Util __instance, ref string name)
        {

            //Plugin.log.LogInfo("Patch_Util2 : " + Helpers.CustomUnescape(name));

        }
    }
    [HarmonyPatch]

    /*class Patch_Util3
    {

        static System.Reflection.MethodBase TargetMethod()
        {

            return AccessTools.GetDeclaredMethods(typeof(Util)).Where(x => x.Name == "PropertySet").Where(x => x.IsGenericMethod).First().MakeGenericMethod(typeof(Il2CppSystem.Object));

        }

        static void Postfix(Util __instance, ref string name)
        {

            Plugin.log.LogInfo("Patch_Util3 : " + Helpers.CustomUnescape(name));

        }
    }*/
    [HarmonyPatch]

    class Patch_Util4
    {

        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();


            yield return AccessTools.Method(typeof(Util), "GetResourceInfo");
        }

        static void Postfix(Util __instance, ref ResourcesIndex.ReUnit __result)
        {

            Plugin.log.LogInfo("Patch_Util4 : " + __result.name);

        }
    }
    //String... Tried both Substring, still incomplete, need to check the rest.
    /*[HarmonyPatch]
    static class Patch05_StringL
    {
        static System.Reflection.MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(UnhollowerBaseLib.Il2CppStringArray), "AllocateArray");
        }
        static void Postfix(UnhollowerBaseLib.Il2CppStringArray __instance)
        {

                Plugin.log.LogInfo("ii = " + __instance.Count);
                for(int i = 0; i < __instance.Count; i++)
            {
                Plugin.log.LogInfo(__instance[i]);
            }

        }
    }*/


    public static class Helpers
    {
        public static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static bool IsChinese(string s)
        {
            return cjkCharRegex.IsMatch(s);
        }

        public static Dictionary<string, string> LoadFileToDictionary(string dir)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();

            IEnumerable<string> lines = File.ReadLines(Path.Combine(Plugin.sourceDir, "Translations", dir));

            foreach (string line in lines)
            {

                var arr = line.Split('¤');
                if (arr[0] != arr[1])
                {
                    var pair = new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);

                    if (!dict.ContainsKey(pair.Key))
                        dict.Add(pair.Key, pair.Value);
                    else
                        Debug.Log($"Found a duplicated line while parsing {dir}: {pair.Key}");


                }
            }

            return dict;

            //return File.ReadLines(Path.Combine(BepInEx.Paths.PluginPath, "Translations", dir))
            //    .Select(line =>
            //    {
            //        var arr = line.Split('¤');
            //        return new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);
            //    })
            //    .GroupBy(kvp => kvp.Key)
            //    .Select(x => x.First())
            //    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, comparer);
        }

        public static string CustomEscape(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
        public static string CustomUnescape(string s)
        {
            return s.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
        }
    }
    public class utils
    {
        public static List<string> PopulateTexts(JObject jObject)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (JProperty jProperty in ((JToken)jObject))
                {
                    JToken jTokenValue = jProperty.Value;
                    ProcessToken(jTokenValue, list);
                }
                return list;
            }
            catch (System.Exception ex)
            {
                Plugin.log.LogError(ex);
                return null;
            }
        }

        public static void ProcessToken(JToken jTokenValue, List<string> list)
        {
            switch (jTokenValue.Type)
            {
                case JTokenType.String:
                    JValue value = (JValue)jTokenValue;
                    if (value.Value is string text && Helpers.IsChinese(text))
                    {
                        list.Add(text);
                    }
                    break;
                case JTokenType.Object:
                    PopulateTexts((JObject)jTokenValue);
                    break;
                case JTokenType.Array:
                    foreach (JToken arrayItem in ((JArray)jTokenValue))
                    {
                        ProcessToken(arrayItem, list);
                    }
                    break;
            }
        }
    }
}

