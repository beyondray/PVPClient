using UnityEngine;
using KBEngine;
using System;

using UnityEngine.UI;

public class Register : MonoBehaviour
{
    public Text registerInfo;

    public Text usernameTip;
    public InputField username;
    private bool overUsername;

    public Text passwordTip;
    public InputField password;
    private bool overPassword;

    // Use this for initialization
    void Start()
    {
        installEvents();
    }

    void installEvents()
    {
        KBEngine.Event.registerOut("onCreateAccountResult", this, "onCreateAccountResult");
    }

    void Update()
    {
        Info.showInputTip(username, usernameTip, "用户名", ref overUsername);
        Info.showInputTip(password, passwordTip, "密码", ref overPassword);
    }

    void info(String _info, Color color)
    {
        registerInfo.text = _info;
        registerInfo.color = color;
    }

    public void onRegister()
    {
        if(Info.check(username) == Info.Format.Right && 
            Info.check(password) == Info.Format.Right)
        {
            KBEngine.Event.fireIn("createAccount", username.text, password.text, System.Text.Encoding.UTF8.GetBytes("register test"));
            info("正在请求注册账号...", Info.green);
        }
    }

    public void onBack()
    {
        Switch.toLogin();
        registerInfo.text = "";
    }

    public void onCreateAccountResult(UInt16 retcode, byte[] datas)
    {
        if(retcode == 0)
        {
            info("账号注册成功!", Info.green);
        }
        else
        {
            string _info = "账号注册失败:" + KBEngineApp.app.serverErr(retcode);
            info(_info, Info.red);
        }
    }
}
