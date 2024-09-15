using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    // 오디오 클립을 재생하기 위한 오디오 소스 리스트
    private List<AudioSource> audioSources = new List<AudioSource>();

    public float SFXvolume;

    // 오디오 소스 오브젝트의 수
    public int initialAudioSourceCount = 10;


    // 브금 컨트롤러
    public float BGMvolume;

    public float fadeDuration = 1.0f; // 페이드 전환 시간

    public AudioSource activeSource; // 현재 활성화된 오디오 소스
    public AudioSource inactiveSource; // 현재 비활성화된 오디오 소스




    void Start()
    {
        // 초기화할 때 AudioSource 오브젝트 생성
        for (int i = 0; i < initialAudioSourceCount; i++)
        {
            GameObject soundObject = new GameObject("AudioSource_" + i);
            soundObject.transform.parent = this.transform;  // SoundManager의 자식으로 추가
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSources.Add(audioSource);
        }

        GameObject bgmObject1 = new GameObject("bgmObject " + 1);
        bgmObject1.transform.parent = this.transform;  // SoundManager의 자식으로 추가
        AudioSource audioSource1 = bgmObject1.AddComponent<AudioSource>();
        audioSource1.loop = true;

        GameObject bgmObject2 = new GameObject("bgmObject " + 2);
        bgmObject2.transform.parent = this.transform;  // SoundManager의 자식으로 추가
        AudioSource audioSource2 = bgmObject2.AddComponent<AudioSource>();
        audioSource2.loop = true;

        activeSource = audioSource1;
        inactiveSource = audioSource2;

        SetBGMvolume(BGMvolume);
    }

    public void SetBGMvolume(float value)
    {
        activeSource.volume = value;
        inactiveSource.volume = value;
    }

    public void SetFadeDuration(float value)
    {
        fadeDuration = value;
    }

    public void BGM(AudioClip newClip)
    {
        StartCoroutine(SwitchToNewClip(newClip));
    }

    private IEnumerator SwitchToNewClip(AudioClip newClip)
    {
        // 현재 활성화된 소스의 볼륨을 줄이기
        float startVolume = activeSource.volume;

        // 페이드 아웃
        while (activeSource.volume > 0)
        {
            activeSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = startVolume;

        // 비활성화된 소스에 새 클립 설정 및 재생
        inactiveSource.clip = newClip;
        inactiveSource.Play();
        inactiveSource.volume = 0;

        // 페이드 인
        while (inactiveSource.volume < startVolume)
        {
            inactiveSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        inactiveSource.volume = startVolume;

        // 소스 전환
        // 활성화된 소스를 비활성화된 소스로 전환
        AudioSource temp = activeSource;
        activeSource = inactiveSource;
        inactiveSource = temp;
    }


    public void SFX(AudioClip clip)
    {
        PlaySound(clip);
    }
    public void SetSFXVolume(float volume)
    {
        SFXvolume = volume;
    }

    // 사운드를 재생하는 함수, 위치가 없으면 0,0,0에 생성
    public void PlaySound(AudioClip clip, float volume = 0, Vector3? position = null)
    {
        // 사용 가능한 오디오 소스를 찾음
        AudioSource availableSource = GetAvailableAudioSource();

        if (availableSource != null)
        {
            // 오디오 소스 위치 설정 (값이 없으면 0,0,0 사용)
            availableSource.transform.position = position ?? Vector3.zero;

            // 오디오 클립 설정 및 재생
            availableSource.clip = clip;
            availableSource.volume = volume == 0 ? SFXvolume : volume;
            availableSource.Play();
        }
        else
        {
            Debug.LogWarning("사용 가능한 오디오 소스가 없습니다.");
        }
    }

    // 사용 가능한 오디오 소스를 찾는 함수
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (!audioSource.isPlaying)  // 재생 중이지 않은 오디오 소스 찾기
                return audioSource;
        }

        // 모든 오디오 소스가 사용 중일 경우 null 반환
        return null;
    }
}
