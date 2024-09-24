using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interaction_Gimmick : GimmickTrigger
{

    public LayerMask DetectionLayer; // 중돌 감지 레이어

    public RectTransform InteractionImge; // UI 이미지의 RectTransform
    Camera mainCamera; // 메인 카메라

    public Transform targetObject; // 3D 물체의 Transform
    private bool isPlayerInTrigger;

    public enum einteractiontype
    {
        basic_Interaction,
        Gauge_Based_Interaction
    }
    [Header("상호작용 종류")]
    public einteractiontype InteractionType;

    [Header("Gauge_Based_Interaction를 선택했을때만 사용되는 변수들")]
    public float maxGaugeValue = 100f; // 게이지 최대 값
    public float gaugeFillRate = 10f; // 게이지 채우는 속도

    public Slider gaugeFillSlider;

    [HideInInspector]public float currentGaugeValue = 0f;
    private bool isInteracting = false;

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        if (InteractionImge != null)
        {
            InteractionImge.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        InteractionImge.gameObject.SetActive(false);

    }
    private void Update()
    {
        switch (InteractionType)
        {
            case einteractiontype.basic_Interaction:
                if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.G))
                {
                    InvokeEventRunOnTrigger();
                }
                if (isTriggered && isPlayerInTrigger)
                {
                    InteractionImge.gameObject.SetActive(false);
                }
                break;
            case einteractiontype.Gauge_Based_Interaction:
                if (isPlayerInTrigger && Input.GetKey(KeyCode.G))
                {
                    // 게이지를 채웁니다
                    currentGaugeValue += gaugeFillRate * Time.deltaTime;
                    currentGaugeValue = Mathf.Clamp(currentGaugeValue, 0, maxGaugeValue);

                    if (gaugeFillSlider != null)
                    {
                        gaugeFillSlider.maxValue = maxGaugeValue;
                        gaugeFillSlider.value = currentGaugeValue;
                    }

                    // 게이지가 최대에 도달하면 상호작용을 실행합니다
                    if (currentGaugeValue >= maxGaugeValue && !isInteracting)
                    {
                        isInteracting = true;
                        InvokeEventRunOnTrigger();
                    }
                }
                else
                {
                    // 상호작용 키를 놓으면 게이지를 초기화합니다
                    currentGaugeValue = 0;
                   isInteracting = false;
                }
                if (Input.GetKeyUp(KeyCode.G))
                {
                    gaugeFillSlider.value = 0;
                }
                break;
            default:
                break;
        }


    }



    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0)
        {
            InteractionImge.gameObject.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0 )
        {
            // 3D 물체의 위치를 화면 좌표로 변환
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetObject.position);

            // 화면 좌표를 UI 좌표로 변환
            InteractionImge.position = screenPos;
            isPlayerInTrigger = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & DetectionLayer) != 0)
        {
            InteractionImge.gameObject.SetActive(false);
            isPlayerInTrigger = false;
            if(gaugeFillSlider != null)
                gaugeFillSlider.value = 0;
        }
    }
}
