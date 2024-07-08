using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct PanelInfo
{
    public string description;
    public string btnOneValue;
    public string btnTwoValue;
    public UnityAction BtnOneCallBack;
    public UnityAction BtnTwoCallBack;
}

public class CheckPanel : MonoBehaviour
{
    private TextMeshProUGUI _descriptionTMP;
    private TextMeshProUGUI _btnOneTMP, _btnTwoTMP;
    private Button _btnOne;
    private Button _btnTwo;

    private void Awake()
    {
        _descriptionTMP = transform.Find("Description").GetComponent<TextMeshProUGUI>();
        _btnOne = transform.Find("Btn_One").GetComponent<Button>();
        _btnTwo = transform.Find("Btn_Two").GetComponent<Button>();
        _btnOneTMP = _btnOne.transform.Find("TMP").GetComponent<TextMeshProUGUI>();
        _btnTwoTMP = _btnTwo.transform.Find("TMP").GetComponent<TextMeshProUGUI>();
        HideCheckPanel();
    }

    public void ShowCheckPanel(PanelInfo panelInfo)
    {
        gameObject.SetActive(true);
        _descriptionTMP.SetText(panelInfo.description);
        _btnOneTMP.SetText(panelInfo.btnOneValue);
        _btnTwoTMP.SetText(panelInfo.btnTwoValue);
        _btnOne.onClick.RemoveAllListeners();
        _btnOne.onClick.AddListener(panelInfo.BtnOneCallBack);
        _btnTwo.onClick.RemoveAllListeners();
        _btnTwo.onClick.AddListener(panelInfo.BtnTwoCallBack);
    }

    public void HideCheckPanel()
    {
        gameObject.SetActive(false);
    }
}