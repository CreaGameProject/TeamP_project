using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;


public class Boss : MonoBehaviour
{
    //ボスのステージ突進攻撃

    //[SerializeField]
    float position1 = 220f;

    //[SerializeField]
    float position2 = 184f;

    private int mode = 0;

    private void Start()
    {
        
    }

    void Update()
    {
        if (mode == 0) //1->2へ
        {
            Debug.Log("BossMode0");

            Vector3 pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z - 0.3f);

            if(pos.z <= position2)
            { 
                mode += 1;
            }
        }

        else if (mode == 1)
        {
            Debug.Log("BossMode1");

            transform.Rotate(new Vector3(0, -10, 0));

            if (transform.localEulerAngles.y <= 10)
            {
                mode += 1; 
            }
        }

        else if (mode == 2)
        {
            Debug.Log("BossMode2");

            Vector3 pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + 0.3f);

            if (pos.z >= position1)
            {
                mode += 1;
            }
        }

        else if (mode == 3)
        {
            Debug.Log("BossMode3");

            transform.Rotate(new Vector3(0, 10, 0));

            if (transform.localEulerAngles.y >= 170)
            {
                mode = 0;
            }
        }


        //float NowTime = 60.0f - Time.time;

        //Debug.Log(NowTime);

    }
    

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
