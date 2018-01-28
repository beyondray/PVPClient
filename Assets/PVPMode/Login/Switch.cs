using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Switch
{
    static string[] _switchs = { "login", "register", "reset", "modify" };

    static void lockOn(string name)
    {
        GameObject canvas = GameObject.Find("Canvas");

        for(int i = 0; i < _switchs.Length; i++)
        {
            string _switch = _switchs[i];
            Transform trans = canvas.transform.Find(_switch);
            if (trans != null)
            {
                if (_switch == name)
                    trans.gameObject.SetActive(true);
                else
                    trans.gameObject.SetActive(false);
            }
        }
    }

    public static void toRegister()
    {
        lockOn("register");
    }

    public static void toReset()
    {
        lockOn("reset");
    }

    public static void toModify()
    {
        lockOn("modify");
    }

    public static void toLogin()
    {
        lockOn("login");
    }
}
