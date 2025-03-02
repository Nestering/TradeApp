using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadExample : MonoBehaviour
{
    public void StartWellWebView()
    {
        SceneManager.LoadScene("WellWebViewDemoScene", LoadSceneMode.Single);
    }

    public void StartWellSafeBrowser()
    {
        SceneManager.LoadScene("WellSafeBrowserDemoScene", LoadSceneMode.Single);
    }
}
