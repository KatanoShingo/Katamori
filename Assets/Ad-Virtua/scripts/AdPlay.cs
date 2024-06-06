using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.IO;

[System.Serializable]
public class Url
{
    public string p_req_id;
    public string video_1;
    public string video_2;
    public string video_3;
    public bool is_valid;
}
public class ReqPostData
{
    public string unit_id;
    public string device_ui;
    public string device_type = SystemInfo.deviceType.ToString();
    public string device_model = SystemInfo.deviceModel;
    public string os = SystemInfo.operatingSystem;
    public string data = "1.6.0";
}
public class PlayPostData
{
    public string play_request_id;
    public string device_ui;
    public byte order = 0;
    public string data;
}
public class AdPlay : MonoBehaviour
{
    public string advirtuaUnitId;
    private string localFallbackVideoPath = "Ad-Virtua/nointernet.mp4";
    private string reqId;
    private bool isValid = false;
    private bool isBeforeEnd = false;
    private bool isPrepared = false;
    private float elapsedTime;
    private float allTime;
    private string sceneData;
    public Camera targetCamera;
    private static int advirtuaPlayCount = 0;
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        StartCoroutine(InternetAccessChecker.Check((isConnected) =>
        {
            if (!isConnected)
            {
                Debug.Log("[Ad-Virtua] No Internet Connection.");
                isBeforeEnd = true;
                PlayMovie(Path.Combine(Application.streamingAssetsPath, localFallbackVideoPath));
            }
            else if (advirtuaUnitId == "")
            {
                Debug.Log("[Ad-Virtua] Please input Unit ID of Ad-Virtua. https://docs.ad-virtua.com");
            }
            else
            {
                isBeforeEnd = true;
                StartCoroutine("REQ_POST", advirtuaUnitId);
            }
        }));
    }
    private void Update()
    {
        if (isBeforeEnd)
        {
            allTime += Time.deltaTime;

            if (IsObjectVisible(targetCamera))
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= 7 && isValid)
                {
                    StartCoroutine("PLAY_POST");
                    isValid = false;
                    Debug.Log("[Ad-Virtua] Video Reached at the Check Point with camera view.");
                }

                if (isPrepared)
                {
                    isPrepared = false;
                    VideoPlayer video = this.GetComponent<VideoPlayer>();
                    video.Play();
                }
            }
        }
    }
    private bool IsObjectVisible(Camera camera)
    {
        RaycastHit hit;

        if (Physics.Raycast(camera.transform.position, transform.position - camera.transform.position, out hit))
        {
            if (hit.transform != transform)
            {
                return false;
            }
        }
        Vector3 screenPos = camera.WorldToViewportPoint(transform.position);
        return screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1 && screenPos.z > 0;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        advirtuaPlayCount = 0;
    }
    private void OnStartedVideo(VideoPlayer vp)
    {
        advirtuaPlayCount++;
        Debug.Log("[Ad-Virtua] Video Started. This is video number: " + advirtuaPlayCount);

        string sceneName = this.gameObject.scene.name;
        Vector3 position = this.transform.position;
        Vector3 cameraPosition = targetCamera.transform.position;
        Renderer renderer = this.GetComponent<Renderer>();
        Vector3 size = Vector3.zero;
        if (renderer != null)
        {
            size = renderer.bounds.size;
        }
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        sceneData = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
            sceneName, size.x, size.y, size.z, position.x, position.y, position.z,
            cameraPosition.x, cameraPosition.y, cameraPosition.z, screenWidth, screenHeight, advirtuaPlayCount);
    }
    private void OnLoopPointReached(VideoPlayer vp)
    {
        isBeforeEnd = false;

        if (isValid)
        {
            if (elapsedTime > 1)
            {
                StartCoroutine("PLAY_POST");
                isValid = false;
                Debug.Log("[Ad-Virtua] Video Reached at the Loop Point with camera view.");
            }
            else
            {
                elapsedTime = 0;
                allTime = 0;
                isBeforeEnd = true;
                Debug.Log("[Ad-Virtua] Video Reached at the Loop Point without camera view.");
            }
        }
        else
        {
            Debug.Log("[Ad-Virtua] Video Reached at the Loop Point.");
        }
    }
    private void PlayMovie(string url)
    {
        VideoPlayer video = this.GetComponent<VideoPlayer>();
        video.url = url;
        video.isLooping = true;
        video.started += OnStartedVideo;
        video.loopPointReached += OnLoopPointReached;
        isPrepared = true;
    }
    private IEnumerator REQ_POST(string unitId)
    {
        ReqPostData reqPostData = new ReqPostData();
        reqPostData.device_ui = SystemInfo.deviceUniqueIdentifier;
        reqPostData.unit_id = unitId;
        string myJson = JsonUtility.ToJson(reqPostData);
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(myJson);

        UnityWebRequest request = new UnityWebRequest("https://ad-virtua.net/v1/play-request/loop", "POST");

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(byteData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("[Ad-Virtua]" + request.result);
            PlayMovie("https://storage.googleapis.com/ad-virtua-jp-videos/default/adv_logomv_error.mp4");
        }
        else
        {
            Debug.Log("[Ad-Virtua] Video Received.");
            Url url = JsonUtility.FromJson<Url>(request.downloadHandler.text);
            PlayMovie(url.video_1);
            reqId = url.p_req_id;
            isValid = url.is_valid;
        }
        request.Dispose();
    }
    private IEnumerator PLAY_POST()
    {
        PlayPostData playPostData = new PlayPostData();
        playPostData.device_ui = SystemInfo.deviceUniqueIdentifier;
        playPostData.play_request_id = reqId;
        playPostData.data = allTime.ToString("0.0") + "," + elapsedTime.ToString("0.0") + "," + sceneData;
        string myJson = JsonUtility.ToJson(playPostData);
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(myJson);

        UnityWebRequest request = new UnityWebRequest("https://ad-virtua.net/v1/play/loop", "POST");

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(byteData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("[Ad-Virtua]" + request.result);
        }
        request.Dispose();

    }
}
public static class InternetAccessChecker
{
    private static readonly List<string> CHECK_TARGET_URL_LIST = new List<string>() {
    "https://ad-virtua.net/v1/status"
  };

    public static IEnumerator Check(Action<bool> callback, int timeOut = 2)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback(false);
            yield break;
        }

        foreach (var url in CHECK_TARGET_URL_LIST)
        {
            UnityWebRequest request = null;
            try
            {
                request = new UnityWebRequest(url) { timeout = timeOut };
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    continue;
                }

                callback(true);
                yield break;
            }
            finally
            {
                if (request != null)
                {
                    request.Dispose();
                }
            }
        }

        callback(false);
    }
}