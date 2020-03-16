using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ToggleDefineSymbol : Editor
{
    [MenuItem("CommonUnityPaytmGames/Set GOOGLE_MOBILE_GAMES")]
    public static void Set_GOOGLE_MOBILE_GAMES()
    {
        Unset_PAYTM_FIRST_GAMES();
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.Android);

        if (!symbols.Contains("GOOGLE_MOBILE_GAMES"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Android, symbols + ";GOOGLE_MOBILE_GAMES");
        }
    }

    //[MenuItem("Tools/Unset GOOGLE_MOBILE_GAMES")]
    public static void Unset_GOOGLE_MOBILE_GAMES()
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.Android);

        if (symbols.Contains("GOOGLE_MOBILE_GAMES"))
        {
            symbols = symbols.Replace(";GOOGLE_MOBILE_GAMES", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Android, symbols);
        }

        if (symbols.Contains("GOOGLE_MOBILE_GAMES"))
        {
            symbols = symbols.Replace("GOOGLE_MOBILE_GAMES", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Android, symbols);
        }
    }

    [MenuItem("CommonUnityPaytmGames/Set PAYTM_FIRST_GAMES")]
    public static void Set_PAYTM_FIRST_GAMES()
    {
        Unset_GOOGLE_MOBILE_GAMES();
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.Android);

        if (!symbols.Contains("PAYTM_FIRST_GAMES"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Android, symbols + ";PAYTM_FIRST_GAMES");
        }
    }

    //[MenuItem("Tools/Unset PAYTM_FIRST_GAMES")]
    public static void Unset_PAYTM_FIRST_GAMES()
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(
            BuildTargetGroup.Android);

        if (symbols.Contains("PAYTM_FIRST_GAMES"))
        {
            symbols = symbols.Replace(";PAYTM_FIRST_GAMES", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Android, symbols);
        }

        if (symbols.Contains("PAYTM_FIRST_GAMES"))
        {
            symbols = symbols.Replace("PAYTM_FIRST_GAMES", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Android, symbols);
        }
    }
}