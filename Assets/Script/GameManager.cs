using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform[] Pos_obg;
    [SerializeField] private Transform folder;
    [SerializeField] private GameObject[] one_slot;
    [SerializeField] private int max_Count_Type;
    private List<GameObject> selected_elements;

    [SerializeField] private GameObject panel_menu;
    [SerializeField] private Text text_menu;
    [SerializeField] private GameObject skull;
    [SerializeField] private GameObject cup;
    
    public static GameManager gameManager;
    void Awake()=> gameManager = this; 
    void Start()
    {   
        // формируем список всех элементов     
        List<GameObject> spawn_list = Generate_list();
        Spawn_All(spawn_list);
    }
    async private void Spawn_All(List<GameObject> spawn_list)
    {
        foreach (var prefab in spawn_list)
        {
            await Task.Delay(100);
            GameObject obg = Instantiate(prefab); 
            obg.transform.SetParent(folder);  
            obg.transform.position = folder.transform.position;
            obg.transform.localScale = Vector2.one;
        }
    }
    private List<GameObject> Generate_list(){
        List<GameObject> spawn_List = new List<GameObject>();
        for (int i = 0; i < one_slot.Length; i++)
        {
            for (int j = 0; j < max_Count_Type * 3; j++)
            {
                spawn_List.Add(one_slot[i]);
            }
        }
        return Sort_List(spawn_List);
    }
    private List<GameObject> Sort_List(List<GameObject> list){
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;        
    }
    // вызывается при нашатии на обьект
    public void Ckick_Obg(GameObject selected_element){
        Add_to_Slot(selected_element);
        Destroy(selected_element);
    }
    // добавляет в слот из 5
    private void Add_to_Slot(GameObject selected_element){
        foreach(Transform pos in Pos_obg)
        {
            if(pos.childCount == 0){
                GameObject element = Instantiate(selected_element);                
                element.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;  
                element.GetComponent<Collider2D>().enabled = false;          
                element.transform.SetParent(pos,true);
                element.transform.localPosition = Vector2.zero;
                element.transform.localRotation = Quaternion.identity;
                element.transform.localScale = Vector2.one;
                // проверить список на 3 подряд
                Has_Three_Consecutive();
                return;
            }
        }
        Fail_Game();
    }

    async void Has_Three_Consecutive()
    {
        List<GameObject> elements_number = new List<GameObject>();
        foreach(Transform pos in Pos_obg)
        {   
            if(pos.childCount !=0){
                GameObject element = pos.GetChild(0).gameObject;
                elements_number.Add(element);
            }
        }        
        if(elements_number.Count < 3)
            return;                
        if(elements_number[elements_number.Count -1].name == elements_number[elements_number.Count -2].name && 
        elements_number[elements_number.Count -1].name == elements_number[elements_number.Count -3].name  ){
            await Task.Delay(300);
            Destroy(elements_number[elements_number.Count -1]);
            Destroy(elements_number[elements_number.Count -2]);
            Destroy(elements_number[elements_number.Count -3]);
            if(folder.childCount == 0)
                Pass_Game();            
        }
    }

    private void Pass_Game(){        
        panel_menu.SetActive(true);
        text_menu.text = "ВЫ ВЫИГРАЛИ";
        cup.SetActive(true);
        skull.SetActive(false);
    }
    private void Fail_Game(){
        panel_menu.SetActive(true);
        text_menu.text = "ВЫ ПРОИГРАЛИ";
        cup.SetActive(false);
        skull.SetActive(true);
    }

    public void Restart_game(){
        if (folder.childCount > 0){
            foreach(Transform slot in folder.transform){
                if(slot != null)            
                    Destroy(slot.gameObject);
            }
        } 
        foreach(Transform slot in Pos_obg){
            if(slot.childCount >0){
                if(slot.GetChild(0) != null)
                    Destroy(slot.GetChild(0).gameObject);
            }            
        }
        Start();
        panel_menu.SetActive(false);
    }
}
