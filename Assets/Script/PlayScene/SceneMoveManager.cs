using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneMoveManager : MonoBehaviour
{
    static string nextSceneName;
    static Fader fader;
    
    public TextMeshProUGUI loadingText;

    private bool loadStart = false;
    private bool loadEnd = false;

    private AsyncOperation asyncOperation;

    private int progress;

    private void Start()
    {
        Debug.Log(loadStart);
        progress = 0;
        //StartCoroutine(LoadScene());
        //Invoke("LoadStart", 1f);
        //LoadStart();
    }
    
    private void Update()
    {
        if(fader.fadeTime <= 0 && !loadStart)
        {
            loadStart = true;
            StartCoroutine(LoadScene());
        }
    }

    public static void LoadSceneInfo(string _nextSceneName, Fader _fader)
    {
        if(NonLoadingScene(_nextSceneName))
        {
            SceneManager.LoadScene(_nextSceneName);
            return;
        }

        nextSceneName = _nextSceneName;
        fader = _fader;
        SceneManager.LoadScene("LoadingScene");
    }

    public void ProgressWrite()
    {
        progress = (int)(asyncOperation.progress * 100);
        loadingText.text = $"{progress}%";
    }

    IEnumerator LoadScene()
    {
        //로딩 진행 상황을 자연스럽게 연출하게 위해 ProgressWrite 와 yield return null 을 여러번 사용하여 진행
        asyncOperation = SceneManager.LoadSceneAsync(nextSceneName);
        asyncOperation.allowSceneActivation = false;

        ProgressWrite();

        float progressEnd = 0;

        yield return null;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress < 0.9f)
            {
                ProgressWrite();
            }
            else
            {
                if (progress < 100)
                {
                    //Time.unscaledDeltaTime 사용시 처음에 숫자가 크게 찍히는 현상이 있음 수정해야함. if문은 임시 방편
                    if (Time.unscaledDeltaTime < 1)
                    {
                        progressEnd += Time.unscaledDeltaTime/10;
                    }

                    //로딩 진행 상황이 100%를 넘어갈수 없으므로 100까지 묶어서 표시
                    progress = (int)((asyncOperation.progress + progressEnd) * 100);
                    progress = Mathf.Clamp(progress, 0, 100);
                    loadingText.text = $"{(int)progress}%";
                }
                else
                {
                    if (!loadEnd)
                    {
                        loadEnd = true;
                        fader.LoadingScence();
                        fader.FadeIn();
                    }

                    if(fader.fadeTime >= fader.fadeInTime)
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                }
            }
            yield return null;
        }
    }

    public static bool NonLoadingScene(string _nextSceneName)
    {
        if (_nextSceneName == "GameOverScene") return true;
        if (_nextSceneName == "MainMenuScene") return true;
        if (_nextSceneName == "IntroScene") return true;
        return false;
    }

    private void OnDestroy()
    {
        fader = null;
    }
}
