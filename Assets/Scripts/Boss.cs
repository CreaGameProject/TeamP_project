using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;


public class Boss : MonoBehaviour
{    
    /*[SerializeField]
    float position1 = 220f;

    [SerializeField]
    float position2 = 184f;*/
    
    int AttackType;         //！Animatorで宣言した
    bool AttackManager;     //！   パラメーターに変えたい
    Animator animator;

    private int count = 0;//繰り返す回数＝＝繰り返した回数かを判定

    public int Bosshp = 5;

    private void Start()
    {
        animator = GetComponent<Animator>();  
    }

    void Update()
    {
        animator.SetInteger("AttackType", 0);     //！ここで
        animator.SetBool("AttackManager", false); //！   いいのか

        int attackTimes = Random.Range(0, 3) + 1;  //攻撃回数

        if (AttackManager == false)  //最初と攻撃終了時に攻撃を選択  
        {
            Debug.Log("攻撃を選択中");
            AttackType = Random.Range(0, 2);
            AttackManager = true;

            Debug.Log("攻撃タイプ：" + AttackType + "　攻撃回数：" + attackTimes);
        }

        else if (AttackManager == true)
        {
            if (AttackType == 0)  //雑魚を投げる攻撃
            {
                if (count != attackTimes)
                {
                    Debug.Log("投げ攻撃中");

                    count += 1;
                }

                else if (count == attackTimes)
                {
                    Debug.Log("投げ攻撃終了");
                    AttackManager = false;
                    count = 0;
                    return;
                }
            }

            else if (AttackType == 1)
            {
                if (count != attackTimes)
                {
                    Debug.Log("突進攻撃中");

                    count += 1;
                }

                else if (count == attackTimes)
                {
                    Debug.Log("突進攻撃終了");
                    AttackManager = false;
                    count = 0;
                    return;
                }
                {/* 
                if (rushMode == 0)
                {
                    //Debug.Log("BossMode0");

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
                        attackType = 0;
                        rushMode = 0;
                        repeat = 0;
                        Debug.Log("突進攻撃終了");
                    }

                    else if (transform.localEulerAngles.y >= 170 && repeat < attackTimes)
                    {
                        rushMode = 0;
                        repeat += 1 ;
                        Debug.Log(repeat + "回目");
                    }
                }*/
                }     
            }
        }
    }

    public void OnCollisionEnter(Collision other) //ボスのHP処理→ゲームクリアシーン読み込み
    {
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
