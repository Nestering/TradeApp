using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShowAppDocs : MonoBehaviour
{
    [TextArea(2, 4), SerializeField] private string site;

    private Button btn;
    private WellWebView webview;

    private void Start()
    {
        btn = GetComponent<Button>();

        btn.onClick.AddListener(() =>
        {
            WellWebView.SetAllowJavaScript(true);


            if (webview != null)
            {
                Destroy(webview);
            }

            webview = gameObject.AddComponent<WellWebView>();
            webview.Init(true);
            webview.OnFinishedInit += (webview) =>
            {
                webview.Android.SetUseWideViewPort(true);
                webview.Android.SetLoadWithOverviewMode(true);
                webview.IOS.AllowBackForwardNavigationGestures(true);
                webview.IOS.SetOpeningAllUrlSchemes(true);
                webview.SetSupportMultipleWindows(true);

                webview.IOS.ShowToolBar(false);

                webview.LoadUrl(site);
                webview.Show(true, 0, WellWebViewAnimation.AnimationType.None);
            };
        });
    }
}
