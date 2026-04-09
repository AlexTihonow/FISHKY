using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class DynamicGrid : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform gridPanel;
    public int numberOfButtons;

    public char[,] world_cells;
    public Button[,] world_cells_buttons;
    public List<Vector2Int> Fishka_shape = new List<Vector2Int>();

    Vector2Int right = new Vector2Int(1,0);
    Vector2Int up = new Vector2Int(0,1);
    Vector2Int down = new Vector2Int(0,-1);
    Vector2Int left = new Vector2Int(-1,0);

    
    public float timer;
    float counter;
    public int scoring;
    public TextMeshProUGUI Score_text, Time_text;
   
    [SerializeField] public List<Color> ColorMap = new List<Color>();
    Color current_color;
    public char type_of_fishka;
    public Image face_of_fishka;
    private static int? previousNumber = null;
    bool pressed_f;
    public string all_colors = "RGBDPYCSMLW";
    public money_effect ME;
    public switch_scene Swsc;

    public List<AudioSource> audioRe;
    public translate trns;
    void Start()
    {
        PlayerPrefs.SetInt("current_score", 0);

        ME = GetComponent<money_effect>();
        counter = timer;
        int r = numberOfButtons*numberOfButtons;
        world_cells = new char[numberOfButtons, numberOfButtons];
        world_cells_buttons = new Button[numberOfButtons,numberOfButtons];

        //'\0'
        for (int i = 0; i < r; i++)
        {
            GameObject add_new_father = Instantiate(buttonPrefab, gridPanel);
            GameObject add_new = add_new_father.transform.GetChild(0).gameObject;
            Vector2Int posi = new Vector2Int(i % numberOfButtons, i / numberOfButtons);
            add_new_father.name = "Fishka_btn_"+posi.x.ToString()+" "+posi.y.ToString();
            Image new_child = add_new.transform.GetChild(0).gameObject.GetComponent<Image>();
            add_new.GetComponent<Button>().onClick.AddListener(() =>
            {
               button_on_ground(new_child, posi);
            });

            world_cells_buttons[posi.x,posi.y] = add_new.GetComponent<Button>();
        }
        plus_score();
        Generate_new_Color();
    }

    void Update()
    {
        timer_update();
    }

    public void GameOvering()
    {
        if(scoring <= -3 || IsValidWorldCells(world_cells))
        {
            Swsc.Transition_to_Game_Over();
        }
    }

    public static bool IsValidWorldCells(char[,] world_cells)
    {
        int rows = world_cells.GetLength(0);
        int cols = world_cells.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (world_cells[i, j] == '\0')
                {
                    return false;
                }
            }
        }
        return true;
    }
        
    public void Generate_pos_for_timer_fishka()
    {
        int xx = Random.Range(0, numberOfButtons);
        int yy = Random.Range(0, numberOfButtons);
        int limi = 0;
        Debug.Log(xx+" "+ yy+" "+ limi);
        while (world_cells[xx, yy] != '\0' && limi<=1000)
        {
            xx = Random.Range(0, numberOfButtons);
            yy = Random.Range(0, numberOfButtons);
            limi++;
            Debug.Log(xx+" "+ yy+" "+ limi);
        }
        Vector2Int positi = new Vector2Int(xx,yy); 
        add_circle(positi);
    }

    public static int GenerateNCRandom(int min, int max)
    {
        int currentNumber;
        do
        {
            currentNumber = Random.Range(min, max);
        } while (currentNumber == previousNumber);

        previousNumber = currentNumber;
        return currentNumber;
    }
    

    public void Generate_new_Color()
    {
        int random_color = GenerateNCRandom(0, ColorMap.Count);
        current_color = ColorMap[random_color];
        type_of_fishka = all_colors[random_color];

        face_of_fishka.color = current_color;
    }
    public void timer_update()
    {
        if (counter <= 0)
        {
            counter = timer;
            if(!pressed_f)
                Generate_pos_for_timer_fishka();
            else
                pressed_f = false;
            
           Generate_new_Color();
        }
        else
        {
            counter -= Time.deltaTime * (0.9f+(scoring/5000f));
            Time_text.text = trns.textLines[2].Replace("#", "\n<color=#e74c3c><b><size=130%>"+ (int)(counter+1) +"</size></b></color>");
        }
        
    }

    public void plus_score()
    {
        Score_text.text = trns.textLines[1]+"\n<color=#667eea><b><size=130%>" + scoring + "</size></b></color>";
        PlayerPrefs.SetInt("current_score", scoring);
    }

    public Vector2Int Base_Search(char typ, Vector2Int posi)
    {
        if(posi.x > -1 && posi.x < numberOfButtons && posi.y > -1 && posi.y < numberOfButtons)
        {
            if(world_cells[posi.x,posi.y] == typ)
                return posi;
            else return new Vector2Int(-1,-1);
        }else return new Vector2Int(-1,-1);
       
    }
    public void check_fishka_neibour(char typ, Vector2Int posi, Vector2Int side)
    {
        Vector2Int right_p = Base_Search(typ, new Vector2Int(posi.x+side.x, posi.y+side.y));

        if(Fishka_shape.Contains(right_p) == false)
        {
            if(right_p != new Vector2Int(-1,-1))
            {
                Fishka_shape.Add(right_p);
                Start_Search(typ, right_p);
            }
        }
        
            
           
    }
    public void Start_Search(char typ, Vector2Int posi)
    {
        check_fishka_neibour(typ, posi, right);
        check_fishka_neibour(typ, posi, left);
        check_fishka_neibour(typ, posi, down);
        check_fishka_neibour(typ, posi, up);
    }

    public void Add_fishka(char typ, Vector2Int posi)
    {
        world_cells[posi.x, posi.y] = typ;
       
        
        if(typ != '\0')
        {
            audioRe[2].pitch = Random.Range(0.8f, 1.2f);
            audioRe[2].Play();
            Fishka_shape = new List<Vector2Int>();
            Fishka_shape.Add(posi);
            Start_Search(typ, posi);
            StartCoroutine(Coloumn_Rows());
        }
        GameOvering();
    }
   
    public IEnumerator Coloumn_Rows()
    {
        yield return new WaitForSeconds(0.000000001f);
        if (Fishka_shape.Count > 3)
        {
            for (int i = 0; i < Fishka_shape.Count; i++)
            {
                delete_circle(Fishka_shape[i]);
                if(i == 0)
                {
                    audioRe[1].Play();
                    StartCoroutine(biu(Fishka_shape[i]));
                }
            }
        }
    }
    
    public IEnumerator biu(Vector2Int mu)
    {
        world_cells_buttons[mu.x,mu.y].interactable = false;
        world_cells[mu.x, mu.y] = '$';
        yield return new WaitForSeconds(120f+(scoring/5f));
        world_cells_buttons[mu.x,mu.y].interactable = true;
        world_cells[mu.x, mu.y] = '\0';

    }

    public void delete_circle(Vector2Int pos)
    {
        
        Add_fishka('\0', pos);
        
        Image rr = world_cells_buttons[pos.x,pos.y].transform.GetChild(0).gameObject.GetComponent<Image>();
        rr.color = new Color(rr.color.r, rr.color.g, rr.color.b, 0);
        Color bna = new Color(rr.color.r, rr.color.g, rr.color.b, 255);
        ME.Make_an_obj(world_cells_buttons[pos.x,pos.y].transform.position, bna);
       
    }

    public void add_circle(Vector2Int pos)
    {
        if(world_cells_buttons[pos.x, pos.y].interactable)
        {
            
            Add_fishka(type_of_fishka, pos);
            Image rr = world_cells_buttons[pos.x,pos.y].transform.GetChild(0).gameObject.GetComponent<Image>();
            rr.color = new Color(current_color.r, current_color.g, current_color.b, 255);
        }
    }

    public void button_on_ground(Image btn_face, Vector2Int posy)
    {
        
        if (world_cells[posy.x, posy.y] == '\0')
        {
            btn_face.color = new Color(current_color.r,current_color.g,current_color.b, 255f);
            Add_fishka(type_of_fishka, posy);
            pressed_f = true;
            counter = 0;
        }
        else if (world_cells[posy.x, posy.y] != '$')
        {
            audioRe[3].pitch = Random.Range(0.8f, 1.2f);
            audioRe[3].Play();
            scoring -= (scoring/10)+1;
            plus_score();

            btn_face.color = new Color(current_color.r,current_color.g,current_color.b, 0);
            Add_fishka('\0' , posy);
            
        } 
    }
}
