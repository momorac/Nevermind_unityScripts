using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using UnityEngine;

public class PuzzleEndingController : MonoBehaviour
{
    [Header("Scene Control")]
    public CinemachineVirtualCamera ClearedVirtualCamera;
    public ClickMovement characterController;
    public Animator characterAnimator;
    public GameObject playerCharacter;
    public Volume volume;


    [Space(10)]
    [Header("Set Plug State")]
    public GameObject PlugObject;
    public AudioSource AttachSound;
    public Vector3 PlugAttached_position;
    public Quaternion PlugAttached_rotation;

    [Space(10)]
    [Header("Particle Effects Control")]
    public GameObject SuccessParticleEffect;
    public GameObject NoiseParticleEffect;

    [Space(10)]
    [Header("AudioSources")]
    public AudioSource lightOnSound;
    public AudioSource SuccessEffectSound;


    void Start()
    {
        // 엔딩씬 버추얼 카메라 활성화
        ClearedVirtualCamera.Priority = 10;

        // 캐릭터 컨트롤 비활성화
        Debug.Log("Set Controller Unable");
        characterController.agent.ResetPath(); // 이동 경로 초기화
        characterController.agent.isStopped = true;
        characterController.agent.enabled = false;

        characterController.isEnded = true;
        characterController.enabled = false;

        // 캐릭터 기뻐하는 애니메이션
        playerCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);
        characterAnimator.SetTrigger("EndTrigger");

        // 플러그 최종상태로 변경
        PlugObject.transform.SetParent(null);

        PlugObject.transform.position = PlugAttached_position;
        PlugObject.transform.rotation = PlugAttached_rotation;
        AttachSound.Play();


        StartCoroutine(SuccessSequence());
    }

    private IEnumerator SuccessSequence()
    {
        // 버추얼카메라 트랜지션을 위채 2초간 대기
        yield return new WaitForSeconds(2);

        NoiseParticleEffect.SetActive(false);
        lightOnSound.Play();
        SuccessParticleEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        SuccessEffectSound.Play();

        yield break;
    }

 
}
