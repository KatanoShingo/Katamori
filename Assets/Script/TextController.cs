using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern bool IsMobile();
#endif
    void Start()
    {
        CheckIfMobile();
    }

    private void CheckIfMobile()
    {
        var isMobile = false;

#if !UNITY_EDITOR && UNITY_WEBGL
        isMobile = IsMobile();
#endif
        GetComponent<Text>().text = isMobile ? "Mobile" : "PC";
    }
}
