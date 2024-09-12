using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzleManager_first : MonoBehaviour
{

    //public GameObject Plug;
    //public GameObject Destination;
    //public GameObject Line;
    //public GameObject Light;
    public GameObject EndImage;

    [Space(20)]
    public bool plugAttached = false;
    private bool pre_plugState = false;

    public bool lightAttached = false;
    private bool pre_lightState = false;

    [Space(20)]
    public bool endTrigger = false;

    void Update()
    {
        bool plugOn = PlugCheck();
        bool lightOn = LightCheck();

        if (plugOn && lightOn)
        {
            endTrigger = true;
            Debug.Log("Stage Clear!");
            EndImage.SetActive(true);
        }
    }

    private bool PlugCheck()
    {
        if (plugAttached)
        {
            pre_plugState = plugAttached;
            Debug.Log("Calble Linked!");
            return true;
        }
        if (pre_plugState == true && !plugAttached)
        {
            pre_plugState = plugAttached;
            Debug.Log("Calble Uninked!");
            return false;
        }
        return false;
    }

    private bool LightCheck()
    {
        if (lightAttached)
        {
            pre_lightState = lightAttached;
            Debug.Log("Light Attached!");
            return true;
        }
        if (pre_lightState && !lightAttached)
        {
            pre_lightState = lightAttached;
            Debug.Log("Light detached!");
            return false;
        }
        return false;
    }


}
