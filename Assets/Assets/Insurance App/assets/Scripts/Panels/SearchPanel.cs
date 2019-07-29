using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchPanel : MonoBehaviour, IPanel
{
    public InputField caseNumberInput;
    public SelectPanel selectPanel;

    private void OnEnable()
    {
        UIManager.Instance.activePanels.Push(this.gameObject);
    }

    public void ProcessInfo()
    {
        // download list of all objects inside s3 storage 
        AWSManager.Instance.GetListOnS3(caseNumberInput.text, () =>
        {
            selectPanel.gameObject.SetActive(true);
        });
        // compare those to caseNumberInput by user

    }
}
