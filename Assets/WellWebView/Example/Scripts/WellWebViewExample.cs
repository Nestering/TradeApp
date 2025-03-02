using UnityEngine;

public class WellWebViewExample : MonoBehaviour
{
    public enum ToolBarType
    {
        Top,
        Bottom,
        None
    }

    public string Url;
    public bool ShowOnStart;
    public bool FullScreen;
    public bool SwipeRefresh;
    public ToolBarType ToolBar;
    public RectTransform TargetRect;
    public RectInt VisibleFrame;

    private WellWebView webview;

    private void Start()
    {
        WellWebView.SetAllowJavaScript(true);
        webview = gameObject.AddComponent<WellWebView>();
        webview.Init(SwipeRefresh);
        webview.OnFinishedInit += (webview) =>
        {
            webview.Android.SetUseWideViewPort(true);
            webview.Android.SetLoadWithOverviewMode(true);
            webview.IOS.AllowBackForwardNavigationGestures(true);
            webview.IOS.SetOpeningAllUrlSchemes(true);
            webview.SetSupportMultipleWindows(true);

            if (!FullScreen)
            {
                if (TargetRect != null)
                    webview.SetTargetRect(TargetRect);
                else if (!VisibleFrame.IsEmpty())
                    webview.SetSize(VisibleFrame.x, VisibleFrame.y, VisibleFrame.width, VisibleFrame.height);
            }
            switch (ToolBar)
            {
                case ToolBarType.Bottom:
                    webview.IOS.ShowToolBar(false);
                    break;

                case ToolBarType.Top:
                    webview.IOS.ShowToolBar(true);
                    break;

                case ToolBarType.None:
                    break;
            }

            if (string.IsNullOrEmpty(Url))
                Debug.LogWarning("WellWebView:: no set url!");
            else
                webview.LoadUrl(Url);

            if (ShowOnStart)
                webview.Show(true, 0, WellWebViewAnimation.AnimationType.None);
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (webview != null && webview.CanGoBack())
                webview.GoBack();
        }
    }
}