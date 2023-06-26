using System;
using UnityEngine;
using ES3Internal;


public enum E_StoreMode
{
    ALPHA,
    BETA,
    RTL
}

public class LocalSaveManager
{
    // BETA目录 Directory = DataPath
    public static readonly string ALPHA_SAVE_PATH = "../Test/";
    public static readonly string BETA_SAVE_PATH = "../SaveDatas/";
    public static readonly string FILEPATH = "save_data.es3";


    /// <summary>
    /// 设置存储路径 游戏启动时修改
    /// </summary>
    /// <param name="测试版本">E_StoreMode.BETA</param>
    /// <param name="发布版本">E_StoreMode.RTL</param>
    public static void SetStorePath(E_StoreMode mode)
    {
        if (mode == E_StoreMode.BETA)
        {
            ES3Settings.defaultSettings.directory = ES3.Directory.DataPath;
            ES3Settings.defaultSettings.path = BETA_SAVE_PATH + FILEPATH;
        }
        else if (mode == E_StoreMode.ALPHA)
        {
            ES3Settings.defaultSettings.directory = ES3.Directory.DataPath;
            ES3Settings.defaultSettings.path = ALPHA_SAVE_PATH + FILEPATH;
        }
        else if (mode == E_StoreMode.RTL)
        {
            ES3Settings.defaultSettings.directory = ES3.Directory.PersistentDataPath;
            ES3Settings.defaultSettings.path = FILEPATH;
        }
    }


    #region Save

    /// <summary>
    /// 数据保存 默认地址路径为 Assest/SaveDatas/
    /// </summary>
    /// <param name="key / value">可存储任意对象，初字节数组外需要调用 SaveBytes 方法 </param>
    /// <param name="filePath">默认保存在"neon_data.es3"中，根据需要自定义</param>
    /// <param name="settings">根据需要 settings = new ES3Settings(ES3.EncryptionType.AES, "myPassword");</param>
    public static void Save(string key, object value, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.Save<object>(key, value, setting);
    }

    /// <summary>
    /// 数据保存 默认地址路径为 Assest/SaveDatas/
    /// </summary>
    /// <param name="key / value">可存储任意对象，初字节数组外需要调用 SaveBytes 方法 </param>
    /// <param name="filePath">默认保存在"neon_data.es3"中，根据需要可自定义</param>
    /// <param name="settings">根据需要 settings = new ES3Settings(ES3.EncryptionType.AES, "myPassword");</param>
    public static void Save<T>(string key, T value, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.Save<T>(key, value, setting);
    }

    // 字节数组
    public static void SaveBytes(byte[] bytes, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.SaveRaw(bytes, setting);
    }

    public static void SaveBytes(string str, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        var bytes = setting.encoding.GetBytes(str);
        ES3.SaveRaw(bytes, setting);
    }

    // 追加数据
    public static void AppendBytes(byte[] bytes, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.AppendRaw(bytes, setting);
    }

    public static void AppendBytes(string str, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.AppendRaw(str, setting);
    }

    // 保存Image
    public static void SaveImage(Texture2D texture, int quality = 75, string imagePath = null,
        ES3Settings settings = null)
    {
        var setting = new ES3Settings(imagePath, settings);
        ES3.SaveImage(texture, quality, setting);
    }

    #endregion

    #region Load

    public static object Load(string key, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        return ES3.Load<object>(key, setting);
    }

    public static T Load<T>(string key, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        return ES3.Load<T>(key, setting);
    }

    public static object Load(string key, object defaultValue, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        return ES3.Load<object>(key, defaultValue, setting);
    }

    public static T Load<T>(string key, T defaultValue, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        return ES3.Load<T>(key, defaultValue, setting);
    }

    // LoadInto

    public static void LoadInto(string key, object obj, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.LoadInto<object>(key, obj, setting);
    }

    public static void LoadInto<T>(string key, T obj, string filePath = null, ES3Settings settings = null)
        where T : class
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.LoadInto<T>(key, obj, setting);
    }


    // 字节数组
    public static byte[] LoadRawBytes(string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        return ES3.LoadRawBytes(setting);
    }

    public static string LoadRawString(string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        var bytes = ES3.LoadRawBytes(setting);
        return setting.encoding.GetString(bytes, 0, bytes.Length);
    }

    public static Texture2D LoadImage(string imagePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(imagePath, settings);
        return ES3.LoadImage(setting);
    }

    public static Texture2D LoadImage(byte[] bytes)
    {
        var texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        return texture;
    }

    public static AudioClip LoadAudio(string audioFilePath, ES3Settings settings = null)
    {
        var setting = new ES3Settings(null, settings);
        return ES3.LoadAudio(audioFilePath, AudioType.UNKNOWN, setting);
    }

    #endregion

    #region Serialize/Deserialize

    public static byte[] Serialize<T>(T value, ES3Settings settings = null)
    {
        return ES3.Serialize(value, settings);
    }

    public static T Deserialize<T>(byte[] bytes, ES3Settings settings = null)
    {
        return ES3.Deserialize<T>(bytes, settings);
    }

    public static void DeserializeInto<T>(byte[] bytes, T obj, ES3Settings settings = null) where T : class
    {
        ES3.DeserializeInto(ES3TypeMgr.GetOrCreateES3Type(typeof(T)), bytes, obj, settings);
    }

    public static void DeserializeInto<T>(ES3Types.ES3Type type, byte[] bytes, T obj, ES3Settings settings = null)
        where T : class
    {
        ES3.DeserializeInto<T>(type, bytes, obj, settings);
    }

    #endregion

    #region Encrypt/Decrypt (bytes/string)

    public static byte[] EncryptBytes(byte[] bytes, string password = null)
    {
        return ES3.EncryptBytes(bytes, password);
    }

    public static byte[] DecryptBytes(byte[] bytes, string password = null)
    {
        return ES3.DecryptBytes(bytes, password);
    }

    public static string EncryptString(string str, string password = null)
    {
        var encoding = ES3Settings.defaultSettings.encoding;
        byte[] bytes = encoding.GetBytes(str);
        byte[] res = ES3.EncryptBytes(bytes, password);
        return encoding.GetString(res);
    }

    public static string DecryptString(string str, string password = null)
    {
        var encoding = ES3Settings.defaultSettings.encoding;
        byte[] bytes = encoding.GetBytes(str);
        byte[] res = ES3.DecryptBytes(bytes, password);
        return encoding.GetString(res);
    }

    #endregion

    #region Delete

    public static void DeleteFile(string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.DeleteFile(setting);
    }

    public static void DeleteDirectory(string directoryPath, ES3Settings settings = null)
    {
        var setting = new ES3Settings(directoryPath);
        ES3.DeleteDirectory(setting);
    }

    public static void DeleteKey(string key, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        ES3.DeleteKey(key, setting);
    }

    #endregion

    #region Get/Exists

    public static bool KeyExists(string key, string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        return ES3.KeyExists(key, setting);
    }

    public static bool FileExists(string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        return ES3.FileExists(setting);
    }

    public static bool DirectoryExists(string folderPath, ES3Settings settings = null)
    {
        var setting = new ES3Settings(folderPath, settings);
        return ES3.DirectoryExists(setting);
    }

    public static string[] GetKeys(string filePath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(filePath, settings);
        return ES3.GetKeys(setting);
    }

    public static string[] GetFiles(string directoryPath = null, ES3Settings settings = null)
    {
        ES3Settings setting;
        if (directoryPath == null)
        {
            setting = new ES3Settings();
            if (setting.location == ES3.Location.File)
            {
                if (setting.directory == ES3.Directory.PersistentDataPath)
                    setting.path = Application.persistentDataPath;
                else
                    setting.path = Application.dataPath;
            }
        }
        else
        {
            setting = new ES3Settings(directoryPath, settings);
        }

        return ES3.GetFiles(setting);
    }

    public static string[] GetDirectories(string directoryPath = null, ES3Settings settings = null)
    {
        var setting = new ES3Settings(directoryPath, settings);
        return ES3.GetDirectories(setting);
    }

    #endregion
}

