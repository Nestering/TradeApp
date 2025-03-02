#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using UnityEngine;

public class WellWebViewAndroidListener : AndroidJavaProxy
{
    public WellWebViewAndroidListener() : base("com.antwell.wellwebview.UnityCallbackListener")
    {
    }
    public WellWebView webview;

    public void OnCallbackActionFinished(string ID,string ReceivedResult,string resultCode)
    {
        if(!string.IsNullOrEmpty(ID))
            webview.OnCallbackActionFinished(ID, ReceivedResult,resultCode);
    }

    public void OnGetHtmlCallback(bool unescape, string ID, string Result)
    {
        if (unescape)
        {
                Result = Regex.Replace(
                    Result,
                    @"\\u(?<Value>[a-zA-Z0-9]{4})",
                    m => {
                        return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                    });
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            byte[] unicodeBytes = unicode.GetBytes(Result);
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            Result = new string(asciiChars);
        }

        if (!string.IsNullOrEmpty(ID))
            webview.OnCallbackActionFinished(ID, Result, "0");
    }

    public void onPageStarted(string url)
    {
        webview.InternalOnPageStarted(webview, url);
    }

    public void onPageFinished(string url)
    {
        webview.InternalOnPageFinished(webview, url);
    }

    public void onMessageReceived(string RawMessage, string Scheme, string host, string dictionary)
    {
        webview.InternalOnReceivedMessage(
            new WellReceivedData(RawMessage, Scheme, host, dictionary));
    }

    public void OnFinishedInit()
    {
        webview.InternalOnFinishedInit();
    }

    public void OnPageProgressLoad(int _progress)
    {
        webview.InternalOnPageProgressLoad(_progress);
    }

    public void OnMultipleWindowOpened()
    {
        webview.InternalOnMultipleWindowOpened();
    }

    public void OnMultipleWindowClosed()
    {
        webview.InternalOnMultipleWindowClosed();
    }

    public void onReceivedError(string Url,int ErrorCode)
    {
        webview.InternalOnReceivedError(Url,ErrorCode);
    }
}
#endif