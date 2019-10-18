using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleController : MonoBehaviour
{
    public GameObject enemy_prefab;

    public Transform sporn_point;

    [SerializeField]
    private float sporn_span = 0.2f;

    private bool call_enemy = false;

    private float t = 0.0f;

    private AudioSource audio_source1;
    private AudioSource audio_source2;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] audio_sources = Camera.main.GetComponents<AudioSource>();
        audio_source1 = audio_sources[0];
        audio_source2 = audio_sources[1];
    }

    // Update is called once per frame
    void Update()
    {
        if (call_enemy)
        {
            t += Time.deltaTime;
            if(t > sporn_span)
            {
                GameObject enemy = Instantiate(enemy_prefab, sporn_point.position, sporn_point.rotation);
                enemy.GetComponent<Zako1>().chase_player = true;
                //enemy.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, ), ForceMode.Impulse);
                t = 0.0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            Debug.Log("In the area!");
            call_enemy = true;
            audio_source1.Stop();
            audio_source2.Play();
            gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }
}
