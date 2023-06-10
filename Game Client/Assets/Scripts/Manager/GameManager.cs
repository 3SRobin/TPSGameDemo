using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    LoginUI loginUI;
    PlayerData playerData;
    static public NetWork host;
    public GameObject otherPlayerPrefab;
    Dictionary<int, GameObject> otherPlayer;
    public GameObject zombiePrefab;
    Dictionary<int, GameObject> allZombie;

    void Awake()
    {
        host = NetWork.GetNetWork();
        loginUI = GameObject.FindGameObjectWithTag("LoginUI").GetComponent<LoginUI>();
        playerData = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<PlayerData>();
    }

    void Start()
    {
        if(host.connected)
            loginUI.SetTips(NetSetting.ONLINE);
        else
            loginUI.SetTips(NetSetting.OFFLINE);
    }

    void Update()
    {
        host.RecvServer();
        while(host.MQ.Count >0)
        {
            ServerMsg serverMessage = host.MQ.Dequeue();
            HandleServerMsg(serverMessage);
        }
        if(Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    //  判断消息的类型
    void HandleServerMsg(ServerMsg Msg)
    {
        switch(Msg.msgType){
            case NetSetting.CONNECT:
                Debug.Log("client id: " + ((ServerConnect)Msg).clientID);
                break;
            case NetSetting.LOGIN_FEEDBACK:
                HandleLogin((ServerLogin)Msg);
                break;
            case NetSetting.PLAYER_REGISTER:
                SetPlayerData((ServerPlayerRegister)Msg);
                break;
            case NetSetting.OTHER_PLAYER_JOIN:
                JoinOtherPlayer((ServerOtherPlayerJoin)Msg);
                break;
            case NetSetting.OTHER_PLAYER_ACTION:
                OtherPlayerAction((ServerOtherPlayerAction)Msg);
                break;
            case NetSetting.OTHER_PLAYER_REMOVE:
                OtherPlayerRemove((ServerOtherPlayerRemove)Msg);
                break;
            case NetSetting.ZOMBIE_PRODUCE:
                ZombieProduce((ServerZombieProduce)Msg);
                break;
            case NetSetting.ZOMBIE_DATA:
                SetZombieData((ServerZombieData)Msg);
                break;
            case NetSetting.ZOMBIE_DEATH:
                ZombieDeath((ServerZombieDeath)Msg);
                break;
        }
    }

    void HandleLogin(ServerLogin Msg)
    {
        if(Msg.code == NetSetting.LOGIN_SUCCESSFUL)
        {
            byte[] data = MsgHandler.PlayerRegisterDataStream(GameSetting.name);
            StartCoroutine(host.SendServer(data));
            otherPlayer = new Dictionary<int, GameObject>();
            allZombie = new Dictionary<int, GameObject>();
        }
        else
            loginUI.SetTips(Msg.code);
    }

    void SetPlayerData(ServerPlayerRegister msg)
    {
        GameSetting.isConnect = true;
        loginUI.GameStart();
        playerData.SetName(msg.name);
        playerData.SetScore(msg.score);
        playerData.SetBlood(msg.blood);
        if(msg.bullet > GameSetting.Gun_Bullet)
            playerData.SetBullet(GameSetting.Gun_Bullet, msg.bullet - GameSetting.Gun_Bullet);
        else
            playerData.SetBullet(msg.bullet, 0);
    }

    void JoinOtherPlayer(ServerOtherPlayerJoin msg)
    {
        Vector3 pos = new Vector3(msg.position[0], msg.position[1], msg.position[2]);
        Vector3 rot = new Vector3(msg.rotation[0], msg.rotation[1], msg.rotation[2]);
        GameObject player = Instantiate (otherPlayerPrefab, pos, Quaternion.Euler(rot));
        otherPlayer.Add(msg.entity_id, player);
    }

    void OtherPlayerAction(ServerOtherPlayerAction msg)
    {
        otherPlayer[msg.entity_id].transform.GetComponent<PlayerPrefab>().SetPrefab(msg);
    }

    void OtherPlayerRemove(ServerOtherPlayerRemove msg)
    {
        Destroy(otherPlayer[msg.entity_id]);
    }

    void ZombieProduce(ServerZombieProduce msg)
    {
        Vector3 pos = new Vector3(msg.position[0], msg.position[1], msg.position[2]);
        Vector3 rot = new Vector3(0, 0, 0);
        GameObject zombie = Instantiate (zombiePrefab, pos, Quaternion.Euler(rot));
        zombie.transform.GetComponent<ZombieController>().target = new Vector3(msg.targetPosition[0], msg.targetPosition[1], msg.targetPosition[2]);
        zombie.transform.GetComponent<ZombieData>().entity_id = msg.entity_id;
        allZombie.Add(msg.entity_id, zombie);
    }

    void SetZombieData(ServerZombieData msg)
    {
        GameObject zombieEntity = allZombie[msg.entity_id];
        zombieEntity.transform.GetComponent<ZombieController>().target = new Vector3(msg.targetPosition[0], msg.targetPosition[1], msg.targetPosition[2]);
        if(zombieEntity.transform.GetComponent<ZombieData>().currentHP > msg.blood)
            zombieEntity.transform.GetComponent<ZombieData>().currentHP = msg.blood;
    }

    void ZombieDeath(ServerZombieDeath msg)
    {
        GameObject zombieEntity = allZombie[msg.entity_id];
        zombieEntity.transform.GetComponent<ZombieData>().currentHP = msg.blood;
        zombieEntity.transform.GetComponent<ZombieData>().Death();
    }
}
