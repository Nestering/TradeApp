using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellSafeBrowser
{
    public string Url { get; private set; }

    private WellSafeBrowserInterface Interface;

    /// <summary>
    /// Сreates a WellSafeBrowser and warms up the url.
    /// </summary>
    /// <param name="Url"></param>
    public void Create(string Url)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX && !UNITY_EDITOR_OSX
        this.Url = Url;
        Interface = new WellSafeBrowserInterface();
        Interface.Create(Url);
#else
        Debug.LogError("WellSafeBrowser::current platform no supported!");
#endif
    }

    /// <summary>
    /// Sets visible the current SafeWebView.
    /// </summary>
    public void Show()
    {
        if (Interface != null)
            Interface.Show();
        else
            Debug.Log("WellSafeBrowser no create! First call Create()");
    }

    /// <summary>
    /// Sets the toolbar color
    /// </summary>
    /// <param name="HexColor">Sets color in Hex-color(Example:"#87cefa")</param>
    public void SetToolBarColor(string HexColor)
    {
        if (Interface != null)
            Interface.SetToolBarColor(HexColor);
        else
            Debug.Log("WellSafeBrowser no create! First call Create()");
    }

    /// <summary>
    /// Returns whether WallSafeBrowser is supported on this device
    /// </summary>
    /// <returns></returns>
    public static bool IsSupportedSafeBrowser()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX && !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX
        return WellSafeBrowserInterface.IsSupportedSafeBrowser();
#else
        return false;
#endif
    }
}
