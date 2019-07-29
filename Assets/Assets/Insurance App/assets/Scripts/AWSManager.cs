using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

public class AWSManager : MonoBehaviour
{
    // singleton
    private static AWSManager _instance;

    public static AWSManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("AWSManager is null!");
            }

            return _instance;
        }
    }
    
    // S3Region
    public string S3Region = RegionEndpoint.CACentral1.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }

    //S3Client
    private AmazonS3Client _s3Client;
    public AmazonS3Client S3Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(new CognitoAWSCredentials(
                    "ca-central-1:d1e0bff7-3d9a-4538-9366-c10dd5aaf72c", // Identity Pool ID
                    RegionEndpoint.CACentral1 // Region
                ), _S3Region);
            }

            return _s3Client;
        }
    }

    private void Awake()
    {
        _instance = this;

        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        //ListBuckets();
    }

    public void ListBuckets()
    {
        S3Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                responseObject.Response.Buckets.ForEach((s3b) =>
                {
                    Debug.Log("Bucket Name: " + s3b.BucketName);
                });
            }
            else
            {
                Debug.Log("AWS Error!\n" + responseObject.Exception);
            }
        });
    }

    public void UploadToS3(string path, string caseID)
    {
        Debug.Log("Retrieving the file...");
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        PostObjectRequest request = new PostObjectRequest()
        {
            Bucket = "caseserviceadjustmentappfiles",
            Key = "case_data/case_" + caseID,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = _S3Region
        };

        S3Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log("Successfully posted the case to bucket.");
            }
            else
            {
                Debug.Log("Exception Occured during uploading: " + responseObj.Exception);
            }
        });

        SceneManager.LoadScene(0);
    }

    public void GetListOnS3(string caseNumber, Action onComplete = null)
    {
        string target = "case_data/case_" + caseNumber;
        Debug.Log(target);
        Debug.Log("AWSManager::GetListFromS3()");

        ListObjectsRequest request = new ListObjectsRequest()
        {
            BucketName = "caseserviceadjustmentappfiles"
        };

       S3Client.ListObjectsAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                bool caseFound = responseObj.Response.S3Objects.Any(obj => obj.Key == target);
                if (caseFound)
                {
                    Debug.Log("Found");
                    S3Client.GetObjectAsync("caseserviceadjustmentappfiles", target, (responseObjGet) =>
                    {
                        var response = responseObjGet.Response;
                        if (response.ResponseStream != null)
                        {
                            byte[] data = null;

                            // use streamreader to read response data
                            using (StreamReader reader = new StreamReader(response.ResponseStream))
                            {
                                //access a memory stream
                                using (MemoryStream memory = new MemoryStream())
                                {
                                    // populate data byte array with memstream data
                                    var buffer = new byte[512];
                                    var bytesRead = default(int);

                                    while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        memory.Write(buffer, 0, bytesRead);
                                    }
                                    data = memory.ToArray();
                                }
                            }

                            // convert bytes to a case (object)
                            using (MemoryStream memory = new MemoryStream(data))
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                Case downloadedCase = bf.Deserialize(memory) as Case;
                                Debug.Log("Downloaded Case Name: " + downloadedCase.name);
                                UIManager.Instance.activeCase = downloadedCase;

                                if (onComplete != null)
                                    onComplete();
                            }
                        }
                    });
                }
                else
                {
                    Debug.Log("Case not found");
                }
            }
            else
            {
                Debug.LogError("Exception Occured during getting list!");
            }
        });
    }
}
