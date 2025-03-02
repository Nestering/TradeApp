using UnityEditor;
using UnityEngine;

namespace WellWebView
{
    public class WellBuildParameters : SingletonScriptableObject<WellBuildParameters>
    {
        [Header("Android")]

        public bool AddUsesClearTextTraffic = false;

        public bool AddReadPermission = false;

        public bool AddWritePermission = false;

        public bool AddCameraPermission = false;

        [Header("Android 11+ version")] 
        [Tooltip("Allows you to run other packages from this application (Example: opening whatsapp by link, etc.)")] 
        public bool AddQueriesIntent = false;

        [Header("Android && IOS")]
        public bool AddGeolocationPermission = false;

        [MenuItem("Assets/WellWebView")]
        public static void SelectedScriptable()
        {
            string path = AssetDatabase.GetAssetPath(Instance.GetInstanceID());
            AssetDatabase.OpenAsset(Instance.GetInstanceID());
        }
    }
}