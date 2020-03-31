using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zako1 : MonoBehaviour
{
    [SerializeField]
    private float move_width;
    [SerializeField]
    private float move_haight;
    [SerializeField]
    private MeshRenderer rend;

    private Vector3 start_pos;
    private Vector3 moved_pos;

    private float t = 0.0f;

    private Rigidbody rb;

    public bool is_active = true;

    public bool is_attacked = false;

    public bool is_boss = false;

    private float offset;

    public GameObject hit_particle;

    public bool chase_player = false;

    private GameObject player_go;

    // Start is called before the first frame update
    void Start()
    {
        //rend = GetComponent<MeshRenderer>();
        start_pos = transform.position;
        rb = GetComponent<Rigidbody>();
        offset = Random.Range(0.0f, 10.0f);
        t += offset;
        player_go = GameObject.FindGameObjectWithTag("Player");

        if (is_boss)
        {
            transform.LookAt(player_go.transform.position);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (is_boss && is_active)
        {
            rb.transform.position += (transform.forward * Time.deltaTime * 20.0f);
        }

        if (rend.isVisible && is_active)
        {
            moved_pos = new Vector3(start_pos.x, start_pos.y + (Mathf.Sin(t) * move_haight), start_pos.z + (Mathf.Sin(t / 4) * move_width));
            rb.position = moved_pos;

            //rb.velocity = new Vector3(0, Mathf.Sin(t) * move_haight, Mathf.Sin(t) * move_width);
        }

        if (is_attacked)
        {
            gameObject.layer = 11;
        }

        t += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (is_attacked == false)
        {
            if (collision.transform.tag == "Bo-")
            {
                //Debug.Log("Hit!!");
                //rb.isKinematic = false;
                is_active = false;
                is_attacked = true;
                GameObject particle = Instantiate(hit_particle, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
                Destroy(particle, 0.9f);
            }

        }

        if (is_attacked == true)
        {
            if (collision.transform.tag == "Enemy")
            {
                if (collision.transform.GetComponent<Zako1>())
                {
                    if (collision.transform.GetComponent<Zako1>().is_attacked)
                    {
                        is_active = false;
                        is_attacked = true;
                    }
                }

                else if (collision.transform.GetComponent<Boss>())
                {
                    //ボスのHPを1減らしたい　関数を作って呼び出す？
                    Destroy(this.gameObject, 3.0f); //プレハブクローンをデストロイしたいけど出来てるのか？
                }
            }
        }
    }
}
