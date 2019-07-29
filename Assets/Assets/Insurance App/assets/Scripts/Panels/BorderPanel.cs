using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BorderPanel : MonoBehaviour
{
    public void OnClickHomeKeyButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClickLeftArrowButton()
    {
        if (UIManager.Instance.activePanels.Count != 0)
        {
            GameObject currentEnabledPanel = UIManager.Instance.activePanels.Pop();
            currentEnabledPanel.SetActive(false);
        }

        if (UIManager.Instance.activePanels.Count == 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
