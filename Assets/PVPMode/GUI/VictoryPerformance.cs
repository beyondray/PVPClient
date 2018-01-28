using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VictoryPerformance : MonoBehaviour {
    public float m_fVictoryUpdateSpeed = 1f;
    public float m_fTextChangeSpeed = 5f;
    public float m_fDestoryDelay = 1.5f;
    public float m_fFontMinSize = 65f;
    public GameObject m_NothingPrefab;
    Image m_VictoryImage;
    GameObject m_TextObj;
    Text m_Text;
    float m_fTextAppearTime = 0f;
    float ratio = 0.2f;
	// Use this for initialization
	void Start () {
        m_VictoryImage = GetComponent<Image>();
        m_TextObj = transform.FindChild("Text").gameObject;
        m_Text = m_TextObj.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_VictoryImage.fillAmount < 1f)
        {
            m_VictoryImage.fillAmount += m_fVictoryUpdateSpeed * Time.deltaTime;
        }
        else
        {
            m_TextObj.SetActive(true);
            
            if (m_Text.fontSize <= m_fFontMinSize)
            {
                m_fTextAppearTime += Time.deltaTime;
                if (m_fTextAppearTime >= m_fDestoryDelay)
                {
                    m_fTextAppearTime = 0f;
                    Instantiate(m_NothingPrefab);
                    Destroy(this.gameObject);
                }
            }
            else
            {
                m_Text.fontSize = (int)Mathf.Lerp(m_Text.fontSize, m_fFontMinSize, ratio * m_fTextChangeSpeed * Time.deltaTime);
                ratio += 0.7f * Time.deltaTime;
            }
        }
	}
}
