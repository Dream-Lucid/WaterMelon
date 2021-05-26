using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Ready =0,
    StanndBy=1,
    InProgress=2,
    GameOver=3,
    CalculateScore=4,
}

public class GameManager : MonoBehaviour
{
    public GameObject[] fruitList;
    public GameObject canvas;
    public GameObject startBtn;
    public static GameManager GameManagerInstance;
    public GameState gameState = GameState.Ready;
    public Vector3 combineScale =new Vector3(0,0,0);
    public float TotalScore = 0f;
    public Text totalScore;
    public Button esc;
    public Button rank;
    public Button fff;
    public Text Name;
    public InputField youName;
    public AudioSource combineSource;
    public AudioSource hitSource;
    private void Awake()
    {
        GameManagerInstance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StarGame()
    {
        CreateFruit();
        gameState = GameState.StanndBy;
        startBtn.SetActive(false);
        esc.gameObject.SetActive(false);
        rank.gameObject.SetActive(false);
    }
    public  void InvokeCreateFruit(float invokeTime)
    {
        Invoke("CreateFruit", invokeTime);
    }
    public void CreateFruit()
    {
        int index = Random.Range(0, 5);
        if (fruitList.Length >= index && fruitList[index] != null)
        {

            GameObject fruitObj = fruitList[index];
            var obj = Instantiate(fruitObj, canvas.transform);
            Vector3 pos = new Vector3(0, 330, 0);
            obj.transform.localPosition = pos;
            obj.GetComponent<Fruit>().fruitState = FruitState.StandBy;
        }
    }

    public void CombineNewFruit(FruitType currentFruitType, Vector3 currentPos, Vector3 collisionPos)
    {
        
        Vector3 centerPos = (currentPos + collisionPos) / 2;
        int index = (int)currentFruitType + 1;
        GameObject combineFruitObj = fruitList[index];
        var combineFruit = Instantiate(combineFruitObj,canvas.transform);
        combineFruit.transform.localPosition = centerPos;
        combineFruit.GetComponent<Rigidbody2D>().gravityScale = 1f;
        combineFruit.GetComponent<Fruit>().fruitState = FruitState.Collision;
        combineFruit.transform.localScale = combineScale;
        //combineSource.Play();
    }

    public void Esc()
    {
        Application.Quit();
    }


    public void Rank()
    {
        startBtn.gameObject.SetActive(false);
        esc.gameObject.SetActive(false);
        //musicOn.gameObject.SetActive(false);
        //musicOff.gameObject.SetActive(false);
        rank.gameObject.SetActive(false);
    }

    public void YouName()
    {
        StartCoroutine(Fenshu1.CreateNewHighScore(Name.text, TotalScore.ToString()));
        youName.gameObject.SetActive(false);
        fff.gameObject.SetActive(true);
    }
}
