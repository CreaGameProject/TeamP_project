using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player_go;  //プレイヤーのオブジェクトを格納する変数
    [SerializeField]
    private Vector3 offset = Vector3.zero;  //プレイヤーとカメラの相対位置を指定するための変数
    private Vector3 pos = Vector3.zero;  //カメラの位置を格納する変数

    [SerializeField]
    private float z_limit1 = 10.0f;  //画面左端の位置
    [SerializeField]
    private float z_limit2;  //何のための変数か忘れた
    [SerializeField]
    private float z_limit3 = 208.0f;  //画面右端の位置
    // Start is called before the first frame update
    void Start()
    {
        player_go = GameObject.FindGameObjectWithTag("Player");  //プレイヤーのオブジェクトを取得する
    }

    // Update is called once per frame
    void Update()
    {
        pos = new Vector3(player_go.transform.position.x + offset.x, offset.y, player_go.transform.position.z + offset.z);  //カメラの仮位置を取得
        if(pos.z < z_limit1)  //仮の位置が画面右端でないとき
        {
            pos += Vector3.forward * (z_limit1 - pos.z);  //カメラの位置を右に指定
        }
        if(pos.z > z_limit3)  //仮の位置が画面左端でないとき
        {
            pos -= Vector3.forward * (pos.z - z_limit3);  //カメラの位置を左に指定
        }
        transform.position = pos;  //指定した位置にカメラを動かす
    }
}
