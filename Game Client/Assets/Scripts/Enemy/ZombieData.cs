using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieData : MonoBehaviour
{
    public int currentHP;
    private GameUI gameUI;

    Animator zom_anim;
    ZombieController zombieController;
    ZombieUi zombieUi;
    public GameObject ammoBox;
    public int entity_id = -1;

    void Awake()
    {
        zom_anim = GetComponent<Animator>();
        gameUI = GameObject.FindGameObjectWithTag("GameUI").GetComponent<GameUI>();
        zombieController = GetComponent<ZombieController>();
        zombieUi = GetComponentInChildren<ZombieUi>();
        currentHP = GameSetting.Zombie_HP;
    }

    void Update()
    {
    }

    public void Buckle_blood(int blood, Vector3 hitPoint)
    {
        if(currentHP <= 0)
            return;
        currentHP -= blood;
        gameUI.Update_EnemyHP(currentHP);
        if(blood > 0 && currentHP > 0)
        {
            zombieController.hitPar(hitPoint);
            zombieUi.Show(blood);
        }
        if(currentHP <= 0)
        {
            Death ();
            zombieController.playerData.AddScore(10);
            if(GameSetting.isConnect)
            {
                byte[] data = MsgHandler.ZombieDeathStream(entity_id);
                StartCoroutine(GameManager.host.SendServer(data));
            }
        }
    }

    public void Death()
    {
        zom_anim.SetTrigger ("Death");
        Instantiate(ammoBox, transform.position + new Vector3(0, 0.2f, 0), transform.rotation);
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<SphereCollider>());
        Destroy(gameObject, GameSetting.Zombie_Destroy);
    }
}
