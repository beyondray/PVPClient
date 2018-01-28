using UnityEngine;
using System.Collections;

public class RoleHUD : MonoBehaviour
{
    public Color fontColor = Color.yellow;
    public float nameHeight = 1.0f;
    private Camera playerCamera = null;
    private string name = "";
    public string entity_name
    {
        get{
            KBEngine.PropsEntity e = GetComponent<SyncPosRot>().entity;
            if(e != null)
            {
                return e.name;
            }
            return name;
        }
        set
        {
            name = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void OnGUI()
    {
        Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + nameHeight, transform.position.z);

        //to 2d screen pos
        Vector2 uiposition = playerCamera.WorldToScreenPoint(worldPosition);

        //get screen pos over head
        uiposition = new Vector2(uiposition.x, Screen.height - uiposition.y);

        //calculate size
        Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(entity_name));

        //set color
        GUI.color = fontColor;

        //set name
        GUI.Label(new Rect(uiposition.x - (nameSize.x / 2), uiposition.y - nameSize.y - 5.0f, nameSize.x, nameSize.y), entity_name);
    }

}
