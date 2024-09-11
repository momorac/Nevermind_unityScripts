
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    [Header("current status field")]
    private GameObject Player;
    [Range(0, 100)]
    public float playerHP = 100;  //플레이어 현재 체력
    public float skillCharged = 0f;
    public int crystalRemain;
    public int crystalCount = 0;
    public int SparkCount = 0;
    public int puzzleStage = 0;
    public bool isPuzzleOpen = false;
    public bool isCharacterPowerMode = false;
    public int item_index = 0;


    [Space(10)]
    [Header("stat value")]
    public float skillChargeAmount;
    public float SparkAtkAmount = 0.01f;
    public float effectDuration = 5f;
    public int puzzleUnlockCount;


    private int highHP = 80;
    private int midHP = 50;
    private int lowHP = 30;
    private int fewHP = 10;

    [Space(5)]
    [Header("Item Count")]
    public int heal = 0;
    public int recharge = 0;
    public int bomb = 0;
    public int speedUp = 0;
    public int shield = 0;
    public int healAmount = 50;

    [Space(10)]
    [Header("UI Component")]
    public Image skilChargeImage;
    public Image hpBarImage;
    public Animator UI_HPanime;
    public RectTransform[] ItemList = new RectTransform[5];
    public GameObject puzzleEnterLabel;

    [Space(5)]
    public AudioSource itemTabSound;
    public Image itemUseEffect_image;
    public TMP_Text itemUseEffect_text;
    public Animator itemUseEffectAnimator;
    public Sprite[] ItemUseImages = new Sprite[5];

    public Image itemGetEffect;
    public Animator itemGetEffectAnimator;
    public Sprite[] ItemGetImages = new Sprite[5];


    [Space(5)]
    public TMP_Text healLabel;
    public TMP_Text rechargeLabel;
    public TMP_Text bombLabel;
    public TMP_Text speedUpLabel;
    public TMP_Text shieldLabel;

    [Space(5)]
    public GameObject gameoverImage;
    public GameObject bombPrefab;

    [Space(10)]
    [Header("effect audio source")]
    public AudioSource itemGetSound;
    public AudioSource hitSound;
    public AudioSource powerModeSound;
    public AudioSource healSound;
    public AudioSource speedUpSound;
    public AudioSource chargeSound;
    public AudioSource puzzleOpenSound;

    [Space(10)]
    [Header("puzzle crystal objects")]
    public GameObject[] mainCrystalObjects = new GameObject[7];
    private LinkedList<GameObject> maincrystalList = new LinkedList<GameObject>();

    // system reference variable
    private FirstPersonController playerController;

    private float walkSpeed_origin;
    private float jumpForce_origin;

    private Vector3 initScale;

    [HideInInspector]
    public int EffectCount = 3;

    private void Awake()
    {
        playerController = Player.GetComponent<FirstPersonController>();
    }

    void Start()
    {
        // 캐릭터 기본 state 초기화
        walkSpeed_origin = playerController.walkSpeed;
        jumpForce_origin = playerController.jumpForce;
        initScale = Player.transform.localScale;

        skillCharged = 100f;
        puzzleStage = 0;

        // 메인 크리스탈 오브젝트들 linkedlist에 할당                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
        for (int i = 0; i < 7; i++)
        {
            maincrystalList.AddLast(mainCrystalObjects[i]);
        }
    }


    void Update()
    {
        // skil charge control
        skilChargeImage.fillAmount = skillCharged / 100;
        skillCharged += skillChargeAmount;

        // hp ui animation setting
        if (playerHP < 0)
        {
            gameoverImage.SetActive(true);
        }
        else if (playerHP >= highHP)
        {
            UI_HPanime.SetBool("isLowHp", false);
        }
        else if (playerHP >= midHP)
        {
            UI_HPanime.SetBool("isLowHp", false);
        }
        else if (playerHP >= lowHP)
        {
            UI_HPanime.SetBool("isLowHp", false);
        }
        else if (playerHP >= fewHP)
        {
            UI_HPanime.SetBool("isLowHp", true);
        }


        // item usage
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            itemTabSound.Play();
            item_index++;

            if (item_index > 4)
            {
                item_index = 0;
                swapItemPosition(ItemList[4], ItemList[0]);
            }
            else if (item_index <= 4)
                swapItemPosition(ItemList[item_index], ItemList[item_index - 1]);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (item_index == 0) ItemUse_heal();
            else if (item_index == 1) ItemUse_recharge();
            else if (item_index == 2) ItemUse_bomb();
            else if (item_index == 3) ItemUse_speed();
            // else if (idx == 4) Item_shield();
        }

        // puzzlie scene load
        // -----test code-----
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log(SearchProximateCrystal().gameObject.name);
        }
    }

    private GameObject target = null;
    private void FixedUpdate()
    {
        // 일정 개수 이상 크리스탈 파괴 시 퍼즐게임 씬 진입하는 코드
        if (!isPuzzleOpen && crystalCount >= (puzzleStage + 1) * puzzleUnlockCount)
        {
            isPuzzleOpen = true;
            puzzleOpenSound.Play();

            target = SearchProximateCrystal();

            if (target != null)
            {

                LightObjectAllocator lightObjectAllocator = target.GetComponent<LightObjectAllocator>();
                lightObjectAllocator.lightObject.SetActive(true);
            }
        }

        else if (isPuzzleOpen && target != null)
        {
            float distance = Vector3.Distance(Player.transform.position, target.transform.position);

            if (distance < 1.5f && !puzzleEnterLabel.activeSelf)
            {
                puzzleEnterLabel.SetActive(true);
            }
            else if (distance >= 1.5f && puzzleEnterLabel.activeSelf)
            {
                puzzleEnterLabel.SetActive(false);
            }
        }
    }

    private void swapItemPosition(RectTransform a, RectTransform b)
    {
        Vector2 tmp = a.position;
        a.position = b.position;
        b.position = tmp;
    }

    public void Attacked()
    {
        playerHP -= SparkAtkAmount;
        hpBarImage.fillAmount = playerHP / 100;
        UI_HPanime.SetBool("isAttacked", true);

        if (!hitSound.isPlaying)
        {
            hitSound.Play();
        }
    }

    public void UnAttacked()
    {
        UI_HPanime.SetBool("isAttacked", false);
    }

    //크리스탈 파괴 시 캐릭터 효과 부여
    public void CrystalBreakEffect()
    {
        int seed = Random.Range(0, 100);
        Debug.Log("Crystal Destroyed!");
        if (seed < 50)
        { }
        else if (seed >= 50 && seed < 55)
        {
            Effect_getHeal();
        }
        else if (seed >= 55 && seed < 60)
        {
            Effect_getRecharge();
        }
        else if (seed >= 60 && seed < 65)
        {
            Effect_getSpeedUp();
        }
        else if (seed >= 65 && seed < 70)
        {
            Effect_getShield();
        }
        else if (seed >= 70 && seed < 75)
        {
            Effect_getBomb();
        }
        else if (seed > 95 && seed < 100)
        {
            if (!isCharacterPowerMode)
                Effect_powerMode();
            Debug.Log("Power Mode!!");
        }
    }

    private void Effect_powerMode()
    {
        Player.transform.localScale = initScale * 2;
        powerModeSound.Play();
        isCharacterPowerMode = true;
        Invoke("InitStat", 3f);
    }

    private void Effect_getHeal()
    {
        itemGetSound.Play();
        heal++;
        healLabel.text = heal + "";
        Debug.Log("getHeal");

        itemGetEffect.sprite = ItemGetImages[0];
        itemGetEffectAnimator.SetTrigger("itemGetTrigger");
    }

    private void Effect_getRecharge()
    {
        itemGetSound.Play();
        recharge++;
        rechargeLabel.text = recharge + "";
        Debug.Log("getRechrge");

        itemGetEffect.sprite = ItemGetImages[1];
        itemGetEffectAnimator.SetTrigger("itemGetTrigger");

    }
    private void Effect_getBomb()
    {
        itemGetSound.Play();
        bomb++;
        bombLabel.text = bomb + "";
        Debug.Log("getBomb");

        itemGetEffect.sprite = ItemGetImages[2];
        itemGetEffectAnimator.SetTrigger("itemGetTrigger");

    }
    private void Effect_getShield()
    {
        itemGetSound.Play();
        shield++;
        shieldLabel.text = shield + "";
        Debug.Log("getShield");

        itemGetEffect.sprite = ItemGetImages[4];
        itemGetEffectAnimator.SetTrigger("itemGetTrigger");

    }
    private void Effect_getSpeedUp()
    {
        itemGetSound.Play();
        speedUp++;
        speedUpLabel.text = speedUp + "";

        Debug.Log("getSpeedUp");

        itemGetEffect.sprite = ItemGetImages[3];
        itemGetEffectAnimator.SetTrigger("itemGetTrigger");
    }

    private void ItemUse_heal()
    {
        Debug.Log("Use Heal Item");
        if (heal > 0)
        {
            heal--;
            healLabel.text = heal + "";

            itemUseEffect_image.sprite = ItemUseImages[0];
            itemUseEffect_text.text = "체력 회복!";
            itemUseEffectAnimator.SetTrigger("itemUseTrigger");

            healSound.Play();
            playerHP += healAmount;
            if (playerHP > 100) playerHP = 100;
            hpBarImage.fillAmount = playerHP / 100;
        }
    }

    private void ItemUse_recharge()
    {
        if (recharge > 0)
        {
            recharge--;
            rechargeLabel.text = recharge + "";

            itemUseEffect_image.sprite = ItemUseImages[1];
            itemUseEffect_text.text = "스킬 쿨타임 회복!";
            itemUseEffectAnimator.SetTrigger("itemUseTrigger");

            chargeSound.Play();
            skillCharged = 100f;
        }
    }

    private void ItemUse_bomb()
    {
        if (bomb > 0)
        {
            bomb--;
            Instantiate(bombPrefab, Player.transform.position, quaternion.identity);

            itemUseEffect_image.sprite = ItemUseImages[2];
            itemUseEffect_text.text = "  ";
            itemUseEffectAnimator.SetTrigger("itemUseTrigger");

            bombLabel.text = bomb + "";
        }
    }
    private void ItemUse_speed()
    {
        if (speedUp > 0)
        {
            speedUpSound.Play();
            speedUp--;
            speedUpLabel.text = speedUp + "";

            itemUseEffect_image.sprite = ItemUseImages[3];
            itemUseEffect_text.text = "이동속도 증가!";
            itemUseEffectAnimator.SetTrigger("itemUseTrigger");

            playerController.walkSpeed = walkSpeed_origin * 2;
            Invoke("InitStat", 5f);
        }
    }
    //private void Item_shield();



    private GameObject SearchProximateCrystal()
    {
        GameObject proximate = null;
        float min = float.MaxValue;

        foreach (GameObject crystal in maincrystalList)
        {
            float dist = Vector3.Distance(Player.transform.position, crystal.transform.position);
            if (dist < min)
            {
                proximate = crystal;
                min = dist;
            }
        }

        return proximate;
    }


    public void InitStat()
    {
        playerController.walkSpeed = walkSpeed_origin;
        playerController.jumpForce = jumpForce_origin;
        Player.transform.localScale = initScale;
        isCharacterPowerMode = false;
    }

    public void setGetBomb()
    {
        Effect_getBomb();
    }


}
