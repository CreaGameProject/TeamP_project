using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int Bosshp = 5;

    public void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.transform.name);

        //GetConmponent 別のスクリプトを取得
        if (other.transform.GetComponent<Zako1>())
        {
            if (other.transform.GetComponent<Zako1>().is_attacked)
            {

                //雑魚がボスに五回当たったらゲームクリア
                if (other.transform.tag == "Enemy")
                {
                    Bosshp--;

                    //images[Bosshp].SetActive(false);余裕があれば表示させます

                    if (Bosshp == 0)
                    {
                        Destroy(transform.root.gameObject);

                        SceneManager.LoadScene("GameClear");
                    }

                    Debug.Log(other.transform.name);
                    Debug.Log(Bosshp);

                }
             }
        }
    }
    /*public void GoToGameClear()
    {
        SceneManager.LoadScene("GameClearScene");
    }*/

}
