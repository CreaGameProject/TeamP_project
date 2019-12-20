using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Boss : MonoBehaviour
{
    int Bosshp = 5;

    public void OnCollisionEnter(Collision other)
    {
        //雑魚がボスに五回当たったらゲームクリア
        if (other.gameObject.tag == "Enemy")
        {
            Bosshp--;

            //images[Bosshp].SetActive(false);余裕があれば表示させます

            if (Bosshp == 0)
            {
                Destroy(transform.root.gameObject);

                SceneManager.LoadScene("GameClear");
            }

            Debug.Log(other.gameObject.name);

        }
    }
    /*public void GoToGameClear()
    {
        SceneManager.LoadScene("GameClearScene");
    }

}
