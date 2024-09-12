using System.Collections;
using TMPro;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{
    [Header("오브젝트가 활성화 될 때 텍스트 생성하기")]
    public bool isEnableActing;
    TextMeshPro textUI;  // UI를 사용하는 경우 TextMeshProUGUI로 변경해야 함

    private void Start()
    {
        // TextMeshProUGUI를 사용한다면 이 부분을 TextMeshProUGUI로 변경
        textUI = GetComponent<TextMeshPro>();

        // 만약 UI 텍스트일 경우
        // textUI = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (isEnableActing)
        {
            Act("Hello, World!", 0, 0.05f);  // 오브젝트가 활성화될 때 기본 값으로 텍스트 표시
        }
    }

    public  void Act(string message = "", int number = 0, float value = 0f)
    {
        // StartCoroutine을 올바르게 호출
        StartCoroutine(TypeText(message, value));
    }

    // 코루틴으로 타이핑 효과 구현
    private IEnumerator TypeText(string fullText, float typingSpeed)
    {
        textUI.text = "";  // 처음에는 빈 텍스트로 설정

        // 문자열을 순차적으로 한 글자씩 추가
        for (int i = 0; i < fullText.Length; i++)
        {
            textUI.text += fullText[i];  // 한 글자씩 추가
            yield return new WaitForSeconds(typingSpeed);  // 지정된 시간만큼 대기
        }
    }
}
