#if UNITY_IOS && !UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WellWebViewInterface : AbstractWellWebViewInterface
{
    public WellWebView WebView;

    [DllImport("__Internal")]
    private static extern void ex_ChangeOrientation(string ID, bool PortraitOrientation);
    [DllImport("__Internal")]
    private static extern void ex_init(string ID);
    [DllImport("__Internal")]
    private static extern void ex_LoadUrl(string ID,string url);
    [DllImport("__Internal")]
    private static extern void ex_SetSize(string ID, int x, int y, int width, int height);
    [DllImport("__Internal")]
    private static extern void ex_Show(string ID, bool visible,bool fade,float duration, int animation,string IdCallback);
    [DllImport("__Internal")]
    private static extern string ex_getUserAgent(string ID);
    [DllImport("__Internal")]
    private static extern void ex_setUserAgent(string ID,string _newUserAgent);
    [DllImport("__Internal")]
    private static extern void ex_setAlpha(string ID, float _alpha);
    [DllImport("__Internal")]
    private static extern bool ex_canGoBack(string ID);
    [DllImport("__Internal")]
    private static extern void ex_GoBack(string ID);
    [DllImport("__Internal")]
    private static extern bool ex_canGoForward(string ID);
    [DllImport("__Internal")]
    private static extern void ex_GoForward(string ID);
    [DllImport("__Internal")]
    private static extern string ex_getUrl(string ID);
    [DllImport("__Internal")]
    private static extern void ex_reload(string ID);
    [DllImport("__Internal")]
    private static extern void ex_stopLoading(string ID);
    [DllImport("__Internal")]
    private static extern void ex_evaluateJavaScript(string ID,string JS, string IdCallback);
    [DllImport("__Internal")]
    private static extern void ex_addJavaScript(string ID, string JS, string IdCallback);
    [DllImport("__Internal")]
    private static extern void ex_scrollTo(string ID, int x, int y,bool animated);
    [DllImport("__Internal")]
    private static extern void ex_print(string ID);
    [DllImport("__Internal")]
    private static extern int ex_getPageHeight(string ID);
    [DllImport("__Internal")]
    private static extern void ex_AllowJavascript(bool allow);
    [DllImport("__Internal")]
    private static extern void ex_JavaScriptCanOpenWindowsAutomatically(bool allow);
    [DllImport("__Internal")]
    private static extern void ex_SetAllowAutoPlay(bool allow);
    [DllImport("__Internal")]
    private static extern string ex_getCookie(string ID,string domain,string key);
    [DllImport("__Internal")]
    private static extern void ex_setCookie(string ID, string Cookie);
    [DllImport("__Internal")]
    private static extern void ex_ClearCookie(string ID);
    [DllImport("__Internal")]
    private static extern void ex_allowBackForwardNavigationGestures(string ID, bool allow);
    [DllImport("__Internal")]
    private static extern void ex_ShowToolBar(string ID,bool OnTop,bool Animate, bool AdjustInset);
    [DllImport("__Internal")]
    private static extern void ex_HideToolBar(string ID,bool Animate);
    [DllImport("__Internal")]
    private static extern void ex_ShowProgressBar(string ID, string hexColor);
    [DllImport("__Internal")]
    private static extern void ex_HideProgressBar(string ID);
    [DllImport("__Internal")]
    private static extern void ex_getHtmlCode(string ID,bool unescape ,string IdCallback);
    [DllImport("__Internal")]
    private static extern void ex_AddUrlCustomScheme(string ID, string scheme);
    [DllImport("__Internal")]
    private static extern void ex_RemoveUrlCustomScheme(string ID, string scheme);
    [DllImport("__Internal")]
    private static extern void ex_SetBackButtonText(string ID, string _Title);
    [DllImport("__Internal")]
    private static extern void ex_SetForwardButtonText(string ID, string _Title);
    [DllImport("__Internal")]
    private static extern void ex_SetDoneButtonText(string ID, string _Title);
    [DllImport("__Internal")]
    private static extern void ex_SetToolbarTextSize(string ID, int _sizeText);
    [DllImport("__Internal")]
    private static extern void ex_SetUserInteraction(string ID, bool _Interaction);
    [DllImport("__Internal")]
    private static extern void ex_LoadHTMLString(string ID, string _HTMLCode,string BaseUrl);
    [DllImport("__Internal")]
    private static extern void ex_SetZoom(string WbId, bool Enable);
    [DllImport("__Internal")]
    private static extern void ex_SetHorizontalScrollBarEnabled(string WbId, bool Enable);
    [DllImport("__Internal")]
    private static extern void ex_SetVerticalScrollBarEnabled(string WbId, bool Enable);
    [DllImport("__Internal")]
    private static extern void ex_Destroy(string ID);
    [DllImport("__Internal")]
    private static extern void ex_ShowSpinner(string ID, bool Enable);
    [DllImport("__Internal")]
    private static extern void ex_SetSpinnerText(string ID, string Title);
    [DllImport("__Internal")]
    private static extern void ex_SetContextMenu(string ID, bool Enable);
    [DllImport("__Internal")]
    private static extern int ex_getNativeWidth(bool PortraitOrientation);
    [DllImport("__Internal")]
    private static extern int ex_getNativeHeight(bool PortraitOrientation);
    [DllImport("__Internal")]
    private static extern void ex_AddHTTPHeader(string ID, string Key,string Value);
    [DllImport("__Internal")]
    private static extern void ex_RemoveHTTPHeader(string ID, string Key);
    [DllImport("__Internal")]
    private static extern void ex_AddMimeType(string ID, string MimeType, string FileExtension);
    [DllImport("__Internal")]
    private static extern void ex_RemoveMimeType(string ID, string MimeType);
    [DllImport("__Internal")]
    private static extern void ex_AllowInlinePlay(bool Enable);
    [DllImport("__Internal")]
    private static extern void ex_SetToolbarTintColor(string ID, string hexColor,float alpha);
    [DllImport("__Internal")]
    private static extern void ex_SetToolbarTextColor(string ID, string hexColor);
    [DllImport("__Internal")]
    private static extern void ex_SetSupportMultipleWindow(string ID, bool Support);
    [DllImport("__Internal")]
    private static extern void ex_ForceDestroyMultipleWindow(string ID);
    [DllImport("__Internal")]
    private static extern void ex_SetOpeningAllUrlSchemes(string ID, bool Opening);

    internal void ChangeOrientation(bool PortraitOrientation)
    {
        ex_ChangeOrientation(WebView.Identifier, PortraitOrientation);
    }

    internal override void Init(bool allowSwipeToRefresh)
    {
        Debug.Log("WellWebView inited identifier: " + WebView.Identifier);
        WellWebViewIOSListener.webviews.Add(WebView.Identifier, WebView);
        if (!WellWebViewIOSListener.registered)
        {
            WellWebViewIOSListener.RegisterCallbacks();
            WellWebViewIOSListener.registered = true;

        }
        ex_init(WebView.Identifier);
    }

    internal override void ForceDestroyMultipleWindow()
    {
        ex_ForceDestroyMultipleWindow(WebView.Identifier);
    }

    internal override void LoadUrl(string url)
    {
        ex_LoadUrl(WebView.Identifier,url);
    }
    
    internal override void Show(bool fade, float duration, WellWebViewAnimation.AnimationType animation, string IdCallback)
    {
        ex_Show(WebView.Identifier, true,fade,duration,(int)animation,IdCallback);
    }

    internal override void Hide(bool fade, float duration, WellWebViewAnimation.AnimationType animation, string IdCallback)
    {
        ex_Show(WebView.Identifier, false, fade, duration, (int)animation,IdCallback);
    }

    internal override void SetSize(int x, int y, int width, int height)
    {
        ex_SetSize(WebView.Identifier, x, y, width, height);
    }

    internal override void AddCookie(string Domain, string Cookie)
    {
        ex_setCookie(WebView.Identifier, Cookie);
    }

    internal void AllowBackForwardNavigationGestures(bool allow)
    {
        ex_allowBackForwardNavigationGestures(WebView.Identifier, allow);
    }

    internal void ShowToolBar(bool OnTop, bool Animate, bool AdjustInset)
    {
        ex_ShowToolBar(WebView.Identifier,OnTop,Animate,AdjustInset);
    }

    internal void HideToolBar(bool Animate)
    {
        ex_HideToolBar(WebView.Identifier,Animate);
    }

    internal override void AddDomainInSslException(string domain)
    {
        throw new System.NotImplementedException();
    }

    internal override void AddJavaScript(string IdCallback, string jsCode)
    {
        ex_addJavaScript(WebView.Identifier, jsCode, IdCallback);
    }

    internal override void AddUrlCustomScheme(string scheme)
    {
        ex_AddUrlCustomScheme(WebView.Identifier, scheme);
    }

    internal override void AllowShowSslDialog(bool _enable)
    {
        throw new System.NotImplementedException();
    }

    internal override bool CanGoBack()
    {
        return ex_canGoBack(WebView.Identifier);
    }

    internal override bool CanGoForward()
    {
        return ex_canGoForward(WebView.Identifier);
    }

    internal override void ClearCookies()
    {
        ex_ClearCookie(WebView.Identifier);
    }

    internal override void Destroy()
    {
        ex_Destroy(WebView.Identifier);
        WellWebViewIOSListener.webviews.Remove(WebView.Identifier);
    }

    internal override void EvaluateJavaScript(string IdCallback, string jsCode)
    {
        ex_evaluateJavaScript(WebView.Identifier, jsCode,IdCallback);
    }

    internal override string GetCookie(string Domain,string Key)
    {
        return ex_getCookie(WebView.Identifier, Domain,Key);
    }

    internal override void GetHTMLCode(bool unescape,string IdCallback)
    {
        ex_getHtmlCode(WebView.Identifier, unescape, IdCallback);
    }

    internal override int GetMaxScrolledPage()
    {
        return ex_getPageHeight(WebView.Identifier);
    }

    internal override string GetUrl()
    {
        return ex_getUrl(WebView.Identifier);
    }

    internal override string GetUserAgent()
    {
        return ex_getUserAgent(WebView.Identifier);
    }

    internal override void GoBack()
    {
        ex_GoBack(WebView.Identifier);
    }

    internal override void GoForward()
    {
        ex_GoForward(WebView.Identifier);
    }

    internal override void HideProgressBar()
    {
        ex_HideProgressBar(WebView.Identifier);
    }

    internal override void Print()
    {
        ex_print(WebView.Identifier);
    }

    internal override void Reload()
    {
        ex_reload(WebView.Identifier);
    }

    internal override void RemoveDomainInSslException(string domain)
    {
        
    }
    
    internal override void RemoveUrlCustomScheme(string scheme)
    {
        ex_RemoveUrlCustomScheme(WebView.Identifier, scheme);
    }

    internal override void ScrollTo(int x, int y, bool animate)
    {
        ex_scrollTo(WebView.Identifier, x, y, animate);
    }

    internal static void SetAllowAutoPlay(bool autoPlay)
    {
        ex_SetAllowAutoPlay(autoPlay);
    }

    internal override void SetAllowContextMenu(bool enable)
    {
        ex_SetContextMenu(WebView.Identifier, enable);
    }

    internal static void SetAllowJavaScript(bool enable)
    {
        ex_AllowJavascript(enable);
    }

    internal override void SetAlpha(float _alpha)
    {
        ex_setAlpha(WebView.Identifier, _alpha);
    }

    internal override void SetHorizontalScrollBarEnabled(bool enable)
    {
        ex_SetHorizontalScrollBarEnabled(WebView.Identifier, enable);
    }

    internal static void SetJavaScriptCanOpenWindowsAutomatically(bool enable)
    {
        ex_JavaScriptCanOpenWindowsAutomatically(enable);
    }

    internal override void SetSupportMultipleWindows(bool enable)
    {
        ex_SetSupportMultipleWindow(WebView.Identifier, enable);
    }

    internal override void SetUserAgent(string userAgent)
    {
        ex_setUserAgent(WebView.Identifier, userAgent);
    }

    internal override void SetUserInteraction(bool enable)
    {
        ex_SetUserInteraction(WebView.Identifier, enable);
    }

    internal override void SetVerticalScrollBarEnabled(bool enable)
    {
        ex_SetVerticalScrollBarEnabled(WebView.Identifier, enable);
    }

    internal override void SetZoomEnabled(bool enable)
    {
        ex_SetZoom(WebView.Identifier, enable);
    }

    internal override void ShowProgressBar(string HexColor)
    {
        ex_ShowProgressBar(WebView.Identifier, HexColor);
    }

    internal override void StopLoading()
    {
        ex_stopLoading(WebView.Identifier);
    }

    internal override void UpdateCookie()
    {
        
    }

    internal void SetBackButtonText(string Text)
    {
        ex_SetBackButtonText(WebView.Identifier, Text);
    }

    internal void SetForwardButtonText(string Text)
    {
        ex_SetForwardButtonText(WebView.Identifier, Text);
    }

    internal void SetDoneButtonText(string Text)
    {
        ex_SetDoneButtonText(WebView.Identifier, Text);
    }

    internal void SetToolbarTextSize(int _TextSize)
    {
        ex_SetToolbarTextSize(WebView.Identifier, _TextSize);
    }

    internal override void LoadHTMLString(string html, string BaseUrl)
    {
        ex_LoadHTMLString(WebView.Identifier, html, BaseUrl);
    }

    internal override void ShowSpinner(bool enable)
    {
        ex_ShowSpinner(WebView.Identifier, enable);
    }

    internal override void SetSpinnerText(string text)
    {
        ex_SetSpinnerText(WebView.Identifier, text);
    }

    internal override int GetNativeWidth()
    {
        return ex_getNativeWidth(WebView.PortraitOrientation);
    }

    internal override int GetNativeHeight()
    {
        return ex_getNativeHeight(WebView.PortraitOrientation);
    }

    internal override void AddHTTPHeader(string key, string value)
    {
        ex_AddHTTPHeader(WebView.Identifier, key, value);
    }

    internal override void RemoveHTTPHeader(string key)
    {
        ex_RemoveHTTPHeader(WebView.Identifier, key);
    }

    internal override void SetOpeningAllUrlSchemes(bool Opening)
    {
        ex_SetOpeningAllUrlSchemes(WebView.Identifier, Opening);
    }

    internal void AddMimeType(string MimeType,string FileExtension)
    {
        ex_AddMimeType(WebView.Identifier, MimeType, FileExtension);
    }

    internal void RemoveMimeType(string MimeType)
    {
        ex_RemoveMimeType(WebView.Identifier, MimeType);
    }

    internal void SetToolbarTintColor(string hexColor, float alpha)
    {
        ex_SetToolbarTintColor(WebView.Identifier, hexColor,alpha);
    }

    internal void SetToolbarTextColor(string hexColor)
    {
        ex_SetToolbarTextColor(WebView.Identifier, hexColor);
    }

    internal static void AllowInlinePlay(bool Enable)
    {
        ex_AllowInlinePlay(Enable);
    }
}
#endif
