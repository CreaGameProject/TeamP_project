using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player_go;
    [SerializeField]
    private Vector3 offset = Vector3.zero;
    private Vector3 pos = Vector3.zero;

    [SerializeField]
    private float z_limit1 = 10.0f;
    [SerializeField]
    private float z_limit2;
    [SerializeField]
    private float z_limit3 = 208.0f;
    // Start is called before the first frame update
    void Start()
    {
        player_go = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        pos = new Vector3(player_go.transform.position.x + offset.x, offset.y, player_go.transform.position.z + offset.z);
        if(pos.z < z_limit1)
        {
            pos += Vector3.forward * (z_limit1 - pos.z);
        }
        if(pos.z > z_limit3)
        {
            pos -= Vector3.forward * (pos.z - z_limit3);
        }
        transform.position = pos;
    }
}
