using UnityEngine;

public class LimitFramerate : MonoBehaviour 
{

    private int frameRate = 90;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
    }
}
