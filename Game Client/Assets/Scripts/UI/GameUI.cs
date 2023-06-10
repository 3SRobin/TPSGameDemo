using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private float PlayerHP;
    private float EnemyHP;
    private int GunBullet;
    private int AdditionalBullet;
    private int Score;
    private Text NameText;
    private Text ScoreText;
    private Text BulletText;
    private Slider PlayerHPSlider;
    private Slider EnemyHPSlider;
    private Slider GunBulletSlider;
    private Image GunIcon;

    public Color flashColour; 
    
    void Awake()
    {
        PlayerHP = 100f;
        EnemyHP = 100f;
        Score = 0;
        NameText = transform.Find("Name").GetComponent<Text>();
        ScoreText = transform.Find("Score").GetComponent<Text>();
        PlayerHPSlider = transform.Find("PlayerHP").GetComponent<Slider>();
        EnemyHPSlider = transform.Find("EnemyHP").GetComponent<Slider>();
        GunBulletSlider = transform.Find("GunBullet").GetComponent<Slider>();
        BulletText = transform.Find("Bullet").GetComponent<Text>();
        GunIcon = transform.Find("Gun").GetComponent<Image>();
    }

    public void Update_PlayerHP(float HP)
    {
        PlayerHP = HP;
    }

    public void Update_EnemyHP(float HP)
    {
        EnemyHP = HP;
    }

    public void SetScore(int score)
    {
        Score = score;
    }

    public void EnemyHPSlider_Hide()
    {
        EnemyHPSlider.gameObject.SetActive(false);
    }

    public void EnemyHPSlider_Show()
    {
        EnemyHPSlider.gameObject.SetActive(true);
    }

    public void SetBulletMax(int count)
    {
        GunBullet = count;
        GunBulletSlider.maxValue = count;
    }

    public void SetAdditionalBullet(int count)
    {
        AdditionalBullet = count;
    }

    public void Updaet_Bullet(int bullet, bool GunIconFlash)
    {
        GunBullet = bullet;
        if(GunIconFlash || GunBullet == 0)
            GunIcon.color = flashColour;
        else
            GunIcon.color = Color.Lerp (GunIcon.color, new Color(255f, 255f, 255f, 255f), Time.deltaTime);
    }

    public void SetNameText(string name)
    {
        NameText.text = "Name  : " + name;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerHPSlider.value = PlayerHP;
        EnemyHPSlider.value = EnemyHP;
        GunBulletSlider.value = GunBullet;
        ScoreText.text = "Score  : " + Score;
        BulletText.text = AdditionalBullet + " / " + GunBullet;
    }
}
