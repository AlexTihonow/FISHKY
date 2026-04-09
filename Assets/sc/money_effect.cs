using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class money_effect : MonoBehaviour
{
    public GameObject money_obj;
    public Transform end_point;
    public Transform start_point;
    public float duration = 2f;
    public Color end_color;
    public DynamicGrid DiGr;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async UniTask Acti(Vector2 spawn, Color start_color)
    {
        GameObject some_dude = Instantiate(money_obj, new Vector2(spawn.x,spawn.y), Quaternion.identity);
        some_dude.GetComponent<SpriteRenderer>().color = start_color;
        await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0.2f,0.4f)));
        
        
       
        some_dude.GetComponent<SpriteRenderer>().DOColor(end_color, duration).SetEase(Ease.InCirc);
        some_dude.transform.DOScale(Vector2.zero, duration).SetEase(Ease.InCirc);
        Vector3 abc = new Vector3(end_point.transform.position.x,end_point.transform.position.y,0);
        await some_dude.transform.DOMove(abc, duration).SetEase(Ease.InBack).AsyncWaitForCompletion();
        DiGr.audioRe[0].Play();
        await end_point.DOPunchScale(Vector3.one/10, 0.1f).SetEase(Ease.InOutElastic).AsyncWaitForCompletion();
        end_point.DOScale(new Vector2(1f,1f), 0.2f).SetEase(Ease.OutElastic);
       
        Destroy(some_dude,5f);
    }

    public async void Make_an_obj(Vector2 spawn, Color start_color)
    {
        await Acti(spawn, start_color);
        DiGr.scoring += 1;
        DiGr.plus_score();
       
    }
}
