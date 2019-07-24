using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class OverviewPanel : MonoBehaviour, IPanel
{
    public Text caseNumberText;
    public Text nameText;
    public Text dateText;
    public RawImage mapRawImage;
    public Text locationNotesText;
    public RawImage photoTakenRawImage;
    public Text photoNotesText;

    public void OnEnable()
    {
        UIManager.Instance.activePanels.Push(this.gameObject);
        caseNumberText.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
        nameText.text = UIManager.Instance.activeCase.name;
        UIManager.Instance.activeCase.date = DateTime.Today.ToLongDateString();
        dateText.text = UIManager.Instance.activeCase.date;

        Texture2D reconstructedMap = new Texture2D(1, 1);
        reconstructedMap.LoadImage(UIManager.Instance.activeCase.map);
        Texture map = reconstructedMap as Texture;

        Texture2D reconstructedImg = new Texture2D(1, 1);
        reconstructedImg.LoadImage(UIManager.Instance.activeCase.photoTaken);
        Texture img = reconstructedImg as Texture;

        mapRawImage.texture = map;
        photoTakenRawImage.texture = img;
        locationNotesText.text = "LOCATION NOTES: \n" + UIManager.Instance.activeCase.locationNotes;
        photoNotesText.text = "PHOTO NOTES: \n" + UIManager.Instance.activeCase.photoNotes;
    }

    public void ProcessInfo()
    {
        
    }
}
