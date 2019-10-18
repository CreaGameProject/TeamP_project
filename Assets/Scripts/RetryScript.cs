using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RetryScript : MonoBehaviour
{

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Stage1");
        }
    }
}