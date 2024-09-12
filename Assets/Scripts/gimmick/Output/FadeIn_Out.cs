using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn_Out : GimmickOutput
{
    public enum FadeType { FadeIn, FadeOut }

    public float duration = 1f; // 페이드 효과의 지속 시간
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // 기본 애니메이션 커브
    public FadeType fadeType; // 페이드 인 또는 페이드 아웃 여부

    public Image targetImage; // 페이드를 적용할 UI 이미지 컴포넌트

    private void Start()
    {
        targetImage.gameObject.SetActive(false);
    }

    public override void Act()
    {
        // targetImage가 할당되지 않은 경우 경고를 출력합니다.
        if (targetImage == null)
        {
            Debug.LogError("targetImage가 할당되지 않았습니다. 인스펙터에서 UI Image를 할당하세요.");
            return;

        }
        targetImage.gameObject.SetActive(true);
        // 페이드 효과를 시작합니다.
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float elapsedTime = 0f;
        float startAlpha = fadeType == FadeType.FadeIn ? 0f : 1f;
        float endAlpha = fadeType == FadeType.FadeIn ? 1f : 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            float alpha = fadeCurve.Evaluate(normalizedTime);

            Color color = targetImage.color;
            color.a = Mathf.Lerp(startAlpha, endAlpha, alpha);
            targetImage.color = color;

            yield return null;
        }

        // 최종 알파 값이 정확히 설정되도록 보장합니다.
        Color finalColor = targetImage.color;
        finalColor.a = endAlpha;
        targetImage.color = finalColor;
        isDone = true;
        if (fadeType == FadeType.FadeOut)
        {
            targetImage.gameObject.SetActive(false);
        }
    }
}
