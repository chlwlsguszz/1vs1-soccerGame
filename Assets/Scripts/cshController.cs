using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cshController : MonoBehaviour
{
    public float m_moveSpeed = 2.0f;
    public GameObject ball;
    public float shootingPower = 0.5f; // And this
    public float shootingDistance = 2f; // 슈팅 가능 거리

    public float h, v;

    public bool CanMove { get; set; } = true;  // Add this line

    public AudioSource shootingAudioSource;

    private Animator animator; // reference to the animator component

    private bool shootFlag = false;

    private void Awake()
    {
        animator = GetComponent<Animator>(); // get the animator component
    }

    void Update()
    {
        if (CanMove)
        {
            PlayerMove();

            // Add this block
            animator.SetBool("IsMoving", h != 0 || v != 0);
            //Debug.Log(h!=0 || v!=0);
            if(Input.GetKeyDown(KeyCode.Space) && !shootFlag)
            {
                animator.SetTrigger("Shoot");
                Invoke("ShootBall", 0.45f);
                shootFlag = true;
                //ShootBall();
            }

            // else
            // {
            //     animator.SetBool("isShooting", false); // set IsShooting to false
            // }
        }
    }

    void PlayerMove()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = Vector3.right * h;
        Vector3 moveVertical = Vector3.forward * v;
        Vector3 velocity = (moveHorizontal + moveVertical).normalized;

        transform.LookAt(transform.position + velocity);

        transform.Translate(velocity * m_moveSpeed * Time.deltaTime, Space.World);
 	
    }
   void ShootBall()
    {
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if(ballRb != null && IsBallInRange())
        {
            shootingAudioSource.Play();
            Vector3 shootingDirection = transform.forward;
            ballRb.AddForce(shootingDirection * shootingPower, ForceMode.Impulse);
        }
        animator.SetBool("IsShooting", false); // set IsShooting to false
        shootFlag = false;
    }

    bool IsBallInRange()
    {
        float distance = Vector3.Distance(transform.position, ball.transform.position);
        return distance <= shootingDistance;
    }

    public void DisableMovementForSeconds(float seconds)
    {
        StartCoroutine(DisableMovementCoroutine(seconds));
        animator.SetBool("IsMoving", false);
    }

    private IEnumerator DisableMovementCoroutine(float seconds)
    {
        CanMove = false;
        yield return new WaitForSeconds(seconds);
        CanMove = true;
    }

}
