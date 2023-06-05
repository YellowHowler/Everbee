using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Honeycomb))]
public class HoneycombEditor : Editor
{
    Honeycomb _this = null;

    private void OnEnable()
    {
        _this = target as Honeycomb;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        if ( GUILayout.Button("Top Left")== true )
        {
            GameObject go = Instantiate(_this.gameObject) as GameObject;

            go.transform.parent = _this.transform.parent;            
            go.transform.position = _this.transform.position + Vector3.left * 0.825f + Vector3.up * Mathf.Sqrt(3) * 0.825f;
            go.transform.name = "Honeycomb" + _this.transform.parent.childCount.ToString();
        }

        if (GUILayout.Button("Top Right") == true)
        {
            GameObject go = Instantiate(_this.gameObject) as GameObject;
            
            go.transform.parent = _this.transform.parent;
            go.transform.position = _this.transform.position + Vector3.right * 0.825f + Vector3.up * Mathf.Sqrt(3) * 0.825f;
            go.transform.name = "Honeycomb" + _this.transform.parent.childCount.ToString();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Left") == true)
        {
            GameObject go = Instantiate(_this.gameObject) as GameObject;

            go.transform.parent = _this.transform.parent;
            go.transform.position = _this.transform.position + Vector3.left * 1.65f;
            go.transform.name = "Honeycomb" + _this.transform.parent.childCount.ToString();
        }

        if (GUILayout.Button("Right") == true)
        {
            GameObject go = Instantiate(_this.gameObject) as GameObject;

            go.transform.parent = _this.transform.parent;
            go.transform.position = _this.transform.position + Vector3.right * 1.65f;
            go.transform.name = "Honeycomb" + _this.transform.parent.childCount.ToString();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Bottom Left") == true)
        {
            GameObject go = Instantiate(_this.gameObject) as GameObject;

            go.transform.parent = _this.transform.parent;
            go.transform.position = _this.transform.position + Vector3.left * 0.825f + Vector3.down * Mathf.Sqrt(3) * 0.825f;
            go.transform.name = "Honeycomb" + _this.transform.parent.childCount.ToString();
        }
        
        if (GUILayout.Button("Bottom Right") == true)
        {
            GameObject go = Instantiate(_this.gameObject) as GameObject;

            go.transform.parent = _this.transform.parent;
            go.transform.position = _this.transform.position + Vector3.right * 0.825f + Vector3.down * Mathf.Sqrt(3) * 0.825f;
            go.transform.name = "Honeycomb" + _this.transform.parent.childCount.ToString();
        }
        GUILayout.EndHorizontal();
    }
}