using UnityEngine;
using System.Collections;

public class RoleList
{
    //role career
    private static string archer = "ashe";
    private static string saber = "ali";
    private static string[] roleName = { "", archer, saber };

    private static string carArcher = "寒冰射手";
    private static string carGunner = "战斗法师";
    private static string[] careerName = { "", carArcher, carGunner };

    public static byte getRoleNumber(string name)
    {
        if (name == archer)
        {
            return 1;
        }
        else if (name == saber)
        {
            return 2;
        }
        return 0;
    }

    public static string getCareerName(byte number)
    {
        return careerName[number]; 
    }

    public static string getRoleName(byte number)
    {
        return roleName[number];
    }
}
