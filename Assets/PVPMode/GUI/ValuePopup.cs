using UnityEngine;
using System.Collections;

public class ValuePopup : MonoBehaviour
{
    #region public
    public GameObject m_Player;

    [SerializeField]
    public MotionMode m_MotionMode;
    public float m_VxSpeed = 10f;
    public float m_VySpeed = 10f;
    public Vector3 m_Gravity = Physics.gravity;
    public string m_InitText = "-";
    public bool m_bUseValue = true;
    public int m_Value;                 //伤害数值
    public float m_ContentWidth = 100;  //文本宽度
    public float m_ContentHeight = 50;  //文本高度
    public float FreeTime = 1.5f;       //销毁时间
    public GUIStyle m_GUIStyle;
    #endregion
   
    #region internal
    private Vector3 m_Screen;   //屏幕坐标
    private Vector2 m_GUIPoint; //GUI坐标
    private float m_MotionTime;
    private Vector3 m_Offest = Vector3.zero;
    private float m_PlayerHeight;
    private Vector3 m_PopPos;
    #endregion

    [System.Serializable]
    public enum MotionMode
    {
        Static = 0,
        ObliqueProjectile = 1,  //斜抛模式
        UpVerticle = 2          //垂直向上模式
    };

    // Use this for initialization
	void Start () 
	{
        //height
        if(m_Player != null)
        {
            float size_y = m_Player.GetComponent<Collider>().bounds.size.y;
            float scal_y = m_Player.transform.localScale.y;
            m_PlayerHeight = (size_y * scal_y) + 0.5f;
        }

        //rotate
        int x = Random.value > 0.5f ? 1 : -1;
        float angle = Random.Range(60, 120) * x;
        transform.Rotate(Vector3.up, angle);
        StartCoroutine("Free");
    }

	// Update is called once per frame
	void Update()
	{
        switch(m_MotionMode)
        {
            case MotionMode.ObliqueProjectile:
                m_Offest += transform.right * m_VxSpeed * Time.deltaTime;
                m_Offest += Vector3.up * m_VySpeed * Time.deltaTime + 0.5f * m_Gravity * Time.deltaTime * (2 * m_MotionTime + Time.deltaTime); 
                m_MotionTime += Time.deltaTime;
                break;

            case MotionMode.UpVerticle:
                m_Offest += Vector3.up * m_VySpeed * Time.deltaTime;
                break;

            case MotionMode.Static:
                break;
        }

        //set pos
        m_PopPos = new Vector3(m_Player.transform.position.x, m_Player.transform.position.y + m_PlayerHeight * 2 / 5, m_Player.transform.position.z + 0.5f);
        transform.position = m_PopPos + m_Offest;

		//to gui pos
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		m_Screen= Camera.main.WorldToScreenPoint(pos);
		m_GUIPoint=new Vector2(m_Screen.x,Screen.height-m_Screen.y);
	}

	void OnGUI()
	{
		if(m_Screen.z>0)
		{
            string s = m_bUseValue? m_InitText+m_Value.ToString():m_InitText;
		   GUI.Label(new Rect(m_GUIPoint.x,m_GUIPoint.y,m_ContentWidth,m_ContentHeight), s, m_GUIStyle);
		}
	}

	IEnumerator Free()
	{
		yield return new WaitForSeconds(FreeTime);
		Destroy(this.gameObject);
	}
}

