using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    PlayerData playerData;
    private float produceTime = GameSetting.Zombie_ProduceTime;
    public GameObject zombiePrefab;
    public Transform[] spawnPos;

    void Awake ()
    {
        playerData = GameObject.FindGameObjectWithTag ("Player").transform.GetComponent<PlayerData>();
        InvokeRepeating ("Produce", produceTime, produceTime);
    }

    void Produce ()
    {
        if(playerData.currentHP > 0f && GameSetting.isBegin && !GameSetting.isConnect)
        {
            int spawnPointIndex = Random.Range(0, spawnPos.Length);
            Instantiate (zombiePrefab, spawnPos[spawnPointIndex].position, spawnPos[spawnPointIndex].rotation);
        }
    }
}
