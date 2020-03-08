using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;


public class UIControler : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject RetryButton;
    public GameObject EndButton;
    public GameObject TitleImage;
    public GameObject Life;

    [SerializeField]
    PostProcessVolume postProcessVolume;  //PostProcessの適用を管理してるクラス
    PostProcessProfile postProcessProfile;  //適用する内容が保存されてるクラス

    DepthOfField dof;  //DepthOfFieldの設定の内容が入るクラス

    private Player player_sc;  //Playerクラス継承用
    private bool is_gameover = false;  //ゲームオーバー時の処理を一回のみ行うための変数

    public AudioClip gameoverSE;


    public AudioClip startSE;
    AudioSource audioSource;

    

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player_sc = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();  //Playerクラスを取得
        postProcessProfile = postProcessVolume.profile;  //PostProcessProfileを取得
        dof = postProcessProfile.GetSetting<DepthOfField>();  //DepthOfFieldを取得
        dof.focusDistance.Override(0.1f);  //ぼかしをかける
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーのHPが0になったとき一度だけ画面をぼかす
        if (player_sc.hp == 0 && !is_gameover)
        {
            audioSource.PlayOneShot(gameoverSE);
            is_gameover = true;
            dof.focalLength.Override(0.1f);
        }
    }
    public void Startclick()
    {
        audioSource.PlayOneShot(startSE);
        StartButton.SetActive(false);
        EndButton.SetActive(false);
        TitleImage.SetActive(false);
        Life.SetActive(true);
        FixDOF();
    }
    public void Retryclick()
    {
        SceneManager.LoadScene("Stage1");
    }

    //ぼかすのを治す
    void FixDOF()
    {
        dof.focusDistance.Override(10);
    }
    //ゲーム終了
    public void EndGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
		Application.Quit();
    }

}
