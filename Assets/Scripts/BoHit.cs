using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoHit : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject player_go;
    private Player player_sc;
    public bool enable_hit = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        player_go = GameObject.FindGameObjectWithTag("Player");
        player_sc = player_go.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (enable_hit)
        {
            if (other.tag == "Terrain")
            {
                enable_hit = false;
                if (player_sc.is_ground)
                {
                    player_sc.StartCoroutine("Jump_Set", other.ClosestPoint(transform.position));
                    //Debug.Log("Go To Coroutine");
                }
                else
                {
                    player_sc.StartCoroutine("WallJump", other.ClosestPoint(transform.position));
                }
            }
        }
    }
}
