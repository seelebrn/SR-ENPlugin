using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Reflection;
using System.IO;
using Il2CppSystem;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using UnhollowerBaseLib;
using UnityEngine;
using Il2CppSystem.Linq;
using UnhollowerRuntimeLib;
using Newtonsoft.Json.Linq;

namespace SRLinesPuller
{

    [BepInPlugin("Cadenza.SRLinesPuller.EN.MOD", "SRLinesPuller", "0.5")]
    public class Plugin : BasePlugin
    {
        public static string[] forbidden = new string[] { "Sprite", "AnimationClip", "RuntimeAnimatorController", "Texture2D", "PlaceIcon", "AudioClip", "Material", "SpriteAtlas", "Font", "Shader", "TMP_FontAsset" };
      

        public static BepInEx.Logging.ManualLogSource log;


        public override void Load()
        {
            AddComponent<mbmb>();
            log = Log;

        }



    }

    class mbmb : MonoBehaviour
    {
        public mbmb(System.IntPtr handle) : base(handle) { }
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F8) == true)
            {
                if (File.Exists(Path.Combine(BepInEx.Paths.PluginPath, "MasterList.txt")))
                {
                    File.Delete(Path.Combine(BepInEx.Paths.PluginPath, "MasterList.txt"));
                }

                System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(BepInEx.Paths.PluginPath, "Assets"));

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles_Native())
                {
                    //Plugin.log.LogInfo("Bundle Name =" + ab.name);
                    UnityEngine.Object[] objarr = bundle.LoadAllAssets<UnityEngine.Object>();

                    foreach (var x in objarr)
                    {
                        if(!Plugin.forbidden.Contains(x.GetIl2CppType().Name))
                        { 
                        List<string> list = new List<string>();

                            var y = UnityEngine.JsonUtility.ToJsonInternal(x, true);

                            //Plugin.log.LogInfo("Y = " + y);
                            var p = JObject.Parse(y);
                            //Plugin.log.LogInfo("P = " + p);


                            if (p != null)
                            {
                                Plugin.log.LogInfo("Non Null");
                                foreach (var a in p.DescendantsAndSelf())
                                {
                                    if (a is JObject obj)
                                        foreach (var prop in obj.Properties())
                                            if (!(prop.Value is JObject) && !(prop.Value is JArray))
                                            {
                                                try
                                                {
                                                    if (JObject.Parse(prop.Value.ToString()).HasValues)
                                                    {
                                                        var subjson = JObject.Parse(prop.Value.ToString());
                                                        foreach (var b in subjson.DescendantsAndSelf())
                                                        {
                                                            if (b is JObject obj2)
                                                            {
                                                                foreach (var prop2 in obj2.Properties())
                                                                {
                                                                    if (!(prop2.Value is JObject) && !(prop2.Value is JArray) && prop2.Value != null)
                                                                    {
                                                                        if (Helpers.IsChinese(prop2.Value.ToString()))
                                                                        {
                                                                            Plugin.log.LogInfo("SubValue = " + prop2.Value.ToString());
                                                                            list.Add(prop2.Value.ToString().Replace("\n", ""));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }


                                                }
                                                catch
                                                {

                                                }
                                                if (Helpers.IsChinese(prop.Value.ToString()) && !prop.Value.ToString().StartsWith("{"))
                                                {
                                                    Plugin.log.LogInfo("Value = " + prop.Value.ToString().Replace("\n", ""));
                                                    list.Add(prop.Value.ToString().Replace("\n", ""));

                                                }



                                            }



                                }
                            }
                            else
                            {
                                Plugin.log.LogInfo("Null");
                            }


                            using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "Assets", x.name + " - " + x.GetIl2CppType().Name + x.GetHashCode() + ".txt"), append: true))
                            {

                                foreach (string s in list.Distinct())
                                {
                                    if (Helpers.IsChinese(s))
                                    {
                                        tw.Write(s + Il2CppSystem.Environment.NewLine);
                                    }
                                }
                                tw.Close();
                            }
                            using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "MasterList.txt"), append: true))
                            {

                                foreach (string s in list.Distinct())
                                {
                                    if (Helpers.IsChinese(s))
                                    {
                                        tw.Write(s + Il2CppSystem.Environment.NewLine);
                                    }
                                }
                                tw.Close();
                            }

                        }
                    }
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