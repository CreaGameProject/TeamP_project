using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToTitleScript : MonoBehaviour
{

    public void ToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}