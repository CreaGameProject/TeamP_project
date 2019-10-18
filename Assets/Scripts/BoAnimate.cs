using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoAnimate : MonoBehaviour
{
    [SerializeField]
    private Transform bo_end;

    private float angle = 0.0f;

    [SerializeField]
    private float bo_size = 14.0f;

    private GameObject player_go;

    [SerializeField]
    private GameObject parent_go;

    private float distance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        player_go = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        angle = Vector3.Angle(transform.position - bo_end.position,transform.up);
        Debug.Log(angle);
        transform.rotation = Quaternion.Euler(0, 90, angle);
        parent_go.transform.LookAt(player_go.transform.position);

        distance = Vector3.Distance(player_go.transform.position, transform.position);
        parent_go.transform.localScale = new Vector3(1, 1, distance / bo_size);
    }
}
