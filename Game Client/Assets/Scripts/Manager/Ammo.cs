using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    private GameObject player;
    private PlayerData playerData;

    private float timer;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag ("Player");
        playerData = player.transform.GetComponent<PlayerData>();      
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > GameSetting.AmmoBox_Destroy)
            Destroy(gameObject, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            playerData.AddBullet(GameSetting.AmmoBox_Bullet);
            Destroy(gameObject, 0);
        }
    }
}
