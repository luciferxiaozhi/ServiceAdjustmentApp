using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocationPanel : MonoBehaviour, IPanel
{
    public Text caseNumberText;
    public RawImage mapRawImage;
    public InputField mapNotesInput;

    public string apiKey;
    public float xCord, yCord;
    public int zoom;
    public int imgSize;
    public string url = "https://maps.googleapis.com/maps/api/staticmap?";

    private void OnEnable()
    {
        caseNumberText.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
        UIManager.Instance.activePanels.Push(this.gameObject);
    }

    IEnumerator Start()
    {
        // check if user has location service is enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled");
            yield break;
        }

        // start service
        Input.location.Start();

        // wait for initializing and check for time out.
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1.0f);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.Log("Timed out!");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location...");
        }
        else
        {
            xCord = Input.location.lastData.latitude;
            yCord = Input.location.lastData.longitude;

//            xCord = 43.78f;
//            yCord = -79.41f;
        }

        Input.location.Stop();

        // get map and display
        StartCoroutine(GetMap());
    }

    IEnumerator GetMap()
    {
        Debug.Log("LocationPanel::getMap()");
        // construct url
        url = url + "center=" + xCord + "," + yCord + "&zoom=" + zoom + "&size=" + imgSize + "x" + imgSize +
              "&maptype=roadmap&key=" + apiKey;

        // download static map
        using (UnityWebRequest mapRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return mapRequest.SendWebRequest();

            if (mapRequest.isNetworkError || mapRequest.isHttpError)
            {
                Debug.LogError("Map Error: " + mapRequest.error);
            }

            mapRawImage.texture = DownloadHandlerTexture.GetContent(mapRequest);
        }
    }
    public void ProcessInfo()
    {
        if (!string.IsNullOrEmpty(mapNotesInput.text))
        {
            UIManager.Instance.activeCase.locationNotes = mapNotesInput.text;
        }

        Texture2D convertedMap = mapRawImage.texture as Texture2D;
        byte[] mapData = convertedMap.EncodeToPNG();
        UIManager.Instance.activeCase.map = mapData;
    }

    
}
