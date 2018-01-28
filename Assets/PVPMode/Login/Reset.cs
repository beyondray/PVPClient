using UnityEngine;
using KBEngine;
using System;

using UnityEngine.UI;

public class Reset: MonoBehaviour
{
    public Text resetInfo;

    public Text usernameTip;
    public InputField username;
    private bool overUsername;

    // Use this for initialization
    void Start()
    {
        installEvents();
    }

    void installEvents()
    {
        KBEngine.Event.registerOut("onAccountResetPassword", this, "onAccountResetPassword");
    }

    void Update()
    {
        Info.showInputTip(username, usernameTip, "用户名", ref overUsername);
    }

    void info(String _info, Color color)
    {
        resetInfo.text = _info;
        resetInfo.color = color;
    }

    public void onReset()
    {
        if (Info.check(username) == Info.Format.Right)
        {
            KBEngine.Event.fireIn("resetPassword", username.text);
            info("正在请求发送重置密码邮件...", Info.green);
        }
    }

    public void onBack()
    {
        Switch.toLogin();
        resetInfo.text = "";
    }

    public void onAccountResetPassword(UInt16 retcode)
    {
        if (retcode == 0)
        {
            info("重置密码邮件发送成功!", Info.green);
        }
        else
        {
            string _info = "重置密码邮件发送失败:" + KBEngineApp.app.serverErr(retcode);
            info(_info, Info.red);
        }
    }
}
