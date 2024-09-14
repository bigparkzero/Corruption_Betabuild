using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    public RectTransform InformationWindow;
    public TextMeshProUGUI InformationWindowText;
    public float basicduration;
    public bool textruning;

    private void Start()
    {
        InformationWindow.localScale = new Vector3(0, 1, 1);  // X축을 0으로 시작
    }
    private void OnValidate()
    {
        if (InformationWindow != null)
        {
            TextMeshProUGUI FindTMP = InformationWindow.GetComponentInChildren<TextMeshProUGUI>(true);
            if (FindTMP != null)
                InformationWindowText = FindTMP;
        }
    }

    public void SetBasicDuration(float duration)
    {
        basicduration = duration;
    }
    public void SetTextStringGimmick(string talk)
    {
        SetTextString(talk);
    }
    public void SetTextString(string talk, float duration = 0)
    {
        textruning = true;
        WindowIntro();

        InformationWindowText.text = talk;
        InformationWindowText.maxVisibleCharacters = 0;
        float typingDuration = duration == 0 ? basicduration : duration;

        // DOTween을 사용해 타이핑 효과
        DOTween.To(() => InformationWindowText.maxVisibleCharacters,
                    x => InformationWindowText.maxVisibleCharacters = (int)x,
                    InformationWindowText.text.Length, typingDuration).OnComplete(() =>
                    {
                        // 타이핑이 끝난 후 5초 후에 아웃트로 시작
                        Invoke("WindowOutroV", 5f);
                        textruning = false;
                    });
    }
    private void Update()
    {
        if (!textruning && Input.GetKeyDown(KeyCode.G))
        {
            WindowOutroV();
        }
    }
    // 인트로 코루틴 (X축 스케일을 천천히 0에서 1로 변경)
    void WindowIntro()
    {
        StopCoroutine(WindowOutro());
        InformationWindow.localScale = new Vector3(0, 1, 1);  // X축을 0으로 시작
        InformationWindow.DOScaleX(1, 0.15f);  // 0.5초 동안 X축 스케일을 1로
    }

    // 아웃트로 실행
    void WindowOutroV()
    {
        StopCoroutine(WindowOutro());
        StartCoroutine(WindowOutro());
    }

    // 아웃트로 코루틴 (X축 스케일을 점점 줄여서 사라지게)
    IEnumerator WindowOutro()
    {
        float awefrakuhg = basicduration / (InformationWindowText.text.Length*10);
        while (InformationWindowText.text.Length > 0)
        {
            InformationWindowText.text = InformationWindowText.text.Substring(0, InformationWindowText.text.Length - 1); // 마지막 글자 제거
            yield return new WaitForSeconds(awefrakuhg);  // 일정 시간 대기

        }
        InformationWindow.DOScaleX(0, 0.15f);  // 0.5초 동안 X축 스케일을 0으로 줄이기
    }
}
