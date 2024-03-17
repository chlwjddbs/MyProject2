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
        AudioManager.instance.StopBGM(fadeInTime);
        float t = 0.0f;
        while (t <= fadeInTime)
        {
            //t += Time.deltaTime;
            t += Time.unscaledDeltaTime;
            faderImage.color = new Color(0, 0, 0, t/ fadeInTime);
            yield return null;
        }
        Debug.Log("fadeIN");
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

        float t = 0.0f;
        while (t <= fadeInTime)
        {
            //t += Time.deltaTime;
            t += Time.unscaledDeltaTime;
            faderImage.color = new Color(0, 0, 0, t / fadeInTime);
            yield return null;
        }

        yield return null;

        SceneMoveManager.LoadSceneInfo(_sceneName);
    }

    //FadeOut�� �� ���� �� �ƴ϶� Ư�� ��Ȳ������ ����� ��� �� �� �����Ƿ� ���� �ؼ� ����
    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(C_FadeOut());
    }

    IEnumerator C_FadeOut()
    {
        //StopAllCoroutines();
        yield return null;

        //AudioManager.instance.PlayBGM("MainMenuBGM");
        float t = fadeOutTime;
        while (t >= 0)
        {
            t -= Time.unscaledDeltaTime;
            faderImage.color = new Color(0, 0, 0, t/ fadeOutTime);
            yield return null;
        }

        yield return null;
    }

    public void SceneStartFadeOut(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name == "LoadingScene")  
        {  
            return;
        }

        StartCoroutine(C_FadeOut());
        //Debug.Log("fade");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneStartFadeOut;
    }


}
