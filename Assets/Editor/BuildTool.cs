using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XLuaFramework;
using XLuaFramework.Util;

public class BuildTool : Editor
{
    [MenuItem("Tools/Build Windows Bundle")]
    static void BundleWindowsBuild()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build Android Bundle")]
    static void BundleAndroidBuild()
    {
        Build(BuildTarget.Android);
    }
    static void Build(BuildTarget buildTarget)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();//相当于打标签的那个

        //文件信息列表
        List<string> bundleInfos = new List<string>();
        //
        string bundleName;

        var TargetDirectory = PathUtil.BuildResourcesPath;
        //按文件夹
        //Directory.GetDirectories()
        //按文件
        string[] files = Directory.GetFiles(TargetDirectory, "*", SearchOption.AllDirectories);
        //过滤meta文件及构造 bundle 信息
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
                continue;
            Debug.Log($"file:{files[i]}");
            AssetBundleBuild assetBundle = BuildBundle(files[i],out bundleName);
            assetBundleBuilds.Add(assetBundle);

            //添加文件信息和依赖信息
            var assetName =  assetBundle.assetNames[0];
            List<string> dependenceInfo = GetDependence(assetName);
            var bundleInfo = assetName + "|" + bundleName + AppConst.BUNDLE_EXTENSION;

            if(dependenceInfo.Count > 0)
                bundleInfo = bundleInfo + "|" + string.Join("|",dependenceInfo);

            bundleInfos.Add(bundleInfo);



        }
       

        //生成目录
        if (Directory.Exists(PathUtil.BuildOutPath))
            Directory.Delete(PathUtil.BuildOutPath,true);
        Directory.CreateDirectory(PathUtil.BuildOutPath);

        //构建 Bundle
        BuildPipeline.BuildAssetBundles(PathUtil.BuildOutPath,assetBundleBuilds.ToArray(),BuildAssetBundleOptions.None, buildTarget);

        //输出版本文件
        File.WriteAllLines(PathUtil.BuildOutPath + "/" + AppConst.FILE_LIST_NAME, bundleInfos);

        //刷新资源目录
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 构建 Bundle 信息
    /// </summary>
    /// <param name="file">asset 文件</param>
    /// <returns></returns>
    static AssetBundleBuild BuildBundle(string file,out string bundleName)
    {
        AssetBundleBuild assetBundle = new AssetBundleBuild();

        string fileName = PathUtil.GetStandardPath(file);


        string assetName = PathUtil.GetUnityPath(fileName);
        Debug.Log($"file:{assetName}");
        //构建包信息，准备打包
        assetBundle.assetNames = new string[] { assetName };
        bundleName = file.Replace(PathUtil.BuildResourcesPath + "\\", "").ToLower();//
        assetBundle.assetBundleName = bundleName + AppConst.BUNDLE_EXTENSION;
        return assetBundle;
    }

    /// <summary>
    /// 获取依赖文件列表
    /// </summary>
    /// <param name="curFile"></param>
    /// <returns></returns>
    static List<string> GetDependence(string curFile)
    {
        List<string> dependence = new List<string>();
        string[] files = AssetDatabase.GetDependencies(curFile);
        dependence = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToList();  
        return dependence;
    }
}
