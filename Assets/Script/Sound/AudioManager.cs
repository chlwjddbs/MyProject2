using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //sound ������ ������ ���� sound ���
    public Sound[] sounds;
    public List<Sound> externalSound = new List<Sound>();
    public Sound[] skillSound;

    //���������� ���� ����� �ͼ�
    public AudioMixer audioMixer;
    public AudioMixerGroup[] audioGroups;

    //���� ������� bgm�̸�
    private string currentBGM;
    private string currentAm;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //����� �ͼ��� "Master" ��� �̸��� �׷��� ã�� �־��ش�. 
        //����� �θ� �׷��� Master, �׷� ����� BGM SFX Effext�� �ִ�.
        audioGroups = audioMixer.FindMatchingGroups("Master");
       
        //�غ��� ���带 ��� �� �����ϱ� ���� ����� �Ŵ����� �߰����ش�.
        foreach (var s in sounds)
        {
            //���� AudioManager ������Ʈ�� ���ο� AudioSource ������Ʈ�� �����Ͽ�
            s.source = gameObject.AddComponent<AudioSource>();
            //Sound Ŭ������ ���� ���� ���� ������ ������ AudioSource�� ������ �ش�.
            //Sound Ŭ�������� ����� ����� clip�� 
            //�� Ŭ���� ������ ����� �̸�, ó�� ������ volume�� pitch���� �ݺ� ���θ� ������ �������ش�.
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            //���帶�� ����� �뵵�� �ٸ��� ������ �뵵�� ���� �����Ͽ� �ͼ� �׷쿡 ������ �ش�.

            //bgm�� bgm �ͼ��� ����
            if (s.soundType == SoundType.BGM)
            {
                s.source.outputAudioMixerGroup = audioGroups[1];
            }
            //ȿ������ sfx�� ����
            else if(s.soundType == SoundType.SFX)
            {
                s.source.outputAudioMixerGroup = audioGroups[2];
            }
            //������(����� ȯ����) Ambience�� ����
            else
            {
                s.source.outputAudioMixerGroup = audioGroups[3];
            }
        }
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += SceneStart;
    }

    private void SceneStart(Scene scene, LoadSceneMode mode) 
    {
        PlayBGM(SceneManager.GetActiveScene().name);
    }

    // Start is called before the first frame update
    void Start()
    {       
        /*
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].loop)
            {
                sounds[i].source.Play();
                break;
            }
        }
        */
        //PlayeSound("Footstep");
    }

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayBGM("MainMenuBGM");
        }
    }
    */

    //bgm�� ������ ���带 �����Ͽ� Play �Լ��� ������� bgm�� ������ ���� ������ �ڵ����� stop�� ��Ű�� �ٸ� bgm�� play�ϱ� ����
    public void PlayBGM(string _bgmName , float fadeTime = 0)
    {
        //���� ������� bgm�� Play�϶�� ���� Bgm�� ���ٸ� ���� �����ش�.
        //���� bgm�� �ٽ� ����ϸ� bgm�� �߰��� ����� ó������ ����Ǳ� ����.
        if (currentBGM == _bgmName)
        {
            return;
        }

        //���� ������� bgm�� ���� ��Ų��.
        Stop(currentBGM);

        Sound sound = null;
        
        //�ݺ����� ���� ��û ���� ������ �ִ��� Ȯ���� ��
        foreach (var s in sounds)
        {
            if(s.name == _bgmName)
            {
                currentBGM = _bgmName;
                sound = s;
                break;
            }
        }

        //������ ������ ����
        if(sound == null) 
        {
            Debug.Log("������ �����ϴ�.");
            return;
        }

        StartCoroutine(fadeOutBgm(sound, fadeTime));

        //������ ���� ���       
        //sound.source.Play();

        //sound�� ���� ���簡 �Ǿ��� ������ ����� �Ŵ����� �����Ͽ� sound�� ����Ǿ ����� �Ŵ����� ������ �ش�.
        //sound�� ���������� �ż��� ���� �� �������.
    }

    IEnumerator fadeOutBgm(Sound _sound , float fadeTime)
    {     
        float t = 0;
        _sound.source.volume = t;
        _sound.source.Play();

        while (t <= fadeTime)
        {
            t += Time.unscaledDeltaTime;
            _sound.source.volume = t / fadeTime;
            yield return null;
        }

        _sound.source.volume = 1f;
    }

    public void StopBGM(float fadeTime)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if (s.name == currentBGM)
            {
                sound = s;
                break;
            }
        }

        if (sound == null)
        {
            Debug.Log("������� BGM�� �����ϴ�");
            return;
        }
        
        //bgm ���� �۾����� �ϱ�
        StartCoroutine(fadeInBgm(sound, fadeTime));
        
        currentBGM = null;
    }

    IEnumerator fadeInBgm(Sound _sound ,float fadeTime)
    {
        float t = fadeTime;
        while (0 <= t)
        {
            t -= Time.unscaledDeltaTime;
            _sound.source.volume = t / fadeTime;
            yield return null;
        }

        _sound.source.Stop();
        _sound.source.volume = 1f;
    }

    public void Stop(string _soundName)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if(s.name == _soundName)
            {
                sound = s;
                break;
            }
        }

        if(sound == null)
        {
            return;
        }
        sound.source.Stop();
    }

    public void PlayeSound(string _soundName)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if(s.name == _soundName)
            {
                sound = s;
                break;
            }        
        }

        if(sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.Log(_soundName);
        }
    }

    public void PlayAmSond(string _soundName)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if (s.name == _soundName)
            {
                sound = s;
                break;
            }
        }
        
        if(currentAm == _soundName)
        {
            return;
        }

        if (sound != null)
        {
            currentAm = sound.name;
            sound.source.Play();
        }
        else
        {
            Debug.Log(_soundName);
        }
    }

    public void StopAm(string _soundName)
    {
        Sound sound = null;
        foreach (var s in sounds)
        {
            if (s.name == _soundName)
            {
                sound = s;
                break;
            }
        }

        if (sound == null)
        {
            return;
        }

        currentAm = null;
        sound.source.Stop();
    }

    public void AddExternalSound(Sound s)
    {
        if (!externalSound.Contains(s))
        {
            externalSound.Add(s);
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
         
            if (s.soundType == SoundType.BGM)
            {
                s.source.outputAudioMixerGroup = audioGroups[1];
            }
            else if (s.soundType == SoundType.SFX)
            {
                s.source.outputAudioMixerGroup = audioGroups[2];
            }
            else
            {
                s.source.outputAudioMixerGroup = audioGroups[3];
            }
        }
    }

    public void PlayExSound(string _soundName)
    {
        Sound sound = null;
        foreach (var s in externalSound)
        {
            if (s.name == _soundName)
            {
                sound = s;
                break;
            }
        }

        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.Log(_soundName);
        }
    }

    public void AddSkillSound(Sound s , int buttonNum)
    {
        skillSound[buttonNum] = s;
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        if (s.soundType == SoundType.BGM)
        {
            s.source.outputAudioMixerGroup = audioGroups[1];
        }  
        else if (s.soundType == SoundType.SFX)
        {
            s.source.outputAudioMixerGroup = audioGroups[2];
        }
        else
        {
            s.source.outputAudioMixerGroup = audioGroups[3];
        }
    }

    public void RemoveSkillSound(int buttonNum)
    {
        if(skillSound[buttonNum] == null)
        {
            return;
        }

        Destroy(skillSound[buttonNum].source);
        skillSound[buttonNum] = null;
    }

    public void PlayerSkillSound(int buttonNum)
    {
        skillSound[buttonNum].source.Play();
    }

    public void AllAmStop()
    {
        foreach (var item in sounds)
        {
            if(item.soundType == SoundType.Ambience)
            {
                item.source.Stop();
            }
        }

        foreach (var item in externalSound)
        {
            if (item.soundType == SoundType.Ambience)
            {
                item.source.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneStart;
    }
}
