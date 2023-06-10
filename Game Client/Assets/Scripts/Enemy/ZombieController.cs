using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    NavMeshAgent nav;
    Animator zom_anim; 
    ZombieData zombieData;
    ParticleSystem hitParticles;
    private float timeBetweenAttacks = 0.5f;

    bool playerInRange;
    float timer;
    float tick;
    public Vector3 target;

    public PlayerData playerData;

    void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerData = player.transform.GetComponent<PlayerData>();
        playerController = player.transform.GetComponent<PlayerController>();
        hitParticles = GetComponentInChildren<ParticleSystem>();             // 不能用Find 会出错
        zom_anim = GetComponent<Animator>();
        zombieData = GetComponent<ZombieData>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Move();
        if(playerInRange)                                                    // 接触一段时间后才攻击 掉血
            timer += Time.deltaTime;
        else
            timer = 0f;

        if(timer >= timeBetweenAttacks && playerInRange && zombieData.currentHP > 0)
            Zombie_Attack();
        SendZombieData();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
           playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
            playerInRange = false;
    }

    void Move()
    {
        if(zombieData.currentHP > 0 && playerData.currentHP > 0 && !GameSetting.isConnect)                      //   enemyHealth.currentHealth > 0 && 
            nav.SetDestination(player.transform.position);
        if(zombieData.currentHP > 0 && GameSetting.isConnect)
            nav.SetDestination(target);
        else if(zombieData.currentHP > 0 && playerData.currentHP <= 0)
        {
            zom_anim.SetTrigger("Idle");
            nav.enabled = false;
        }
    }

    void Zombie_Attack()
    {
        timer = 0f;
        if(playerData.currentHP > 0)
        {
            zom_anim.SetTrigger("Attack");
            playerController.Damage(GameSetting.Zombie_Power);
        }
    }

    public void hitPar(Vector3 Pos)
    {
        hitParticles.transform.position = Pos;
        hitParticles.Play();
    }

    void SendZombieData()
    {
        tick += Time.deltaTime;
        if(GameSetting.isConnect && tick > GameSetting.Zombie_Tick && zombieData.currentHP > 0)
        {
            float[] pos = new float[3] {transform.position.x, transform.position.y, transform.position.z};
            byte[] data = MsgHandler.ZombieDataStream(zombieData.entity_id, zombieData.currentHP, pos);
            StartCoroutine(GameManager.host.SendServer(data));
        }
    }
}
