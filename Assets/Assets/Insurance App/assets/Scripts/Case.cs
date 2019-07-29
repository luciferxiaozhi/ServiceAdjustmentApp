using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] // allow to see this class in inspector 
public class Case
{
    public string caseID;
    public string name;
    public string date;
    public byte[] map;
    public string locationNotes;
    public byte[] photoTaken;
    public string photoNotes;
}
