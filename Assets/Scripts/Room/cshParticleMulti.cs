using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class cshParticleMulti : MonoBehaviourPun
{
    public GameObject prefab;
    public GameObject prefab2;
    public GameObject particleSpawnPoint; // 파티클 생성 위치
    public GameObject particleSpawnPoint2; // 파티클 생성 위치


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "goal")
        {
            photonView.RPC("InstantiateParticles", RpcTarget.All, particleSpawnPoint2.transform.position, 1);
        }
        else if (collision.gameObject.tag == "goal2")
        {
            photonView.RPC("InstantiateParticles", RpcTarget.All, particleSpawnPoint.transform.position, 2);
        }
    }

    [PunRPC]
    void InstantiateParticles(Vector3 position, int type)
    {
        GameObject prefabToInstantiate = (type == 1) ? prefab : prefab2;
        Instantiate(prefabToInstantiate, position, Quaternion.identity);
    }
}
