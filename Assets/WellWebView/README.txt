Thank you for purchasing WellWebView!
--
The documentation is available at this URL: https://xantwell.com/Products/WellWebView
--
If you have any questions or suggestions, please send a message to: support@xantwell.com
--
If you find an error or you think that something is not working.Write a message with data such as: Platform, how to repeat the error, what behavior is now and what behavior do you expect

@@
--Getting Started--

To load a web page, you need to use the following code:

WellWebView webview = gameObject.AddComponent<WellWebView>();//Create new GameObject and add WellWebView
webview.Init();//Initialization
webview.OnFinishedInit += (initWebView) =>//Waiting for initialization to complete
	{
		webview.Android.SetUseWideViewPort(true);//Sets wide view (required for a similar user experience as in the browser in Android)
        webview.Android.SetLoadWithOverviewMode(true);//Sets OverviewMode (required for a similar user experience as in the browser in Android)
        webview.Show();//Show
        webview.LoadUrl("https://xantwell.com/Products/WellWebView/");//Load web page
	};

--
For further study of possible settings and features, go to the website: https://xantwell.com/Products/WellWebView