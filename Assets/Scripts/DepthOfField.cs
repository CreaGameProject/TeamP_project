using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManipulator : MonoBehaviour
{

    // Post Process Volume がついているGameObject
    [SerializeField]
    GameObject postProcessGameObject;

    void Start()
    {
        Invoke("FixDOF", 1f);
    }

    void FixDOF()
    {
        var dof = ScriptableObject.CreateInstance<DepthOfField>();
        dof.focusDistance.Override(4);
        PostProcessManager.instance.QuickVolume(postProcessGameObject.layer, 1, dof);
    }
}