using System;
using System.Text;
using Newtonsoft.Json;

public class MsgHandler
{
    //  将  接收到  服务器的信息打包
    static public ServerMsg BuiltServerMsg(short msgType, string dataStr)
    {
        ServerMsg serverMsg = null;
        switch(msgType){
            case NetSetting.CONNECT:
                serverMsg = JsonConvert.DeserializeObject<ServerConnect>(dataStr);
                break; 
            case NetSetting.LOGIN_FEEDBACK:
                serverMsg = JsonConvert.DeserializeObject<ServerLogin>(dataStr);
                break;
            case NetSetting.PLAYER_REGISTER:
                serverMsg = JsonConvert.DeserializeObject<ServerPlayerRegister>(dataStr);
                break;
            case NetSetting.OTHER_PLAYER_JOIN:
                serverMsg = JsonConvert.DeserializeObject<ServerOtherPlayerJoin>(dataStr);
                break;
            case NetSetting.OTHER_PLAYER_ACTION:
                serverMsg = JsonConvert.DeserializeObject<ServerOtherPlayerAction>(dataStr);
                break;
            case NetSetting.OTHER_PLAYER_REMOVE:
                serverMsg = JsonConvert.DeserializeObject<ServerOtherPlayerRemove>(dataStr);
                break;
            case NetSetting.ZOMBIE_PRODUCE:
                serverMsg = JsonConvert.DeserializeObject<ServerZombieProduce>(dataStr);
                break;
            case NetSetting.ZOMBIE_DATA:
                serverMsg = JsonConvert.DeserializeObject<ServerZombieData>(dataStr);
                break;
            case NetSetting.ZOMBIE_DEATH:
                serverMsg = JsonConvert.DeserializeObject<ServerZombieDeath>(dataStr);
                break;
        } 
        if(serverMsg != null)
            serverMsg.msgType = msgType;
        return serverMsg;
    }

    //  将  发送到  服务器的信息转换成字节流
    public static byte[] BuiltClientMsg(short mid, short fid, string msgData)
    {
        byte[] midByte = BitConverter.GetBytes(mid);                           //  ModuleID
        byte[] fidByte = BitConverter.GetBytes(fid);                           //  FuntionID
        byte[] msgByte = BitConverter.GetBytes(NetSetting.MSG_CLIENT);
        byte[] dataLenByte = BitConverter.GetBytes(msgData.Length);
        byte[] dataByte = Encoding.Default.GetBytes(msgData);

        int pos = 0;
        int len = midByte.Length + fidByte.Length + msgByte.Length + dataLenByte.Length + dataByte.Length;
        byte[] res = new byte[len];

        Array.Copy(midByte, 0, res, pos, midByte.Length);
        Array.Copy(fidByte, 0, res, pos += midByte.Length, fidByte.Length);
        Array.Copy(msgByte, 0, res, pos += fidByte.Length, msgByte.Length);
        Array.Copy(dataLenByte, 0, res, pos += msgByte.Length, dataLenByte.Length);
        Array.Copy(dataByte, 0, res, pos += dataLenByte.Length, dataByte.Length);
        return res;
    }

    public static byte[] LoginDataStream(string name, string password)
    {
        ClientLogin msg = new ClientLogin{name=name, password=password};
        string msgJson = JsonConvert.SerializeObject(msg);
        byte[] res = BuiltClientMsg(NetSetting.MODULE_LOGIN, NetSetting.MODULE_LOGIN_FUN_LOGIN, msgJson);
        return res;
    }

    public static byte[] RegisterDataStream(string name, string password)
    {
        ClientLogin msg = new ClientLogin{name=name, password=password};
        string msgJson = JsonConvert.SerializeObject(msg);
        byte[] res = BuiltClientMsg(NetSetting.MODULE_LOGIN, NetSetting.MODULE_LOGIN_FUN_REGISTER, msgJson);
        return res;
    }

    public static byte[] PlayerRegisterDataStream(string name)
    {
        ClientPlayerRegister msg = new ClientPlayerRegister{name=name};
        string msgJson = JsonConvert.SerializeObject(msg);
        byte[] res = BuiltClientMsg(NetSetting.MODULE_PLAYER, NetSetting.MODULE_PLAYER_FUN_REGISTER, msgJson);
        return res;
    }

    public static byte[] PlayerDataStream(int blood, int bullet, int score, float[] pos, float[] rot, float moveX, float moveY, bool shoot)
    {
        ClientPlayerData msg = new ClientPlayerData{blood =blood, bullet=bullet, score=score, position=pos, rotation=rot,
        moveX=moveX, moveY=moveY, shoot=shoot};
        string msgJson = JsonConvert.SerializeObject(msg);
        byte[] res = BuiltClientMsg(NetSetting.MODULE_PLAYER, NetSetting.MODULE_PLAYER_FUN_DATA, msgJson);
        return res;
    }

    public static byte[] ZombieDataStream(int entity_id, int blood, float[] pos)
    {
        ClientZombieData msg = new ClientZombieData{entity_id=entity_id, blood=blood, position=pos};
        string msgJson = JsonConvert.SerializeObject(msg);
        byte[] res = BuiltClientMsg(NetSetting.MODULE_ZOMBIE, NetSetting.MODULE_ZOMBIE_FUN_DATA, msgJson);
        return res;
    }

    public static byte[] ZombieDeathStream(int entity_id)
    {
        ClientZombieDeath msg = new ClientZombieDeath{entity_id=entity_id};
        string msgJson = JsonConvert.SerializeObject(msg);
        byte[] res = BuiltClientMsg(NetSetting.MODULE_ZOMBIE, NetSetting.MODULE_ZOMBIE_FUN_DEATH, msgJson);
        return res;
    }

}





public class ServerMsg                              // 接收服务端的信息格式
{
    public int msgType { get; set; }
}

public class ServerConnect : ServerMsg
{
    public int clientID { get; set; }
    public string state { get; set; }
}

public class ServerLogin : ServerMsg
{
    public short code { get; set; }
}

public class ServerPlayerRegister : ServerMsg
{
    public string name { get; set; }
    public int score { get; set; }
    public int blood { get; set; }
    public int bullet { get; set; }
}

public class ServerOtherPlayerJoin : ServerMsg
{
    public int entity_id { get; set; }
    public float[] position {get; set; }
    public float[] rotation {get; set; }
}

public class ServerOtherPlayerAction : ServerMsg
{
    public int entity_id {get; set; }
    public int score { get; set; }
    public int blood { get; set; }
    public int bullet { get; set; }
    public float[] position {get; set; }
    public float[] rotation {get; set; }
    public float moveX;
    public float moveY;
    public bool shoot;
}

public class ServerOtherPlayerRemove : ServerMsg
{
    public int entity_id { get; set; }
}

public class ServerZombieProduce : ServerMsg
{
    public int entity_id { get; set; }
    public float[] targetPosition { get; set; }
    public float[] position { get; set; }
}

public class ServerZombieData : ServerMsg
{
    public int entity_id { get; set; }
    public int blood { get; set; }
    public float[] targetPosition { get; set; }
}

public class ServerZombieDeath : ServerMsg
{
    public int entity_id { get; set; }
    public int blood { get; set; }
}







public class ClientMSg                              // 发送到服务端的消息格式
{

}

public class ClientLogin : ClientMSg
{
    public string name;
    public string password;
}

public class ClientPlayerRegister : ClientMSg
{
    public string name;
}

public class ClientPlayerData : ClientMSg
{
    public int bullet;
    public int blood;
    public int score;
    public float[] position;
    public float[] rotation;
    public float moveX;
    public float moveY;
    public bool shoot;
}

public class ClientZombieData : ClientMSg
{
    public int entity_id;
    public int blood;
    public float[] position;
}

public class ClientZombieDeath : ClientMSg
{
    public int entity_id;
}