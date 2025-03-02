#if !UNITY_ANDROID && !UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellSafeBrowserInterface
{
    public void Create(string Url)
    {
        Debug.LogError("WellSafeBrowser::current platform no supported!");
    }

    public void Show()
    {
        Debug.LogError("WellSafeBrowser::current platform no supported!");
    }

    public void SetToolBarColor(string HexColor)
    {
        Debug.LogError("WellSafeBrowser::current platform no supported!");
    }

    public static bool IsSupportedSafeBrowser()
    {
        return false;
    }
}
#endif
