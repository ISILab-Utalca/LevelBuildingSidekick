using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LBS.Settings
{
    public class SettingsEditor
    {
        public static void SearchSettingsInstance()
        {
            LBSSettings.Instance = Utility.DirectoryTools.GetScriptable<LBSSettings>();
        }
    }
}