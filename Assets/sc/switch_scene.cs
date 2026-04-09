using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class switch_scene : MonoBehaviour
{
    public Image back_panel;
    public GameObject intface_panel;
    public TextMeshProUGUI Game_over_text;
    public float duration;
    public List<string> funny_frases = new List<string>();
    public float[] probability;
    public translate trns;
    void Start()
    {
        back_panel.raycastTarget = true;
        back_panel.color = new Color(back_panel.color.r, back_panel.color.g, back_panel.color.b, 1f);
        Start_fageout();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public int GetRandomNumber(int[] numbers, float[] probabilities)
    {
        float sum = probabilities.Sum();
        float[] normalizedProbabilities = probabilities.Select(p => p / sum).ToArray();

        float[] cumulativeProbabilities = new float[normalizedProbabilities.Length];
        cumulativeProbabilities[0] = normalizedProbabilities[0];
        for (int i = 1; i < normalizedProbabilities.Length; i++)
        {
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + normalizedProbabilities[i];
        }
        float randomValue = Random.value;

        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (randomValue <= cumulativeProbabilities[i])
            {
                return numbers[i];
            }
        }

        return numbers[numbers.Length - 1];
    }


    public void Random_fras()
    {
        int[] numbersy = Enumerable.Range(3, 7).ToArray();
        int index = GetRandomNumber(numbersy, probability);
        Game_over_text.text = trns.textLines[index];
    }
    public async void Transition_to_Game_Over()
    {
        back_panel.raycastTarget = true;
        Random_fras();
        await back_panel.DOFade(1f, duration).SetEase(Ease.InCirc).AsyncWaitForCompletion();
        if(intface_panel != null)
            intface_panel.transform.DOScale(new Vector3(1f,1f,1f), 1f).SetEase(Ease.InOutExpo);
    }

    public async void next_scene()
    {
        await intface_panel.transform.DOScale(new Vector3(0f,0f,0f), 1f).SetEase(Ease.InOutExpo).AsyncWaitForCompletion();
        SceneManager.LoadScene("menu");
    }

    public async void Start_fageout()
    {
        await back_panel.DOFade(0f, duration).SetEase(Ease.InCirc).AsyncWaitForCompletion();
        back_panel.raycastTarget = false;
    }
}
