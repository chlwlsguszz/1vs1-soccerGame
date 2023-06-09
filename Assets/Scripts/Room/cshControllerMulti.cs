using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
//using UnityStandardAssets.utility;
//using UnityEngine.SceneManagement;

public class cshControllerMulti : MonoBehaviourPunCallbacks
{
    public float m_moveSpeed = 2.0f;
    public GameObject ball;
    public float shootingPower = 0.5f; // And this
    public float shootingDistance = 2f; // 슈팅 가능 거리
    public Material blueMaterial;

    private PhotonView pv;

    void Start() 
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // the host.
        }
        else
        {
            // This player is not the local player, so change its color to blue.
            this.GetComponent<Renderer>().material = blueMaterial;
        }
    }

    void Awake() 
    {
        pv = GetComponent<PhotonView>();

        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    void Update()
    {
        if (pv.IsMine) 
        {
            PlayerMove();

            // Add this block
            if(Input.GetKeyDown(KeyCode.Space) && IsBallInRange())
            {
                Debug.Log("shooting");
                ShootBall();
            }
        }
    }

    void PlayerMove()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = Vector3.right * h;
        Vector3 moveVertical = Vector3.forward * v;
        Vector3 velocity = (moveHorizontal + moveVertical).normalized;

        transform.LookAt(transform.position + velocity);

        transform.Translate(velocity * m_moveSpeed * Time.deltaTime, Space.World);
 	
    }
   void ShootBall()
    {
        // Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        // if(ballRb != null)
        // {
        //     Vector3 shootingDirection = transform.forward;
        //     ballRb.AddForce(shootingDirection * shootingPower, ForceMode.Impulse);
        // }
        pv.RPC("ShootBallRPC", RpcTarget.All);
    }

    bool IsBallInRange()
    {
        float distance = Vector3.Distance(transform.position, ball.transform.position);
        return distance <= shootingDistance;
    }

    [PunRPC]
    void ShootBallRPC()
    {
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if(ballRb != null)
        {
            Vector3 shootingDirection = transform.forward;
            ballRb.AddForce(shootingDirection * shootingPower, ForceMode.Impulse);
        }
    }

}
