using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefab : MonoBehaviour
{
    private Animator Player_anim; 
    private Rigidbody rb;
    private ParticleSystem gunParticles;
    private AudioSource Player_audio;
    private AudioSource Gun_audio;
    private Light gunLight;
    string path = "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/GunEffect";
    int currentHP = 0;

    public AudioClip [] audios ;

    void Awake ()
    {
        gunParticles = transform.Find (path).gameObject.GetComponent<ParticleSystem>();
        gunLight = transform.Find (path).gameObject.GetComponent<Light>();
        Player_audio = GetComponent<AudioSource>();
        Gun_audio = transform.Find(path).gameObject.GetComponent<AudioSource>();
        Player_anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Gun_audio.Stop();
        Player_audio.Stop();
        gunLight.enabled = false;
    }

    public void SetPrefab(ServerOtherPlayerAction msg)
    {
        Vector3 pos = new Vector3(msg.position[0], msg.position[1], msg.position[2]);
        Vector3 rot = new Vector3(msg.rotation[0], msg.rotation[1], msg.rotation[2]);
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rot), Time.deltaTime * 5f);
        if(msg.moveX > 0 || msg.moveY > 0)
        {
            Player_audio.clip = audios[0];
            if(!Player_audio.isPlaying)
                Player_audio.Play();
        }
        else
            Player_audio.Stop();
        MoveAnimating(msg.moveX, msg.moveY);
        if(msg.blood == 0 && currentHP != 0)
            Player_anim.SetTrigger("Death");
        else if(msg.blood < currentHP)
            Player_anim.SetTrigger("Attack");
        currentHP = msg.blood;
        if(msg.shoot)
        {
            gunLight.enabled = true;
            Gun_audio.clip = audios[2];
            Gun_audio.Play();
            gunParticles.Stop ();
            gunParticles.Play ();
        }
        else
        {
            gunLight.enabled = false;
            Gun_audio.Stop();
        }
    }

    void MoveAnimating(float x, float y)
    {
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
    }
}
