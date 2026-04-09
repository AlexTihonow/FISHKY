using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

public class menu : MonoBehaviour
{
    public int current_score;
    public int best_score;
    public translate tr;
    public Animator score_anim;
    public Image back_panel;
    public float duration;
    public List<GameObject> Main_gameObjects, Settings_gameObjects, Credits_gameObjects;
    public Transform left, center, right;
    public RandomAudioPlayer mg;
    // Start is called before the first frame update
    void Start()
    {
        Update_score();
        Start_fageout();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Update_score()
    {
        current_score = PlayerPrefs.GetInt("current_score");
        best_score = PlayerPrefs.GetInt("best_score");

        if(current_score > best_score)
        {
            PlayerPrefs.SetInt("best_score", current_score);
            best_score = current_score;
            score_anim.Play("new_best");
        }

        tr.update_all_text();
    }

    public async void Play()
    {
        if(mg.isMusicOn)
            mg.StartCoroutine(mg.Fade(0.118f, 0));
        await back_panel.DOFade(1f, duration).SetEase(Ease.InCirc).AsyncWaitForCompletion();
        SceneManager.LoadScene("level");
    }

    public async void Start_fageout()
    {
        back_panel.color = new Color(back_panel.color.r,back_panel.color.g,back_panel.color.b,1f);
        back_panel.raycastTarget = true;
        await back_panel.DOFade(0f, duration).SetEase(Ease.InCirc).AsyncWaitForCompletion();
        back_panel.raycastTarget = false;
    }

    public async void Settings()
    {
        await Acti_move_a_lot(Main_gameObjects, center.position, right.position);
        await Acti_move_a_lot(Settings_gameObjects, left.position, center.position);
    }

    public async void From_Settings_TO_MENU()
    {
        await Acti_move_a_lot(Settings_gameObjects, center.position, right.position);
        await Acti_move_a_lot(Main_gameObjects, left.position, center.position);
    }

    public async void Credits()
    {
        await Acti_move_a_lot(Main_gameObjects, center.position, left.position);
        await Acti_move_a_lot(Credits_gameObjects, right.position, center.position); 
    }

    public async void From_Credits_to_menu()
    {
        await Acti_move_a_lot(Credits_gameObjects, center.position, left.position);
        await Acti_move_a_lot(Main_gameObjects, right.position, center.position);
        
    }

    public async UniTask Acti_move_a_lot(List<GameObject> gam, Vector3 start, Vector3 moving)
    {
        for (int i = 0; i < gam.Count; i++)
        {
            gam[i].transform.position = new Vector3(start.x, gam[i].transform.position.y, gam[i].transform.position.z);
        }

        for (int i = 0; i < gam.Count; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            Vector3 anm = new Vector3(moving.x, gam[i].transform.position.y, gam[i].transform.position.z);
            gam[i].transform.DOMove(anm, 2f).SetEase(Ease.InOutExpo);
        }
    }
    public void Language()
    {
        if( tr.lang != "RU")
            tr.lang = "RU";
        else tr.lang = "EN";

        PlayerPrefs.SetString("lang", tr.lang);
        tr.update_all_text();
    }
}
