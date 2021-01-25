using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CrystalDesigner))]
public class CrystalDesingerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CrystalDesigner cd = (CrystalDesigner)target;
        CrystalDesigner.X = EditorGUILayout.IntField(CrystalDesigner.X);
        CrystalDesigner.Y = EditorGUILayout.IntField(CrystalDesigner.Y);
        if (GUILayout.Button("Create Array"))
        {
            cd.CreateEmptyArray();
        }
        try
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < CrystalDesigner.Y; y++)
            {
                EditorGUILayout.BeginVertical();
                for (int x = 0; x < CrystalDesigner.X; x++)
                {

                    cd.columns[x].rows[y] = EditorGUILayout.Toggle(cd.columns[x].rows[y]);
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndHorizontal();
        }
        catch
        {
            cd.CreateEmptyArray();

        }
        if (GUILayout.Button("Generate Crystals"))
        {
            cd.GenerateCrystals();
        }


    }
}
