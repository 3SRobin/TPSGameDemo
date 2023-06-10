public class NetSetting
{
    public const int Port = 12345;
    public const string Addr = "127.0.0.1";   //"192.168.31.208";    //"113.102.120.94";      
    public const int NET_HEAD_LENGTH_SIZE = 4;
    public const short OFFLINE = 0x0000;
    public const short ONLINE = 0x0001;
    
    public const short MSG_CLIENT = 0x1200;


    public const short MODULE_LOGIN = 0x2100;
    public const short MODULE_PLAYER = 0x2200;
    public const short MODULE_ZOMBIE = 0x2300;


    public const short MODULE_LOGIN_FUN_LOGIN = 0x2110;
    public const short MODULE_LOGIN_FUN_REGISTER = 0x2120;
    public const short MODULE_PLAYER_FUN_REGISTER = 0x2210;
    public const short MODULE_PLAYER_FUN_DATA = 0x2220;
    public const short MODULE_ZOMBIE_FUN_DATA = 0x2310;
    public const short MODULE_ZOMBIE_FUN_DEATH = 0x2320;


    // 结果代码
    public const short LOGIN_SUCCESSFUL = 0x2111;
    public const short LOGIN_WRONG = 0x2112;
    public const short REGISTER_SUCCESSFUL = 0x2121;
    public const short REGISTER_WRONG = 0x2122;

    // 消息类型
    public const short CONNECT = 0x3000;
    public const short LOGIN_FEEDBACK = 0x3010;
    public const short PLAYER_REGISTER = 0x3020;
    public const short OTHER_PLAYER_JOIN = 0x3030;
    public const short OTHER_PLAYER_ACTION = 0x3040;
    public const short OTHER_PLAYER_REMOVE = 0x3050;
    public const short ZOMBIE_PRODUCE = 0x3060;
    public const short ZOMBIE_DATA = 0x3070;
    public const short ZOMBIE_DEATH = 0x3080;
}