using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBuilder : MonoBehaviour
{
    [MenuItem("AssetBundle/CopyABToAssetString")]
    public static void Build()
    {
        Debug.Log("Building...");
        // BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);  //Applicateion.datePatch指向Asset文件夹

        //生成fileList.json(filename, md5)
        string[] files = Directory.GetFiles(Application.dataPath + "/../AssetBundles/StandaloneWindows/", "*_ab");

        Dictionary<string, string> fileDictionary = new Dictionary<string, string>();
        foreach (var file in files)
        {
            string fileName = new FileInfo(file).Name;
            fileDictionary[fileName] = Ablib.GetMd5(file);
        }

        //别忘了AssetBundles也要加入文件清单
        fileDictionary["StandaloneWindows"] = Ablib.GetMd5(Application.dataPath + "/../AssetBundles/StandaloneWindows/StandaloneWindows");
        File.WriteAllText(Application.dataPath + "/../AssetBundles/StandaloneWindows/filelist.json", JsonUtility.ToJson(new FileList(fileDictionary)));

        //生成resourceList.josn(abName, prefabName)
        string[] resourceLists = Directory.GetFiles(Application.dataPath + "/../AssetBundles/StandaloneWindows/", "*.manifest");

        Dictionary<string, string> resourceListDictionary = new Dictionary<string, string>();
        foreach (var file in resourceLists)
        {
            FileInfo fileInfo = new FileInfo(file);
            string fileName = fileInfo.Name;
            string[] fileLines =  File.ReadAllLines(fileInfo.DirectoryName + "/" + fileName);

            //读取Assets到Dependencies中的内容
            var state = 0;
            foreach (var str in fileLines)
            {
                if (state == 0)  //表示尚未读取到"Assets"
                {
                    if (str.Contains("Assets:"))
                    {
                        state = 1;
                    }
                }
                else if (state == 1) //读到Assets,开始处理
                {
                    if (str.Contains("Dependencies:"))
                    {
                        state = 2;
                    }
                    else
                    {
                        string[] subStrings = str.Split(' ');
                        resourceListDictionary.Add(subStrings[subStrings.Length -1], fileName);
                    }
                }
                else if (state == 2)
                {
                    break;
                }
            }
        }
        File.WriteAllText(Application.dataPath + "/../AssetBundles/StandaloneWindows/resourceList.json", JsonUtility.ToJson(new FileList(resourceListDictionary)));




        // 拷贝打包文件到AssetString目录
        if (Directory.Exists(Application.streamingAssetsPath))
            Directory.Delete(Application.streamingAssetsPath, true);

        Directory.CreateDirectory(Application.streamingAssetsPath);
        Ablib.CopyDirectory(Application.dataPath + "/../AssetBundles/StandaloneWindows", Application.streamingAssetsPath);

        Debug.Log("Build Finish");
    }
}
