
public class GameSetting
{
    static public bool isBegin = false;
    static public bool isConnect = false;
    static public string name = "";
    
    static public float Point_Shoot_Time = 0.6f;
    static public float Continue_Shoot_TIme = 0.15f;
    static public float Shoot_Range = 50f;

    static public int Player_HP = 100;
    static public float Player_WalkSpeed = 1.5f;
    static public float Player_Quicken = 1.5f;
    static public float player_TICK = 0.1f;

    static public int Zombie_HP = 100;
    static public int Zombie_Power = 2;
    static public float Zombie_ProduceTime = 10f;
    static public float Zombie_Destroy = 10f;
    static public float Zombie_Tick = 0.1f;

    static public float AmmoBox_Destroy = 10f;
    static public int AmmoBox_Bullet = 20;

    static public int Gun_Bullet = 30;
    static public int Gun_Additional_Bullet = 40;
    static public int Gun_Power = 10;
}
