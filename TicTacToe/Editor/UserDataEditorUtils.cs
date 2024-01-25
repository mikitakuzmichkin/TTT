using System;
using System.Diagnostics;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TicTacToe.Editor
{
    public static class UserDataEditorUtils
    {
        [MenuItem("Tools/Open Persistent Data Directory")]
        private static void OpenPersistentDataDir()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
        
        [MenuItem("Tools/Delete all PlayerPrefs")]
        private static void DeleteAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Tools/Open PlayerPrefs")]
        public static void OpenPlayerPrefs()
        {
            var registryLocation =
                $@"HKEY_CURRENT_USER\Software\Unity\UnityEditor\{Application.companyName}\{Application.productName}";
            const string registryLastKey =
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit";
            try
            {
                // Set LastKey value that regedit will go directly to
                Registry.SetValue(registryLastKey, "LastKey", registryLocation);
                Process.Start("regedit.exe");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}