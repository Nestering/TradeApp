#if UNITY_IOS && !UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

public class WellWebViewIOSListener
{
    [DllImport("__Internal")]
    private static extern void RegisterFinishInitedHandler(MonoPFinishInitedDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterCallbackActionHandler(MonoPCallbackActionDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterOnPageStartedHandler(MonoPOnPageStartedDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterOnPageFinishedHandler(MonoPOnPageFinishedDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterOnPageProgressLoadHandler(MonoPOnPageProgressLoadDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterGetHTMLCallbackHandler(MonoPGetHTMLCallbackDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterOnMessageReceivedHandler(MonoPOnMessageReceivedDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterDoneButtonClickHandler(MonoPDoneButtonClickDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterErrorHandler(MonoPErrorDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterOpenMultipleWindowHandler(MonoPOpenMultipleWindowDelegate callback);

    [DllImport("__Internal")]
    private static extern void RegisterCloseMultipleWindowHandler(MonoPCloseMultipleWindowDelegate callback);

    public static Dictionary<string, WellWebView> webviews = new Dictionary<string, WellWebView>();
    public static bool registered = false;

    public delegate void MonoPFinishInitedDelegate(string ID);

    public delegate void MonoPCallbackActionDelegate(string ID, string IDCallback, string ReceivedResult,
        string ResultCode);

    public delegate void MonoPOnPageStartedDelegate(string ID, string url);

    public delegate void MonoPOnPageFinishedDelegate(string ID, string url);

    public delegate void MonoPOnPageProgressLoadDelegate(string ID, float progress);

    public delegate void MonoPGetHTMLCallbackDelegate(string ID, string IdCallback, string html, bool unescape);

    public delegate void MonoPOnMessageReceivedDelegate(string ID, string RawMessage, string Scheme, string host, string dictionary);

    public delegate void MonoPDoneButtonClickDelegate(string ID);

    public delegate void MonoPErrorDelegate(string ID, string Url, int ErrorCode);

    public delegate void MonoPOpenMultipleWindowDelegate(string ID);

    public delegate void MonoPCloseMultipleWindowDelegate(string ID);

    [AOT.MonoPInvokeCallback(typeof(MonoPFinishInitedDelegate))]
    private static void OnFinishedInit(string ID)
    {
        webviews[ID].InternalOnFinishedInit();
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPCallbackActionDelegate))]
    private static void OnCallbackActionFinished(string ID, string IDCallback, string ReceivedResult, string resultCode)
    {
        if (!string.IsNullOrEmpty(IDCallback))
            webviews[ID].OnCallbackActionFinished(IDCallback, ReceivedResult, resultCode);
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPOnPageStartedDelegate))]
    private static void OnPageStarted(string ID, string url)
    {
        webviews[ID].InternalOnPageStarted(webviews[ID], url);
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPOnPageFinishedDelegate))]
    private static void OnPageFinished(string ID, string url)
    {
        webviews[ID].InternalOnPageFinished(webviews[ID], url);
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPOnPageProgressLoadDelegate))]
    private static void OnPageProgressLoad(string ID, float _progress)
    {
        webviews[ID].InternalOnPageProgressLoad((int)_progress);
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPGetHTMLCallbackDelegate))]
    private static void OnGetHTMLCallback(string ID, string IDCallback, string html, bool unescape)
    {
        if (unescape)
        {
            html = Regex.Replace(
                    html,
                    @"\\u(?<Value>[a-zA-Z0-9]{4})",
                    m =>
                    {
                        return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                    });
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            byte[] unicodeBytes = unicode.GetBytes(html);
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            html = new string(asciiChars);
        }

        if (!string.IsNullOrEmpty(IDCallback))
            webviews[ID].OnCallbackActionFinished(IDCallback, html, "0");
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPOnMessageReceivedDelegate))]
    private static void onMessageReceived(string ID, string RawMessage, string Scheme, string host, string dictionary)
    {
        webviews[ID].InternalOnReceivedMessage(
            new WellReceivedData(RawMessage, Scheme, host, dictionary));
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPDoneButtonClickDelegate))]
    private static void onDoneButtonClicked(string ID)
    {
        webviews[ID].InternalOnDoneButtonClicked();
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPErrorDelegate))]
    private static void onError(string ID, string Url, int ErrorCode)
    {
        webviews[ID].InternalOnReceivedError(Url, ErrorCode);
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPOpenMultipleWindowDelegate))]
    private static void onOpenMultipleWindow(string ID)
    {
        webviews[ID].InternalOnMultipleWindowOpened();
    }

    [AOT.MonoPInvokeCallback(typeof(MonoPCloseMultipleWindowDelegate))]
    private static void onCloseMultipleWindow(string ID)
    {
        webviews[ID].InternalOnMultipleWindowClosed();
    }

    public static void RegisterCallbacks()
    {
        RegisterFinishInitedHandler(OnFinishedInit);
        RegisterCallbackActionHandler(OnCallbackActionFinished);
        RegisterOnPageStartedHandler(OnPageStarted);
        RegisterOnPageFinishedHandler(OnPageFinished);
        RegisterOnPageProgressLoadHandler(OnPageProgressLoad);
        RegisterGetHTMLCallbackHandler(OnGetHTMLCallback);
        RegisterOnMessageReceivedHandler(onMessageReceived);
        RegisterDoneButtonClickHandler(onDoneButtonClicked);
        RegisterErrorHandler(onError);
        RegisterOpenMultipleWindowHandler(onOpenMultipleWindow);
        RegisterCloseMultipleWindowHandler(onCloseMultipleWindow);
    }
}

#endif