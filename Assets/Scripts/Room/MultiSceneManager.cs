using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MultiSceneManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    public override void OnLeftRoom() 
    {
        if (GameManager.instance != null) 
        {
            Destroy(GameManager.instance.gameObject);
            GameManager.instance = null;  // Remember to nullify the instance to avoid further access.
        }
        if (PhotonNetwork.IsConnected) // Check if still connected
            {
                PhotonNetwork.Disconnect(); // Force disconnect if still connected
            }
        SceneManager.LoadScene("second"); // 여기서 "GameScene"은 게임 씬의 이름입니다.
    }
}
