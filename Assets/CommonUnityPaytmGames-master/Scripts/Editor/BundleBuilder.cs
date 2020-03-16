using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BundleBuilder : Editor
{
    [MenuItem("CommonUnityPaytmGames/ AndroidBuildAssetBundles")]
    static void BuildAssetBundles()
    {
        if (!Directory.Exists("Assets/AssetBundles"))
        {
            Directory.CreateDirectory("Assets/AssetBundles");
        }
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
