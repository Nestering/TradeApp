using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellSafeBrowserExample : MonoBehaviour
{
    public string Url;
    public bool ShowOnStart;

    private WellSafeBrowser safeBrowser;

    void Start()
    {
        if (WellSafeBrowser.IsSupportedSafeBrowser())
        {
            safeBrowser = new WellSafeBrowser();
            safeBrowser.Create(Url);
            if(ShowOnStart)
                safeBrowser.Show();
        }
        else
        {
            Debug.LogError("WellSafeBrowser::current platform or device no supported!");
        }
    }
}
