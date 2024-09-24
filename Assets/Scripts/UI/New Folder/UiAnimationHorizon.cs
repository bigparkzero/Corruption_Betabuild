using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAnimationHorizon : MonoBehaviour
{
    public RectTransform uiElement; // 이동시킬 UI 요소
    public float moveDistance = 500f; // 이동 거리
    public float duration = 1f;      // 이동 시간
    public float minDelay = 0.5f;    // 최소 지연 시간
    public float maxDelay = 2f;      // 최대 지연 시간

    void Start()
    {
        // 왼쪽에서 오른쪽으로 이동 후 다시 랜덤 딜레이로 되돌아오는 애니메이션
        MoveUI(uiElement);
    }

    void MoveUI(RectTransform uiElement)
    {
        // UI의 현재 위치를 기준으로 오른쪽으로 이동
        uiElement.DOAnchorPosX(moveDistance, duration).SetEase(Ease.InOutQuad)
            // 오른쪽으로 이동한 뒤 다시 왼쪽으로 되돌아감
            .OnComplete(() =>
            {
                // 왼쪽으로 되돌아오기 전에 랜덤 딜레이 설정
                float randomDelay = Random.Range(minDelay, maxDelay);
                uiElement.DOAnchorPosX(-moveDistance, duration).SetEase(Ease.InOutQuad)
                    .SetDelay(randomDelay) // 랜덤 지연 시간 추가
                    .OnComplete(() =>
                    {
                        // 다시 랜덤 딜레이로 이동 반복
                        randomDelay = Random.Range(minDelay, maxDelay);
                        MoveUI(uiElement);
                    });
            });
    }
}
