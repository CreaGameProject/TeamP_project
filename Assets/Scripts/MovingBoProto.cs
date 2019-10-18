using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBoProto : MonoBehaviour
{
    //このスクリプトは子オブジェクトに棒オブジェクトを持ったプレイヤーオブジェクトの子オブジェクトである空オブジェクトにアタッチする
    [SerializeField]
    private GameObject bo; //動かす棒のオブジェクト
    [SerializeField]
    private float move_time = 0.4f; //棒が動く時間
    private float dead_value = 0.4f;
    private bool enable_move = true; //操作を受け付けるかどうか
    private bool pushing_now = false; //棒を押しだす状態かどうか
    private bool pulling_now = false; //棒を引く状態かどうか
    private Rigidbody bo_rb; //棒オブジェクトのRigidBody
    [SerializeField]
    private float speed = 2.0f; //棒を動かすスピード
    [SerializeField]
    private float wait_time = 0.4f;
    private GameObject player_go;
    private Rigidbody player_rb;
    private Player player_sc;
    [SerializeField]
    private GameObject hit_go;
    private BoHit hit_sc;

    private Vector3 move_pos = Vector3.zero;
    private Vector3 default_pos;

    private GameObject[] enemies;

    [SerializeField]
    private float auto_angle = 20.0f;

    [SerializeField]
    private GameObject bo_fake;

    // Start is called before the first frame update
    void Start()
    {
        player_go = GameObject.FindGameObjectWithTag("Player");
        bo_rb = bo.transform.GetComponent<Rigidbody>(); //子オブジェクトのRigidBodyを取得
        player_rb = player_go.GetComponent<Rigidbody>();
        player_sc = player_go.GetComponent<Player>();
        hit_sc = hit_go.GetComponent<BoHit>();
        default_pos = transform.localPosition;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input_vector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); //画面サイズに関係なくマウスの移動量を取得
        Vector2 joystick_vector = new Vector2(Input.GetAxis("Horizontal R"), Input.GetAxis("Vertical R"));
        //Debug.Log(joystick_vector);
        if (input_vector.magnitude > dead_value && enable_move && Judge_Front(input_vector.x)) //マウスの移動量の距離が0.4より大きくてかつ、enable_moveがtrueかどうか
        {
            StartCoroutine(Switch_State(SerchEnemy(Direction()))); //下のSwitch_Stateコルーチンを呼び出し、引数にマウス移動量を与える
        }

        if(joystick_vector.magnitude > dead_value && enable_move && Judge_Front(joystick_vector.x))
        {
            StartCoroutine(Switch_State(SerchEnemy(DirectionJoystick())));
        }

        if (pushing_now) //棒を押す状態のとき
        {
            //bo_rb.velocity = player_rb.velocity + bo.transform.up * speed; //棒オブジェクトを一定の速度にする
            //bo_rb.AddForce(player_rb.velocity + bo.transform.up * speed, ForceMode.VelocityChange);
            move_pos += bo.transform.up * speed * Time.deltaTime;
            Bo_trans();
        }
        else if (pulling_now) //棒を引く状態のとき
        {
            //bo_rb.velocity = player_rb.velocity + bo.transform.up * -speed; //棒オブジェクトを押すときの反対方向に一定の速度にする
            //bo_rb.AddForce(player_rb.velocity + bo.transform.up * -speed, ForceMode.VelocityChange);
            move_pos -= bo.transform.up * speed * Time.deltaTime;
            Bo_trans();
        }
        else
        {
            bo.transform.localPosition = Vector3.zero; //棒が動いていないときは最初の位置に戻す
            move_pos = Vector3.zero;
            bo.transform.localScale = Vector3.zero;
        }
    }

    private Vector3 Direction()
    {
        int flames = 10;
        Vector2 movement_of_mouse = Vector2.zero;
        Vector2 input_of_mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
        if (enable_move)
        {
            for(int n = 0; n < flames; n++)
            {
                input_of_mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                input_of_mouse = input_of_mouse.normalized;
                movement_of_mouse += input_of_mouse;
            }
            movement_of_mouse /= flames;
            //Debug.Log(movement_of_mouse);
            return movement_of_mouse;
        }
        else
        {
            Debug.Log(enable_move);
            return movement_of_mouse;
        }
    }

    private Vector3 DirectionJoystick()
    {
        int flames = 10;
        Vector2 movement_of_joystick = Vector2.zero;
        Vector2 input_of_joystick = new Vector2(Input.GetAxis("Horizontal R"), Input.GetAxis("Vertical R"));

        if (enable_move)
        {
            for (int n = 0; n < flames; n++)
            {
                input_of_joystick = new Vector2(Input.GetAxis("Horizontal R"), Input.GetAxis("Vertical R"));
                movement_of_joystick += input_of_joystick;
            }
            movement_of_joystick /= flames;
            return movement_of_joystick;
        }
        else
        {
            return movement_of_joystick;
        }
    }

    private bool Judge_Front(float mouse_input_x)
    {
        if(mouse_input_x * player_sc.forward > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private IEnumerator Switch_State (Vector2 input_v)
    {
        // Debug.Log(input_v); //マウスの移動量を表示する
        enable_move = false; //操作を受け付けなくする
        pushing_now = true; //棒を押している状態にする
        hit_sc.enable_hit = true;
        player_sc.enable_turn = false;
        transform.LookAt(transform.position + new Vector3(0, input_v.y, input_v.x)); //空オブジェクトを回転させて棒を目的の方向に向ける
        yield return new WaitForSeconds(move_time / 2.0f); //棒が動く時間の半分の時間待つ
        pushing_now = false; //棒を押している状態ではなくする
        pulling_now = true; //棒を引いている状態にする
        bo.transform.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(move_time / 2.0f); //棒が動く時間の半分の時間待つ
        bo.transform.GetComponent<CapsuleCollider>().enabled = true;
        pulling_now = false; //棒を引いている状態ではなくする
        yield return new WaitForSeconds(wait_time);
        enable_move = true; //操作を受け付けるようにする
        hit_sc.enable_hit = false;
        player_sc.enable_turn = true;
    }

    private Vector2 SerchEnemy(Vector3 input_vec)
    {
        float angle_min = auto_angle;
        Vector2 for_return = Vector2.zero;
        foreach(GameObject e_go in enemies)
        {
            Vector3 pos_v = e_go.transform.position - transform.position;
            if (pos_v.magnitude < 10.0f)
            {
                float angle = Vector2.Angle(new Vector2 (pos_v.z, pos_v.y), input_vec);
                //Debug.Log(angle);
                if (angle < angle_min)
                {
                    angle_min = angle;
                    
                    for_return = new Vector2(pos_v.z, pos_v.y).normalized;
                }
            }
        }
        if(angle_min != auto_angle)
        {
            //Debug.Log(for_return);
            return for_return;
        }
        else
        {
            return input_vec;
        }
    }

    private void Bo_trans()
    {
        bo_rb.MovePosition(player_go.transform.position + default_pos + move_pos);
        if (!player_sc.bo_lock)
        {
            bo_fake.transform.position = (transform.position + bo.transform.position) / 2.0f;
            bo_fake.transform.LookAt(player_go.transform.position);
        }
        bo_fake.transform.localScale = (Vector3.Distance(player_go.transform.position, bo.transform.position) / 13.0f) * new Vector3(1, 1, 1);
    }
}
