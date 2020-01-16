using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class displaymanager : MonoBehaviour
{
    [SerializeField]
    private GameObject postProcessGameObject;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
