using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ValueUpdate : MonoBehaviour
{
    #region public
    [System.Serializable]
    public enum Props
    {
        HP = 0,
        MP = 1,  
        PhysPower = 2,
    };
    public Props m_ChooseProp = Props.HP;
    public float m_ElapseSpeed = 35f;
    #endregion

    #region temporary
    float m_Value
    {
        get
        {
            switch (m_ChooseProp)
            {
                case Props.HP:
                    return m_CombatProps.m_HP;

                case Props.MP:
                    return m_CombatProps.m_MP;

                case Props.PhysPower:
                    return m_CombatProps.m_PhysPower;

                default:
                    return 0;
            }
        }
    }
    float m_MaxValue;
    float m_Ratio;
    float m_RealTimeRatio;
    #endregion

    #region internal
    private GameObject m_Player;
    CombatProps m_CombatProps;
    Image m_Image;
    #endregion

    // Use this for initialization
    void Start()
    {
        m_Player = GameObject.FindWithTag("localPlayer");
        m_CombatProps = m_Player.GetComponent<CombatProps>();
        m_Image = GetComponent<Image>();

        //init value
        switch (m_ChooseProp)
        {
            case Props.HP:
                m_MaxValue = m_CombatProps.m_MaxHP;
                m_RealTimeRatio = m_CombatProps.m_HP / m_MaxValue;
                break;

            case Props.MP:
                m_MaxValue = m_CombatProps.m_MaxMP;
                m_RealTimeRatio = m_CombatProps.m_MP / m_MaxValue;
                break;

            case Props.PhysPower:
                m_MaxValue = m_CombatProps.m_MaxPhysPower;
                m_RealTimeRatio = m_CombatProps.m_PhysPower / m_MaxValue;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_RealTimeRatio = Mathf.Lerp(m_RealTimeRatio, m_Ratio, 0.5f * m_ElapseSpeed * Time.deltaTime);
        m_Image.fillAmount = m_RealTimeRatio;
    }

    void FixedUpdate()
    {
        m_Ratio = m_Value / m_MaxValue;
    }

}
