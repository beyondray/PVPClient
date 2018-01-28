using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BloodHUD : MonoBehaviour
{
    CombatProps m_CombatProps;

    public string entity_name
    {
        get
        {
            KBEngine.PropsEntity e = GetComponent<SyncPosRot>().entity;
            if (e != null)
            {
                return e.name;
            }
            return "name";
        }
    }

    float roleHeight;

    //blood 
    public GameObject bloodPref;
    private RectTransform bloodRect;
    private Image bloodImage;
    private Text playerName;

    void Start()
    {
        //props
        m_CombatProps = GetComponent<CombatProps>();
        
        //blood
        Transform canvas = gameObject.transform.Find("Canvas").transform;
        GameObject blood = Canvas.Instantiate(bloodPref, canvas) as GameObject;
        bloodRect = blood.GetComponent<RectTransform>();
        bloodImage = blood.GetComponent<Image>();
        playerName = blood.transform.Find("role_name").GetComponent<Text>();
        playerName.text = entity_name;

        //height
        float size_y = GetComponent<Collider>().bounds.size.y;
        float scal_y = transform.localScale.y;
        roleHeight = (size_y * scal_y) + 0.5f;
    }

    void Update()
    {
        // get pos
        Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + roleHeight, transform.position.z);
        Vector2 pos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 namePos = new Vector2(pos.x, Screen.height - pos.y);
        pos = new Vector2(pos.x - Screen.width / 2, pos.y - Screen.height / 2);

        // set blood rect
        bloodRect.anchoredPosition = pos;
        bloodImage.fillAmount = m_CombatProps.m_HP / m_CombatProps.m_MaxHP;
    }
   
    public void setBloodUIVisible(bool value)
    {
        if (bloodRect != null)
        {
            bloodRect.gameObject.SetActive(value);
        }
    }
}