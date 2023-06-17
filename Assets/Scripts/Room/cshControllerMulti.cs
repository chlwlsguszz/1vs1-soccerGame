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

    public bool CanMove { get; set; } = true;  // Add this line

    public AudioSource shootingAudioSource;

    void Start() 
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (pv.IsMine)
            {
                // the host.
            }
            else if(!pv.IsMine)
            {
                // the user.
                this.GetComponent<Renderer>().material = blueMaterial;
            }
        }
        else
        {
            if (pv.IsMine)
            {
                // the user.
                this.GetComponent<Renderer>().material = blueMaterial;
            }
            else if(!pv.IsMine)
            {
                // the host.
            }
        }

        shootingAudioSource = GameObject.FindGameObjectWithTag("shootingSound").GetComponent<AudioSource>();
    }

    void Awake() 
    {
        pv = GetComponent<PhotonView>();

        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    void Update()
    {
        if (CanMove && pv.IsMine) 
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

    public void DisableMovementForSeconds(float seconds)
    {
        StartCoroutine(DisableMovementCoroutine(seconds));
    }

    private IEnumerator DisableMovementCoroutine(float seconds)
    {
        CanMove = false;
        yield return new WaitForSeconds(seconds);
        CanMove = true;
    }

    [PunRPC]
    void ShootBallRPC()
    {
        shootingAudioSource.Play();
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if(ballRb != null)
        {
            Vector3 shootingDirection = transform.forward;
            ballRb.AddForce(shootingDirection * shootingPower, ForceMode.Impulse);
        }
    }

}
