using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string m_ButtonName = "name";
    CrossPlatformInputManager.VirtualButton m_VirtualButton;

    void CreateVirtualButton()
    {
        if(m_ButtonName!="")
        {
            if (CrossPlatformInputManager.ButtonExists(m_ButtonName))
            {
                CrossPlatformInputManager.UnRegisterVirtualButton(m_ButtonName);
            }
            m_VirtualButton = new CrossPlatformInputManager.VirtualButton(m_ButtonName);
            CrossPlatformInputManager.RegisterVirtualButton(m_VirtualButton);
        }
    }

    void Start()
    {
         CreateVirtualButton();
    }
	
    public void OnPointerDown(PointerEventData data)
    {
        m_VirtualButton.Pressed();
    }

    public void OnPointerUp(PointerEventData data)
    {
        m_VirtualButton.Released();
    }

}
