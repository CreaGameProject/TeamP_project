using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;


public class Player : MonoBehaviour
{
    private Rigidbody rb;  //rigidbodyを入れる変数
    private bool enable_move = true;
    [SerializeField]  //これを書いた下の変数はpublicと同じようにUnityEditor上で指定できる
    private float speed = 5.0f;  //speedって書いてるけどプレイヤーの加速度
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

        if (enable_move)  //操作を受け付けているとき
        {
            if (enable_turn)
            {
                if (input_x < 0)  //左に入力されたとき
                {
                    forward = -1;
                    //nose.transform.localPosition = Vector3.forward * -0.2f;
                    animator.SetInteger("state", 1);  //アニメーションを歩きにする

                    player_model.transform.rotation = Quaternion.Euler(0, 180, 0);  //モデルの向きを左にする
                }
                else if (input_x > 0)  //右に入力されたとき
                {
                    forward = 1;
                    //nose.transform.localPosition = Vector3.forward * 0.2f;
                    animator.SetInteger("state", 1);  //アニメーションを歩きにする

                    player_model.transform.rotation = Quaternion.Euler(0, 0, 0);  //モデルの向きを右にする
                }
                else
                {
                    animator.SetInteger("state", 0);  //アニメーションを待機にする
                    if(is_ground) rb.velocity *= 0.2f * Time.deltaTime;
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
            bo_fake.transform.position = (bo_end_point + transform.position) / 2.0f;  //棒の見た目の位置を判定の位置にする
            bo_fake.transform.LookAt(hand_object.transform.position);  //棒の向きを手の方向に向ける
        }

        if (!freeze_move && rb.velocity.z * forward < max_speed && !touching_wall　&& enable_move)  //右に移動中の時
        {
            rb.velocity += new Vector3(0, 0, input_x * speed * Time.deltaTime);  //プレイヤーの速度を上げる
            if(max_speed - (rb.velocity.z * forward) < 0.3f)  //プレイヤーの速度が最高速近くのとき、
            {
                animator.SetInteger("state", 2);  //アニメーションを走るモーションにする
            }
        }
        else
        {
            if(max_speed > rb.velocity.z * forward && enable_move)  //左に移動中、プレイヤーの速度が最高速より小さいとき
            {
                rb.velocity += new Vector3 (0, 0, speed * Time.deltaTime * forward);  //プレイヤーの速度を上げる

            }
            else
            {
                animator.SetInteger("state", 2);  //プレイヤーの速度が最高速より大きいとき、アニメーションを走るモーションにする
            }

            if (!freeze_move && !touching_wall)  //プレイヤーの速度が最高速より大きいとき、アニメーションを走るモーションにする
            {
                animator.SetInteger("state", 2);
            }
        }

        RaycastHit hit;
        var isHit = Physics.SphereCast(transform.position, 0.5f, transform.up * -1, out hit, 0.55f);  //プレイヤーの真下にRaycast
        if (isHit)  //Raycastに何かヒットしたとき
        {
            if (!is_ground)  //それが地面だったとき
            {
                ASs[0].Play();  //着地音を鳴らす
            }
            if (hit.transform.tag == "Terrain")
            {
                is_ground = true;  //地面に接している状態と記録する
                touching_wall = false;  //壁に接していない状態と記録する
            }
        }
        else
        {
            is_ground = false;  //Raycastに何もヒットしなかったとき、地面に接していない状態と記録する
        }
        //Debug.Log(freeze_move);

        bo_fake.transform.LookAt(transform.position);  //棒の見た目の向きをリセットする
    }

    public IEnumerator Jump_Set(Vector3 point)  //ジャンプの一連の処理
    {
        if (rb.velocity.z > 0.2f || rb.velocity.z < -0.2f)  //プレイヤーの速度が十分速いとき
        {
            //Debug.Log("StartCoroutine");
            //Debug.Log(point);
            debug_sphere.transform.position = point;  //デバッグ用
            freezed_velocity = rb.velocity;  //プレイヤーの速度を記録する
            freeze_move = true;  //ジャンプするまでプレイヤーの速度を固定する

            bo_end_point = point;  //棒が触れた位置を記録する
            bo_lock = false;  //棒の固定を解除する

            //Debug.Log("freezemove -> " + freeze_move);
            if(forward > 0)  //プレイヤーが前を向いているとき
            {
                jump_distance = point.z - transform.position.z;  //ジャンプする位置はプレイヤーの位置から棒が当たった場所の半分の位置にする
                debug_sphere2.transform.position = new Vector3(0, 0, transform.position.z + (jump_distance / 2.0f));  //デバッグ用
            }
            else  //プレイヤーが後ろを向いてるとき
            {
                jump_distance = transform.position.z - point.z;  //ジャンプする位置はプレイヤーの位置から棒が当たった場所の半分の位置にする
                debug_sphere2.transform.position = new Vector3(0, 0, transform.position.z - (jump_distance / 2.0f));  //デバッグ用
            }


            bo_fake.transform.localScale = (Vector3.Distance(transform.position , bo_end_point) / 13.0f) * new Vector3(1, 1, 1);  //棒の見た目を伸ばす

            float distance = Mathf.Infinity;  //棒の当たった位置とプレイヤーの距離を格納する変数
            while (true)  //プレイヤーがジャンプする位置に来るまで
            {
                yield return null;  //１フレーム待つ
                //Debug.Log("jump_distance");
                if(forward > 0)  //プレイヤーが前を向いているとき
                {
                    distance = point.z - transform.position.z;  //棒の当たった位置とプレイヤーの距離を測る
                }
                else  //プレイヤーが後ろを向いているとき
                {
                    distance = transform.position.z - point.z;  //棒の当たった位置とプレイヤーの距離を測る
                }


                //Debug.Log("distance: " + distance);
                if (distance < jump_distance / 2)  //ジャンプする位置に来たら
                {
                    break;  //次の処理に進む
                }
            }
            float jump_limited;  //ジャンプ力を格納する変数
            if(jump_force * jump_distance > max_jump_force)  //ジャンプ力が大きすぎそうなとき
            {
                jump_limited = max_jump_force;  //ジャンプのパワーを最大値にする
            }
            else  //それ以外のとき
            {
                jump_limited = jump_force * jump_distance;  //棒の当たった場所からの距離からジャンプ力を決める
            }
            rb.AddForce(Vector3.up * jump_limited, ForceMode.Impulse);  //プレイヤーの上方向に力を与えてジャンプさせる
            freeze_move = false;  //操作の制限を解除する
            yield return null;
        }
        else
        {
            //Debug.Log("Ignore");
        }

        if (Input.GetKey(KeyCode.X)) animator.SetInteger("state", 3);
    }

    public IEnumerator WallJump(Vector3 point)  //壁ジャンプの一連の処理
    {
        //float w_jump_distance = Vector3.Distance(transform.position, point);
        rb.velocity = Vector3.zero;  //プレイヤーの速度を0にする
        debug_sphere.transform.position = point;  //デバッグ用
        Vector3 w_jump_vector = new Vector3 (0, point.y - transform.position.y, point.z - transform.position.z);  //ジャンプする方向を棒を突き出した方向にする
        //Debug.Log(w_jump_vector);
        debug_sphere2.transform.position = transform.position - w_jump_vector;  //デバッグ用
        rb.AddForce(-w_jump_vector.normalized * wall_jump_force, ForceMode.Impulse);  //指定した方向にジャンプする
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)  //プレイヤーに何か当たったとき
    {
        if(!is_ground)  //接地していたら
        {
            touching_wall = true;  //壁に触っていることにする
        }

        if(collision.transform.tag == "Enemy")  //当たったのが敵だったら
        {
            hp--;  //HPを１減らす
            images[hp].SetActive(false);  //HP表示のハートを一つ非表示にする
            if(hp == 0)  //HPが0になったら
            {
                //GoToGameOver();
                GameOverCanvas.SetActive(true);
                FixDOF();
            }
            else
            {
                Damage();  //HPが0でなければ無敵時間を発生させる
            }
        }
    }

    private IEnumerator Damage()  //ダメージを受けたときの処理
    {
        gameObject.layer = 10;  //プレイヤーを無敵状態にする
        enable_move = false;  //操作を受け付けなくする
        //eyes[0].SetActive(false);  //プレイヤーキャラクターの目を瞑らせる
        //eyes[1].SetActive(false);
        //eyes[2].SetActive(true);
        //eyes[3].SetActive(true);
        ASs[1].Play();  //ダメージ音を再生
        animator.SetInteger("state", 3);  //ダメージ時のアニメーションに遷移

        yield return new WaitForSeconds(1.0f);  //１秒無敵時間を継続
        gameObject.layer = 0;  //プレイヤーの無敵状態を解除
        enable_move = true;  //操作できるようにする
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
        dof.focusDistance.Override(0.1f);
        PostProcessManager.instance.QuickVolume(postProcessGameObject.layer, 1, dof);
    }

}
