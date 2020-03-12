using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfootSE : MonoBehaviour
{
    [SerializeField]
    private AudioSource AS;

    [SerializeField]
    private AudioClip footSE1;

    [SerializeField]
    private AudioClip footSE2;
    public void SE1()
    {
        AS.PlayOneShot(footSE1);
    }


    public void SE2()
    {
        AS.PlayOneShot(footSE2);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
