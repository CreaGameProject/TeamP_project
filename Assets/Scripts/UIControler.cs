using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class UIControler : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject EndButton;
    public GameObject TitleImage;
    [SerializeField]
    GameObject postProcessGameObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Onclick()
    {
        StartButton.SetActive(false);
        EndButton.SetActive(false);
        TitleImage.SetActive(false);
        Invoke("FixDOF", 1f);
    }
<<<<<<< HEAD
    public void Retryclick()
    {

    }

    //ぼかすのを治す
=======
>>>>>>> parent of 3a3f9d3... Merge branch 'master' of https://github.com/CreaGameProject/TeamP_project
    void FixDOF()
    {
        var dof = ScriptableObject.CreateInstance<DepthOfField>();
        dof.focusDistance.Override(4);
        PostProcessManager.instance.QuickVolume(postProcessGameObject.layer, 1, dof);
    }

    public void EndGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
		Application.Quit();
    }
}
