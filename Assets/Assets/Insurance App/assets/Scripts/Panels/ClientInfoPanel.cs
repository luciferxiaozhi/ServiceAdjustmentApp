using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientInfoPanel : MonoBehaviour, IPanel
{
    public Text caseNumberText;
    public InputField firstNameInput, lastNameInput;
    public LocationPanel locationPanel;

    public void OnEnable()
    {
        caseNumberText.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
        UIManager.Instance.activePanels.Push(this.gameObject);
    }

    public void ProcessInfo()
    {
        if (string.IsNullOrEmpty(firstNameInput.text) || string.IsNullOrEmpty(lastNameInput.text))
        {
            Debug.Log("Either the first name or last name is empty, we cannot continue.");
        }
        else
        {
            UIManager.Instance.activeCase.name = firstNameInput.text + " " + lastNameInput.text;
            locationPanel.gameObject.SetActive(true);
        }
    }
}
