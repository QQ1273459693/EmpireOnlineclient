using System;
using System.Collections.Generic;
using System.IO;
using TEngine;
using UnityEngine;

public static class SettingsUtils
{
    private static readonly string GlobalSettingsPath = $"TEngineGlobalSettings";
    private static TEngineSettings _globalSettings;

    public static TEngineSettings GlobalSettings
    {
        get
        {
            if (_globalSettings == null)
            {
                _globalSettings = GetSingletonAssetsByResources<TEngineSettings>(GlobalSettingsPath);
            }

            return _globalSettings;
        }
    }

    public static FrameworkGlobalSettings FrameworkGlobalSettings => GlobalSettings.FrameworkGlobalSettings;

    public static HybridCLRCustomGlobalSettings HybridCLRCustomGlobalSettings => GlobalSettings.BybridCLRCustomGlobalSettings;

    public static ResourcesArea ResourcesArea => GlobalSettings.FrameworkGlobalSettings.ResourcesArea;

    public static void SetHybridCLRHotUpdateAssemblies(List<string> hotUpdateAssemblies)
    {
        HybridCLRCustomGlobalSettings.HotUpdateAssemblies = hotUpdateAssemblies;
    }

    public static void SetHybridCLRAOTMetaAssemblies(List<string> aOTMetaAssemblies)
    {
        HybridCLRCustomGlobalSettings.AOTMetaAssemblies = aOTMetaAssemblies;
    }

    public static bool EnableUpdateData()
    {
        return FrameworkGlobalSettings.EnableUpdateData;
    }

    public static string GetUpdateDataUrl()
    {
        string url = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        url = FrameworkGlobalSettings.WindowsUpdateDataUrl;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            url = FrameworkGlobalSettings.MacOSUpdateDataUrl;
#elif UNITY_IOS
            url = FrameworkGlobalSettings.IOSUpdateDataUrl;
#elif UNITY_ANDROID
            url = FrameworkGlobalSettings.AndroidUpdateDataUrl;
#endif
        return url;
    }

    public static string GetResDownLoadPath(string fileName = "")
    {
        return Path.Combine(CompleteDownLoadPath, $"{ResourcesArea.ResAdminType}_{ResourcesArea.ResAdminCode}", GetPlatformName(), fileName).Replace("\\", "/");
    }

    public static string CompleteDownLoadPath
    {
        get
        {
            string url = "";
            if (ResourcesArea.ServerType == ServerTypeEnum.Extranet)
            {
                url = ResourcesArea.ExtraResourceSourceUrl;
            }
            else if (ResourcesArea.ServerType == ServerTypeEnum.Formal)
            {
                url = ResourcesArea.FormalResourceSourceUrl;
            }
            else
            {
                url = ResourcesArea.InnerResourceSourceUrl;
            }

            return url;
        }
    }

    private static ServerIpAndPort FindServerIpAndPort(string channelName = "")
    {
        if (string.IsNullOrEmpty(channelName))
        {
            channelName = FrameworkGlobalSettings.CurUseServerChannel;
        }

        foreach (var serverChannelInfo in FrameworkGlobalSettings.ServerChannelInfos)
        {
            if (serverChannelInfo.ChannelName.Equals(channelName))
            {
                foreach (var serverIpAndPort in serverChannelInfo.ServerIpAndPorts)
                {
                    if (serverIpAndPort.ServerName.Equals(serverChannelInfo.CurUseServerName))
                    {
                        return serverIpAndPort;
                    }
                }
            }
        }

        return null;
    }

    public static string GetServerIp(string channelName = "")
    {
        ServerIpAndPort serverIpAndPort = FindServerIpAndPort(channelName);
        if (serverIpAndPort != null)
        {
            return serverIpAndPort.Ip;
        }

        return string.Empty;
    }

    public static int GetServerPort(string channelName = "")
    {
        ServerIpAndPort serverIpAndPort = FindServerIpAndPort(channelName);
        if (serverIpAndPort != null)
        {
            return serverIpAndPort.Port;
        }

        return 0;
    }

    private static T GetSingletonAssetsByResources<T>(string assetsPath) where T : ScriptableObject, new()
    {
        string assetType = typeof(T).Name;
#if UNITY_EDITOR
        string[] globalAssetPaths = UnityEditor.AssetDatabase.FindAssets($"t:{assetType}");
        if (globalAssetPaths.Length > 1)
        {
            foreach (var assetPath in globalAssetPaths)
            {
                Debug.LogError($"Could not had Multiple {assetType}. Repeated Path: {UnityEditor.AssetDatabase.GUIDToAssetPath(assetPath)}");
            }

            throw new Exception($"Could not had Multiple {assetType}");
        }
#endif
        T customGlobalSettings = Resources.Load<T>(assetsPath);
        if (customGlobalSettings == null)
        {
            Log.Error($"Could not found {assetType} asset，so auto create:{assetsPath}.");
            return null;
        }

        return customGlobalSettings;
    }

    /// <summary>
    /// 平台名字
    /// </summary>
    /// <returns></returns>
    public static string GetPlatformName()
    {
#if UNITY_ANDROID
        return "Android";
#elif UNITY_IOS
        return "IOS";
#else
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                return "Windows64";
            case RuntimePlatform.WindowsPlayer:
                return "Windows64";

            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return "MacOS";

            case RuntimePlatform.IPhonePlayer:
                return "IOS";

            case RuntimePlatform.Android:
                return "Android";
            default:
                throw new System.NotSupportedException(string.Format("Platform '{0}' is not supported.",
                    Application.platform.ToString()));
        }
#endif
    }

    public static string GetConfigAsset(string assetName)
    {
        return GlobalSettings.FrameworkGlobalSettings.ConfigFolderName + assetName;
    }

    public static string GetDictionaryAsset(string assetName, bool fromBytes)
    {
        // return Utility.Text.Format("Assets/GameMain/Localization/{0}/Dictionaries/{1}.{2}",
        //     GameEntry.GetModule<LocalizationComponent>().Language.ToString(), assetName, fromBytes ? "bytes" : "xml");
        return string.Empty;
    }
}