using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelPatern))]
public class LevelPaternEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelPatern lp = (LevelPatern)target;
        LevelPatern.X = EditorGUILayout.IntField(LevelPatern.X);
        LevelPatern.Y = EditorGUILayout.IntField(LevelPatern.Y);
        if (GUILayout.Button("Create Array"))
        {
            lp.CreateEmptyArray();
        }
        try
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < CrystalDesigner.Y; y++)
            {
                EditorGUILayout.BeginVertical();
                for (int x = 0; x < CrystalDesigner.X; x++)
                {

                    lp.columns[x].rows[y] = EditorGUILayout.Toggle(lp.columns[x].rows[y]);
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndHorizontal();
        }
        catch
        {
            lp.CreateEmptyArray();

        }
        if (GUILayout.Button("Generate Crystals"))
        {

        }


    }
}
