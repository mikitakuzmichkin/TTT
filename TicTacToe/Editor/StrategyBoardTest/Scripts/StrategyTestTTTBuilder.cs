using TicTacToe;
using TicTacToe.Mechanics;
using TicTacToe.Services.Saving;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestStrategyBoard))]
[CanEditMultipleObjects]
public class StrategyTestTTTBuilder : Editor
{
    public string[] signs = {ETttType.Cross.ToString(), ETttType.Noughts.ToString()};

    private Color _normalColor;
    private SaveManager_Json_PlayerPrefs _saver;

    public void OnEnable()
    {
        _normalColor = GUI.backgroundColor;
        var strategy = (TestStrategyBoard) target;
        if (!strategy.IsInitialized)
        {
            strategy.Initialize();
        }

        _saver = new SaveManager_Json_PlayerPrefs();
        EditorUtility.SetDirty(target);
    }

    public override void OnInspectorGUI()
    {
        var strategy = (TestStrategyBoard) target;

        DrawDefaultInspector();

        var index = EditorGUILayout.Popup("Turn", strategy.Turn == ETttType.Cross ? 0 : 1, signs);
        strategy.Turn = index == 0 ? ETttType.Cross : ETttType.Noughts;

        index = EditorGUILayout.Popup("Your sign", strategy.YourSign == ETttType.Cross ? 0 : 1, signs);
        strategy.YourSign = index == 0 ? ETttType.Cross : ETttType.Noughts;

        var normalColor = GUI.backgroundColor;
        EditorGUILayout.BeginVertical();
        for (var ys = 0; ys < 3; ys++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var xs = 0; xs < 3; xs++)
            {
                if (strategy[xs, ys] == ETttType.None)
                {
                    EditorGUILayout.BeginVertical();
                    CreateAlternative(strategy, xs, ys);
                    if (GUILayout.Button("", GUILayout.Width(160), GUILayout.Height(50)))
                    {
                        strategy[xs, ys] = ETttType.Cross;
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    switch (strategy[xs, ys])
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

                    if (GUILayout.Button("", GUILayout.Width(200), GUILayout.Height(200)))
                    {
                        switch (strategy[xs, ys])
                        {
                            case ETttType.Cross:
                                strategy[xs, ys] = ETttType.Noughts;
                                break;
                            case ETttType.Noughts:
                                strategy[xs, ys] = ETttType.Draw;
                                break;
                            case ETttType.Draw:
                                strategy[xs, ys] = ETttType.None;
                                break;
                        }
                    }
                }

                CreateLine(10, 200);
            }

            EditorGUILayout.EndHorizontal();
            CreateLine(650, 10);
        }

        EditorGUILayout.EndVertical();

        EditorUtility.SetDirty(target);

        if (GUILayout.Button("Generate"))
        {
            _saver.CreateSave(EGameMode.Strategy, strategy.YourPersonType, strategy.GetStrategyModel(strategy.Turn),
                              strategy.YourSign);
            _saver.CurrentSave.ForceSave();
        }
    }

    private void CreateAlternative(TestStrategyBoard strategy, int xs, int ys)
    {
        EditorGUILayout.BeginVertical();
        for (var ya = 0; ya < 3; ya++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var xa = 0; xa < 3; xa++)
            {
                if (strategy[xs, ys, xa, ya] == ETttType.None)
                {
                    EditorGUILayout.BeginVertical();
                    CreateClassic(strategy, xs, ys, xa, ya);
                    if (GUILayout.Button("", GUILayout.Width(30), GUILayout.Height(10)))
                    {
                        strategy[xs, ys, xa, ya] = ETttType.Cross;
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    switch (strategy[xs, ys, xa, ya])
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
                        switch (strategy[xs, ys, xa, ya])
                        {
                            case ETttType.Cross:
                                strategy[xs, ys, xa, ya] = ETttType.Noughts;
                                break;
                            case ETttType.Noughts:
                                strategy[xs, ys, xa, ya] = ETttType.Draw;
                                break;
                            case ETttType.Draw:
                                strategy[xs, ys, xa, ya] = ETttType.None;
                                break;
                        }
                    }
                }

                CreateLine(5, 40);
            }

            EditorGUILayout.EndHorizontal();
            CreateLine(200, 5);
        }

        EditorGUILayout.EndVertical();
    }

    private void CreateClassic(TestStrategyBoard strategy, int xs, int ys, int xa, int ya)
    {
        EditorGUILayout.BeginVertical();
        for (var yc = 0; yc < 3; yc++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var xc = 0; xc < 3; xc++)
            {
                switch (strategy[xs, ys, xa, ya, xc, yc])
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
                    switch (strategy[xs, ys, xa, ya, xc, yc])
                    {
                        case ETttType.None:
                            strategy[xs, ys, xa, ya, xc, yc] = ETttType.Cross;
                            break;
                        case ETttType.Cross:
                            strategy[xs, ys, xa, ya, xc, yc] = ETttType.Noughts;
                            break;
                        case ETttType.Noughts:
                            strategy[xs, ys, xa, ya, xc, yc] = ETttType.None;
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