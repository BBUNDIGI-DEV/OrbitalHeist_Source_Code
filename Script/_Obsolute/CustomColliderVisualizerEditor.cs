using UnityEngine;
using UnityEditor;
using System;

[Obsolete]
[InitializeOnLoad]
public static class ColliderVisualizerEditor
{
    private static readonly LayerMask HIT_BOX_LAYER = LayerMask.NameToLayer("Hitbox");
    private static readonly Color HIT_BOX_COLOR = new Color(1f, 0f, 0f, 1.0f); // Red with some alpha

    static ColliderVisualizerEditor()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        //GameObject selectionObject = Selection.activeGameObject;
        //if (selectionObject == null)
        //{
        //    return;
        //}

        //Collider[] colliders = selectionObject.GetComponentsInChildren<Collider>();

        //Color prevColor = Handles.color;
        //Handles.color = HIT_BOX_COLOR;
        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    Vector3 scale = colliders[i].transform.localScale;

        //    float combineScale = Mathf.Max(scale.x, scale.z);
        //    combineScale = Mathf.Max(combineScale, 1);
        //    scale.Set(combineScale, 1, combineScale);
        //    Handles.matrix = Matrix4x4.TRS(colliders[i].bounds.center, colliders[i].transform.rotation, scale);

        //    // Draw shapes
        //    if (colliders[i] is BoxCollider box)
        //    {
        //        Handles.DrawWireCube(box.bounds.center, box.bounds.size);
        //    }
        //    else if (colliders[i] is SphereCollider sphere)
        //    {
        //        Handles.DrawWireDisc(sphere.bounds.center, sceneView.camera.transform.forward, sphere.radius * sphere.transform.lossyScale.x);
        //    }
        //    else if (colliders[i] is CapsuleCollider capsule)
        //    {
        //        float halfHeight = (capsule.height * colliders[i].transform.lossyScale.y) / 2.0f;
        //        float discOffset = halfHeight - capsule.radius;
        //        Debug.Log($"{capsule.bounds.extents.y}, {capsule.radius}");
        //        discOffset = Mathf.Max(discOffset, 0.0f);

        //        Handles.DrawWireDisc(new Vector3(0.0f, discOffset, 0.0f), Vector3.up, capsule.radius);
        //        Handles.DrawWireDisc(new Vector3(0.0f, -discOffset, 0.0f), Vector3.up, capsule.radius);

        //        Vector3 straitLineTop = Vector3.forward * capsule.radius + new Vector3(0.0f, discOffset, 0.0f);
        //        for (int j = 0; j < 4; j++)
        //        {
        //            straitLineTop = Quaternion.AngleAxis(90, Vector3.up) * straitLineTop;
        //            Vector3 straitLineBottom = new Vector3(straitLineTop.x, -straitLineTop.y, straitLineTop.z);
        //            Handles.DrawLine(straitLineTop, straitLineBottom);
        //        }

        //        Vector3 arcNormal = Vector3.forward;
        //        Vector3 arcFrom = Vector3.right;
        //        Vector3 arcCenter = new Vector3(0.0f, discOffset, 0.0f);
        //        Handles.DrawWireArc(arcCenter, arcNormal, arcFrom, 180, capsule.radius);
        //        Handles.DrawWireArc(-arcCenter, arcNormal, -arcFrom, 180, capsule.radius);


        //        arcNormal = Vector3.right;
        //        arcFrom = Vector3.back;
        //        Handles.DrawWireArc(arcCenter, arcNormal, arcFrom, 180, capsule.radius);
        //        Handles.DrawWireArc(-arcCenter, arcNormal, -arcFrom, 180, capsule.radius);
        //    }
        //}
        //Handles.color = prevColor;
    }
}

