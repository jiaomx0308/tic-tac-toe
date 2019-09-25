using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionaary() { return target; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }

    public void OnAfterDeserialize()
    {
        int Cout = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(Cout);
        for (int i = 0; i < Cout; i++)
        {
            target.Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }
}


[Serializable]
public class FileList : Serialization<string, string>
{
    public FileList(Dictionary<string, string> target) : base(target) { }
}

public class Ablib
{
    //MD5生成工具：
    public static string GetMd5(string fileName)
    {
        string result = "";
        using (FileStream fs = File.OpenRead(fileName))
        {
            result = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(fs));
            fs.Close();
        }
        return result;
    }

    //文件夹内拷贝工具：
    public static void CopyDirectory(string srcPath, string destPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();

            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)
                {
                    if (!Directory.Exists(destPath + "\\" + i.Name))
                    {
                        Directory.CreateDirectory(destPath + "\\" + i.Name);
                    }
                    CopyDirectory(i.FullName, destPath + "\\" + i.Name); //递归拷贝子文件夹下的文件
                }
                else
                {
                    File.Copy(i.FullName, destPath + "\\" + i.Name, true);
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

