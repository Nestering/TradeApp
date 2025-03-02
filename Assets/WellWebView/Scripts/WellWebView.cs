using System;
using System.Collections.Generic;
using UnityEngine;
using WellPlatform;

public class WellWebView : MonoBehaviour
{
    public delegate void PageStartDelegate(WellWebView webview, string Url);

    public event PageStartDelegate OnPageStarted;

    public delegate void PageFinishedDelegate(WellWebView webview, string Url);

    public event PageFinishedDelegate OnPageFinished;

    public delegate void FinishedInitDelegate(WellWebView webview);

    public event FinishedInitDelegate OnFinishedInit;

    private event FinishedInitDelegate OnPrivateFinishedInit;

    public delegate void ReceivedDataDelegate(WellWebView webview, WellReceivedData message);

    public event ReceivedDataDelegate OnReceivedData;

    public delegate void OnProgressDelegate(WellWebView webview, int progress);

    public event OnProgressDelegate OnPageProgressLoad;

    public delegate void OnMultipleWindow(WellWebView webview);

    public event OnMultipleWindow OnMultipleWindowOpened;

    public event OnMultipleWindow OnMultipleWindowClosed;

    public delegate void OnErrorDelegate(WellWebView webview, string Url, int ErrorCode);

    public event OnErrorDelegate OnReceivedError;

    public delegate void OnCloseDelegate(WellWebView webview);

    public event OnCloseDelegate OnClose;

    public delegate void OnClickDoneDelegate(WellWebView webview);

    public event OnClickDoneDelegate OnDoneButtonClicked; //ONLY IOS

    //PUBLIC VARIABLES

    public string Identifier { get; private set; }
    public WellWebViewInterface listener { get; private set; }
    public Android Android { get; private set; }
    public IOS IOS { get; private set; }
    public bool PortraitOrientation { get; private set; } = false;

    public float Alpha
    {
        get { return alpha; }
        set
        {
            if (inited)
            {
                if (value >= 0f && value <= 1f)
                {
                    alpha = value;
                    listener.SetAlpha(value);
                }
                else
                    Debug.LogError("alpha can only be in the range between 0.0f and 1.0f. Alpha no change");
            }
            else
                Debug.LogError("Webview is not initialized");
        }
    }

    public bool FullScreen
    {
        get { return fullScreen; }
        set
        {
            if (inited)
            {
                if (fullScreen != value)
                {
                    fullScreen = value;
                    if (fullScreen)
                    {
#if UNITY_IOS
                        listener.ChangeOrientation(PortraitOrientation);
#endif
                        if (TargetRect != null)
                            SetTargetRect(null);
                        ViewRect = new RectInt(0, 0, GetNativeWidth(), GetNativeHeight());
                        listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height);
                    }
                }
            }
            else
                Debug.LogError("Webview is not initialized");
        }
    }

    //PRIVATE VARIABLES

    private float alpha = 1f;
    private bool fullScreen = false;
    private bool inited = false;
    private bool _focus = true;
    private RectInt ViewRect;
    private RectTransform TargetRect;

    private List<Action> Events = new List<Action>();

    private List<Action> _preEvents = new List<Action>();

    private Dictionary<string, Action<ResultCallback>> CallbackActions =
        new Dictionary<string, Action<ResultCallback>>();

    public static WellWebView CreateWebView(Transform parent = null)
    {
        GameObject webviewG = new GameObject("WellWebView");
        if (parent != null)
            webviewG.transform.SetParent(parent);
        return webviewG.AddComponent<WellWebView>();
    }

    public void SetTargetRect(RectTransform targetRect)
    {
        if (targetRect == null)
        {
            TargetRect = null;
            return;
        }
        else
        {
            TargetRect = targetRect;
            FullScreen = false;
            ViewRect = GetScreenCoordinates(TargetRect);
            if (IsInitialized())
            {
                listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height);
            }
            else
            {
                _preEvents.Add(() => listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height));
            }
            
        }
    }

    private void Update()
    {
        if (Events.Count > 0)
        {
            Events[0].Invoke();
            Events.RemoveAt(0);
        }

        if (!IsInitialized() || !_focus)
            return;

        ScreenOrientation orientation = Screen.orientation;

        bool newOrientation = orientation == ScreenOrientation.Portrait ||
                              orientation == ScreenOrientation.PortraitUpsideDown;

        if (newOrientation != PortraitOrientation)
        {
            PortraitOrientation = newOrientation;
#if UNITY_IOS
            listener.ChangeOrientation(PortraitOrientation);
#endif
            if (FullScreen)
            {
                int width = GetNativeWidth();
                int height = GetNativeHeight();
                ViewRect = new RectInt(0, 0, width, height);
                listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height);
            }
            else if (TargetRect != null)
            {
                RectInt newFrame = GetScreenCoordinates(TargetRect);
                if (!ViewRect.Equal(newFrame))
                {
                    ViewRect = newFrame;
                    listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height);
                }
            }
            else if (!ViewRect.IsEmpty())
            {
                listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height);
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        _focus = hasFocus;
        if (hasFocus && FullScreen)
        {
            int width = GetNativeWidth();
            int height = GetNativeHeight();
            ViewRect = new RectInt(0, 0, width, height);
            listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height);
        }
    }

    public void Init(bool SwipeToRefresh = false)
    {
#if (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        if (inited)
        {
            Debug.LogError("WellWebView::the current web view is already initialized!");
            return;
        }

        ScreenOrientation orientation = Screen.orientation;
        PortraitOrientation = orientation == ScreenOrientation.Portrait ||
                              orientation == ScreenOrientation.PortraitUpsideDown;

        _focus = true;
        listener = new WellWebViewInterface();
        listener.WebView = this;
        Android = new Android(this, ref _preEvents);
        IOS = new IOS(this, ref _preEvents);
        OnPrivateFinishedInit += FinishInited;
        Identifier = Guid.NewGuid().ToString();
        listener.Init(SwipeToRefresh);
        gameObject.name = "WellWebView::" + Identifier.ToString();
#else
        Debug.LogError("WellWebView::current platform no supported!");
#endif
    }

    private RectInt GetScreenCoordinates(RectTransform uiElement)
    {
        var worldCorners = new Vector3[4];
        uiElement.GetWorldCorners(worldCorners);
        Vector3 topLeft = worldCorners[1];
        Vector3 bottomRight = worldCorners[3];
        float widthFactor = (float)GetNativeWidth() / (float)Screen.width;
        float heightFactor = (float)GetNativeHeight() / (float)Screen.height;
        var canvas = uiElement.GetComponentInParent<Canvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera ||
            canvas.renderMode == RenderMode.WorldSpace)
        {
            Camera camera = canvas.worldCamera;
            if (camera == null)
            {
                Debug.Log("Need a camera to correctly calculate the position of the frame");
            }

            topLeft = camera.WorldToScreenPoint(worldCorners[1]);
            bottomRight = camera.WorldToScreenPoint(worldCorners[3]);
        }

        return new RectInt(
            (int)(topLeft.x * widthFactor),
            (int)((Screen.height - topLeft.y) * heightFactor),
            (int)((bottomRight.x - topLeft.x) * widthFactor),
            (int)((topLeft.y - bottomRight.y) * heightFactor));
    }

    private void FinishInited(WellWebView _webView)
    {
        OnPrivateFinishedInit -= FinishInited;
        inited = true;
        if (ViewRect.IsEmpty())
        {
            ViewRect = new RectInt(0, 0, Screen.width, Screen.height);
            listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height);
            FullScreen = true;
        }
        else
            listener.SetSize(ViewRect.x, ViewRect.y, ViewRect.width, ViewRect.height);

        foreach (Action preEvent in _preEvents)
        {
            preEvent?.Invoke();
        }

        _preEvents.Clear();
        Events.Add(new Action(() => OnFinishedInit?.Invoke(this)));
    }

    /// <summary>
    ///Loads a url in current web view.
    /// </summary>
    /// <param name="url">The url to be loaded.</param>
    internal void LoadUrl(string url)
    {
        if (inited)
            listener.LoadUrl(url);
        else
        {
            _preEvents.Add(() => listener.LoadUrl(url));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Loads a HTML in current web view.
    /// </summary>
    /// <param name="Html">html to be used as the page content</param>
    /// <param name="BaseUrl">The url to use as the page's base url</param>
    internal void LoadHTMLString(string Html, string BaseUrl)
    {
        if (inited)
            listener.LoadHTMLString(Html, BaseUrl);
        else
        {
            _preEvents.Add(() => listener.LoadHTMLString(Html, BaseUrl));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Get the correct url for the local site located in the StreamingAssets Folder.
    /// If the file is stored in the path "StreamingAssets/sites/site_1.html" then you only need to write "sites/site_1.html"
    /// </summary>
    /// <param name="path">path starting from the StreamingAssets folder</param>
    /// <returns></returns>
    public static string GetStreamingAssetURL(string path)
    {
#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        return System.IO.Path.Combine("file://" + Application.streamingAssetsPath, path);
#elif UNITY_ANDROID && !UNITY_EDITOR_WIN && !UNITY_EDITOR_LINUX
        return System.IO.Path.Combine("file:///android_asset/", path);
#else
        Debug.Log("WellWebView:: platform no supported!");
        return string.Empty;
#endif
    }

    /// <summary>
    /// Getting the screen width
    /// </summary>
    /// <returns></returns>
    internal int GetNativeWidth()
    {
        if (inited)
            return listener.GetNativeWidth();
        else
        {
            Debug.LogError("Webview is not initialized");
            return 0;
        }
    }

    /// <summary>
    /// Getting the screen height
    /// </summary>
    /// <returns></returns>
    internal int GetNativeHeight()
    {
        if (inited)
            return listener.GetNativeHeight();
        else
        {
            Debug.LogError("Webview is not initialized");
            return 0;
        }
    }

    /// <summary>
    /// Destroying the current web view
    /// </summary>
    internal void Destroy()
    {
        if (inited)
        {
            listener.Destroy();
            Destroy(gameObject);
        }
        else
            Debug.LogError("Webview is not initialized");
    }

    /// <summary>
    /// Stop loading the current page
    /// </summary>
    internal void StopLoading()
    {
        if (inited)
            listener.StopLoading();
        else
        {
            _preEvents.Add(() => listener.StopLoading());
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Reloading the current page
    /// </summary>
    internal void Reload()
    {
        if (inited)
            listener.Reload();
        else
        {
            _preEvents.Add(() => listener.Reload());
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Getting the current page in the current web view
    /// </summary>
    /// <returns></returns>
    internal string GetUrl()
    {
        if (inited)
            return listener.GetUrl();
        else
        {
            Debug.LogError("Webview is not initialized");
            return null;
        }
    }

    /// <summary>
    /// Sets visible the current web view.
    /// </summary>
    /// <param name="fade">Show with fade-in animation</param>
    /// <param name="duration">Animation duration</param>
    /// <param name="animation">Selecting the animation type</param>
    /// <param name="Callback">Callback at the end of the animation</param>
    internal void Show(bool fade = false, float duration = 0,
        WellWebViewAnimation.AnimationType animation = WellWebViewAnimation.AnimationType.None,
        Action<ResultCallback> Callback = null)
    {
        if (inited)
        {
            string ID = "";
            if (Callback != null)
            {
                ID = Guid.NewGuid().ToString();
                if (CallbackActions.ContainsKey(ID))
                    ID += UnityEngine.Random.Range(0, 65000);
                CallbackActions.Add(ID, Callback);
            }
            listener.Show(fade, duration, animation, ID);
        }
        else
        {
            _preEvents.Add(() => Show(fade,duration,animation,Callback));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Sets invisible the current web view.
    /// </summary>
    /// <param name="fade">Hide with fade-out animation</param>
    /// <param name="duration">Animation duration</param>
    /// <param name="animation">Selecting the animation type</param>
    /// <param name="Callback">Callback at the end of the animation</param>
    internal void Hide(bool fade = false, float duration = 0,
        WellWebViewAnimation.AnimationType animation = WellWebViewAnimation.AnimationType.None,
        Action<ResultCallback> Callback = null)
    {
        if (inited)
        {
            string ID = "";
            if (Callback != null)
            {
                ID = Guid.NewGuid().ToString();
                if (CallbackActions.ContainsKey(ID))
                    ID += UnityEngine.Random.Range(0, 65000);
                CallbackActions.Add(ID, Callback);
            }

            listener.Hide(fade, duration, animation, ID);
        }
        else
        {
            _preEvents.Add(() => Hide(fade,duration,animation,Callback));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Evaluate JavaScript in current page.
    /// Important: JavaScript must be enabled, otherwise there will be no callback
    /// </summary>
    /// <param name="jsCode">JavaScript string</param>
    /// <param name="Callback">Callback after EvaluateJavaScript</param>
    internal void EvaluateJavaScript(string jsCode, Action<ResultCallback> Callback = null)
    {
        if (inited)
        {
            string ID = "";
            if (Callback != null)
            {
                ID = Guid.NewGuid().ToString();
                if (CallbackActions.ContainsKey(ID))
                    ID += UnityEngine.Random.Range(0, 65000);
                CallbackActions.Add(ID, Callback);
            }

            listener.EvaluateJavaScript(ID, jsCode);
        }
        else
        {
            _preEvents.Add(() => EvaluateJavaScript(jsCode,Callback));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Adds JavaScript in current page.
    /// Important: JavaScript must be enabled, otherwise there will be no callback
    /// </summary>
    /// <param name="jsCode">JavaScript string</param>
    /// <param name="Callback">Callback after AddJavaScript</param>
    internal void AddJavaScript(string jsCode, Action<ResultCallback> Callback = null)
    {
        if (inited)
        {
            string ID = "";
            if (Callback != null)
            {
                ID = Guid.NewGuid().ToString();
                if (CallbackActions.ContainsKey(ID))
                    ID += UnityEngine.Random.Range(0, 65000);
                CallbackActions.Add(ID, Callback);
            }

            listener.AddJavaScript(ID, jsCode);
        }
        else
        {
            _preEvents.Add(() => AddJavaScript(jsCode,Callback));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Get the html code loaded in the current webview.
    /// Important: JavaScript must be enabled, otherwise there will be no callback
    /// </summary>
    /// <param name="unescape">Do I need to replace escape unicode characters with ascii or delete them if conversion is not possible (Example: "\u0442" is converted to "T")</param>
    /// <param name="Callback">Callback with the resulting html result</param>
    internal void GetHTMLCode(bool unescape, Action<ResultCallback> Callback)
    {
        if (inited)
        {
            string ID = Guid.NewGuid().ToString();
            if (CallbackActions.ContainsKey(ID))
                ID += UnityEngine.Random.Range(0, 65000);
            CallbackActions.Add(ID, Callback);
            listener.GetHTMLCode(unescape, ID);
        }
        else
            Debug.LogError("Webview is not initialized");
    }

    /// <summary>
    /// Sets the size and position of the current web view.
    /// </summary>
    /// <param name="x">Position by X</param>
    /// <param name="y">Position by Y</param>
    /// <param name="width">Width of the current web view</param>
    /// <param name="height">Height of the current web view</param>
    internal void SetSize(int x, int y, int width, int height)
    {
        if (inited)
        {
            ViewRect.x = x;
            ViewRect.y = y;
            ViewRect.width = width;
            ViewRect.height = height;
            FullScreen = false;
            TargetRect = null;
            listener.SetSize(x, y, width, height);
        }
        else
        {
            _preEvents.Add(() => SetSize(x,y,width,height));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Scroll to the specified coordinates in the current web view.
    /// </summary>
    /// <param name="x">Position by X</param>
    /// <param name="y">Position by Y</param>
    /// <param name="animate">Smooth scrolling to the specified coordinates</param>
    internal void ScrollTo(int x, int y, bool animate = false)
    {
        if (inited)
            listener.ScrollTo(x, y, animate);
        else
            Debug.LogError("Webview is not initialized");
    }

    /// <summary>
    /// Return the page height in the current web view.
    /// </summary>
    /// <returns></returns>
    internal int GetMaxScrolledPage()
    {
        if (inited)
            return listener.GetMaxScrolledPage();
        else
        {
            Debug.LogError("Webview is not initialized");
            return -1;
        }
    }

    /// <summary>
    /// Returns whether the web view is initialized.
    /// </summary>
    /// <returns></returns>
    internal bool IsInitialized()
    {
        return inited;
    }

    /// <summary>
    /// Adding a schema to manually process it in OnReceivedData.
    /// </summary>
    /// <param name="scheme">scheme without ://</param>
    internal void AddUrlCustomScheme(string scheme)
    {
        if (inited)
            listener.AddUrlCustomScheme(scheme);
        else
        {
            _preEvents.Add(() => AddUrlCustomScheme(scheme));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Remove the schema for manual processing.
    /// </summary>
    /// <param name="scheme">scheme without ://</param>
    internal void RemoveUrlCustomScheme(string scheme)
    {
        if (inited)
            listener.RemoveUrlCustomScheme(scheme);
        else
        {
            _preEvents.Add(() => RemoveUrlCustomScheme(scheme));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Sets the user agent in the current web view
    /// </summary>
    /// <param name="UserAgent"></param>
    internal void SetUserAgent(string UserAgent)
    {
        if (inited)
            listener.SetUserAgent(UserAgent);
        else
        {
            _preEvents.Add(() => SetUserAgent(UserAgent));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Return the user agent from the current web view.
    /// </summary>
    /// <returns></returns>
    internal string GetUserAgent()
    {
        if (inited)
            return listener.GetUserAgent();
        else
        {
            Debug.LogError("Webview is not initialized");
            return null;
        }
    }

    /// <summary>
    /// Print the loaded page in the current web view
    /// </summary>
    internal void Print()
    {
        if (inited)
            listener.Print();
        else
            Debug.LogError("Webview is not initialized");
    }

    /// <summary>
    /// Show the progress bar while loading pages
    /// </summary>
    /// <param name="HexColor">Progress fill color in Hex-color</param>
    internal void ShowProgressBar(string HexColor = "#87cefa")
    {
        if (inited)
            listener.ShowProgressBar(HexColor);
        else
        {
            _preEvents.Add(() => ShowProgressBar(HexColor));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Hide the progress bar
    /// </summary>
    internal void HideProgressBar()
    {
        if (inited)
            listener.HideProgressBar();
        else
        {
            _preEvents.Add(HideProgressBar);
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Allow autoplay media playback in the current web view
    /// (call before initializing the web view, otherwise the option will not work)
    /// </summary>
    /// <param name="AutoPlay"></param>
    internal static void SetAllowAutoPlay(bool AutoPlay)
    {
        WellWebViewInterface.SetAllowAutoPlay(AutoPlay);
    }

    /// <summary>
    /// Allow JavaScript in the web view
    /// (call before initializing the web view, otherwise the option will not work)
    /// </summary>
    /// <param name="EnableJavaScript"></param>
    internal static void SetAllowJavaScript(bool EnableJavaScript)
    {
        WellWebViewInterface.SetAllowJavaScript(EnableJavaScript);
    }

    /// <summary>
    /// Allow zoom content in the current web view
    /// </summary>
    /// <param name="EnableZoom"></param>
    internal void SetZoomEnabled(bool EnableZoom)
    {
        if (inited)
            listener.SetZoomEnabled(EnableZoom);
        else
        {
            _preEvents.Add(() => SetZoomEnabled(EnableZoom));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Enabling the horizontal scroll bar in the current web view
    /// </summary>
    /// <param name="Enable"></param>
    internal void SetHorizontalScrollBarEnabled(bool Enable)
    {
        if (inited)
            listener.SetHorizontalScrollBarEnabled(Enable);
        else
        {
            _preEvents.Add(() => SetHorizontalScrollBarEnabled(Enable));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Enabling the vertical scroll bar in the current web view
    /// </summary>
    /// <param name="Enable"></param>
    internal void SetVerticalScrollBarEnabled(bool Enable)
    {
        if (inited)
            listener.SetVerticalScrollBarEnabled(Enable);
        else
        {
            _preEvents.Add(() => SetVerticalScrollBarEnabled(Enable));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Allow the context menu in the current web view
    /// </summary>
    /// <param name="Enable"></param>
    internal void SetAllowContextMenu(bool Enable)
    {
        if (inited)
            listener.SetAllowContextMenu(Enable);
        else
        {
            _preEvents.Add(() => SetAllowContextMenu(Enable));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// User interaction with the current web view
    /// </summary>
    /// <param name="Interaction"></param>
    internal void SetUserInteraction(bool Interaction)
    {
        if (inited)
            listener.SetUserInteraction(Interaction);
        else
        {
            _preEvents.Add(() => SetUserInteraction(Interaction));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Enable support for multiple windows in the current web view
    /// </summary>
    /// <param name="support"></param>
    internal void SetSupportMultipleWindows(bool support)
    {
        if (inited)
            listener.SetSupportMultipleWindows(support);
        else
        {
            _preEvents.Add(() => SetSupportMultipleWindows(support));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Allow windows to open automatically
    /// (call before initializing the web view, otherwise the option will not work)
    /// </summary>
    /// <param name="Enable"></param>
    internal static void SetJavaScriptCanOpenWindowsAutomatically(bool Enable)
    {
        WellWebViewInterface.SetJavaScriptCanOpenWindowsAutomatically(Enable);
    }

    /// <summary>
    /// Returns whether it is possible to return to the previous page
    /// </summary>
    /// <returns></returns>
    internal bool CanGoBack()
    {
        if (inited)
            return listener.CanGoBack();
        else
        {
            Debug.LogError("Webview is not initialized");
            return false;
        }
    }

    internal void ForceDestroyMultipleWindow()
    {
        if (inited)
            listener.ForceDestroyMultipleWindow();
        else
        {
            _preEvents.Add(ForceDestroyMultipleWindow);
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Returns whether it is possible to return to the forward page
    /// </summary>
    /// <returns></returns>
    internal bool CanGoForward()
    {
        if (inited)
            return listener.CanGoForward();
        else
        {
            Debug.LogError("Webview is not initialized");
            return false;
        }
    }

    /// <summary>
    /// Go back to the previous page in the current web view
    /// </summary>
    internal void GoBack()
    {
        if (inited)
            listener.GoBack();
        else
            Debug.LogError("Webview is not initialized");
    }

    /// <summary>
    /// Go forward page in the current web view
    /// </summary>
    internal void GoForward()
    {
        if (inited)
            listener.GoForward();
        else
            Debug.LogError("Webview is not initialized");
    }

    /// <summary>
    /// allow to show the ssl error dialog box,
    /// to prohibit or allow page loading (Android only)
    /// </summary>
    /// <param name="Allow"></param>
    internal void AllowShowSslDialog(bool Allow) //ONLY ANDROID
    {
        if (inited)
            listener.AllowShowSslDialog(Allow);
        else
        {
            _preEvents.Add(() => AllowShowSslDialog(Allow));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Force call to save cookies
    /// </summary>
    internal void UpdateCookie()
    {
        if (inited)
            listener.UpdateCookie();
        else
        {
            _preEvents.Add(UpdateCookie);
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    ///  Clears all cookies from web view.
    /// </summary>
    internal void ClearCookies()
    {
        if (inited)
            listener.ClearCookies();
        else
        {
            _preEvents.Add(ClearCookies);
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Adding a cookie to the current web view
    /// </summary>
    /// <param name="url">The URL for which the cookie will be set.</param>
    /// <param name="Cookies">The cookie string to set</param>
    internal void AddCookie(string url, string Cookies)
    {
        if (inited)
            listener.AddCookie(url, Cookies);
        else
        {
            _preEvents.Add(() => AddCookie(url, Cookies));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Returns the value of the cookie under the URL and key.
    /// </summary>
    /// <param name="url">The url (domain) where the target cookie is.</param>
    /// <param name="Key">The key for target cookie value.</param>
    /// <returns></returns>
    internal string GetCookie(string url, string Key)
    {
        if (inited)
            return listener.GetCookie(url, Key);
        else
        {
            Debug.LogError("Webview is not initialized");
            return null;
        }
    }

    /// <summary>
    /// Show the loading spinner while loading pages
    /// </summary>
    /// <param name="Enable"></param>
    internal void ShowSpinner(bool Enable)
    {
        if (inited)
            listener.ShowSpinner(Enable);
        else
        {
            _preEvents.Add(() => ShowSpinner(Enable));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Add text to loading spinner
    /// </summary>
    /// <param name="text"></param>
    internal void SetSpinnerText(string text)
    {
        if (inited)
            listener.SetSpinnerText(text);
        else
        {
            _preEvents.Add(() => SetSpinnerText(text));
            // Debug.LogError("Webview is not initialized");
        }
    }

    /// <summary>
    /// Adding custom headers to queries in the current web view. Work ONLY Get requests
    /// </summary>
    /// <param name="key">Header key</param>
    /// <param name="value">Header value</param>
    internal void AddHTTPHeader(string key, string value)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
        {
            Debug.LogError("WellWebview:: AddHTTPHeader failed.key or value is missing");
        }
        else
        {
            if (inited)
                listener.AddHTTPHeader(key, value);
            else
            {
                _preEvents.Add(() => AddHTTPHeader(key,value));
                // Debug.LogError("Webview is not initialized");
            }
        }
    }

    /// <summary>
    /// Removing custom request headers in the current web view
    /// </summary>
    /// <param name="key">Header key</param>
    internal void RemoveHTTPHeader(string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            if (inited)
                listener.RemoveHTTPHeader(key);
            else
            {
                _preEvents.Add(() => RemoveHTTPHeader(key));
                // Debug.LogError("Webview is not initialized");
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="allow"></param>
    public static void AllowInlinePlay(bool allow) //only IOS
    {
#if UNITY_IOS
        WellWebViewInterface.AllowInlinePlay(allow);
#endif
    }

    #region EventZone

    internal void InternalOnPageStarted(WellWebView webview, string URL)
    {
        Events.Add(new Action(() => OnPageStarted?.Invoke(webview, URL)));
    }

    internal void InternalOnPageFinished(WellWebView webview, string URL)
    {
        Events.Add(new Action(() => OnPageFinished?.Invoke(webview, URL)));
    }

    internal void InternalOnReceivedMessage(WellReceivedData receivedData)
    {
        Events.Add(new Action(() => OnReceivedData?.Invoke(this, receivedData)));
    }

    internal void OnCallbackActionFinished(string ID, string receiveResult, string _resultCode)
    {
        Events.Add(new Action(() =>
            InvokeCallbackAction(ID, receiveResult, _resultCode)
        ));
    }

    internal void InternalOnFinishedInit()
    {
        Events.Add(new Action(() => OnPrivateFinishedInit?.Invoke(this)));
    }

    internal void InternalOnPageProgressLoad(int _progress)
    {
        Events.Add(new Action(() => OnPageProgressLoad?.Invoke(this, _progress)));
    }

    internal void InternalOnMultipleWindowOpened()
    {
        Events.Add(new Action(() => OnMultipleWindowOpened?.Invoke(this)));
    }

    internal void InternalOnMultipleWindowClosed()
    {
        Events.Add(new Action(() => OnMultipleWindowClosed?.Invoke(this)));
    }

    internal void InternalOnDoneButtonClicked()
    {
        Events.Add(new Action(() => OnDoneButtonClicked?.Invoke(this)));
    }

    internal void InternalOnClose()
    {
        Events.Add(new Action(() => OnClose?.Invoke(this)));
    }

    internal void InternalOnReceivedError(string Url, int ErrorCode)
    {
        Events.Add(new Action(() => OnReceivedError?.Invoke(this, Url, ErrorCode)));
    }

    protected void InvokeCallbackAction(string ID, string receiveResult, string _resultCode)
    {
        CallbackActions[ID].Invoke(new ResultCallback(receiveResult, _resultCode));
        CallbackActions.Remove(ID);
    }

    #endregion EventZone
}

namespace WellPlatform
{
    [Serializable]
    public class Android
    {
        private WellWebView webView;
        private List<Action> _preEvents;
        public Android(WellWebView webView,ref List<Action> preEvents)
        {
            this.webView = webView;
            _preEvents = preEvents;
        }

        /// <summary>
        /// Sets whether the WebView loads pages in the browse mode,
        /// that is, reduces the content scale to the screen width.
        /// </summary>
        /// <param name="overview"></param>
        public void SetLoadWithOverviewMode(bool overview)
        {
#if UNITY_ANDROID && !UNITY_EDITOR_OSX
            if (webView.IsInitialized())
                webView.listener.SetLoadWithOverviewMode(overview);
            else
            {
                _preEvents.Add(() => webView.listener.SetLoadWithOverviewMode(overview));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Sets whether the WebView should enable support for the "viewport" HTML meta tag or should use a wide viewport.
        /// </summary>
        /// <param name="use"></param>
        public void SetUseWideViewPort(bool use)
        {
#if UNITY_ANDROID && !UNITY_EDITOR_OSX
            if (webView.IsInitialized())
                webView.listener.SetUseWideView(use);
            else
            {
                _preEvents.Add(() => webView.listener.SetUseWideView(use));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Sets the default font size in the current web view
        /// </summary>
        /// <param name="fontSize">int: a non-negative integer between 1 and 72. Any number outside the specified range will be pinned.</param>
        public void SetDefaultFontSize(int fontSize)
        {
#if UNITY_ANDROID && !UNITY_EDITOR_OSX
            if (webView.IsInitialized())
                webView.listener.SetDefaultFontSize(fontSize);
            else
            {
                _preEvents.Add(() => webView.listener.SetDefaultFontSize(fontSize));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Adding a navigation bar (do not enable full-screen)
        /// </summary>
        /// <param name="show"></param>
        public void ShowNavigationBar(bool show)
        {
#if UNITY_ANDROID && !UNITY_EDITOR_OSX
            if (webView.IsInitialized())
                webView.listener.ShowNavigationBar(show);
            else
            {
                _preEvents.Add(() => webView.listener.ShowNavigationBar(show));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }
    }

    [Serializable]
    public class IOS
    {
        private WellWebView webView;
        private List<Action> _preEvents;

        public IOS(WellWebView webView, ref List<Action> preEvents)
        {
            this.webView = webView;
            _preEvents = preEvents;
        }

        /// <summary>
        /// Allow support horizontal swipe gestures trigger backward and forward page navigation.
        /// </summary>
        /// <param name="allow"></param>
        public void AllowBackForwardNavigationGestures(bool allow)
        {
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (webView.IsInitialized())
                webView.listener.AllowBackForwardNavigationGestures(allow);
            else
            {
                _preEvents.Add(() => webView.listener.AllowBackForwardNavigationGestures(allow));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Show the toolbar, with the back and forward buttons located on it, and a button with the ability to get a return response in Ondonebuttoncliked
        /// </summary>
        /// <param name="OnTop">Toolbar position</param>
        /// <param name="Animate">Show smooth</param>
        /// <param name="AdjustInset">Should the toolbar shift the web view?</param>
        public void ShowToolBar(bool OnTop = true, bool Animate = false, bool AdjustInset = false)
        {
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (webView.IsInitialized())
                webView.listener.ShowToolBar(OnTop, Animate, AdjustInset);
            else
            {
                _preEvents.Add(() => webView.listener.ShowToolBar(OnTop, Animate, AdjustInset));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Hide the toolbar
        /// </summary>
        /// <param name="Animate">Hide smooth</param>
        public void HideToolBar(bool Animate = false)
        {
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (webView.IsInitialized())
                webView.listener.HideToolBar(Animate);
            else
            {
                _preEvents.Add(() => webView.listener.HideToolBar(Animate));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Sets the text for the Back button in the toolbar
        /// </summary>
        /// <param name="Title"></param>
        public void SetBackButtonText(string Title)
        {
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (webView.IsInitialized())
                webView.listener.SetBackButtonText(Title);
            else
            {
                _preEvents.Add(() => webView.listener.SetBackButtonText(Title));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Sets the text for the Forward button in the toolbar
        /// </summary>
        /// <param name="Title"></param>
        public void SetForwardButtonText(string Title)
        {
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (webView.IsInitialized())
                webView.listener.SetForwardButtonText(Title);
            else
            {
                _preEvents.Add(() => webView.listener.SetForwardButtonText(Title));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Sets the text for the Done button in the toolbar
        /// </summary>
        /// <param name="Title"></param>
        public void SetDoneButtonText(string Title)
        {
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (webView.IsInitialized())
                webView.listener.SetDoneButtonText(Title);
            else
            {
                _preEvents.Add(() => webView.listener.SetDoneButtonText(Title));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Sets the size of the button text in the toolbar
        /// </summary>
        /// <param name="TextSize"></param>
        public void SetToolbarTextSize(int TextSize)
        {
#if UNITY_IOS
            if (webView.IsInitialized())
                webView.listener.SetToolbarTextSize(TextSize);
            else
            {
                _preEvents.Add(() => webView.listener.SetToolbarTextSize(TextSize));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Adds the MIME type to the download files checklist
        /// </summary>
        /// <param name="MimeType">MIME type</param>
        /// <param name="FileExtension">The default file extension for this type is MIME type (if it is not specified in the file download request itself)</param>
        public void AddDownloadMimeType(string MimeType, string FileExtension)
        {
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (webView.IsInitialized())
                webView.listener.AddMimeType(MimeType, FileExtension);
            else
            {
                _preEvents.Add(() => webView.listener.AddMimeType(MimeType, FileExtension));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Remove the MIME type in the download files checklist
        /// </summary>
        /// <param name="MimeType">MIME type</param>
        public void RemoveDownloadMimeType(string MimeType)
        {
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (webView.IsInitialized())
                webView.listener.RemoveMimeType(MimeType);
            else
            {
                _preEvents.Add(() => webView.listener.RemoveMimeType(MimeType));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Sets the toolbar color
        /// </summary>
        /// <param name="HexColor">Sets color in Hex-color(Example:"#87cefa")</param>
        public void SetToolbarTintColor(string HexColor, float alpha = 1f)
        {
#if UNITY_IOS
            if (webView.IsInitialized())
                webView.listener.SetToolbarTintColor(HexColor,alpha);
            else
            {
                _preEvents.Add(() => webView.listener.SetToolbarTintColor(HexColor,alpha));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Sets the toolbar text color
        /// </summary>
        /// <param name="HexColor">Sets color in Hex-color(Example:"#87cefa")</param>
        public void SetToolbarTextColor(string HexColor)
        {
#if UNITY_IOS
            if (webView.IsInitialized())
                webView.listener.SetToolbarTextColor(HexColor);
            else
            {
                _preEvents.Add(() => webView.listener.SetToolbarTextColor(HexColor));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Adds html code to the site to request and allow Device MotionEvent. Call when the site is loaded, after the OnPageFinished event
        /// </summary>
        public void AddMotionEventInCurrentSite()
        {
#if UNITY_IOS
            if (webView.IsInitialized())
            {
                webView.EvaluateJavaScript("DeviceMotionEvent.requestPermission().then(response => { if (response == 'granted') {window.addEventListener('devicemotion', (e) => {})}}).catch(console.error)");
            }
            else
            {
                _preEvents.Add(AddMotionEventInCurrentSite);
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }

        /// <summary>
        /// Allows you to check whether the scheme can be opened or not. <br/><br/>
        /// If false: To open external applications via schemas, you need to add them to the info.plist in LSApplicationQueriesSchemes.<br/><br/>
        /// If true: Then the external schema (if it is not added for manual processing in AddUrlCustomScheme) will try to open in an external application, and if this is not possible,
        /// toast will appear with a message that the application for this scheme is not installed.<br/>
        /// Default value: false
        /// </summary>
        /// <param name="opening">check whether it is possible to open the circuit or not.</param>
        public void SetOpeningAllUrlSchemes(bool opening)
        {
#if UNITY_IOS
            if (webView.IsInitialized())
                webView.listener.SetOpeningAllUrlSchemes(opening);
            else
            {
                _preEvents.Add(() => webView.listener.SetOpeningAllUrlSchemes(opening));
                // Debug.LogError("Webview is not initialized");
            }
#endif
        }
    }
}