using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int Bosshp = 5;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.tag == "Enemy")
        {
            Bosshp--;

            //images[Bosshp].SetActive(false);余裕があれば表示させます

            if (Bosshp == 0)
            {
                GoToGameClear();
            }

            Debug.Log(collision.gameObject.name);

        }
    }
    private void GoToGameClear()
    {
        SceneManager.LoadScene("GameClearScene");
    }
    
}
