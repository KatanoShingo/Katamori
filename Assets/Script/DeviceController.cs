using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceController : MonoBehaviour
{
    // isMobileをstatic変数として定義
    static bool isMobile = false;

    // isMobileの値を取得するためのstaticプロパティ
    public static bool IsMobileDevice => isMobile;

#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    static extern bool IsMobile();
#endif

    void Start()
    {
        CheckIfMobile();
    }

    void CheckIfMobile()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        isMobile = IsMobile();
#elif UNITY_ANDROID
        isMobile = true;
        GetComponent<Text>().text = isMobile ? "Mobile" : "PC";
#elif UNITY_EDITOR
        GetComponent<Text>().text = isMobile ? "Mobile" : "PC";
#endif
    }
}
