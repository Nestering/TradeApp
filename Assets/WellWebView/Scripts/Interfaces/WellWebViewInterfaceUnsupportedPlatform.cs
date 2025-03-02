#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX

using UnityEngine;

public class WellWebViewInterface : AbstractWellWebViewInterface
{
    internal static void SetAllowAutoPlay(bool allow)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal static void SetAllowJavaScript(bool EnableJavaScript)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal static void SetJavaScriptCanOpenWindowsAutomatically(bool Enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void AddCookie(string Domain, string Cookie)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void AddDomainInSslException(string domain)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void AddHTTPHeader(string key, string value)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void AddJavaScript(string IdCallback, string jsCode)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void AddUrlCustomScheme(string scheme)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void AllowShowSslDialog(bool _enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override bool CanGoBack()
    {
        Debug.LogError("WellWebView::current platform no supported!");
        return false;
    }

    internal override bool CanGoForward()
    {
        Debug.LogError("WellWebView::current platform no supported!");
        return false;
    }

    internal override void ClearCookies()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void ForceDestroyMultipleWindow()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void Destroy()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void EvaluateJavaScript(string IdCallback, string jsCode)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override string GetCookie(string Domain, string Key)
    {
        Debug.LogError("WellWebView::current platform no supported!");
        return "";
    }

    internal override void GetHTMLCode(bool unescape, string IdCallback)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override int GetMaxScrolledPage()
    {
        Debug.LogError("WellWebView::current platform no supported!");
        return -1;
    }

    internal override int GetNativeHeight()
    {
        Debug.LogError("WellWebView::current platform no supported!");
        return -1;
    }

    internal override int GetNativeWidth()
    {
        Debug.LogError("WellWebView::current platform no supported!");
        return -1;
    }

    internal override string GetUrl()
    {
        Debug.LogError("WellWebView::current platform no supported!");
        return "";
    }

    internal override string GetUserAgent()
    {
        Debug.LogError("WellWebView::current platform no supported!");
        return "";
    }

    internal override void GoBack()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void GoForward()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void Hide(bool fade, float duration, WellWebViewAnimation.AnimationType animation, string IdCallback)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void HideProgressBar()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void Init(bool allowSwipeToRefresh)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void LoadHTMLString(string html, string BaseUrl)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void LoadUrl(string url)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void Print()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void Reload()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void RemoveDomainInSslException(string domain)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void RemoveHTTPHeader(string key)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetOpeningAllUrlSchemes(bool Opening)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void RemoveUrlCustomScheme(string scheme)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void ScrollTo(int x, int y, bool animate)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetAllowContextMenu(bool enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetAlpha(float _alpha)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetHorizontalScrollBarEnabled(bool enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetSize(int x, int y, int width, int height)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetSpinnerText(string text)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetSupportMultipleWindows(bool enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetUserAgent(string userAgent)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetUserInteraction(bool enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetVerticalScrollBarEnabled(bool enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void SetZoomEnabled(bool enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void Show(bool fade, float duration, WellWebViewAnimation.AnimationType animation, string IdCallback)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void ShowProgressBar(string HexColor)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void ShowSpinner(bool enable)
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void StopLoading()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }

    internal override void UpdateCookie()
    {
        Debug.LogError("WellWebView::current platform no supported!");
    }
}

#endif