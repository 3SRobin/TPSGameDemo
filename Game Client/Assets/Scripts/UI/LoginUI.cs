using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    private Button loginButton;
    private Button RegisterButton;
    private Button OffLineButton;
    private InputField NameInput;
    private InputField PasswordInput; 
    private Text Tips;
    Camera nowCamera;
    Camera mainCamera;
    Canvas gameUI;


    void Awake()
    {
        nowCamera = transform.Find("Camera").GetComponent<Camera>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gameUI = GameObject.FindGameObjectWithTag("GameUI").GetComponent<Canvas>();

        loginButton = transform.Find("LoginButton").GetComponent<Button>();
        RegisterButton = transform.Find("RegisterButton").GetComponent<Button>();
        OffLineButton = transform.Find("OffLineButton").GetComponent<Button>();

        NameInput = transform.Find("NameInput").GetComponent<InputField>();
        PasswordInput = transform.Find("PasswordInput").GetComponent<InputField>();

        Tips = transform.Find("Tips").GetComponent<Text>();
    }

    void Start()
    {
        mainCamera.enabled = false;
        gameUI.enabled = false;
        loginButton.onClick.AddListener(LoginButtonClick);
        RegisterButton.onClick.AddListener(RegisterButtonClick);
        OffLineButton.onClick.AddListener(OffLineButtonClick);
    }

    void LoginButtonClick()
    {
        string name = NameInput.text;
        string password = PasswordInput.text;
        if(name.Length > 0 && password.Length > 0)
        {
            byte[] data = MsgHandler.LoginDataStream(name, password);
            GameSetting.name = name;
            StartCoroutine(GameManager.host.SendServer(data));
        }
    }

    void RegisterButtonClick()
    {
        string name = NameInput.text;
        string password = PasswordInput.text; 
        if(name.Length > 0 && password.Length > 0)
        {
            byte[] data = MsgHandler.RegisterDataStream(name, password);
            StartCoroutine(GameManager.host.SendServer(data));
        }
    }

    void OffLineButtonClick()
    {
        GameStart();
    }

    public void GameStart()
    {
        nowCamera.enabled = false;
        loginButton.enabled = false;
        RegisterButton.enabled = false;
        OffLineButton.enabled = false;
        NameInput.enabled = false;
        PasswordInput.enabled = false;
        GetComponent<Canvas>().enabled = false;

        mainCamera.enabled = true;
        gameUI.enabled = true;
        GameSetting.isBegin = true;
    }
    
    public void SetTips(short code)
    {
        switch(code)
        {
            case NetSetting.LOGIN_WRONG:
                Tips.text = "Password Wrong!";
                break;
            case NetSetting.REGISTER_SUCCESSFUL:
                Tips.text = "Register Successfully, Login Please!";
                break;
            case NetSetting.REGISTER_WRONG:
                Tips.text = "Register Wrong, This Name Already Exist!";
                break;
            case NetSetting.ONLINE:
                Tips.text = "Connection ...";
                break;
            case NetSetting.OFFLINE:
                Tips.text = "Disconnection ...";
                break;
        }
    }


}
