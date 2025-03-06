using UnityEngine;

public class LimitFramerate : MonoBehaviour 
{

#if UNITY_EDITOR
    private int frameRate = 90;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
    }

#endif

}
