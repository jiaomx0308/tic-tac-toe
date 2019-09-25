using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class ABLoader : MonoBehaviour
{

    private string serverName = "http://127.0.0.1:6688/StandaloneWindows/";
    public const string abPreName = "Assets/Resources/";

    const string luaABName = "lua_ab";
    private bool shouldUpdateLua = false;

    private int assetCount = 0;
    private int downLoadOverCount = 0;
    private bool isDownLoadOver = false;
    private bool isLoadAsset = false;
    private List<AssetBundle> assetBundleUnloadList = new List<AssetBundle>();

    public static ABLoader Instance;
    private Dictionary<string, string> resToAB = new Dictionary<string, string>();

    private void Awake()
    {
        Instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    IEnumerator Start()
    {
        print(Application.persistentDataPath + $"Start DownLoad AB {++assetCount}");
        //第一次启动程序，把StreamingAssetsPath中的资源包移动到PersistantDataPath中
        if (PlayerPrefs.GetInt("IsFirstLoad", 1) == 1)
        {
            print("FirstLoad");
            Ablib.CopyDirectory(Application.streamingAssetsPath, Application.persistentDataPath);
            PlayerPrefs.SetInt("IsFirstLoad", 0);

            shouldUpdateLua = true;
        }

        //读取本地文件的MD5码
        FileList fileListLocal = JsonUtility.FromJson<FileList>(File.ReadAllText(Application.persistentDataPath + "/filelist.json"));

        //读取服务器上的MD5码
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(serverName + "filelist.json");
        yield return unityWebRequest.SendWebRequest();


        FileList fileListService = JsonUtility.FromJson<FileList>(unityWebRequest.downloadHandler.text);

        //本地和服务器相比较,用于新增和文件改变
        foreach (var fileMd5 in fileListService.ToDictionaary())
        {
            if (!fileListLocal.ToDictionaary().ContainsKey(fileMd5.Key)) //没有这个文件，表示新增
            {
                print($"Add New File : {fileMd5.Key}, assetCount : {++assetCount}");
                StartCoroutine(AppendAB(fileMd5.Key, fileListLocal));

                if (fileMd5.Key == luaABName)
                    shouldUpdateLua = true;
            }
            else         //更新
            {
                string md5Local = fileListLocal.ToDictionaary()[fileMd5.Key];
                string md5Server = fileMd5.Value;

                if (md5Local != md5Server)
                {
                    print($"Changed File : ${fileMd5.Key}, assetCount : {++assetCount}");
                    StartCoroutine(UpdateAB(fileMd5.Key, fileListLocal));

                    if (fileMd5.Key == luaABName)
                        shouldUpdateLua = true;
                }
            }
        }

        // 服务器和本地比较，用于删除多余文件
        foreach (var fileMd5 in fileListLocal.ToDictionaary())
        {
            if (!fileListService.ToDictionaary().ContainsKey(fileMd5.Key)) //表明服务器已经删除了该文件
            {
                StartCoroutine(DeleteAB(fileMd5.Key, fileListLocal));
            }
        }

        //下载资源对应AB表:
        UnityWebRequest webRequestResourceList = UnityWebRequest.Get(serverName + "resourceList.json");
        yield return webRequestResourceList.SendWebRequest();

        resToAB = JsonUtility.FromJson<FileList>(webRequestResourceList.downloadHandler.text).ToDictionaary();



        print($"Wait for DownLoad AB {++downLoadOverCount}");
    }

    private IEnumerator DeleteAB(string fileName, FileList fileListLocal)
    {
        yield return null;

        //删除资源
        File.Delete(Application.persistentDataPath + "/" + fileName);
        File.Delete(Application.persistentDataPath + "/" + fileName + ".manifest");

        //删除json中对应的MD5码
        fileListLocal.ToDictionaary().Remove(fileName);
        File.WriteAllText(Application.persistentDataPath + "/filelist.json", JsonUtility.ToJson(fileListLocal));

        Debug.Log($"Delete {fileName} Over");
    }
    private IEnumerator UpdateAB(string fileName, FileList fileListLocal)
    {
        //删除资源
        File.Delete(Application.persistentDataPath + "/" + fileName);
        File.Delete(Application.persistentDataPath + "/" + fileName + ".manifest");

        //下载资源文件
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(serverName + fileName);
        yield return unityWebRequest.SendWebRequest();

        using (FileStream fs = File.OpenWrite(Application.persistentDataPath + "/" + fileName))
        {
            fs.Write(unityWebRequest.downloadHandler.data, 0, unityWebRequest.downloadHandler.data.Length);
        }

        //下载文件清单
        unityWebRequest = UnityWebRequest.Get(serverName + fileName + ".manifest");
        yield return unityWebRequest.SendWebRequest();

        using (FileStream fs = File.OpenWrite(Application.persistentDataPath + "/" + fileName + ".manifest"))
        {
            fs.Write(unityWebRequest.downloadHandler.data, 0, unityWebRequest.downloadHandler.data.Length);
        }

        //更新json中的MD5码
        fileListLocal.ToDictionaary()[fileName] = Ablib.GetMd5(Application.persistentDataPath + "/" + fileName);
        File.WriteAllText(Application.persistentDataPath + "/filelist.json", JsonUtility.ToJson(fileListLocal));

        Debug.Log($"Update {fileName} Over, downLoadOverCout : {++downLoadOverCount}");
    }

    private IEnumerator AppendAB(string fileName, FileList fileListLocal)
    {
        //下载资源文件
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(serverName + fileName);  //不同协程的资源可能存在竞争，最后用局部变量，或者解决竞争关系
        yield return unityWebRequest.SendWebRequest();

        using (FileStream fs = File.OpenWrite(Application.persistentDataPath + "/" + fileName))
        {
            fs.Write(unityWebRequest.downloadHandler.data, 0, unityWebRequest.downloadHandler.data.Length);
        }

        //下载文件清单
        unityWebRequest = UnityWebRequest.Get(serverName + fileName + ".manifest");
        yield return unityWebRequest.SendWebRequest();

        using (FileStream fs = File.OpenWrite(Application.persistentDataPath + "/" + fileName + ".manifest"))
        {
            fs.Write(unityWebRequest.downloadHandler.data, 0, unityWebRequest.downloadHandler.data.Length);
        }

        //更新json中的MD5码
        fileListLocal.ToDictionaary().Add(fileName, Ablib.GetMd5(Application.persistentDataPath + "/" + fileName));
        File.WriteAllText(Application.persistentDataPath + "/filelist.json", JsonUtility.ToJson(fileListLocal));

        Debug.Log($"Append {fileName} Over, downLoadOverCout : {++downLoadOverCount}");
    }

    public void LoadLuaFile()
    {
        AssetBundle luaAssetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/lua_ab");
        string[] allLines =  File.ReadAllLines(Application.persistentDataPath + "/lua_ab.manifest");
        int state = 0;

        foreach (var str in allLines)
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
                    TextAsset luaAsset = luaAssetBundle.LoadAsset<TextAsset>(subStrings[subStrings.Length - 1]);

                    string luaName = ABNameToLuaName(subStrings[subStrings.Length - 1]);
                    string directoryName = Application.persistentDataPath + "/" + luaName.Remove(luaName.LastIndexOf('/'));


                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    File.WriteAllText(Application.persistentDataPath + "/" + luaName, luaAsset.text);
                    
                }
            }
            else if (state == 2)
            {
                break;
            }
        }
    }

    public GameObject LoadPrefab(string abName, string assetName)
    {
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/StandaloneWindows");
        AssetBundleManifest assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("StandaloneWindows");

        string[] dep = assetBundleManifest.GetAllDependencies(abName);
        for (int i = 0; i < dep.Length; i++)
        {
            string s = dep[i];
            assetBundleUnloadList.Add(AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + s)); //把相应依赖资源加载到AssetBundle后，我们Instantiate GameObject后unity找到对于资源的依赖关系就可以加载
        }

        AssetBundle abGameObject = AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + abName);
        GameObject gameObject = abGameObject.LoadAsset<GameObject>(assetName);

        GameObject go = Instantiate(gameObject);

        assetBundleUnloadList.Add(abGameObject);
        return go;
    }



    void StartGame()
    {

        Debug.Log("Start Logic");

        //资源下载完毕，开始load场景
        //LoadPrefab("modles.ab", "MyCube.prefab");

        SaveGame.saveGame.Init();
        Logic.Instance.Init();


        //场景Load完毕后，清理现场
        foreach (var assetBundle in assetBundleUnloadList)
        {
            assetBundle.Unload(false); //false表示正在使用的资源不做清理
        }

        assetBundleUnloadList.Clear();
    }





    public string ABNameToLuaName(string abName)
    {
        string luaByte = abName.Substring(abPreName.Length);
        return luaByte.Remove(luaByte.LastIndexOf('.'));
    }




    public object LoadABAsset(string assetName)
    {
        string abManifest = resToAB[assetName];
        string abName = abManifest.Remove(abManifest.LastIndexOf('.'));
        var ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abName); 
        var asset = ab.LoadAsset(assetName);

        ab.Unload(false);
        return asset;
    }


    private void Update()
    {
        if (!isDownLoadOver && assetCount == downLoadOverCount)
        {
            isDownLoadOver = true;

            if (shouldUpdateLua)
                LoadLuaFile();

            StartGame();
        }
    }
}
