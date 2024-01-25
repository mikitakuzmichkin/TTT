using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TicTacToe.Editor
{
    public static class ScreenshotsUtil
    {
        private const string SCREENSHOTS_DIR_NAME = "Screenshots";

        [MenuItem("Tools/Take Screenshot")]
        private static void Screenshot()
        {
            if (!Directory.Exists(SCREENSHOTS_DIR_NAME))
            {
                Directory.CreateDirectory(SCREENSHOTS_DIR_NAME);
            }

            var filename = $"{SCREENSHOTS_DIR_NAME}/{Guid.NewGuid()}.png";
            ScreenCapture.CaptureScreenshot(filename, 1);
            Debug.Log($"Screenshot \"{filename}\" created!");
        }
    }
}