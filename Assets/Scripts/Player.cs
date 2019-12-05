using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;


public class Player : MonoBehaviour
{
    private Rigidbody rb;  //rigidbodyを入れる変数
    [SerializeField]  //これを書いた下の変数はpublicと同じようにUnityEditor上で指定できる
    private float speed = 3.0f;  //speedって書いてるけどプレイヤーの加速度
    [SerializeField]
    private float max_speed = 5.0f;  //プレイヤーの最高速度
    public int forward = 1;  //プレイヤーの向きを表す(前:1 後:-1)
    private float jump_distance = Mathf.Infinity;  //プレイヤーがジャンプするx座標

    private Vector3 freezed_velocity = Vector3.zero;  //ジャンプ操作をした瞬間の速度を記録する変数
    private bool freeze_move = false;  //プレイヤーの速度を固定するかどうか

    [SerializeField]
    private GameObject nose;  //プレイヤーの向きを確認するデバッグ用

    [SerializeField]
    private float jump_force = 10.0f;  //ジャンプの強さ
    [SerializeField]
    private float max_jump_force = 15.0f;  //跳びすぎないようにするためのジャンプの最大パワー

    [SerializeField]
    private float wall_jump_force = 20.0f;  //壁ジャンプのパワー

    public bool is_ground = true;  //プレイヤーが着地しているかどうか

    private RaycastHit hit;  //着地判定用のraycastの内容

    public GameObject debug_sphere;  //デバッグ用 棒の当たった位置
    public GameObject debug_sphere2;  //デバッグ用 ジャンプする位置

    private bool break_coroutine = false;  //使われてないっぽい変数

    public bool enable_turn= true;  //ジャンプするまで方向転換できなくする

    private bool touching_wall = false;  //ジャンプするまでに壁に触れたかどうか

    [SerializeField]
    private Animator animator;  //プレイヤーのanimatorを格納する変数

    [SerializeField]
    private GameObject bo_fake;  //棒の見た目のオブジェクト(判定ではない)
    [SerializeField]
    private float bo_length = 14.0f;  //棒が伸びる最大値
    private Vector3 bo_end_point = Vector3.zero;  //棒の到達位置

    [SerializeField]
    private GameObject hand_object;  //手の位置を入れておく

    public bool bo_lock = false;  //棒の見た目を動かさないかどうか

    [SerializeField]
    private GameObject player_model;  //モデルのオブジェクトを格納する

    private int max_hp = 3;  //HPの最大値(初期化用)
    public int hp;  //プレイ中のHP

    [SerializeField]
    private GameObject[] images = new GameObject[3];  //HPのUIの画像

    [SerializeField]
    private GameObject[] eyes = new GameObject[4];  //目の画像(0:左目通常 1:右目通常 2:左目瞑り 3:右目瞑り)

    [SerializeField]
    private AudioSource[] ASs;  //AudioSourceを格納する(配列じゃなくていいかも)

    [SerializeField]
    private AudioClip[] ACs;  //音声素材を格納する

    public GameObject GameOverCanvas;

    [SerializeField]
    GameObject postProcessGameObject;


    // Start is called before the first frame update
    void Start()
    {
        //いろいろ初期化
        rb = transform.GetComponent<Rigidbody>();
        animator.SetInteger("state", 0);
        hp = max_hp;
        ASs = gameObject.GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float input_x = Input.GetAxis("Horizontal");  //プレイヤーの入力(-1 ~ 1)

        if (!bo_lock)  //棒が操作されていないとき
        {
            //棒を動かないようにする(仮の対処)
            bo_fake.transform.position = transform.position + Vector3.up;
            bo_fake.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        if (!freeze_move)  //ジャンプ動作中でないとき
        {
            if (enable_turn)
            {
                if (input_x < 0)  //左に入力されたとき
                {
                    forward = -1;
                    nose.transform.localPosition = Vector3.forward * -0.2f;
                    animator.SetInteger("state", 1);  //アニメーションを歩きにする

                    player_model.transform.rotation = Quaternion.Euler(0, 180, 0);  //モデルの向きを左にする
                }
                else if (input_x > 0)  //右に入力されたとき
                {
                    forward = 1;
                    nose.transform.localPosition = Vector3.forward * 0.2f;
                    animator.SetInteger("state", 1);  //アニメーションを歩きにする

                    player_model.transform.rotation = Quaternion.Euler(0, 0, 0);  //モデルの向きを右にする
                }
                else
                {
                    animator.SetInteger("state", 0);  //アニメーションを待機にする
                }
            }
            bo_lock = false;  //ジャンプ動作中でないなら棒の動きを固定する
        }
        else
        {
            if(rb.velocity.z < 0.01f && rb.velocity.z > -0.01f)  //動いてないとき
            {
                //ジャンプ動作をしているなら止める
                StopCoroutine("Jump_Set");
                freeze_move = false;
            }
            bo_fake.transform.position = (bo_end_point + transform.position) / 2.0f;  
            bo_fake.transform.LookAt(hand_object.transform.position);
        }

        if (!freeze_move && rb.velocity.z * forward < max_speed && !touching_wall)
        {
            rb.velocity += new Vector3(0, 0, input_x * speed * Time.deltaTime);
            if(max_speed - (rb.velocity.z * forward) < 0.3f)
            {
                animator.SetInteger("state", 2);
            }
        }
        else
        {
            if(max_speed > rb.velocity.z * forward)
            {
                rb.velocity += new Vector3 (0, 0, speed * Time.deltaTime * forward);
                
            }
            else
            {
                animator.SetInteger("state", 2);
            }

            if (!freeze_move && !touching_wall)
            {
                animator.SetInteger("state", 2);
            }
        }

        var isHit = Physics.SphereCast(transform.position, 0.5f, transform.up * -1.05f, out hit);
        if (isHit)
        {
            if (!is_ground)
            {
                ASs[0].Play();
            }
            is_ground = true;
            touching_wall = false;
        }
        else
        {
            is_ground = false;
        }
        //Debug.Log(freeze_move);

        bo_fake.transform.LookAt(transform.position);
    }

    public IEnumerator Jump_Set(Vector3 point)
    {
        if (rb.velocity.z > 0.2f || rb.velocity.z < -0.2f)
        {
            //Debug.Log("StartCoroutine");
            //Debug.Log(point);
            debug_sphere.transform.position = point;
            freezed_velocity = rb.velocity;
            freeze_move = true;

            bo_end_point = point;
            bo_lock = false;

            //Debug.Log("freezemove -> " + freeze_move);
            if(forward > 0)
            {
                jump_distance = point.z - transform.position.z;
                debug_sphere2.transform.position = new Vector3(0, 0, transform.position.z + (jump_distance / 2.0f));
            }
            else
            {
                jump_distance = transform.position.z - point.z;
                debug_sphere2.transform.position = new Vector3(0, 0, transform.position.z - (jump_distance / 2.0f));
            }


            bo_fake.transform.localScale = (Vector3.Distance(transform.position , bo_end_point) / 13.0f) * new Vector3(1, 1, 1);
            
            float distance = Mathf.Infinity;
            while (true)
            {
                yield return null;
                //Debug.Log("jump_distance");
                if(forward > 0)
                {
                    distance = point.z - transform.position.z;
                }
                else
                {
                    distance = transform.position.z - point.z;
                }
                
                
                //Debug.Log("distance: " + distance);
                if (distance < jump_distance / 2)
                {
                    break;
                }
            }
            float jump_limited;
            if(jump_force * jump_distance > max_jump_force)
            {
                jump_limited = max_jump_force;
            }
            else
            {
                jump_limited = jump_force * jump_distance;
            }
            rb.AddForce(Vector3.up * jump_limited, ForceMode.Impulse);
            freeze_move = false;
            yield return null;
        }
        else
        {
            //Debug.Log("Ignore");
        }
    }

    public IEnumerator WallJump(Vector3 point)
    {
        //float w_jump_distance = Vector3.Distance(transform.position, point);
        rb.velocity = Vector3.zero;
        debug_sphere.transform.position = point;
        Vector3 w_jump_vector = new Vector3 (0, point.y - transform.position.y, point.z - transform.position.z);
        //Debug.Log(w_jump_vector);
        debug_sphere2.transform.position = transform.position - w_jump_vector;
        rb.AddForce(-w_jump_vector.normalized * wall_jump_force, ForceMode.Impulse);
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!is_ground)
        {
            touching_wall = true;
        }

        if(collision.transform.tag == "Enemy")
        {
            hp--;
            images[hp].SetActive(false);
            if(hp == 0)
            {
                //GoToGameOver();
                GameOverCanvas.SetActive(true);
                Invoke("FixDOF", 1f);

            }
            else
            {
                Damage();
            }
        }
    }

    private IEnumerator Damage()
    {
        gameObject.layer = 10;
        //eyes[0].SetActive(false);
        //eyes[1].SetActive(false);
        //eyes[2].SetActive(true);
        //eyes[3].SetActive(true);
        ASs[1].Play();

        yield return new WaitForSeconds(1.0f);
        gameObject.layer = 0;
        //eyes[0].SetActive(true);
        //eyes[1].SetActive(true);
        //eyes[2].SetActive(false);
        //eyes[3].SetActive(false);
    }

    //private void gotogameover()
    //{
    //    scenemanager.loadscene("gameoverscene");
    //}

    //ぼかす
    void FixDOF()
    {
        var dof = ScriptableObject.CreateInstance<DepthOfField>();
        dof.focusDistance.Override(4);
        PostProcessManager.instance.QuickVolume(postProcessGameObject.layer, 1, dof);
    }

}
