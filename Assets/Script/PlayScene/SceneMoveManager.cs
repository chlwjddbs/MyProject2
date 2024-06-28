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
        //�ε� ���� ��Ȳ�� �ڿ������� �����ϰ� ���� ProgressWrite �� yield return null �� ������ ����Ͽ� ����
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
                    //Time.unscaledDeltaTime ���� ó���� ���ڰ� ũ�� ������ ������ ���� �����ؾ���. if���� �ӽ� ����
                    if (Time.unscaledDeltaTime < 1)
                    {
                        progressEnd += Time.unscaledDeltaTime/10;
                    }

                    //�ε� ���� ��Ȳ�� 100%�� �Ѿ�� �����Ƿ� 100���� ��� ǥ��
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
