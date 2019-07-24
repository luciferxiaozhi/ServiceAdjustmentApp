using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Error: The UIManager is null!");
            }
            return _instance;
        }
    }

    public Case activeCase;
    public ClientInfoPanel clientInfoPanel;
    public GameObject borderPanel;
    public Stack<GameObject> activePanels;

    private void OnEnable()
    {
        activePanels = new Stack<GameObject>();
    }

    private void Awake()
    {
        _instance = this;
    }

    public void CreateNewCase()
    {
        activeCase = new Case();
        

        // generate a caseID
        string randNumber = Random.Range(0, 1000).ToString();
//        while (AWSManager.Instance.IsExistCaseOnS3(randNumber).Result)
//        {
//            randNumber = Random.Range(0, 1000).ToString();
//        }
        
        activeCase.caseID = "" + randNumber;

        clientInfoPanel.gameObject.SetActive(true);
        borderPanel.SetActive(true);
    }

    public void SubmitButton()
    {
        // create a new case to save
        // populate the case data
        // open a data stream to turn that object(file) into a file

        Case awsCase = DeepCloneCase(activeCase);

        string filePath = Application.persistentDataPath + "/case#" + awsCase.caseID + ".dat";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, awsCase);
        file.Close();

        Debug.Log("Application Data Path: " + Application.persistentDataPath);

        // send to aws
        AWSManager.Instance.UploadToS3(filePath, awsCase.caseID);

    }

    public Case DeepCloneCase(Case targetCase)
    {
        Case newCase = new Case();
        newCase.caseID = targetCase.caseID;
        newCase.name = targetCase.name;
        newCase.date = targetCase.date;
        newCase.locationNotes = targetCase.locationNotes;
        newCase.map = targetCase.map;
        newCase.photoTaken = targetCase.photoTaken;
        newCase.photoNotes = targetCase.photoNotes;

        return newCase;
    }
}
