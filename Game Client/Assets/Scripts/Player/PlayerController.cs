using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator Player_anim; 
    private Rigidbody rb;
    private PlayerData playerData;
    private ParticleSystem gunParticles;
    private AudioSource Player_audio;
    private AudioSource Gun_audio;
    private Light gunLight;
    private GameUI gameUI;
    GameObject gunEffect;

    private float shootTime = GameSetting.Continue_Shoot_TIme;

    private float timer;
    private float tick;
    private int shootableMask;
    private float effectsDisplayTime = 0.2f;
    private bool onGround = true;
    private bool reloadBullet = false;
    private bool shoot = false;

    float moveX;
    float moveY;
    string path = "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/GunEffect";

    public AudioClip [] audios ;

    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable");
        gunParticles = transform.Find (path).gameObject.GetComponent<ParticleSystem>();
        gunLight = transform.Find (path).gameObject.GetComponent<Light>();
        gameUI = GameObject.FindGameObjectWithTag("GameUI").GetComponent<GameUI>(); 
        Player_audio = GetComponent<AudioSource>();
        Gun_audio = transform.Find(path).gameObject.GetComponent<AudioSource>();
        playerData = GetComponent<PlayerData>();
        Player_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Gun_audio.Stop();
        Player_audio.Stop();
    }

    void Update()
    {
        ReloadBullet();
        SendPlayerData();
    }

    void FixedUpdate()
    {
        if (playerData.currentHP > 0 && GameSetting.isBegin)
            Move();

        timer += Time.deltaTime;
        
        RaycastHit shootHit;
        ZombieData zombieData = null;
        if(GameSetting.isBegin)
        {
            Ray shootRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
            if(Physics.Raycast(shootRay, out shootHit, GameSetting.Shoot_Range, shootableMask))
            {
                zombieData = shootHit.collider.GetComponent<ZombieData>();
                if(zombieData != null)
                {
                    zombieData.Buckle_blood(0, shootHit.point);
                    gameUI.EnemyHPSlider_Show();
                }
            }
            else
                gameUI.EnemyHPSlider_Hide();
            if(Input.GetMouseButton(0) && timer >= shootTime && playerData.bullet > 0 && !reloadBullet && playerData.currentHP > 0)
                Shoot(zombieData, shootHit);
            else
            {
                gameUI.Updaet_Bullet(playerData.bullet, false);
                shoot = false;
            }
        }
        if(timer >= shootTime * effectsDisplayTime)
            gunLight.enabled = false;
    }

    void ReloadBullet()
    {
        if(Input.GetKeyDown(KeyCode.R) && playerData.bullet < GameSetting.Gun_Bullet && playerData.additionalBullet > 0 && !reloadBullet)                                           //键盘R键 换弹夹
        {
            reloadBullet = true;
            Player_anim.SetTrigger("Reload_Bullet");
            Gun_audio.clip = audios[3];
            Gun_audio.Play();
        }
        
        AnimatorStateInfo animatorInfo = Player_anim.GetCurrentAnimatorStateInfo(1);
        if((animatorInfo.normalizedTime > 1.0f) && (animatorInfo.IsName("combat_reload_generic")))
        {
            reloadBullet = false;
            playerData.Reload_Bullet();
        }
    }

    void Move()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        Vector3 _xDistance = transform.right * moveX;
        Vector3 _zDistance = transform.forward * moveY;

        Vector3 _vel = (_xDistance + _zDistance).normalized * GameSetting.Player_WalkSpeed;
        if(Input.GetKey(KeyCode.LeftShift))                                       //键盘leftShift键 加速
            _vel *= GameSetting.Player_Quicken;
        if(moveX != 0 || moveY != 0)
        {
            if(!Player_audio.isPlaying)
                Player_audio.Play();
        }
        else
            Player_audio.Stop();
        MoveAnimating(moveX, moveY);
        if(Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            onGround = false;
            rb.velocity += new Vector3(0, 3, 0);                                    //添加加速度
            rb.AddForce(Vector3.up * 50);                                           //给刚体一个向上的力，力的大小为Vector3.up*mJumpSpeed
 　     }
        
        rb.MovePosition(rb.position + _vel * Time.fixedDeltaTime);
    }

    void MoveAnimating(float x, float y)
    {
        // Player_anim.SetBool("Forward", Input.GetKey(KeyCode.W));
        // Player_anim.SetBool("Back", Input.GetKey(KeyCode.S));
        // Player_anim.SetBool("Left", Input.GetKey(KeyCode.A));
        // Player_anim.SetBool("Right", Input.GetKey(KeyCode.D)); 
        if(y > 0)
        {
            Player_anim.SetBool("Forward", true);
            Player_anim.SetBool("Back", false);
        }
        else if(y < 0)
        {
            Player_anim.SetBool("Forward", false);
            Player_anim.SetBool("Back", true);
        }
        else if(y == 0)
        {
            Player_anim.SetBool("Forward", false);
            Player_anim.SetBool("Back", false);
        }

        if(x > 0)
        {
            Player_anim.SetBool("Right", true); 
            Player_anim.SetBool("Left", false);
        }
        else if(x < 0)
        {
            Player_anim.SetBool("Right", false); 
            Player_anim.SetBool("Left", true);
        }
        else if(x == 0)
        {
            Player_anim.SetBool("Right", false); 
            Player_anim.SetBool("Left", false);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Player_anim.SetFloat("Fast", 1.25f);
            Player_audio.clip = audios[1];
        }
        else
        {
            Player_anim.SetFloat("Fast", 0.8f);
            Player_audio.clip = audios[0];
        }
        
        if (Input.GetMouseButtonDown(2))                         //鼠标中键 切换点射和连射
        {
            if(shootTime == GameSetting.Point_Shoot_Time)
                shootTime = GameSetting.Continue_Shoot_TIme;
            else
                shootTime = GameSetting.Point_Shoot_Time;
            Player_anim.SetBool("Shoot_Switch", !Player_anim.GetBool("Shoot_Switch"));
        }
    }

    void Shoot(ZombieData zombieData, RaycastHit shootHit)
    {
        timer = 0f;
        playerData.Updaet_Bullet();
        shoot = true;
        gunLight.enabled = true;
        gunParticles.Stop ();
        gunParticles.Play ();
        Gun_audio.clip = audios[2];
        Gun_audio.Play();
        Player_anim.SetTrigger("Shoot");
        if(zombieData != null)
            zombieData.Buckle_blood(GameSetting.Gun_Power, shootHit.point);
    }

    void OnCollisionEnter(Collision other)
    {
        onGround = true;
    }
    
    public void Damage(int blood)
    {
        reloadBullet = false;
        playerData.Buckle_blood(blood);
    }

    void SendPlayerData()
    {
        tick += Time.deltaTime;
        if(GameSetting.isConnect && tick > GameSetting.player_TICK)
        {
            float[] playerPos = new float[3] {transform.position.x, transform.position.y, transform.position.z};
            float[] playerRot = new float[3] {transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z};
            byte[] data = MsgHandler.PlayerDataStream(playerData.currentHP, playerData.bullet + playerData.additionalBullet, 
            playerData.score, playerPos, playerRot, moveX, moveY, shoot);
            StartCoroutine(GameManager.host.SendServer(data));
        }
    }
}
