using TicTacToe;
using TicTacToe.Mechanics;
using TicTacToe.Services.Saving;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestAlternativeBoard))]
[CanEditMultipleObjects]
public class AlternativeTestTTTBuilder : Editor
{
    public string[] signs = {ETttType.Cross.ToString(), ETttType.Noughts.ToString()};

    private Color _normalColor;
    private SaveManager_Json_PlayerPrefs _saver;

    public void OnEnable()
    {
        _normalColor = GUI.backgroundColor;
        var alternative = (TestAlternativeBoard) target;
        if (!alternative.IsInitialized)
        {
            alternative.Initialize();
        }

        _saver = new SaveManager_Json_PlayerPrefs();
        EditorUtility.SetDirty(target);
    }

    public override void OnInspectorGUI()
    {
        var alternative = (TestAlternativeBoard) target;

        DrawDefaultInspector();

        var index = EditorGUILayout.Popup("Turn", alternative.Turn == ETttType.Cross ? 0 : 1, signs);
        alternative.Turn = index == 0 ? ETttType.Cross : ETttType.Noughts;

        index = EditorGUILayout.Popup("Your sign", alternative.YourSign == ETttType.Cross ? 0 : 1, signs);
        alternative.YourSign = index == 0 ? ETttType.Cross : ETttType.Noughts;

        var normalColor = GUI.backgroundColor;
        EditorGUILayout.BeginVertical();
        CreateAlternative(alternative);
        EditorGUILayout.EndVertical();
       

        EditorUtility.SetDirty(target);

        if (GUILayout.Button("Generate"))
        {
            _saver.CreateSave(EGameMode.Alternative, alternative.YourPersonType, alternative.GetAlternativeModel(alternative.Turn),
                alternative.YourSign);
            _saver.CurrentSave.ForceSave();
        }
    }

    private void CreateAlternative(TestAlternativeBoard alternative)
    {
        EditorGUILayout.BeginVertical();
        for (var ya = 0; ya < 3; ya++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var xa = 0; xa < 3; xa++)
            {
                if (alternative[xa, ya] == ETttType.None)
                {
                    EditorGUILayout.BeginVertical();
                    CreateClassic(alternative, xa, ya);
                    if (GUILayout.Button("", GUILayout.Width(30), GUILayout.Height(10)))
                    {
                        alternative[xa, ya] = ETttType.Cross;
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    switch (alternative[xa, ya])
                    {
                        case ETttType.Cross:
                            GUI.backgroundColor = Color.red;
                            break;
                        case ETttType.Noughts:
                            GUI.backgroundColor = Color.blue;
                            break;
                        case ETttType.Draw:
                            GUI.backgroundColor = Color.green;
                            break;
                    }

                    if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        switch (alternative[xa, ya])
                        {
                            case ETttType.Cross:
                                alternative[xa, ya] = ETttType.Noughts;
                                break;
                            case ETttType.Noughts:
                                alternative[xa, ya] = ETttType.Draw;
                                break;
                            case ETttType.Draw:
                                alternative[xa, ya] = ETttType.None;
                                break;
                        }
                    }
                }

                CreateLine(5, 40);
            }

            EditorGUILayout.EndHorizontal();
            CreateLine(100, 5);
        }

        EditorGUILayout.EndVertical();
    }

    private void CreateClassic(TestAlternativeBoard alternative, int xa, int ya)
    {
        EditorGUILayout.BeginVertical();
        for (var yc = 0; yc < 3; yc++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var xc = 0; xc < 3; xc++)
            {
                switch (alternative[xa, ya, xc, yc])
                {
                    case ETttType.Cross:
                        GUI.backgroundColor = Color.red;
                        break;
                    case ETttType.Noughts:
                        GUI.backgroundColor = Color.blue;
                        break;
                    case ETttType.Draw:
                        GUI.backgroundColor = Color.green;
                        break;
                }

                if (GUILayout.Button("", GUILayout.Width(10), GUILayout.Height(10)))
                {
                    switch (alternative[xa, ya, xc, yc])
                    {
                        case ETttType.None:
                            alternative[xa, ya, xc, yc] = ETttType.Cross;
                            break;
                        case ETttType.Cross:
                            alternative[xa, ya, xc, yc] = ETttType.Noughts;
                            break;
                        case ETttType.Noughts:
                            alternative[xa, ya, xc, yc] = ETttType.None;
                            break;
                    }
                }

                CreateLine(2, 10);
            }

            EditorGUILayout.EndHorizontal();
            CreateLine(40, 2);
        }

        EditorGUILayout.EndVertical();
    }

    private void CreateLine(float width, float height)
    {
        GUI.backgroundColor = Color.black;
        if (GUILayout.Button("", GUILayout.Width(width), GUILayout.Height(height)))
        {
        }

        GUI.backgroundColor = _normalColor;
    }
}