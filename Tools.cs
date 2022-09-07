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

namespace Mod
{
    public class Tools
    {
        public static IEnumerable<Il2CppSystem.Reflection.FieldInfo> GetSubFields(Il2CppSystem.Object x)
        {
            Il2CppReferenceArray<Il2CppSystem.Reflection.FieldInfo> memberInfos = x.GetIl2CppType().GetFields();
            foreach (Il2CppSystem.Reflection.FieldInfo f in memberInfos)
            {
                yield return f;
            }
        }
        public static IEnumerable<Il2CppSystem.Reflection.PropertyInfo> GetSubProperties(Il2CppSystem.Object x)
        {
            Il2CppReferenceArray<Il2CppSystem.Reflection.PropertyInfo> memberInfos = x.GetIl2CppType().GetProperties();
            foreach (Il2CppSystem.Reflection.PropertyInfo p in memberInfos)
            {
                yield return p;
            }
        }
        public static IEnumerable<Il2CppSystem.Object> GetValues(IEnumerable<Il2CppSystem.Reflection.FieldInfo> fi, Il2CppSystem.Object x)
        {
            foreach (var f in fi)
            {
                Plugin.log.LogInfo(f.Name);
                if (f.GetValue(x) != null)
                {

                yield return f.GetValue(x);
                }
            }
        }
        public static IEnumerable<Il2CppSystem.Object> GetValues(IEnumerable<Il2CppSystem.Reflection.PropertyInfo> fi, Il2CppSystem.Object x)
        {
            foreach (var f in fi)
            {
                Plugin.log.LogInfo(f.Name);
                if (f.GetValue(x) != null)
                {
                    yield return f.GetValue(x);
                }
            }
        }
        public static bool FieldArrayValuesContainCHString(IEnumerable<Il2CppSystem.Reflection.FieldInfo> fi, Il2CppSystem.Object x)
        {
            int i = 0;
            foreach (var f in fi)
            {
                if (f.GetValue(x).GetIl2CppType().Name == "String")
                {
                    if (Helpers.IsChinese(f.GetValue(x).ToString()))
                    {
                        i = i + 1;
                    }
                }
            }
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool PropertyArrayValuesContainCHString(IEnumerable<Il2CppSystem.Reflection.PropertyInfo> fi, Il2CppSystem.Object x)
        {
            int i = 0;
            foreach (var f in fi)
            {
                if (f.GetValue(x).GetIl2CppType().Name == "String")
                {
                    if (Helpers.IsChinese(f.GetValue(x).ToString()))
                    {
                        i = i + 1;
                    }
                }
            }
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void GetAllCHStrings(IEnumerable<Il2CppSystem.Reflection.FieldInfo> fi, Il2CppSystem.Object x)
        {
            foreach (var f in fi)
            {


                if (f.GetValue(x).GetIl2CppType() == Il2CppType.Of<string>())
                {
                    if (Helpers.IsChinese(f.GetValue(x).ToString()))
                    {
                        try
                        {
                            Plugin.log.LogInfo("Found String : " + f.GetValue(x).ToString() + ", was found in : " + f.Name + " + Field Value Type = " + f.GetValue(x).GetIl2CppType().Name);
                        }
                        catch
                        {

                        }
                        

                        //yield return f.GetValue(x).ToString();
                    }

                }
                else
                {


                }

            }
        }
        public static void GetAllCHStrings(IEnumerable<Il2CppSystem.Reflection.PropertyInfo> fi, Il2CppSystem.Object x)
        {
            foreach (var f in fi)
            {


                if (f.GetValue(x).GetIl2CppType() == Il2CppType.Of<string>())
                {
                    if (Helpers.IsChinese(f.GetValue(x).ToString()))
                    {
                        try
                        {
                            Plugin.log.LogInfo("Found String : " + f.GetValue(x).ToString() + ", was found in : " + f.Name + " + Property Value Type = " + f.GetValue(x).GetIl2CppType().Name);
                        }
                        catch
                        {

                        }

                        //yield return f.GetValue(x).ToString();
                    }

                }
                else
                {
                    

                }

            }
        }
        public static IEnumerable<Il2CppSystem.Collections.Generic.List<Il2CppSystem.Object>> GetListValues(IEnumerable<Il2CppSystem.Reflection.FieldInfo> fi, Il2CppSystem.Object x)
        {
            foreach (Il2CppSystem.Reflection.FieldInfo f in fi)
            {
                if (x.TryCast<Il2CppSystem.Collections.IList>() != null)
                {
                    //Plugin.log.LogInfo("    Field name : " + f.Name + " + " + "Field Value : " + f.GetValue(x).ToString());

                    var list = f.GetValue(x).TryCast<Il2CppSystem.Collections.IEnumerable>();
                    var z = list.OfType<Il2CppSystem.Object>();

                    yield return z.ToList();
                }


            }
        }
        public static IEnumerable<Il2CppSystem.Collections.Generic.List<Il2CppSystem.Object>> GetListValues(IEnumerable<Il2CppSystem.Reflection.PropertyInfo> fi, Il2CppSystem.Object x)
        {
            foreach (Il2CppSystem.Reflection.PropertyInfo f in fi)
            {
                if (x.TryCast<Il2CppSystem.Collections.IList>() != null)
                {
                    //Plugin.log.LogInfo("    Field name : " + f.Name + " + " + "Field Value : " + f.GetValue(x).ToString());

                    var list = f.GetValue(x).TryCast<Il2CppSystem.Collections.IEnumerable>();
                    var z = list.OfType<Il2CppSystem.Object>();

                    yield return z.ToList();
                }


            }
        }



        public static void GetAllMemberInfo(IEnumerable<Il2CppSystem.Reflection.FieldInfo> fi, Il2CppSystem.Object x)
        {
            Plugin.log.LogInfo("Fields Info");
            foreach (var f in fi)
            {
                Plugin.log.LogInfo("Field Name = " + f.Name + " + " + "Field Type = " + f.GetValue(x).GetIl2CppType().Name);
            }
        }
        public static void GetAllMemberInfo(IEnumerable<Il2CppSystem.Reflection.PropertyInfo> fi, Il2CppSystem.Object x)
        {
            Plugin.log.LogInfo("Properties Info");
            foreach (var f in fi)
            {
                Plugin.log.LogInfo("Field Name = " + f.Name + " + " + "Field Type = " + f.GetValue(x).GetIl2CppType().Name);
            }
        }

    }
}
