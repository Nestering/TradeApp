#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace WellWebView
{
    public class IOSPostProcessBuild : MonoBehaviour
    {
        [PostProcessBuild]
        public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS && WellBuildParameters.Instance.AddGeolocationPermission)
            {
                // Get plist
                string plistPath = pathToBuiltProject + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));

                // Get root
                PlistElementDict rootDict = plist.root;

                // background location usage key (new in iOS 8)
                rootDict.SetString("NSLocationWhenInUseUsageDescription", "Uses background location in sites");
                // take photo key
                rootDict.SetString("NSCameraUsageDescription", "Allow take photos");
                // Write to file
                File.WriteAllText(plistPath, plist.WriteToString());
                Debug.Log("WellWebView::Add geolocation permission");
            }
        }
    }
}
#endif