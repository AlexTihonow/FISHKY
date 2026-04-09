using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.ComponentModel.Design;


public class translate : MonoBehaviour
{
    [TextArea(3, 10)]
    public string RussianTextArea;
    [TextArea(3, 10)]
    public string EnglishTextArea;

    public List<TextMeshProUGUI> allTextMeshProUGUIObjects;
    public string lang;
    public string[] textLines;
    public menu men;

    // Start is called before the first frame update
    void Start()
    {
        /*
        TextMeshProUGUI[] textObjects = FindObjectsOfType<TextMeshProUGUI>(true);
        allTextMeshProUGUIObjects = new List<TextMeshProUGUI>(textObjects);
        */

        update_all_text();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void update_all_text()
    {
        lang = PlayerPrefs.GetString("lang", "EN");
        check_lang();

        for (int i = 0; i < allTextMeshProUGUIObjects.Count; i++)
        {
            allTextMeshProUGUIObjects[i].text = textLines[i];
        }


        if(men != null)
        {
            allTextMeshProUGUIObjects[1].text += "\n" + men.current_score;
            allTextMeshProUGUIObjects[2].text += "\n" + men.best_score;
            allTextMeshProUGUIObjects[9].text = allTextMeshProUGUIObjects[9].text.Replace("_","\n");
        }
       
    }

    public void check_lang()
    {
        switch (lang)
        {
            case "RU":
                textLines = RussianTextArea.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                break;
            default:
                textLines = EnglishTextArea.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                break;
        }
    }
}
