using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AsteroidController))]
public class AsteroidEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AsteroidController script = (AsteroidController)target;
        if (GUILayout.Button("Generate Asteroids"))
        {
            script.GenerateAsteroids();
        }

        if (GUILayout.Button("Delete Asteroids"))
        {
            script.DeleteAsteroids();
        }
    }
}

#endif
