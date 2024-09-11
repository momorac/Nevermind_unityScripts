using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{

    LoadingSceneController loadingSceneController;

    private void Awake()
    {
        loadingSceneController = GetComponent<LoadingSceneController>();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            loadingSceneController.ChangeScene();
        }
    }
}
