using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public bool enable_turn= true;

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
            if(rb.velocity.z < 0.01f && rb.velocity.z > -0.01f)
            {
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
                GoToGameOver();
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

    private void GoToGameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}
