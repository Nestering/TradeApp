using System.IO;
using System.Text;
using System.Xml;
using UnityEditor.Android;
using UnityEngine;

namespace WellWebView
{
    public class ModifyUnityAndroidAppManifest : IPostGenerateGradleAndroidProject
    {
        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            var androidManifest = new AndroidManifest(GetManifestPath(basePath));

            androidManifest.SetHardwareAccel();
            if (WellBuildParameters.Instance.AddReadPermission)
            {
                androidManifest.SetReadExternalStoragePermission();
            }
            if (WellBuildParameters.Instance.AddWritePermission)
            {
                androidManifest.SetWriteExternalStoragePermission();
            }
            if (WellBuildParameters.Instance.AddGeolocationPermission)
            {
                androidManifest.SetFineLocationPermission();
            }
            if (WellBuildParameters.Instance.AddUsesClearTextTraffic)
            {
                androidManifest.SetUsesClearTextTraffic();
            }
            if (WellBuildParameters.Instance.AddCameraPermission)
            {
                androidManifest.SetCameraPermission();
            }
            if (WellBuildParameters.Instance.AddQueriesIntent)
            {
                if (!androidManifest.IsExistQueriesElement())
                {
                    androidManifest.CreateQueriesElement();
                }
                androidManifest.AddQueriesActionMimeType("android.intent.action.VIEW","*/*");
                androidManifest.AddQueriesActionMimeType("android.intent.action.PICK","*/*");
                androidManifest.AddQueriesActionMimeType("android.intent.action.SEND","*/*");
                androidManifest.AddQueriesActionScheme("android.intent.action.SENDTO", "mailto");
                androidManifest.AddQueriesActionSchemeAndHost("android.intent.action.SENDTO", "smsto","*");
            }

            // Add your XML manipulation routines
            //  androidManifest.SetMicrophonePermission();

            androidManifest.Save();
        }

        public int callbackOrder { get { return 1; } }

        private string _manifestFilePath;

        private string GetManifestPath(string basePath)
        {
            if (string.IsNullOrEmpty(_manifestFilePath))
            {
                var pathBuilder = new StringBuilder(basePath);
                pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
                pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
                pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
                _manifestFilePath = pathBuilder.ToString();
            }
            return _manifestFilePath;
        }
    }

    internal class AndroidXmlDocument : XmlDocument
    {
        private string m_Path;
        protected XmlNamespaceManager nsMgr;
        public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

        public AndroidXmlDocument(string path)
        {
            m_Path = path;
            using (var reader = new XmlTextReader(m_Path))
            {
                reader.Read();
                Load(reader);
            }
            nsMgr = new XmlNamespaceManager(NameTable);
            nsMgr.AddNamespace("android", AndroidXmlNamespace);
        }

        public string Save()
        {
            return SaveAs(m_Path);
        }

        public string SaveAs(string path)
        {
            using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
            {
                writer.Formatting = Formatting.Indented;
                Save(writer);
            }
            return path;
        }
    }

    internal class AndroidManifest : AndroidXmlDocument
    {
        private readonly XmlElement ApplicationElement;

        public AndroidManifest(string path) : base(path)
        {
            ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
        }

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;
            return attr;
        }

        internal XmlNode GetActivityWithLaunchIntent()
        {
            return SelectSingleNode("/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and " +
                    "intent-filter/category/@android:name='android.intent.category.LAUNCHER']", nsMgr);
        }

        internal void SetApplicationTheme(string appTheme)
        {
            ApplicationElement.Attributes.Append(CreateAndroidAttribute("theme", appTheme));
        }

        internal void SetStartingActivityName(string activityName)
        {
            GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("name", activityName));
        }

        internal void SetHardwareAccel()
        {
            GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("hardwareAccelerated", "true"));
            Debug.Log("WellWebView::Set hardwareAccelerated = true.Required for proper webview performance");
        }

        internal void SetUsesClearTextTraffic()
        {
            if (ApplicationElement.GetAttribute("usesCleartextTraffic", AndroidXmlNamespace) != "true")
            {
                ApplicationElement.Attributes.Append(CreateAndroidAttribute("usesCleartextTraffic", "true"));
            }
            Debug.Log("WellWebView:: Add usesCleartextTraffic= true in manifest");
        }

        internal void SetReadExternalStoragePermission()
        {
            var manifest = SelectSingleNode("/manifest");
            XmlNodeList list = SelectNodes("/manifest/uses-permission[@android:name='android.permission.READ_EXTERNAL_STORAGE']", nsMgr);
            if (list.Count > 0)
            {
                foreach (XmlNode node in list)
                {
                    manifest.RemoveChild(node);
                }
            }
            manifest = SelectSingleNode("/manifest");
            XmlElement child = CreateElement("uses-permission");
            manifest.AppendChild(child);
            XmlAttribute newAttribute = CreateAndroidAttribute("name", "android.permission.READ_EXTERNAL_STORAGE");
            child.Attributes.Append(newAttribute);
            Debug.Log("WellWebView:: Add READ_EXTERNAL_STORAGE permission in manifest");
        }

        internal void SetWriteExternalStoragePermission()
        {
            var manifest = SelectSingleNode("/manifest");
            XmlNodeList list = SelectNodes("/manifest/uses-permission[@android:name='android.permission.WRITE_EXTERNAL_STORAGE']", nsMgr);
            if (list.Count > 0)
            {
                foreach (XmlNode node in list)
                {
                    manifest.RemoveChild(node);
                }
            }
            manifest = SelectSingleNode("/manifest");
            XmlElement child = CreateElement("uses-permission");
            manifest.AppendChild(child);
            XmlAttribute newAttribute = CreateAndroidAttribute("name", "android.permission.WRITE_EXTERNAL_STORAGE");
            child.Attributes.Append(newAttribute);
            Debug.Log("WellWebView:: Add WRITE_EXTERNAL_STORAGE permission in manifest");
        }

        internal void SetFineLocationPermission()
        {
            var manifest = SelectSingleNode("/manifest");
            XmlNodeList list = SelectNodes("/manifest/uses-permission[@android:name='android.permission.ACCESS_FINE_LOCATION']", nsMgr);
            if (list.Count > 0)
            {
                foreach (XmlNode node in list)
                {
                    manifest.RemoveChild(node);
                }
            }
            manifest = SelectSingleNode("/manifest");
            XmlElement child = CreateElement("uses-permission");
            manifest.AppendChild(child);
            XmlAttribute newAttribute = CreateAndroidAttribute("name", "android.permission.ACCESS_FINE_LOCATION");
            child.Attributes.Append(newAttribute);
            Debug.Log("WellWebView:: Add ACCESS_FINE_LOCATION permission in manifest");
        }

        internal void SetCameraPermission()
        {
            var manifest = SelectSingleNode("/manifest");
            XmlNodeList list = SelectNodes("/manifest/uses-permission[@android:name='android.permission.CAMERA']", nsMgr);
            if (list.Count > 0)
            {
                foreach (XmlNode node in list)
                {
                    manifest.RemoveChild(node);
                }
            }
            manifest = SelectSingleNode("/manifest");
            XmlElement child = CreateElement("uses-permission");
            manifest.AppendChild(child);
            XmlAttribute newAttribute = CreateAndroidAttribute("name", "android.permission.CAMERA");
            child.Attributes.Append(newAttribute);
            Debug.Log("WellWebView:: Add CAMERA permission in manifest");
        }
        
        internal bool IsExistQueriesElement()
        {
            XmlNode queries = SelectSingleNode("/manifest/queries");
            return queries != null;
        }

        internal void CreateQueriesElement()
        {
            XmlNode manifest = SelectSingleNode("/manifest");
            XmlElement queries = CreateElement("queries");
            manifest.AppendChild(queries);
            Debug.Log("WellWebView:: Add queries in manifest");
        }

        internal void AddQueriesActionMimeType(string action,string mimeType)
        {
            XmlNode queries = SelectSingleNode("/manifest/queries");
            XmlElement intent = CreateElement("intent");
            queries.AppendChild(intent);
            
            XmlElement actionElem = CreateElement("action");
            intent.AppendChild(actionElem);

            XmlAttribute newAttribute = CreateAndroidAttribute("name", action);
            actionElem.Attributes.Append(newAttribute);

            XmlElement dataElem = CreateElement("data");
            intent.AppendChild(dataElem);

            XmlAttribute newAttribute1 = CreateAndroidAttribute("mimeType", mimeType);
            dataElem.Attributes.Append(newAttribute1);
        }
        
        internal void AddQueriesActionScheme(string action,string scheme)
        {
            XmlNode queries = SelectSingleNode("/manifest/queries");
            XmlElement intent = CreateElement("intent");
            queries.AppendChild(intent);
            
            XmlElement actionElem = CreateElement("action");
            intent.AppendChild(actionElem);

            XmlAttribute newAttribute = CreateAndroidAttribute("name", action);
            actionElem.Attributes.Append(newAttribute);

            XmlElement dataElem = CreateElement("data");
            intent.AppendChild(dataElem);

            XmlAttribute newAttribute1 = CreateAndroidAttribute("scheme", scheme);
            dataElem.Attributes.Append(newAttribute1);
        }
        
        internal void AddQueriesActionSchemeAndHost(string action,string scheme,string host)
        {
            XmlNode queries = SelectSingleNode("/manifest/queries");
            XmlElement intent = CreateElement("intent");
            queries.AppendChild(intent);
            
            XmlElement actionElem = CreateElement("action");
            intent.AppendChild(actionElem);

            XmlAttribute newAttribute = CreateAndroidAttribute("name", action);
            actionElem.Attributes.Append(newAttribute);

            XmlElement dataElem = CreateElement("data");
            intent.AppendChild(dataElem);

            XmlAttribute newAttribute1 = CreateAndroidAttribute("scheme", scheme);
            XmlAttribute newAttribute2 = CreateAndroidAttribute("host", host);
            dataElem.Attributes.Append(newAttribute1);
            dataElem.Attributes.Append(newAttribute2);
        }
    }
}