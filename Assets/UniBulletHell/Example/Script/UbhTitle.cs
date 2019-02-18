using UnityEngine;
using UnityEngine.Serialization;

public class UbhTitle : UbhMonoBehaviour
{
    private const string TITLE_PC = "Press X";
    private const string TITLE_MOBILE = "Tap To Start";

    [SerializeField, FormerlySerializedAs("_StartGUIText")]
    private GUIText m_startGUIText;

    private void Start()
    {
        m_startGUIText.text = UbhUtil.IsMobilePlatform() ? TITLE_MOBILE : TITLE_PC;
    }
}