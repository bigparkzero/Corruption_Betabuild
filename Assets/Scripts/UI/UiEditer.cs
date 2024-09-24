using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class UiEditer : MonoBehaviour
{
    public RectTransform mainWindow;  // 메인 윈도우의 RectTransform
    public Vector2 targetPosition = new Vector2(-500f, 300f);  // 메인 윈도우가 이동할 위치
    public float duration = 1f;  // 애니메이션 재생 시간

    private Coroutine openCoroutine;  // 코루틴을 관리하기 위한 변수
    private Coroutine closeCoroutine;

    public bool isopen;

    public RectTransform anykeypresstext;


    public List<RectTransform> textlist;

    private void Update()
    {
        /*
        if (Input.anyKeyDown && !isopen)
        {
            mainwondowopen();
        }
       */
        if(Input.GetKeyDown(KeyCode.O))
            mainwondowopen();
        if (Input.GetKeyDown(KeyCode.C))
            mainwondowcloes();
    }

    public void mainwondowopen()
    {
        // 이미 실행 중인 코루틴이 있으면 중지
        if (openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
        }
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }

        // 새로운 코루틴 시작
        openCoroutine = StartCoroutine(mainwondowopenCoroutine());
    }

    IEnumerator mainwondowopenCoroutine()
    {
        isopen = true;
        // 스케일을 (0.78,0.78,0.78)에서 (0.53,0.53,0.53)으로 애니메이션
        mainWindow.localScale = new Vector3(0.78f, 0.78f, 0.78f);  // 스케일을 초기화
        mainWindow.DOScale(new Vector3(0.53f, 0.53f, 0.53f), duration).SetEase(Ease.OutBack);  // 스케일 애니메이션

        // 위치를 (0,0)에서 targetPosition으로 이동
        mainWindow.DOAnchorPos(targetPosition, duration).SetEase(Ease.InOutQuad);
        anykeypresstext.DOAnchorPos(Vector2.down * 1000, duration / 4).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration);  // 애니메이션이 완료될 때까지 대기
        for (int i = 0; textlist.Count > i; i++)
        {
            textlist[i].DOAnchorPos(new Vector2(-480, -10 - 80 *i), duration / 4).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(duration/16);
        }
    }

    public void mainwondowcloes()
    {
        // 이미 실행 중인 코루틴이 있으면 중지
        if (openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
        }
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
        }

        // 새로운 코루틴 시작
        closeCoroutine = StartCoroutine(mainwondowcloesCoroutine());
    }

    IEnumerator mainwondowcloesCoroutine()
    {
        for (int i = 0; textlist.Count > i; i++)
        {
            textlist[i].DOAnchorPos(new Vector2(-1480,- 10 - 80 * i), duration / 4).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(duration / 16);
        }
        // 스케일을 (0.53,0.53,0.53)에서 (0.78,0.78,0.78)으로 애니메이션
        mainWindow.DOScale(new Vector3(0.78f, 0.78f, 0.78f), duration).SetEase(Ease.InBack);  // 스케일 애니메이션

        // 위치를 targetPosition에서 (0,0)으로 이동
        mainWindow.DOAnchorPos(new Vector2(0,0), duration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration);  // 애니메이션이 완료될 때까지 대기
        anykeypresstext.DOAnchorPos(Vector2.up * -300, duration / 4).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(duration / 4);  // 애니메이션이 완료될 때까지 대기
        isopen = false;
    }
}
