using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WellWebView
{

    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static T _instance = null;
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    string path = "Assets/WellWebView/Editor/WellBuildConfig.asset";
                    _instance = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (!_instance)
                    {
                        if (!File.Exists(path))
                        {
                            T asset = CreateInstance<T>();
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            AssetDatabase.CreateAsset(asset, path);
                            AssetDatabase.SaveAssets();
                            EditorUtility.FocusProjectWindow();
                            _instance = asset;
                        }
                        else
                        {
                            _instance = AssetDatabase.LoadAssetAtPath<T>(path);
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
