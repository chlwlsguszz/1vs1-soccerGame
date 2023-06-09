using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public PhotonView pv;

    public Transform[] spawnPoints;

    // public Text scorePlayer1Text;
    // public Text scorePlayer2Text;

    // private int scorePlayer1 = 0;
    // private int scorePlayer2 = 0;

    private Hashtable CP;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        CP = PhotonNetwork.LocalPlayer.CustomProperties;
        CreatePlayer();
    }

    void CreatePlayer()
    {
        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        Vector3 pos = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].position;
        Quaternion rot = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].rotation;

        GameObject playerTemp = PhotonNetwork.Instantiate("Player_Multi", pos, rot, 0);

        // int colorNum = (int)CP["Color"];
        // Debug.Log($"colorNum : {colorNum}");
        // if (colorNum != -1)
        // {
        //     playerTemp.GetComponent<PlayerCtrl>().InitColor(colorNum - 1);
        // }
    }

    // public void GetScore(int playerNum)
    // {
    //     pv.RPC(nameof(SetScore), RpcTarget.AllViaServer, playerNum);
    // }

    // [PunRPC]
    // public void SetScore(int playerNum)
    // {
    //     if (playerNum == 1)
    //     {
    //         scorePlayer1++;
    //         scorePlayer1Text.text = $"{scorePlayer1:00}";
    //     }
    //     else
    //     {
    //         scorePlayer2++;
    //         scorePlayer2Text.text = $"{scorePlayer2:00}";
    //     }
    // }
}