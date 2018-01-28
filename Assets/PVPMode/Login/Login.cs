using UnityEngine;
using KBEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public Text loginInfo;
    public InputField account;
    public InputField password;

    private bool canModify;
    private static bool bInstallEvent = false;

    // Use this for initialization
    void Start()
    {
        installEvents();
    }

    void installEvents()
    {
        //KBEngine.Event.registerOut("onConnectionState", this, "onConnectionState");
        KBEngine.Event.registerOut("onLoginFailed", this, "onLoginFailed");
        KBEngine.Event.registerOut("onLoginSuccess", this, "onLoginSuccess");
        KBEngine.Event.registerOut("onLoginBaseapp", this, "onLoginBaseapp");

    }

    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }

    public void onLogin()
    {
        info("连接到服务端...", Info.green);
        KBEngine.Event.fireIn("login", account.text, password.text, System.Text.Encoding.UTF8.GetBytes("login test!!") ); 
    }

    void info(String _info, Color color)
    {
        loginInfo.text = _info;
        loginInfo.color = color;
    }

    public void onRegister()
    {
        Switch.toRegister();
        info("", Info.black);
    }

    public void onReset()
    {
        Switch.toReset();
        info("", Info.black);
    }

    public void onModifyChange(bool value)
    {
        canModify = value;
        if (canModify)
        {
            info("修改密码已授权，\n" +
                "绑定邮箱已授权，\n" +
                "登陆成功后即可设定!", Info.green);
        }
        else
        {
            info("已取消账号设定授权!", Info.green);
        }
    }

    public void onConnectionState(bool success)
    {
        if (!success)
        {
            info("连接错误...", Info.red);
        }
        else
        {
            info("连接成功, 请稍后...", Info.green);
        }
        
    }

    public void onLoginFailed(UInt16 code)
    {
        if(code == 20)
        {
            String _info = System.Text.Encoding.ASCII.GetString(KBEngineApp.app.serverdatas());
            info("登陆失败..." + _info, Info.red);
        }
        else if(code == 6)
        {
            info("密码错误...", Info.red);
        }
        else
        {
            info("登陆失败: " + KBEngineApp.app.serverErr(code), Info.red);
        }
    }

    public void onLoginSuccess(ulong uuid, int id, object account)
    {
        if(account!= null)
        {
            info("登陆成功...", Info.green);
            if(canModify)
            {
                Switch.toModify();
                info("", Info.black);
            }
            else
            {
                Application.LoadLevel("Role");
            }
        }
    }

    public void onLoginBaseapp()
    {
        info("连接到网关...", Info.green);
    }
}
