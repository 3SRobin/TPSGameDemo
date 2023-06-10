using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int currentHP;
    public int score;
    public GameUI gameUI;

    Animator Player_anim;
    AudioSource playerAudio;
    string playerName;

    public int bullet = GameSetting.Gun_Bullet;
    public int additionalBullet = GameSetting.Gun_Additional_Bullet;
    bool isDead;

    void Awake ()
    {
        Player_anim = GetComponent <Animator> ();
        gameUI = GameObject.FindGameObjectWithTag("GameUI").GetComponent<GameUI>(); 
        currentHP = GameSetting.Player_HP;
        score = 0;
    }

    void Start()
    {
        SetBullet(bullet, additionalBullet);
    }

    void Update ()
    {
    }

    public void Reload_Bullet()
    {
        additionalBullet += bullet;
        if(additionalBullet >= GameSetting.Gun_Bullet)
        {
            bullet = GameSetting.Gun_Bullet;
            additionalBullet -= GameSetting.Gun_Bullet;
        }
        else
        {
            bullet = additionalBullet;
            additionalBullet = 0;
        }
        gameUI.Updaet_Bullet(bullet, false);
        gameUI.SetAdditionalBullet(additionalBullet);

    }

    public void Updaet_Bullet()
    {
        bullet -= 1;
        gameUI.Updaet_Bullet(bullet, true);
    }

    public void AddBullet(int count)
    {
        additionalBullet += count;
        gameUI.SetAdditionalBullet(additionalBullet);
    }

    public void Buckle_blood(int blood)
    {
        currentHP -= blood;
        gameUI.Update_PlayerHP(currentHP);
        Player_anim.SetTrigger("Attack");
        if(currentHP <= 0 && !isDead)
            Death ();
    }

    void Death ()
    {
        isDead = true;
        Player_anim.SetTrigger("Death");
    }

    public void AddScore(int s)
    {
        score += s;
        gameUI.SetScore(score);
    }

    public void SetScore(int s)
    {
        score = s;
        gameUI.SetScore(s);
    }

    public void SetBullet(int b, int ab)
    {
        bullet = b;
        additionalBullet = ab;
        gameUI.SetBulletMax(b);
        gameUI.SetAdditionalBullet(ab);
    }

    public void SetBlood(int a)
    {
        currentHP = a;
        gameUI.Update_PlayerHP(a);
    }

    public void SetName(string name)
    {
        playerName = name;
        gameUI.SetNameText(name);
    }

}
