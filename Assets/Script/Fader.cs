using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour
{
    public Image faderImage;
    public float fadeInTime = 1f;
    public float fadeOutTime = 2f;

    public float fadeTime;

    /*
    // Start is called before the first frame update
    void Start()
    {
        //faderImage.color = new Color(0, 0, 0, 1);
        
        //FadeOut();
        //Debug.Log("fade start");
        //FadeIn();
    }
    */

    //Fader�� ���� �ٲ� ���� �ҷ��� ����ؾ� �Ѵ�.
    //�׷����� SceneManager.sceneLoaded�� ����Ͽ� ���� ���� �ɋ� �����ϵ��� �Ѵ�.
    //SceneManager.sceneLoaded�� �����ֱ⿡ ������
    //Awake -> enable -> SceneManager.sceneLoaded -> start ���̴�.
    //�׷��Ƿ� start���� ����ؼ� ����ϵ��� ����
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneStartFadeOut;     
    }

    /*
    private void Start()
    {
        Debug.Log("fade start");
    }
    */

    public void FadeIn()
    {
        StartCoroutine(C_FadeIn());
    }

    IEnumerator C_FadeIn()
    {
        //AudioManager.instance.StopBGM(fadeInTime);
        fadeTime = 0.0f;
        while (fadeTime < fadeInTime)
        {
            if (Time.unscaledDeltaTime < 1)
            {
                //t += Time.deltaTime;
                fadeTime += Time.unscaledDeltaTime;
                fadeTime = Mathf.Clamp(fadeTime, 0, fadeInTime);
                faderImage.color = new Color(0, 0, 0, fadeTime / fadeInTime);
            }
            yield return null;
        }
        //Debug.Log("fadeIN");
    }

    public void SceneLoad(string _sceneName)
    {
        StopAllCoroutines();
        StartCoroutine(SL_FadeIn(_sceneName));
    }

    IEnumerator SL_FadeIn(string _sceneName)
    {
        AudioManager.instance.StopBGM(fadeInTime);
        AudioManager.instance.AllAmStop();

        fadeTime = 0.0f;
        while (fadeTime < fadeInTime)
        {
            if (Time.unscaledDeltaTime < 1)
            {
                fadeTime += Time.unscaledDeltaTime;
                fadeTime = Mathf.Clamp(fadeTime, 0, fadeInTime);
                faderImage.color = new Color(0, 0, 0, fadeTime / fadeInTime);
            }
            yield return null;
        }

        yield return null;

        SceneMoveManager.LoadSceneInfo(_sceneName,this);
    }

    //FadeOut�� �� ���� �� �ƴ϶� Ư�� ��Ȳ������ ����� ��� �� �� �����Ƿ� ���� �ؼ� ����
    public void FadeOut()
    {
        //StopAllCoroutines();
        StartCoroutine(C_FadeOut());
    }

    IEnumerator C_FadeOut()
    {
        //StopAllCoroutines();
        yield return null;

        //AudioManager.instance.PlayBGM("MainMenuBGM");
        fadeTime = fadeOutTime;
        while (fadeTime > 0)
        {
            if (Time.unscaledDeltaTime < 1)
            {
                fadeTime -= Time.unscaledDeltaTime;
                fadeTime = Mathf.Clamp(fadeTime, 0, fadeOutTime);
                faderImage.color = new Color(0, 0, 0, fadeTime / fadeOutTime);
            }   
            yield return null;
        }

        yield return null;
    }

    IEnumerator LoadScence_FadeOut()
    {
        //StopAllCoroutines();
        yield return null;

        //AudioManager.instance.PlayBGM("MainMenuBGM");
        fadeTime = 1f;
        while (fadeTime > 0)
        {
            if (Time.unscaledDeltaTime < 1)
            {
                fadeTime -= Time.unscaledDeltaTime;
                fadeTime = Mathf.Clamp(fadeTime, 0, fadeOutTime);
                faderImage.color = new Color(0, 0, 0, fadeTime / fadeOutTime);
            }
            yield return null;
        }

        yield return null;
    }

    public void SceneStartFadeOut(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();

        if (SceneManager.GetActiveScene().name == "LoadingScene")
        {
            StartCoroutine(LoadScence_FadeOut());
            return;
        }

        StartCoroutine(C_FadeOut());
    }

    public void LoadingScence()
    {
        StopAllCoroutines();
        faderImage.color = new Color(0, 0, 0, 0);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneStartFadeOut;
    }
}
