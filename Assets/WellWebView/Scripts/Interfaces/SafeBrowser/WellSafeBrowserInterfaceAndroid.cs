#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellSafeBrowserInterface
{
    public AndroidJavaObject WebViewInterface;
    public void Create(string Url)
    {
        WebViewInterface = new AndroidJavaObject("com.antwell.wellwebview.WellSafeBrowserInterface");
        if (WebViewInterface != null)
            WebViewInterface.Call("CreateSafeBrowser",Url);
        else
            Debug.LogError("WellWebView::no found Android library in Plugins!");
    }

    public void Show()
    {
        if (WebViewInterface != null)
            WebViewInterface.Call("Show");
        else
            Debug.LogError("WellWebView::no found Android library in Plugins!");
    }

    public void SetToolBarColor(string HexColor) 
    {
        if (WebViewInterface != null)
            WebViewInterface.Call("SetToolBarColor", HexColor);
        else
            Debug.LogError("WellWebView::no found Android library in Plugins!");
    }

    public static bool IsSupportedSafeBrowser()
    {
        AndroidJavaObject WebViewInterface = new AndroidJavaObject("com.antwell.wellwebview.WellSafeBrowserInterface");
        if (WebViewInterface != null)
            return WebViewInterface.CallStatic<bool>("IsSupportedSafeBrowser");
        else
        {
            Debug.LogError("WellWebView::no found Android library in Plugins!");
            return false;
        }

    }
}
#endif
