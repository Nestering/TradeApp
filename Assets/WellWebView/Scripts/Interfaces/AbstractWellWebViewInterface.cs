using UnityEngine;

public abstract class AbstractWellWebViewInterface : MonoBehaviour
{
    internal abstract void Show(bool fade, float duration, WellWebViewAnimation.AnimationType animation,string IdCallback);

    internal abstract void ScrollTo(int x, int y, bool animate);

    internal abstract int GetMaxScrolledPage();

    internal abstract void Hide(bool fade, float duration, WellWebViewAnimation.AnimationType animation, string IdCallback);

    internal abstract void Init(bool allowSwipeToRefresh);

    internal abstract void ForceDestroyMultipleWindow();

    internal abstract void Destroy();

    internal abstract void LoadUrl(string url);

    internal abstract void LoadHTMLString(string html,string BaseUrl);

    internal abstract void StopLoading();

    internal abstract void Reload();

    internal abstract string GetUrl();

    internal abstract void SetSize(int x, int y, int width, int height);

    internal abstract void EvaluateJavaScript(string IdCallback, string jsCode);

    internal abstract void AddJavaScript(string IdCallback, string jsCode);

    internal abstract void GetHTMLCode(bool unescape,string IdCallback);

    internal abstract void AddUrlCustomScheme(string scheme);

    internal abstract void RemoveUrlCustomScheme(string scheme);

    internal abstract void AddDomainInSslException(string domain);

    internal abstract void RemoveDomainInSslException(string domain);

    internal abstract void AllowShowSslDialog(bool _enable);

    internal abstract void SetUserAgent(string userAgent);

    internal abstract string GetUserAgent();

    internal abstract void SetAlpha(float _alpha);

    internal abstract void Print();

    internal abstract void ShowProgressBar(string HexColor);

    internal abstract void HideProgressBar();

    internal abstract void ShowSpinner(bool enable);

    internal abstract void SetSpinnerText(string text);

    //internal abstract void SetAllowAutoPlay(bool autoPlay);

    //internal abstract void SetAllowJavaScript(bool enable);

    internal abstract void SetZoomEnabled(bool enable);

    internal abstract void SetHorizontalScrollBarEnabled(bool enable);

    internal abstract void SetVerticalScrollBarEnabled(bool enable);

    internal abstract void SetAllowContextMenu(bool enable);

    internal abstract void SetUserInteraction(bool enable);

    internal abstract void SetSupportMultipleWindows(bool enable);

    //internal abstract void SetJavaScriptCanOpenWindowsAutomatically(bool enable);

    internal abstract bool CanGoBack();

    internal abstract bool CanGoForward();

    internal abstract void GoBack();

    internal abstract void GoForward();

    internal abstract void UpdateCookie();

    internal abstract void ClearCookies();

    internal abstract void AddCookie(string Domain, string Cookie);

    internal abstract string GetCookie(string Domain,string Key);

    internal abstract int GetNativeWidth();

    internal abstract int GetNativeHeight();

    internal abstract void AddHTTPHeader(string key, string value);
    internal abstract void RemoveHTTPHeader(string key);
    internal abstract void SetOpeningAllUrlSchemes(bool Opening);

}

public class ResultCallback
{
    public string Result;

    public enum ResultState
    {
        Completed = 0,
        Failed = 1
    }

    public ResultState resultState;

    public ResultCallback(string _result, string _resultCode)
    {
        Result = _result;
        resultState = (ResultState)int.Parse(_resultCode);
    }
}