using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class UIControler : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject RetryButton;
    public GameObject EndButton;
    public GameObject TitleImage;
    public GameObject Life;
    public GameObject GameOverCanvas;

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
    public void Startclick()
    {
        StartButton.SetActive(false);
        EndButton.SetActive(false);
        TitleImage.SetActive(false);
        Life.SetActive(true);
        Invoke("FixDOF", 1f);
    }
    public void Retryclick()
    {

    }

    //ぼかす
    void FixDOF()
    {
        var dof = ScriptableObject.CreateInstance<DepthOfField>();
        dof.focusDistance.Override(4);
        PostProcessManager.instance.QuickVolume(postProcessGameObject.layer, 1, dof);
    }
    //ゲーム終了
    public void EndGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
		Application.Quit();
    }

}
