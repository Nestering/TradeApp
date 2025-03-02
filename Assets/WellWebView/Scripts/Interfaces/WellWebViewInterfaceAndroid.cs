#if UNITY_ANDROID && !UNITY_EDITOR_OSX
using System.Collections.Generic;
using UnityEngine;

public class WellWebViewInterface : AbstractWellWebViewInterface
{
    public WellWebView WebView;
    public AndroidJavaObject JavaWebViewInterface;

    public static Dictionary<string, AndroidJavaClass> Listeners = new Dictionary<string, AndroidJavaClass>();

    internal override void Init(bool swipeToRefresh)
    {
        WellWebViewAndroidListener androidListener = new WellWebViewAndroidListener();
        androidListener.webview = WebView;
        JavaWebViewInterface = new AndroidJavaObject("com.antwell.wellwebview.WellWebViewInterface");
        if (JavaWebViewInterface != null)
        {
            JavaWebViewInterface.Call("Init", androidListener, WebView.Identifier, swipeToRefresh);
        }
        else
            Debug.LogError("WellWebView::no found Android library in Plugins!");
    }

    internal override void ForceDestroyMultipleWindow()
    {
        JavaWebViewInterface.Call("ForceDestroyMultipleWindow");
    }

    internal override void Show(bool fade, float duration, WellWebViewAnimation.AnimationType animation, string IdCallback)
    {
        JavaWebViewInterface.Call("Show", fade, duration, (int)animation,IdCallback);
    }

    internal override void Hide(bool fade, float duration, WellWebViewAnimation.AnimationType animation, string IdCallback)
    {
        int hideAnimID = (int)(animation == WellWebViewAnimation.AnimationType.None ? 0 : animation + 4);
        JavaWebViewInterface.Call("Hide", fade,duration, hideAnimID, IdCallback);
    }

    internal override void LoadUrl(string url)
    {
        JavaWebViewInterface.Call("Load", url);
    }

    internal override void LoadHTMLString(string html, string BaseUrl)
    {
        JavaWebViewInterface.Call("LoadHTMLString", html, BaseUrl);
    }

    internal override void SetSize(int x, int y, int width, int height)
    {
        JavaWebViewInterface.Call("SetSize", x, y, width, height);
    }

    internal override void ScrollTo(int x, int y, bool animate)
    {
        JavaWebViewInterface.Call("ScrollTo", x, y, animate);
    }

    internal override int GetMaxScrolledPage()
    {
        return JavaWebViewInterface.Call<int>("GetPageHeight");
    }

    internal void ShowNavigationBar(bool show)
    {
        JavaWebViewInterface.Call("ShowNavigationBar", show);
    }


    internal override void EvaluateJavaScript(string IdCallback, string jsCode)
    {
        JavaWebViewInterface.Call("EvaluateJavaScript", IdCallback, jsCode);
    }

    internal override void AddUrlCustomScheme(string scheme)
    {
        JavaWebViewInterface.Call("AddUrlCustomScheme", scheme);
    }

    internal override void RemoveUrlCustomScheme(string scheme)
    {
        JavaWebViewInterface.Call("RemoveUrlCustomScheme", scheme);
    }

    internal override void AddDomainInSslException(string domain)
    {
        JavaWebViewInterface.Call("AddDomainInSslException", domain);
    }

    internal override void RemoveDomainInSslException(string domain)
    {
        JavaWebViewInterface.Call("RemoveDomainInSslException", domain);
    }

    internal void AddDomainAllowPermission(string domain)
    {
        JavaWebViewInterface.Call("AddDomainAllowPermission", domain);
    }

    internal void RemoveDomainAllowPermission(string domain)
    {
        JavaWebViewInterface.Call("RemoveDomainAllowPermission", domain);
    }

    internal override void AllowShowSslDialog(bool enable)
    {
        JavaWebViewInterface.Call("AllowShowSslDialog", enable);
    }

    internal override void AddJavaScript(string IdCallback, string jsCode)
    {
        JavaWebViewInterface.Call("AddJavaScript", IdCallback, jsCode);
    }

    internal override void GetHTMLCode(bool unescape, string IdCallback)
    {
        JavaWebViewInterface.Call("GetHTMLCode", unescape,IdCallback);
    }

    internal override void SetUserAgent(string userAgent)
    {
        JavaWebViewInterface.Call("SetUserAgent", userAgent);
    }

    internal override string GetUserAgent()
    {
        return JavaWebViewInterface.Call<string>("GetUserAgent");
    }

    internal override void SetAlpha(float _alpha)
    {
        JavaWebViewInterface.Call("SetAlpha", _alpha);
    }

    internal override void Print()
    {
        JavaWebViewInterface.Call("Print");
    }

    internal override void ShowProgressBar(string HexColor)
    {
        JavaWebViewInterface.Call("ShowProgressBar", HexColor);
    }

    internal override void HideProgressBar()
    {
        JavaWebViewInterface.Call("HideProgressBar");
    }

    internal void SetUseWideView(bool enable)
    {
        JavaWebViewInterface.Call("SetUseWideView", enable);
    }

    internal static void SetAllowAutoPlay(bool autoPlay)
    {
        AndroidJavaObject TempInterface = new AndroidJavaObject("com.antwell.wellwebview.WellWebViewInterface");
        TempInterface.CallStatic("SetAllowAutoPlay", autoPlay);
    }

    internal static void SetAllowJavaScript(bool enable)
    {
        AndroidJavaObject TempInterface = new AndroidJavaObject("com.antwell.wellwebview.WellWebViewInterface");
        TempInterface.CallStatic("SetAllowJavaScript", enable);
    }

    internal void SetDefaultFontSize(int fontSize)
    {
        JavaWebViewInterface.Call("SetDefaultFontSize", fontSize);
    }

    internal void SetLoadWithOverviewMode(bool _enable)
    {
        JavaWebViewInterface.Call("SetLoadWithOverviewMode", _enable);
    }

    internal override void SetZoomEnabled(bool _enable)
    {
        JavaWebViewInterface.Call("SetZoomEnabled", _enable);
    }

    internal override void SetHorizontalScrollBarEnabled(bool _enable)
    {
        JavaWebViewInterface.Call("SetHorizontalScrollBarEnabled", _enable);
    }

    internal override void SetVerticalScrollBarEnabled(bool _enable)
    {
        JavaWebViewInterface.Call("SetVerticalScrollBarEnabled", _enable);
    }

    internal override void SetAllowContextMenu(bool _enable)
    {
        JavaWebViewInterface.Call("SetAllowContextMenu", _enable);
    }

    internal override void SetUserInteraction(bool _enable)
    {
        JavaWebViewInterface.Call("SetUserInteraction", _enable);
    }

    internal override void SetSupportMultipleWindows(bool _enable)
    {
        JavaWebViewInterface.Call("SetSupportMultipleWindows", _enable);
    }

    internal static void SetJavaScriptCanOpenWindowsAutomatically(bool _enable)
    {
        AndroidJavaObject TempInterface = new AndroidJavaObject("com.antwell.wellwebview.WellWebViewInterface");
        TempInterface.CallStatic("SetJavaScriptCanOpenWindowsAutomatically", _enable);
    }

    internal override bool CanGoBack()
    {
        return JavaWebViewInterface.Call<bool>("CanGoBack");
    }

    internal override bool CanGoForward()
    {
        return JavaWebViewInterface.Call<bool>("CanGoForward");
    }

    internal override void GoBack()
    {
        JavaWebViewInterface.Call("GoBack");
    }

    internal override void GoForward()
    {
        JavaWebViewInterface.Call("GoForward");
    }

    internal override void UpdateCookie()
    {
        JavaWebViewInterface.Call("UpdateCookie");
    }

    internal override void ClearCookies()
    {
        JavaWebViewInterface.Call("ClearCookies");
    }

    internal override void AddCookie(string Domain, string Cookie)
    {
        JavaWebViewInterface.Call("AddCookie", Domain,Cookie);
    }

    internal override string GetCookie(string Domain,string Key)
    {
        return JavaWebViewInterface.Call<string>("GetCookie", Domain,Key);
    }

    internal override string GetUrl()
    {
        return JavaWebViewInterface.Call<string>("GetUrl");
    }

    internal override void Destroy()
    {
        JavaWebViewInterface.Call("Destroy");
        WebView = null;
        JavaWebViewInterface = null;
    }

    internal override void StopLoading()
    {
        JavaWebViewInterface.Call("StopLoading");
    }

    internal override void Reload()
    {
        JavaWebViewInterface.Call("Reload");
    }

    internal override void ShowSpinner(bool enable)
    {
        JavaWebViewInterface.Call("ShowSpinner", enable);
    }

    internal override void SetSpinnerText(string text)
    {
        JavaWebViewInterface.Call("SetSpinnerText", text);
    }

    internal override int GetNativeWidth()
    {
        return JavaWebViewInterface.Call<int>("GetNativeWidth");
    }

    internal override int GetNativeHeight()
    {
        return JavaWebViewInterface.Call<int>("GetNativeHeight");
    }

    internal override void AddHTTPHeader(string key, string value)
    {
        JavaWebViewInterface.Call("AddHTTPHeader", key, value);
    }

    internal override void RemoveHTTPHeader(string key)
    {
        JavaWebViewInterface.Call("RemoveHTTPHeader", key);
    }

    internal override void SetOpeningAllUrlSchemes(bool Opening)
    {
        
    }
}
#endif