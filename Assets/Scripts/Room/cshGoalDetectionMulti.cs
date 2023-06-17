using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using Photon.Pun;
using Photon.Realtime;


public class cshGoalDetectionMulti : MonoBehaviourPunCallbacks
{
    private Vector3 initialPosition;
    private Rigidbody ballRb;   // add
    public GameObject ball;
    public Text redscoreText; // UI Text 컴포넌트를 가리키는 변수
    public Text yelloscoreText; // UI Text 컴포넌트를 가리키는 변수
    private int redScore = 0; // 빨간색 점수
    private int yellowScore = 0; // 노란색 점수


    private bool stopFlag = false;

    public GameObject[] players;
    public GameObject spawnP1;
    public GameObject spawnP2;

    public AudioSource whistleAudioSource;
    public AudioSource goalAudioSource;

    public Text gameInfoText;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        initialPosition = ball.transform.position;
        ballRb = ball.GetComponent<Rigidbody>();   // add
        setRedText();
        setYelloText();

        stopFlag = true;
    }

    // This function is called when a new player enters the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CheckPlayerCount();
    }

    // This function is called when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckPlayerCount();
    }

    private void CheckPlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null) // Ensure we are in a room
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            if (playerCount == 2)
            {
                //MultiGameStart();
                photonView.RPC("MultiGameStart", RpcTarget.AllBuffered);
            }
        }
    }
    [PunRPC]
    private void MultiGameStart() 
    {
        Invoke("WhistlePlay", 3f);

        players = GameObject.FindGameObjectsWithTag("Player");
        
        ResetGame();
    }

    
    IEnumerator StartGameText()
    {
        photonView.RPC("UpdateGameInfoText", RpcTarget.All, "3");
        yield return new WaitForSeconds(1);
        photonView.RPC("UpdateGameInfoText", RpcTarget.All, "2");
        yield return new WaitForSeconds(1);
        photonView.RPC("UpdateGameInfoText", RpcTarget.All, "1");
        yield return new WaitForSeconds(1);
        photonView.RPC("UpdateGameInfoText", RpcTarget.All, "");
    }

    [PunRPC]
    public void GetRedScore(int mount)
    {
        redScore += mount;
        setRedText();
        if (redScore >= 5)
        {
            EndGame("Red Team");
        }

    }

    [PunRPC]
    public void GetYelloScore(int mount)
    {
        yellowScore += mount;
        setYelloText();
        if (yellowScore >= 5)
        {
            EndGame("BLUE Team");
        }

    }

    public void setRedText() 
    { 
        redscoreText.text = "Score : " + redScore.ToString();
    }
    
     public void setYelloText() 
    {    
        yelloscoreText.text = "Score : " + yellowScore.ToString();
    }

    private void EndGame(string winningTeam) 
    {    
        Debug.Log(winningTeam + " 이 게임에 승리 하였습니다 ! ! !");
        photonView.RPC("UpdateGameInfoText", RpcTarget.All, winningTeam + " Win!");

        // 1초 후에 Second 씬을 로드합니다.
        Invoke("LoadSecondScene", 5f);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    private void LoadSecondScene()
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


    private void OnCollisionEnter(Collision collision)
    {
        if (stopFlag) { return; }

        if (PhotonNetwork.IsMasterClient) 
        {
            if (collision.gameObject.tag == "goal")
            {
                
                //goalAudioSource.Play();
                photonView.RPC("GoalPlayRPC", RpcTarget.All);
                Debug.Log("!! Red Team Goal !! !");
                photonView.RPC("UpdateGameInfoText", RpcTarget.All, "Goal!");
            // transform.position = initialPosition; // 위치 초기화
                Invoke("ResetGame", 5f);
                stopFlag = true;
                //GetRedScore(1);
                photonView.RPC("GetRedScore", RpcTarget.AllBuffered, 1);
                //transform.position = initialPosition; // 위치 초기화

                // Instantiate(particlePrefab, transform.position, Quaternion.identity);
                // Instantiate(particlePrefab2, transform.position, Quaternion.identity);
            }
            if (collision.gameObject.tag == "goal2")
            {
                //goalAudioSource.Play();
                photonView.RPC("GoalPlayRPC", RpcTarget.All);
                Debug.Log("!! Yello Team Goal !! !");
                photonView.RPC("UpdateGameInfoText", RpcTarget.All, "Goal!");
            // transform.position = initialPosition; // 위치 초기화
                Invoke("ResetGame", 5f);
                stopFlag = true;
                //GetYelloScore(1);
                photonView.RPC("GetYelloScore", RpcTarget.AllBuffered, 1);
                //transform.position = initialPosition; // 위치 초기화

                // Instantiate(particlePrefab, transform.position, Quaternion.identity);
                // Instantiate(particlePrefab2, transform.position, Quaternion.identity);
            }
        }
    }
    // add
    private void ResetGame() 
    {
        photonView.RPC("ResetGameRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ResetGameRPC()
    {
        ball.transform.position = initialPosition; // 위치 초기화
        ballRb.velocity = Vector3.zero; // 선형 속도 초기화
        ballRb.angularVelocity = Vector3.zero; // 각 속도 초기화
        
        // TODO : user 위치 초기화
        // TODO : 게임 3초 멈추기.


        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        
        foreach(GameObject player in players) 
        {
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (PhotonNetwork.IsMasterClient) // If this is the character the local player controls
            {
                Debug.Log("player is Host");
                player.transform.position = spawnP1.transform.position;
                player.transform.rotation = spawnP1.transform.rotation;
            }
            else
            {
                Debug.Log("player is User");
                player.transform.position = spawnP2.transform.position;
                player.transform.rotation = spawnP2.transform.rotation;
            }
            Debug.Log("playerDisabled");
            player.GetComponent<cshControllerMulti>().DisableMovementForSeconds(3.0f);
        }
        
        Invoke("WhistlePlay", 3f);
        StartCoroutine(StartGameText());

        stopFlag = false;
    }

    private void WhistlePlay() 
    {
        //photonView.RPC("WhistlePlayRPC", RpcTarget.All);
        whistleAudioSource.Play();
        Debug.Log("whistlePlayed");
    }

    // [PunRPC]
    // private void WhistlePlayRPC() 
    // {
    //    whistleAudioSource.Play();
    // }

    [PunRPC]
    private void GoalPlayRPC() 
    {
        goalAudioSource.Play();
    }


    [PunRPC]
    void UpdateGameInfoText(string newText) 
    {
        gameInfoText.text = newText;
    }
    
}
//  public Text redscoreText; // UI Text 컴포넌트를 가리키는 변수
//     public Text yelloscoreText; // UI Text 컴포넌트를 가리키는 변수
//     private int redScore = 0; // 빨간색 점수
//     private int yellowScore = 0; // 노란색 점수


//     private void Start()
//     {
//         setRedText();
//         setYelloText();
//     }

//     public void GetRedScore(int mount)
//     {
//         redScore += mount;
//         setRedText();

//     }
//      public void GetYelloScore(int mount)
//     {
//         yellowScore += mount;
//         setYelloText();

//     }

//     public void setRedText() 
//     { 
//         redscoreText.text = "Score : " + redScore.ToString();
//     }
//      public void setYelloText() 
//     {    
//         yelloscoreText.text = "Score : " + yellowScore.ToString();
//     }

//     private void OnCollisionEnter(Collision coll)
//     {
//         if (coll.gameObject.tag == "SelectableObj")
//         {
//             if (coll.gameObject.tag == "goal")
//             {
//                 GetRedScore(1); // 닿은 공 오브젝트가 goal 태그를 가진 경우 빨간색 점수를 1 증가시킴
//             }
//             else if (coll.gameObject.tag == "goal2")
//             {
//                 GetYelloScore(1); // 닿은 공 오브젝트가 goal2 태그를 가진 경우 노란색 점수를 1 증가시킴
//             }
//         }
//     }
