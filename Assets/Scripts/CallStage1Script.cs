using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CallStage1Script : MonoBehaviour
{

    public void ToStage1()
    {
        SceneManager.LoadSceneAsync("Stage1");
    }
}