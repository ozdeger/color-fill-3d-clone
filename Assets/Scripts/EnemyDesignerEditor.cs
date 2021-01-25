using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyDesigner))]
public class EnemyDesignerEditor : Editor
{


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnemyDesigner ed = (EnemyDesigner)target;
        EnemyDesigner.X = EditorGUILayout.IntField(EnemyDesigner.X);
        EnemyDesigner.Y = EditorGUILayout.IntField(EnemyDesigner.Y);
        if (GUILayout.Button("Create Array"))
        {
            ed.CreateEmptyArray();
        }
        try
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < EnemyDesigner.Y; y++)
            {
                EditorGUILayout.BeginVertical();
                for (int x = 0; x < EnemyDesigner.X; x++)
                {

                    ed.columns[x].rows[y] = EditorGUILayout.Toggle(ed.columns[x].rows[y]);
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndHorizontal();
        }
        catch
        {
            ed.CreateEmptyArray();
        
        }
        if (GUILayout.Button("Generate Enemy"))
        {
            ed.GenerateEnemy();
        }
        
        
    }
}