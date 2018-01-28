using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class Info
{
    private static string mailPattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
    private static string compareStr = "";

    //text color
    public static Color purple = new Color(178f / 255f, 0, 1);
    public static Color green = new Color(0, 180f / 255f, 26f / 255f);
    public static Color red = Color.red;
    public static Color black = Color.black;

    public enum Format
    {
        Right = -1,
        None = 0,
        Ilegal = 1,
        Exist = 2,
        Repeat = 3,
    }

    public static void setCompareStr(string text)
    {
        compareStr = text;
    }

    public static Format check(InputField input)
    {
        if (input.text == "")
            return Format.None;

        if(input.tag == "mailbox")
        {
            bool match = Regex.IsMatch(input.text, mailPattern);
            if (!match) return Format.Ilegal;
        }

        if(input.tag == "newpassword")
        {
            if(compareStr != "")
            {
                if (input.text == compareStr)
                    return Format.Repeat;
            }
        }

        return Format.Right;
    }

    public static void showInputTip(InputField input, Text tip, string name, ref bool isOver)
    {
        if (input.isFocused)
        {
            tip.text = name;
            tip.color = black;
            isOver = true;
        }
        else
        {
            if (isOver)
            {
                Format code = check(input);
                switch (code)
                {
                    case Format.Right:
                        tip.text = name + "(√)";
                        tip.color = green;
                        break;

                    case Format.None:
                        tip.text = name + "(不可以为空)";
                        tip.color = red;
                        break;

                    case Format.Ilegal:
                        tip.text = name + "(格式不合法)";
                        tip.color = red;
                        break;

                    case Format.Exist:
                        tip.text = name + "(已被注册)";
                        tip.color = red;
                        break;

                    case Format.Repeat:
                        tip.text = name + "(与旧值重复)";
                        tip.color = red;
                        break;
                }
            }
            else
            {
                tip.text = name;
                tip.color = black;
            }
        }
    }
}
