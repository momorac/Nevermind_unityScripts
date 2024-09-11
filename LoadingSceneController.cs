using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading;
using TMPro;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class LoadingSceneController : MonoBehaviour
{

    public string nextScene;
    public float loadingTime;

    [Space(7)]
    public GameObject LoadingScene;
    public Image progressBar;
    public TMP_Text loadingMessage;
    public string[] messages = new string[10];



    public void ChangeScene()
    {
        LoadingScene.SetActive(true);
        loadingMessage.text = messages[Random.Range(0, messages.Length)];
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;
        float time = 0f;

        while (!operation.isDone)
        {
            time += Time.deltaTime;

            progressBar.fillAmount = time / loadingTime;
            if (time >= loadingTime)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;

        }
    }
}
