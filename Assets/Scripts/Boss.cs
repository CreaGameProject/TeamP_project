using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;


public class Boss : MonoBehaviour
{ 
    Animator animator;
    public int Bosshp = 5;
    public int attackTimes;
    public int Count;

    public GameObject enemy_prefab;

    public Transform LowerArm_R_end;

    public bool BossZako = false; //ボスが生成した雑魚であることを区別

    public bool is_attacked = false;//雑魚が棒に当たったかをZakoから取得する

    public GameObject zako_prefab;
    public GameObject zako_go;

    private void Start()
    {
        animator = GetComponent<Animator>();  
    }

    void Update()
    {        
        if (animator.GetBool("AttackManager") == false)  //最初と攻撃終了時に攻撃を選択  
        {
            attackTimes = Random.Range(0, 3) + 1;  //攻撃回数
            Debug.Log("攻撃を選択中");
            animator.SetInteger("AttackType", Random.Range(0, 2));
            animator.SetBool("AttackManager", true);

            Debug.Log("攻撃タイプ：" + animator.GetInteger("AttackType") + "　攻撃回数：" + attackTimes);
        }

        else if (animator.GetBool("AttackManager") == true)
        {
            if (animator.GetInteger("AttackType") == 0)  //雑魚を投げる攻撃
            {
                if (animator.GetInteger("count") != attackTimes)
                {
                    Count = animator.GetInteger("count") + 1;
                    Debug.Log("投げ攻撃中" + Count + "回目");
                }

                else if (animator.GetInteger("count") == attackTimes)
                {
                    Count = 0;
                    animator.SetTrigger("ActionEnd");
                    animator.SetBool("AttackManager", false);
                    animator.SetInteger("count", 0);
                    Debug.Log("投げ攻撃終了");
                    return;
                }
            }

            else if (animator.GetInteger("AttackType") == 1)
            {
                if (animator.GetInteger("count") != attackTimes)
                {
                    Count = animator.GetInteger("count") + 1;
                    Debug.Log("突進攻撃中" + Count + "回目");                    
                }

                else if (animator.GetInteger("count") == attackTimes)
                {
                    Count = 0;
                    animator.SetTrigger("ActionEnd");
                    animator.SetBool("AttackManager", false);
                    animator.SetInteger("count", 0);
                    Debug.Log("突進攻撃終了");
                    return;
                }     
            }
        }
    }

    public void ActionCount()
    {
        animator.SetInteger("count", animator.GetInteger("count") + 1);
        //Debug.Log(animator.GetInteger("count") + "回目");
    }

    public void CallBossZako()
    {   //ボスが投げる雑魚のイベントスクリプト

        zako_go = Instantiate(enemy_prefab, LowerArm_R_end.transform.position, LowerArm_R_end.transform.rotation);
        Zako1 zako1_sc;
        zako1_sc = zako_go.GetComponent<Zako1>();
        zako1_sc.is_boss= true;
        BossZako = true;

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
