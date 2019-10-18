using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zako2 : MonoBehaviour
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

    private bool is_active = true;

    private bool is_attacked = false;

    private float offset;

    public GameObject hit_particle;

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rend.isVisible && is_active)
        {
            moved_pos = new Vector3(start_pos.x, start_pos.y + (Mathf.Sin(t) * move_haight), start_pos.z + (Mathf.Sin(t / 4) * move_width));
            rb.position = moved_pos;

            //rb.velocity = new Vector3(0, Mathf.Sin(t) * move_haight, Mathf.Sin(t) * move_width);
        }

        t += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
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

        if (collision.transform.tag == "Enemy")
        {
            if (collision.transform.GetComponent<Zako2>())
            {
                if (collision.transform.GetComponent<Zako2>().is_attacked)
                {
                    is_active = false;
                    is_attacked = true;
                }
            }
        }
    }
}
