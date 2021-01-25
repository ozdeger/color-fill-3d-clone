using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelDesigner))]
public class LevelDesignerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelDesigner ld = (LevelDesigner)target;

        ld.X = EditorGUILayout.IntField(ld.X);
        ld.Y = EditorGUILayout.IntField(ld.Y);
        if (GUILayout.Button("Create Empty Array - Click After Resizing Array"))
        {
            ld.CreateEmptyArray();
        }
        try
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < ld.Y; y++)
            {
                EditorGUILayout.BeginVertical();
                for (int x = 0; x < ld.X; x++)
                {

                    ld.columns[x].rows[y] = EditorGUILayout.Toggle(ld.columns[x].rows[y]);
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndHorizontal();
        }
        catch
        {
            ld.CreateEmptyArray();

        }

        if (GUILayout.Button("Generate Level"))
        {
            ld.GenerateLevel();
        }

        if (GUILayout.Button("Add Level"))
        {
            ld.AddLevel();
        }
        //if (GUILayout.Button("Clear Level"))
        //{
        //    ld.ClearLevel();
        //}
    }
}

