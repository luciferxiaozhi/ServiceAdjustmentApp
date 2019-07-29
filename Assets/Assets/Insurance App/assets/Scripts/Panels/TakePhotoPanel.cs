using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePhotoPanel : MonoBehaviour, IPanel
{
    public Text takePhotoText;
    public RawImage photoTakeRawImage;
    public InputField photoNotesInput;
    public GameObject overviewPanel;

    private string imgPath;

    private void OnEnable()
    {
        takePhotoText.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
        UIManager.Instance.activePanels.Push(this.gameObject);
    }

    public void TakePictureButton()
    {
        if (NativeCamera.IsCameraBusy())
            return;

        TakePicture(512);
    }

    public void ProcessInfo()
    {
        byte[] imgData = null;

        if(!string.IsNullOrEmpty(imgPath))
        {
            Texture2D img = NativeCamera.LoadImageAtPath(imgPath, 512, false);
            imgData = img.EncodeToPNG();
        }

        UIManager.Instance.activeCase.photoNotes = photoNotesInput.text;
        UIManager.Instance.activeCase.photoTaken = imgData;
        overviewPanel.SetActive(true);
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                photoTakeRawImage.texture = texture;
                photoTakeRawImage.gameObject.SetActive(true);
                imgPath = path;
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }
}
