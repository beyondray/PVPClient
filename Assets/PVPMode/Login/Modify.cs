using UnityEngine;
using KBEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class Modify : MonoBehaviour
{
    public Text modifyInfo;

    public InputField password;

    //new password
    public Text newPasswordTip;
    public InputField newPassword;
    private bool overNewPassword;
    public Button modifyBtn;

    //bind mailbox
    public Text mailboxTip;
    public InputField mailbox;
    private bool overMailbox;
    public Button bindMailBtn;

    // Use this for initialization
    void Start()
    {
        installEvents();
    }

    void installEvents()
    {
        KBEngine.Event.registerOut("onNewPasswordModify", this, "onNewPasswordModify");
        KBEngine.Event.registerOut("onBindAccountMail", this, "onBindAccountMail");
    }

    void Update()
    {
        Info.setCompareStr(password.text);
        Info.showInputTip(newPassword, newPasswordTip, "新密码", ref overNewPassword);
        Info.showInputTip(mailbox, mailboxTip, "邮箱", ref overMailbox);
    }

    void info(String _info, Color color)
    {
        modifyInfo.text = _info;
        modifyInfo.color = color;
    }

    public void onModify()
    {
        if (Info.check(newPassword) == Info.Format.Right)
        {
            KBEngine.Event.fireIn("newPassword", password.text, newPassword.text);
            info("正在请求修改密码...", Info.green);
        }
    }

    public void onBindMail()
    {
        if(Info.check(mailbox) == Info.Format.Right)
        {
            KBEngine.Event.fireIn("bindAccountEmail", mailbox.text);
            info("正在请求发送绑定邮件...", Info.green);
        }
    }

    public void onBeginGame()
    {
        Application.LoadLevel("Role");
    }

    public void onBack()
    {
        Switch.toLogin();
        modifyInfo.text = "";
    }

    public void onNewPasswordModify(UInt16 retcode)
    {
        if (retcode == 0)
        {
            info("密码修改成功!", Info.green);
            password.text = newPassword.text;
            overNewPassword = false;
        }
        else
        {
            string _info = "密码修改失败:" + KBEngineApp.app.serverErr(retcode);
            info(_info, Info.red);
        }
    }

    public void onBindAccountMail(UInt16 retcode)
    {
        if(retcode == 0)
        {
            info("已发送绑定邮件至邮箱，请查收邮件进行绑定!", Info.green);
            modifyInfo.color = Info.green;
            overMailbox = false;
        }
        else
        {
            string _info = "绑定邮箱失败:" + KBEngineApp.app.serverErr(retcode);
            info(_info, Info.red);
        }
    }
}
