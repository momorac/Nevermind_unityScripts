using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSceneScript : MonoBehaviour
{
    public float colorChangeDuration = 2.0f;
    public float ambientChangeDuration = 3.0f;

    [Header("Virtual Cameras")]
    public GameObject VCamera_first;
    public GameObject VCamera_second;
    public GameObject VCamera_third;
    public int cameraCount = 0;
    public GameObject light_;


    [Space(10)]
    [Header("Animators")]
    public GameObject animatorParent_first;
    public Animator lilyAnimation;
    public Animator transitionAnimator;


    [Space(10)]
    [Header("Color Changing Materials")]
    public Material skyBox_Material;
    private float skybox_InitValue = 1.3f;
    public float skybox_TargetValue;
    public Material skyHalo_Material;
    public Color skyHalo_InitialColor;
    public Color skyHalo_TargetColor;

    [Space(10)]
    public Material firstFloor_Material;
    private Color firstFloor_InitialColor;
    public Color firstFloor_TargetColor;
    [Space(10)]
    public Material terrain_Material;
    private Color terrain_InitialColor;
    public Color terrain_TargetColor;
    [Space(10)]
    public Material water_Material;
    private float water_InitialMatalic;
    private float water_InitialSmothness;
    [Space(10)]
    public Material firstTree_Material;
    private Color firstTree_InitialColor;
    public Color firstTree_TargetColor;
    [Space(10)]
    public Material secondTree_Material;
    private Color secondTree_InitialColor;
    public Color secondTree_TargetColor;
    [Space(10)]
    public Material thirdTree_Material;
    private Color thirdTree_InitialColor;
    public Color thirdTree_TargetColor;
    [Space(10)]
    public Material grass_Material;
    private Color grass_InitialColor;
    public Color grass_TargetColor;


    void Start()
    {
        firstFloor_InitialColor = firstFloor_Material.color;
        terrain_InitialColor = terrain_Material.color;
        firstTree_InitialColor = firstTree_Material.color;
        secondTree_InitialColor = secondTree_Material.color;
        thirdTree_InitialColor = thirdTree_Material.color;
        grass_InitialColor = grass_Material.color;

        water_InitialMatalic = water_Material.GetFloat("_Metallic");
        water_InitialSmothness = water_Material.GetFloat("_Smoothness");

        skyBox_Material.SetFloat("_D2I", 1.3f);
        skyHalo_Material.color = new Color(0, 0, 0, 0);


    }

    private void OnApplicationQuit()
    {
        firstFloor_Material.color = firstFloor_InitialColor;
        terrain_Material.color = terrain_InitialColor;
        firstTree_Material.color = firstTree_InitialColor;
        secondTree_Material.color = secondTree_InitialColor;
        thirdTree_Material.color = thirdTree_InitialColor;
        grass_Material.color = grass_InitialColor;


        water_Material.SetFloat("_Metallic", water_InitialMatalic);
        water_Material.SetFloat("_Smoothness", water_InitialSmothness);

        skyBox_Material.SetFloat("_D2I", skybox_InitValue);

    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine("SkyAmbientChange");
        }
        if (Input.GetKey(KeyCode.Space))
        {
            StartCoroutine(ChangeCameraScene());
        }
    }

    private IEnumerator SkyAmbientChange()
    {
        float elapsedTime = 0f;


        while (elapsedTime < ambientChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / ambientChangeDuration;

            float newSkyBox = Mathf.Lerp(skybox_InitValue, skybox_TargetValue, t);
            skyBox_Material.SetFloat("_D2I", newSkyBox);

            skyHalo_Material.color = Color.Lerp(skyHalo_InitialColor, skyHalo_TargetColor, t);

            yield return null;
        }
    }

    private IEnumerator GroundColorChange()
    {
        Debug.Log("Color Change Coroutine Start");
        float elapsedTime = 0f;

        while (elapsedTime < colorChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / colorChangeDuration;

            // Lerp를 통해 색상을 서서히 변경
            firstFloor_Material.color = Color.Lerp(firstFloor_InitialColor, firstFloor_TargetColor, t);
            terrain_Material.color = Color.Lerp(terrain_InitialColor, terrain_TargetColor, t);
            firstTree_Material.color = Color.Lerp(firstTree_InitialColor, firstTree_TargetColor, t);
            secondTree_Material.color = Color.Lerp(secondTree_InitialColor, secondTree_TargetColor, t);
            thirdTree_Material.color = Color.Lerp(thirdTree_InitialColor, thirdTree_TargetColor, t);
            grass_Material.color = Color.Lerp(grass_InitialColor, grass_TargetColor, t);

            // Lerp를 통해 Metallic과 Smoothness를 서서히 변경
            float newMetallic = Mathf.Lerp(water_InitialMatalic, 1, t);
            float newSmoothness = Mathf.Lerp(water_InitialSmothness, 1, t);
            water_Material.SetFloat("_Metallic", newMetallic);
            water_Material.SetFloat("_Smoothness", newSmoothness);


            // 한 프레임 대기
            yield return null;
        }

        firstFloor_Material.color = firstFloor_TargetColor;
        terrain_Material.color = terrain_TargetColor;
        firstTree_Material.color = firstTree_TargetColor;
        secondTree_Material.color = secondTree_TargetColor;
        thirdTree_Material.color = thirdTree_TargetColor;
        grass_Material.color = grass_TargetColor;
    }

    private void SetAnimation()
    {
        for (int i = 0; i < animatorParent_first.transform.childCount; i++)
        {
            // 현재 자식 오브젝트 가져오기
            Transform child = animatorParent_first.transform.GetChild(i);

            Transform firstChild = child.GetChild(0);

            // 첫 번째 자식 오브젝트의 Animator 컴포넌트 가져오기
            Animator animator = firstChild.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("RestartTrigger");
            }
        }

        if (cameraCount == 2)
        {
            lilyAnimation.SetTrigger("RestartTrigger");
        }
    }


    private IEnumerator ChangeCameraScene()
    {
        switch (cameraCount)
        {
            case 0:
                SetAnimation();
                StartCoroutine(GroundColorChange());
                VCamera_first.SetActive(true);
                cameraCount++;
                break;
            case 1:
                VCamera_first.SetActive(false);
                SetAnimation();
                StartCoroutine(GroundColorChange());
                VCamera_second.SetActive(true);
                cameraCount++;
                break;
            case 2:
                VCamera_second.SetActive(false);
                SetAnimation();
                StartCoroutine(GroundColorChange());
                VCamera_third.SetActive(true);
                light_.SetActive(true);
                cameraCount++;
                break;
        }
        yield break;
    }

}
