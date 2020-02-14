using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;


public class Boss : MonoBehaviour
{
    [SerializeField]
    float position1 = 220f;

    [SerializeField]
    float position2 = 184f;

    private int rushMode = 0;  //突進攻撃
    private int repeat = 0;  //繰り返す回数＝＝繰り返した回数かを判定
    int attackType = 0;  //攻撃選択
    bool attackManager = false;

    private void Start()
    {
    }

    void Update()
    {
        int attackTimes = Random.Range(0, 3);  //攻撃回数
        float NowTime = 60.0f - Time.time;

        if (attackManager == false)  //最初と攻撃終了時に攻撃を選択  
        {
            Debug.Log("攻撃を選択中");
            int attackType = Random.Range(0, 2);
            attackManager = true;
            Debug.Log("攻撃タイプ" + attackType + attackManager);
        }

        else if (attackManager == true)
        {
            Debug.Log("攻撃タイプ" + attackType + "で" + attackManager + "です。");

            if (attackType == 0)  //攻撃回数秒待機(仮)
            {
                if (NowTime % 3  >= attackTimes)
                {
                    Debug.Log("待機中");
                    Vector3 pos = this.gameObject.transform.position;
                    this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z);
                }

                else
                {
                    Debug.Log("待機終了");
                    attackManager = false;
                }
            }

            
            else if (attackType == 1)  //なぜか入らない部分
            {                
                Debug.Log("突進攻撃中");

                if (rushMode == 0) //1->2へ
                {
                    Debug.Log("BossMode0");

                    Vector3 pos = this.gameObject.transform.position;
                    this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z - 0.3f);

                    if (pos.z <= position2)
                    {
                        rushMode += 1;
                    }
                }

                else if (rushMode == 1)
                {
                    //Debug.Log("BossMode1");

                    transform.Rotate(new Vector3(0, -10, 0));

                    if (transform.localEulerAngles.y <= 10)
                    {
                        rushMode += 1;
                    }
                }

                else if (rushMode == 2)
                {
                    //Debug.Log("BossMode2");

                    Vector3 pos = this.gameObject.transform.position;
                    this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + 0.3f);

                    if (pos.z >= position1)
                    {
                        rushMode += 1;
                    }
                }

                else if (rushMode == 3)
                {
                    //Debug.Log("BossMode3");

                    transform.Rotate(new Vector3(0, 10, 0));

                    if (transform.localEulerAngles.y >= 170 && repeat == attackTimes)
                    {
                        attackManager = false;
                        rushMode = 0;
                        attackType = 0;
                        Debug.Log("突進攻撃終了");
                    }

                    else if (transform.localEulerAngles.y >= 170 && repeat != attackTimes)
                    {
                        rushMode = 0;
                        repeat += 1 ;
                    }
                }
            }
        }
    }

    public int Bosshp = 5;

    public void OnCollisionEnter(Collision other) //ボスのHP処理→ゲームクリアシーン読み込み
    {
        //Debug.Log(other.transform.name);

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

                    //Debug.Log(other.transform.name);
                    Debug.Log("Boss's HP is " +  Bosshp);

                }
             }
        }
    }
    /*public void GoToGameClear()
    {
        SceneManager.LoadScene("GameClearScene");
    }*/

}
