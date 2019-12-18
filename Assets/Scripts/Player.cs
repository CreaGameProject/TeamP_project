using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float max_speed = 5.0f;
    public int forward = 1;
    private float jump_distance = Mathf.Infinity;

    private Vector3 freezed_velocity = Vector3.zero;
    private bool freeze_move = false;

    [SerializeField]
    private GameObject nose;

    [SerializeField]
    private float jump_force = 10.0f;
    [SerializeField]
    private float max_jump_force = 15.0f;

    [SerializeField]
    private float wall_jump_force = 20.0f;

    public bool is_ground = true;

    private RaycastHit hit;

    public GameObject debug_sphere;
    public GameObject debug_sphere2;

    private bool break_coroutine = false;

    public bool enable_turn = true;

    private bool touching_wall = false;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject bo_fake;
    [SerializeField]
    private float bo_length = 14.0f;
    private Vector3 bo_end_point = Vector3.zero;

    [SerializeField]
    private GameObject hand_object;

    public bool bo_lock = false;

    [SerializeField]
    private GameObject player_model;

    private int max_hp = 3;
    public int hp;

    [SerializeField]
    private GameObject[] images = new GameObject[3];

    [SerializeField]
    private GameObject[] eyes = new GameObject[4];

    [SerializeField]
    private AudioSource[] ASs;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        animator.SetInteger("state", 0);
        hp = max_hp;
        ASs = gameObject.GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float input_x = Input.GetAxis("Horizontal");

        if (!bo_lock)
        {
            bo_fake.transform.position = transform.position + Vector3.up;
            bo_fake.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        if (!freeze_move)
        {
            if (enable_turn)
            {
                if (input_x < 0)
                {
                    forward = -1;
                    nose.transform.localPosition = Vector3.forward * -0.2f;
                    animator.SetInteger("state", 1);

                    player_model.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (input_x > 0)
                {
                    forward = 1;
                    nose.transform.localPosition = Vector3.forward * 0.2f;
                    animator.SetInteger("state", 1);

                    player_model.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    animator.SetInteger("state", 0);
                }
            }
            bo_lock = false;
        }
        else
        {
            if (rb.velocity.z < 0.01f && rb.velocity.z > -0.01f)
            {
                StopCoroutine("Jump_Set");
                freeze_move = false;
            }

            bo_fake.transform.position = (bo_end_point + transform.position) / 2.0f;  //棒の見た目の位置を判定の位置にする  
            bo_fake.transform.LookAt(hand_object.transform.position);  //棒の向きを手の方向に向ける

        }

        if (!freeze_move && rb.velocity.z * forward < max_speed && !touching_wall)  //右に移動中の時
        {
            rb.velocity += new Vector3(0, 0, input_x * speed * Time.deltaTime);  //プレイヤーの速度を上げる
            if (max_speed - (rb.velocity.z * forward) < 0.3f)  //プレイヤーの速度が最高速近くのとき、
            {
                animator.SetInteger("state", 2);  //アニメーションを走るモーションにする
            }
        }
        else
        {
            if (max_speed > rb.velocity.z * forward)  //左に移動中、プレイヤーの速度が最高速より小さいとき
            {
                rb.velocity += new Vector3(0, 0, speed * Time.deltaTime * forward);  //プレイヤーの速度を上げる

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

        var isHit = Physics.SphereCast(transform.position, 0.5f, transform.up * -1.05f, out hit);  //プレイヤーの真下にRaycast
        if (isHit)  //Raycastに何かヒットしたとき
        {
            if (!is_ground)  //それが地面だったとき
            {
                ASs[0].Play();  //着地音を鳴らす
            }
            is_ground = true;  //地面に接している状態と記録する
            touching_wall = false;  //壁に接していない状態と記録する
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
            if (forward > 0)  //プレイヤーが前を向いているとき
            {
                jump_distance = point.z - transform.position.z;  //ジャンプする位置はプレイヤーの位置から棒が当たった場所の半分の位置にする
                debug_sphere2.transform.position = new Vector3(0, 0, transform.position.z + (jump_distance / 2.0f));  //デバッグ用
            }
            else  //プレイヤーが後ろを向いてるとき
            {
                jump_distance = transform.position.z - point.z;  //ジャンプする位置はプレイヤーの位置から棒が当たった場所の半分の位置にする
                debug_sphere2.transform.position = new Vector3(0, 0, transform.position.z - (jump_distance / 2.0f));  //デバッグ用
            }


            bo_fake.transform.localScale = (Vector3.Distance(transform.position, bo_end_point) / 13.0f) * new Vector3(1, 1, 1);  //棒の見た目を伸ばす

            float distance = Mathf.Infinity;  //棒の当たった位置とプレイヤーの距離を格納する変数
            while (true)  //プレイヤーがジャンプする位置に来るまで
            {
                yield return null;  //１フレーム待つ
                //Debug.Log("jump_distance");
                if (forward > 0)  //プレイヤーが前を向いているとき
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
            if (jump_force * jump_distance > max_jump_force)  //ジャンプ力が大きすぎそうなとき
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
    }

    public IEnumerator WallJump(Vector3 point)  //壁ジャンプの一連の処理
    {
        //float w_jump_distance = Vector3.Distance(transform.position, point);
        rb.velocity = Vector3.zero;  //プレイヤーの速度を0にする
        debug_sphere.transform.position = point;  //デバッグ用
        Vector3 w_jump_vector = new Vector3(0, point.y - transform.position.y, point.z - transform.position.z);  //ジャンプする方向を棒を突き出した方向にする
        //Debug.Log(w_jump_vector);
        debug_sphere2.transform.position = transform.position - w_jump_vector;  //デバッグ用
        rb.AddForce(-w_jump_vector.normalized * wall_jump_force, ForceMode.Impulse);  //指定した方向にジャンプする
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)  //プレイヤーに何か当たったとき
    {
        if (!is_ground)  //接地していたら
        {
            touching_wall = true;  //壁に触っていることにする
        }

        if (collision.transform.tag == "Enemy")  //当たったのが敵だったら
        {
            hp--;  //HPを１減らす
            images[hp].SetActive(false);  //HP表示のハートを一つ非表示にする
            if (hp == 0)  //HPが0になったら
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
        //eyes[0].SetActive(false);  //プレイヤーキャラクターの目を瞑らせる
        //eyes[1].SetActive(false);
        //eyes[2].SetActive(true);
        //eyes[3].SetActive(true);
        ASs[1].Play();  //ダメージ音を再生

        yield return new WaitForSeconds(1.0f);  //１秒無敵時間を継続
        gameObject.layer = 0;  //プレイヤーの無敵状態を解除
        //eyes[0].SetActive(true);
        //eyes[1].SetActive(true);
        //eyes[2].SetActive(false);
        //eyes[3].SetActive(false);
    }

    private void GoToGameOver()
    {

        var dof = ScriptableObject.CreateInstance<DepthOfField>();
        dof.focusDistance.Override(0.1f);
        PostProcessManager.instance.QuickVolume(postProcessGameObject.layer, 1, dof);

    }
}
