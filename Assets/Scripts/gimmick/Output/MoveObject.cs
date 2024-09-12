using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : GimmickOutput
{
    /*
    public Vector3 targetPos; // 목표 위치
    public Quaternion targetRotation; // 목표 회전
    public AnimationCurve moveCurve; // 이동 애니메이션 커브
    public AnimationCurve rotationCurve; // 회전 애니메이션 커브
    public float moveDuration = 2.0f; // 이동 및 회전에 걸리는 시간

    private Vector3 startPos; // 시작 위치
    private Quaternion startRotation; // 시작 회전

    public bool isDone;

    private void Start()
    {
        isDone = false;
    }

    public override void Act()
    {
        if (!isDone)
        {
            // 현재 오브젝트의 위치와 회전을 시작값으로 저장
            startPos = transform.position;
            startRotation = transform.rotation;

            targetPos = transform.position + targetPos;
            // 코루틴 시작

            StartCoroutine(Movement());
            isDone = true;
        }
    }

    public IEnumerator Movement()
    {
        float elapsedTime = 0f; // 경과 시간 추적

        // 이동 및 회전이 완료될 때까지 반복
        while (elapsedTime < moveDuration)
        {
            // 경과 시간 비율 (0에서 1 사이의 값)
            float t = elapsedTime / moveDuration;

            // 이동 애니메이션 커브를 이용하여 위치 보간
            float moveCurveValue = moveCurve.Evaluate(t);
            float newX =  Mathf.Lerp(startPos.x, targetPos.x, moveCurveValue);
            float newY =  Mathf.Lerp(startPos.y,  targetPos.y, moveCurveValue);
            float newZ =  Mathf.Lerp(startPos.z,  targetPos.z, moveCurveValue);
            transform.position = new Vector3(newX, newY, newZ);

            // 회전 애니메이션 커브를 이용하여 회전 보간
            float rotationCurveValue = rotationCurve.Evaluate(t);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotationCurveValue);

            // 경과 시간 증가
            elapsedTime += Time.deltaTime;

            // 한 프레임 기다림
            yield return null;
        }

        // 이동 및 회전 완료 후 정확한 목표 위치 및 회전으로 설정
        transform.position = new Vector3(
           targetPos.x ,
           targetPos.y ,
           targetPos.z 
        );
        transform.rotation = targetRotation;
    }
    */
    [Header("Position Settings")]
    [HideInInspector] public Vector3 startPosition;  // 시작 위치
    public Vector3 endPosition;  // 목표 위치

    [Header("Rotation Settings")]
    [HideInInspector] public Vector3 startRotation;  // 시작 회전 (Euler angles)
    public Vector3 endRotation;    // 목표 회전 (Euler angles)

    [Header("Animation Settings")]
    public AnimationCurve movementCurve;  // 이동 애니메이션 커브
    public AnimationCurve rotationCurve;  // 회전 애니메이션 커브
    public float duration = 2f;  // 애니메이션 지속 시간

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // 초기 위치 및 회전값 저장
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        isDone = false;
    }

    // 애니메이션을 시작하는 함수 (외부에서 호출)
    public override void Act()
    {
        if (!isDone)
        {
            // 현재 오브젝트의 위치와 회전을 시작값으로 저장
            startPosition = transform.position;
            startRotation = transform.eulerAngles;

            // 코루틴 시작
            StartCoroutine(AnimateMovement(transform.position + endPosition, transform.eulerAngles + endRotation));
        }
    }

    // Coroutine을 통한 애니메이션 처리
    private IEnumerator AnimateMovement(Vector3 targetPos, Vector3 targetRotation)
    {
        float timer = 0f;

        // 시작 위치 및 회전 적용
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // 애니메이션 진행
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);  // 0에서 1까지 진행률 계산

            // 애니메이션 커브 적용
            float curveValue = movementCurve.Evaluate(progress);
            float rotationCurveValue = rotationCurve.Evaluate(progress);

            // 위치 보간
            transform.position = Vector3.Lerp(initialPosition, targetPos, curveValue);

            // 모든 축의 회전을 보간
            Quaternion startRot = initialRotation;
            Quaternion endRot = Quaternion.Euler(targetRotation);
            transform.rotation = Quaternion.Lerp(startRot, endRot, rotationCurveValue);

            yield return null;  // 한 프레임 대기
        }

        // 애니메이션이 끝난 후 정확한 목표 위치 및 회전으로 설정
        transform.position = targetPos;
        transform.rotation = Quaternion.Euler(targetRotation);

        isDone = true;
    }
}
