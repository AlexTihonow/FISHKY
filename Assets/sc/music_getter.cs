using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource; // Источник звука
    public List<AudioClip> audioClips; // Список аудио треков
    public float fadeDuration = 0.3f; // Длительность плавного изменения громкости
    public TextMeshProUGUI musicStatusText, soundStatusText; // Ссылка на TextMesh Pro для отображения статуса музыки

    public bool isMusicOn = true, isSoundOn = true; // Состояние музыки (включена/выключена)
    private Coroutine playCoroutine; // Ссылка на корутину воспроизведения
    public GameObject sounds;

    private void Start()
    {
        // Загружаем состояние музыки из PlayerPrefs
        isMusicOn = PlayerPrefs.GetInt( "IsMusicOn", 1) == 1;
        isSoundOn = PlayerPrefs.GetInt( "IsSoundOn", 1) == 1;
        sounds.SetActive(isSoundOn);

        if (musicStatusText != null)
            musicStatusText.fontStyle = isMusicOn ? FontStyles.Normal : FontStyles.Strikethrough;
        if (soundStatusText != null)
            soundStatusText.fontStyle = isSoundOn ? FontStyles.Normal : FontStyles.Strikethrough;

        if (audioClips.Count > 0 && isMusicOn)
        {
            playCoroutine = StartCoroutine(PlayRandomizedClips());
        }
        else if (audioClips.Count == 0)
        {
            Debug.LogWarning("Список аудио треков пуст!");
        }
    }

    // Метод для переключения состояния музыки
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("IsMusicOn", isMusicOn ? 1 : 0);
        if (musicStatusText != null)
        {
            musicStatusText.fontStyle = isMusicOn ? FontStyles.Normal : FontStyles.Strikethrough;
        }

        if (!isMusicOn && playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            StartCoroutine(Fade(audioSource.volume, 0f));
        }
        else if (isMusicOn)
        {
            playCoroutine = StartCoroutine(PlayRandomizedClips());
        }
    }

    public void TogglSound()
    {
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt("IsSoundOn", isSoundOn ? 1 : 0);
        if (soundStatusText != null)
        {
            soundStatusText.fontStyle = isSoundOn ? FontStyles.Normal : FontStyles.Strikethrough;
        }
        
        sounds.SetActive(PlayerPrefs.GetInt( "IsSoundOn", 1) == 1);
    }

    private IEnumerator PlayRandomizedClips()
    {
        while (isMusicOn) // Цикл работает только если музыка включена
        {
            ShuffleList(audioClips);

            foreach (AudioClip clip in audioClips)
            {
                audioSource.clip = clip;
                audioSource.volume = 0f;
                audioSource.Play();

                yield return StartCoroutine(Fade(0, 0.118f));
                yield return new WaitForSeconds(clip.length - fadeDuration);
                yield return StartCoroutine(Fade(0.118f, 0));
            }
        }
    }

    public IEnumerator Fade(float startVolume, float targetVolume)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // Метод для перемешивания списка
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
