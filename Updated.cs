 [HarmonyPatch(typeof(ResourceControl), "GetAssetNowProgress")]
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
                                    //Plugin.mod.LogInfo("        ScriptableObject = " + obj.GetIl2CppType().Name);
                                    string result = UnityEngine.JsonUtility.ToJson(obj);

                                    //byte[] serializedData = SerializationUtility.SerializeValue(obj.GetIl2CppType(), DataFormat.JSON);
                                    //string result = System.Text.Encoding.UTF8.GetString(serializedData);
                                    if (obj.GetIl2CppType().Name == "AwardGroup") { Plugin.mod.LogInfo("SerializedData = " + result); }
                                    var pattern = "\"([^\"]*)\"";
                                    MatchCollection matchCollection = Regex.Matches(result, pattern);
                                    foreach (var match in matchCollection)
                                    {
                                        if (match != null && Helpers.IsChinese(match.ToString()) && !strings.Contains(match.ToString()))
                                        {
                                            strings.Add(match.ToString().Replace("\"", ""));
                                        }

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
                            }

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
                            }*/


                        }

                    }
                    Plugin.processed.Add(bundle.name);
                }
            }
        }

    }
