#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WellSafeBrowserInterface
{
    [DllImport("__Internal")]
    private static extern void ex_safe_Create(string Url);
    [DllImport("__Internal")]
    private static extern void ex_safe_SetToolBar(string HexColor);
    [DllImport("__Internal")]
    private static extern void ex_safe_Show();


    private bool Inited = false;

    public void Create(string Url)
    {
        ex_safe_Create(Url);
        Inited = true;
    }

    public void Show()
    {
        if (Inited)
            ex_safe_Show();
        else
            Debug.LogError("WellWebView::SafeBrowser no inited!");
    }

    public void SetToolBarColor(string HexColor)
    {
        if (Inited)
            ex_safe_SetToolBar(HexColor);
        else
            Debug.LogError("WellWebView::SafeBrowser no inited!");
    }

    public static bool IsSupportedSafeBrowser()
    {
        return true;
    }
}
#endif