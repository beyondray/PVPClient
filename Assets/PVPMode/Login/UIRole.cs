using UnityEngine;
using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIRole : MonoBehaviour
{
    public Text tipInfo;
    public GameObject chooseRoleUI;
    public InputField roleNameInput;
    public Button jumpToCreateBtn;
    public Button enterGameBtn;
    public GameObject roleList;

    private Dictionary<UInt64, Transform> rolePanelDic = new Dictionary<UInt64, Transform>();
    private Dictionary<UInt64, Dictionary<string, object>> roles;
    private Dictionary<string, GameObject> heros = new Dictionary<string, GameObject>();
    GameObject curHero = null;
    private float newRolePanelPosY = 0;
    private Transform selectRolePanel = null;

    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(gameObject);
        KBEngine.Event.registerOut("recRoleList", this, "recRoleList");
        KBEngine.Event.registerOut("recCreateRole", this, "recCreateRole"); 
        KBEngine.Event.registerOut("recRemoveRole", this, "recRemoveRole");

        reqRoleList();
    }

    private void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }

    private void info(string txt, Color color)
    {
        tipInfo.text = txt;
        tipInfo.color = color;
        CancelInvoke();
        Invoke("hideInfo", 1.5f);
    }

    private void hideInfo()
    {
        tipInfo.text = "";
    }
    
    private GameObject selectHero(string heroName, string roleName)
    {
        //no hero or not the current hero
        if (curHero == null || curHero.name != heroName)
        {
            if (curHero) curHero.SetActive(false);
            if (!heros.ContainsKey(heroName))
            {
                GameObject hero = Resources.Load<GameObject>("Prefabs/Heros/" + heroName + "_choose");
                hero = Instantiate(hero);
                heros[heroName] = hero;
            }
            curHero = heros[heroName];
            curHero.name = heroName;
            curHero.transform.rotation = Quaternion.Euler(0, 180, 0);
            curHero.SetActive(true);
        }
        else //is the current hero
        {
            curHero.SetActive(true);
        }

        RoleHUD rhud = curHero.GetComponent<RoleHUD>();
        if (rhud != null)
        {
            rhud.entity_name = roleName;
        };
        return curHero;
    }

    public void onClickRole(Image img)
    {
        selectHero(img.name, "");
    }

    public void onClickSelectedRole(Transform role)
    {
        selectRolePanel = role;
        Image avatar = selectRolePanel.FindChild("avatar").GetComponent<Image>();
        Text roleName = selectRolePanel.FindChild("name").GetComponent<Text>();
        selectHero(avatar.mainTexture.name, roleName.text);
    }

    private void openChooseUIPanel(bool open)
    {
        chooseRoleUI.gameObject.SetActive(open);
        enterGameBtn.gameObject.SetActive(!open);
        jumpToCreateBtn.gameObject.SetActive(!open);
    }

    public void onBack()
    {
        if(jumpToCreateBtn.gameObject.active)
        {
            GameObject netMain = GameObject.Find("NetMain");
            if (netMain)
            {
                Destroy(netMain);
            }
            Application.LoadLevel("login");

        }
        else
        {
            openChooseUIPanel(false);
        }
    }

    public void onEnterGame()
    {
        Account account = (Account)KBEngineApp.app.player();
        if(account != null)
        {
            if(selectRolePanel != null)
            {
                UInt64 _roleDbid = UInt64.Parse(selectRolePanel.name);
                AsyncSceneMgr.singleton.loadMainScene(_roleDbid);
            }
            else
            {
                info("请选择一个角色...", Info.red);
            }
        }
    }

    public void onJumpToCreateRole()
    {
        openChooseUIPanel(true);
    }

    private int calculSize(string name)
    {
        int size = 0;
        for(int i = 0; i < name.Length; i++)
        {
            size += (int)name[i] > 127 ? 2 : 1;
        }
        return size;
    }

    public void onCreateRole()
    {
        if(roles.Count >= 5)
        {
            info("角色数量已达到上限...", Info.red);
            return;
        }

        if (!curHero)
        {
            info("请选择角色...", Info.red);
            return;
        }

        if (roleNameInput.text == "")
        {
            info("请输入名称...", Info.red);
            return;
        }

        if(calculSize(roleNameInput.text) > 8)
        {
            info("名称最大不超过8个字符!", Info.red);
            return;
        }

        //change UI state
        openChooseUIPanel(false);

        //request create role
        reqCreateRole();
    }

    public void onRemvoeRole()
    {
        if(selectRolePanel != null)
        {
            UInt64 dbid = UInt64.Parse(selectRolePanel.name);
            reqRemoveRole(dbid);
        }
    }

    public void reqRoleList()
    {
        KBEngine.Event.fireIn("reqRoleList");
    }

    public void recRoleList(Byte res, Dictionary<UInt64, Dictionary<string, object>> roleListInfos)
    {
        if(res == 0) //success
        {
            roles = roleListInfos;
            createRoleListPanels();
            updateRoleListPanelsInfo();
        }
    }

    public void reqCreateRole()
    {
        string name = roleNameInput.text;
        Byte career = RoleList.getRoleNumber(curHero.name);
        KBEngine.Event.fireIn("reqCreateRole", new object[] { name, career });
    }

    public void recCreateRole(Byte res, Dictionary<string, object> roleInfo)
    {
        if(res == 0) //success
        {
            Transform role_template = roleList.transform.FindChild("role_template");

            //new panel
            Transform role_panel = Instantiate(role_template, roleList.transform);
            role_panel.gameObject.SetActive(true);
            rolePanelDic[(UInt64)roleInfo["dbid"]] = role_panel;
            selectRolePanel = role_panel;

            //update info
            updateRoleListPanelsInfo();

            //select cur hero
            onClickSelectedRole(selectRolePanel);

            info("角色创建成功!", Info.green);
        }
        else if(res == 1)
        {
            info("创建失败: 角色名已存在!", Info.red);
        }
    }

    public void reqRemoveRole(UInt64 dbid)
    {
        KBEngine.Event.fireIn("reqRemoveRole", new object[]{ dbid});
    }

    public void recRemoveRole(UInt64 dbid)
    {
        if(dbid != 0) // success
        {
            Transform role_panel = rolePanelDic[dbid];
            GameObject.Destroy(role_panel.gameObject);
            rolePanelDic.Remove(dbid);

            //set select role panel
            if(rolePanelDic.Count > 0)
            {
                foreach(UInt64 _dbid in rolePanelDic.Keys)
                {
                    selectRolePanel = rolePanelDic[_dbid];
                    break;
                }
            }
            else
            {
                selectRolePanel = null;
            }

            //update panel info
            updateRoleListPanelsInfo();

            //hide select hero
            if (curHero)
            {
                curHero.SetActive(false);
                curHero = null;
            }

            info("角色删除成功!", Info.green);
        }
    }

    private void createRoleListPanels()
    {
        Transform role_template = roleList.transform.FindChild("role_template");
        foreach(UInt64 dbid in roles.Keys)
        {
            //new panel
            Transform role_panel = Instantiate(role_template, roleList.transform);
            role_panel.gameObject.SetActive(true);
            rolePanelDic[dbid] = role_panel;
        }
    }

    private void updateRoleListPanelsInfo()
    {
        newRolePanelPosY = 0;
        foreach (UInt64 dbid in rolePanelDic.Keys)
        {
            Transform role_panel = rolePanelDic[dbid];
            Dictionary<string, object> info = roles[dbid];

            //set Pos
            RectTransform rectTrans = role_panel.GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(0, newRolePanelPosY);
            newRolePanelPosY -= rectTrans.sizeDelta.y;

            //find component
            Image roleImg = role_panel.FindChild("avatar").gameObject.GetComponent<Image>();
            Text nameTxt = role_panel.FindChild("name").gameObject.GetComponent<Text>();
            Text careerTxt = role_panel.FindChild("career").gameObject.GetComponent<Text>();
            Text levelTxt = role_panel.FindChild("level").gameObject.GetComponent<Text>();

            //set role data
            Byte career = (Byte)info["career"]; 
            role_panel.gameObject.name = ((UInt64)info["dbid"]).ToString();
            nameTxt.text = "名字: " + (string)info["name"];
            careerTxt.text = "职业: " + RoleList.getCareerName(career);
            levelTxt.text = "等级: " + ((UInt16)info["level"]).ToString();
            roleImg.sprite = Resources.Load<Sprite>("ui/" + RoleList.getRoleName(career));
        }
    }
}
