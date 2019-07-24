using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanel : MonoBehaviour, IPanel
{
    public Text informationText;

    public void OnEnable()
    {
        UIManager.Instance.activePanels.Push(this.gameObject);
        informationText.text = UIManager.Instance.activeCase.name;
    }
    public void ProcessInfo()
    {
        
    }
}
