using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameMenu;
using static GameManager;
using static WayPoint;

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    public Task[] tasks;
    public int candyLimit;
    [HideInInspector]
    public int candyCount;
    public bool withoutCaddy;

    public static Dictionary<Task.collectType, int> typeToSprite = new Dictionary<Task.collectType, int>
    {
        { Task.collectType.cucumber, 0 },
        { Task.collectType.carrot, 1 },
        { Task.collectType.melon, 2 },
        { Task.collectType.baguette, 3 },
        { Task.collectType.toothbrush, 4 },
        { Task.collectType.water, 5 },
        { Task.collectType.toiletPaper, 6 },
        { Task.collectType.broom, 7 },
        { Task.collectType.tomato, 8 },
    };
    private Dictionary<string, Task.collectType> spriteNameToType = new Dictionary<string, Task.collectType>
    {
        {"Gurke", Task.collectType.cucumber},
        {"Karotte_bund", Task.collectType.carrot},
        {"Karotte_einzeln", Task.collectType.carrot},
        {"Melone_0", Task.collectType.melon},
        {"Baguette", Task.collectType.baguette},
        {"Zahnbürste", Task.collectType.toothbrush},
        {"Wasserpack", Task.collectType.water},
        {"Klopapier", Task.collectType.toiletPaper},
        {"KlopapierPack", Task.collectType.toiletPaper},
        {"Besen", Task.collectType.broom},
        {"Tomate_0", Task.collectType.tomato},
        {"Tomaten_0", Task.collectType.tomato},
        {"", Task.collectType.none }
    };

    private GameObject shoppingList;
    private Text limitTxt;

    [Header("Folgende Sachen nur mit Gewissheit ändern!!!")]
    public Sprite[] images;

    // Start is called before the first frame update
    void Awake()
    {
        gameController = this;

        shoppingList = gameMenu.transform.Find("HUD").Find("ShoppingList").gameObject;
        limitTxt = shoppingList.transform.Find("Limit").GetComponent<Text>();

        Transform todo = shoppingList.transform.GetChild(0);

        List<Task> oldTasks = new List<Task>(tasks);
        for(int i = 0; i < tasks.Length; i++)
        {
            tasks[i].taskNumber = i;

            tasks[i].taskField = Instantiate(manager.taskField, todo);

            tasks[i].image = tasks[i].taskField.transform.GetChild(0).GetComponent<Image>();
            tasks[i].RemainTxt = tasks[i].taskField.transform.GetChild(1).GetComponent<Text>();
            tasks[i].mark = tasks[i].taskField.transform.GetChild(2).GetChild(0).gameObject;
            tasks[i].image.sprite = images[typeToSprite[tasks[i].type]];

            tasks[i].RemainTxt.text = " x" + tasks[i].collectNumber;
            tasks[i].mark.SetActive(false);
        }
        Canvas.ForceUpdateCanvases();
        todo.GetComponent<VerticalLayoutGroup>().enabled = false;
        todo.GetComponent<VerticalLayoutGroup>().enabled = true;

        limitTxt.text = candyLimit.ToString();
        gameMenu.SetNewCaddy();
    }

    private void Start()
    {
        Waypoints = new List<Vector2>();
        Waypoints.Add(transform.position);
    }

    public int AddToCaddy(string spriteName, int number)
    {
        int taskNumber = CheckIfNeeded(spriteName);
        if (taskNumber == -1) return -1;
        int remainer = tasks[taskNumber].collectNumber - tasks[taskNumber].collectCount - number;
        if (remainer < 0)
        {
            tasks[taskNumber].collectCount = tasks[taskNumber].collectNumber;
            tasks[taskNumber].UpdateTask();
            return -remainer;
        }
        else
        {
            tasks[taskNumber].collectCount += number;
            tasks[taskNumber].UpdateTask();
            return 0;
        }
    }

    public int CheckIfNeeded(string _name)
    {
        int task_needed = -1;
        Task.collectType type;
        if (!spriteNameToType.TryGetValue(_name, out type)) return -1;
        //if (type == Task.collectType.none) return -1;
        foreach (Task task in tasks) {task_needed++; if (task.type == type) break; }

        if (tasks[task_needed].type == type) { return tasks[task_needed].collectCount == tasks[task_needed].collectNumber ? -1 : task_needed; }
        else return -1;
    }

    public bool CheckIfFinished()
    {
        bool complete = true;
        foreach (Task task in tasks) complete &= task.taskComplete;
        return complete;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Center", false);
    }
}

[System.Serializable]
public class Task
{
    public enum collectType { tomato, cucumber, carrot, melon, baguette, water, toothbrush, broom, toiletPaper, candy, none}
    public collectType type;
    public int collectNumber;
    public int collectCount;

    [HideInInspector]
    public int taskNumber;
    public bool taskComplete;

    [HideInInspector]
    public GameObject taskField;
    [HideInInspector]
    public Text RemainTxt;
    [HideInInspector]
    public Image image;
    [HideInInspector]
    public GameObject mark;

    public void UpdateTask()
    {
        if(collectNumber > collectCount)
        {
            RemainTxt.text = " x" + (collectNumber - collectCount).ToString();
            taskComplete = false;
        }
        else
        {
            RemainTxt.text = " x0";
            taskComplete = true;
        }
        mark.SetActive(taskComplete);
    }
}
