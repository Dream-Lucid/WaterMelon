using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FruitType
{
    One =0,
    Two=1,
    Three=2,
    Four=3,
    Five=4,
    Six=5,
    Seven=6,
    Eight=7,
    Nine=8,
    ten=9,
    Eleven=10,
}
public enum FruitState
{
    Ready=0,
    StandBy=1,
    Dropping=2,
    Collision=3,
}

public class Fruit : MonoBehaviour
{

    private bool IsMove = false;
    public FruitType fruitType = FruitType.One;
    public FruitState fruitState = FruitState.Ready;
    public float limit_x = 2f;
    public Vector3 originalScale = Vector3.zero;
    public float scaleSpeed = 0.1f;
    public float fuirtScore = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        originalScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.GameManagerInstance.gameState == GameState.StanndBy && fruitState == FruitState.StandBy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                IsMove = true;
            }
            if (Input.GetMouseButtonUp(0) && IsMove)
            {
                IsMove = false;
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                fruitState = FruitState.Dropping;
                GameManager.GameManagerInstance.gameState = GameState.InProgress;
                GameManager.GameManagerInstance.InvokeCreateFruit(0.5f);
            }
            if (IsMove)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.gameObject.GetComponent<Transform>().position = new Vector3(mousePos.x, this.gameObject.GetComponent<Transform>().position.y, this.gameObject.GetComponent<Transform>().position.z);
            }
        }

        if (this.transform.position.x > limit_x)
        {
            this.transform.position = new Vector3(limit_x, this.transform.position.y, this.transform.position.z);
        }
        if (this.transform.position.x < -limit_x)
        {
            this.transform.position = new Vector3(-limit_x, this.transform.position.y, this.transform.position.z);
        }


        if (this.transform.localScale.x < originalScale.x) 
        {
           

            this.transform.localScale += new Vector3(1, 1, 1) * scaleSpeed;
        }
        if (this.transform.localScale.x > originalScale.x)
        {
            this.transform.localScale = originalScale;
        }

    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(fruitState == FruitState.Dropping)
        {
            if (collision.gameObject.tag.Contains("Floor"))
            {
                GameManager.GameManagerInstance.gameState = GameState.StanndBy;
                fruitState = FruitState.Collision;

                GameManager.GameManagerInstance.hitSource.Play();
            }
            if (collision.gameObject.tag.Contains("Fruit"))
            {
                GameManager.GameManagerInstance.gameState = GameState.StanndBy;
                fruitState = FruitState.Collision;
            }
        }
        
        if ((int)fruitState >= (int)FruitState.Dropping)
        {
            if (collision.gameObject.tag.Contains("Fruit"))
            {
                if (fruitType == collision.gameObject.GetComponent<Fruit>().fruitType)
                {
                    GameManager.GameManagerInstance.combineSource.Play();
                    float thisPosxy = this.transform.localPosition.x + this.transform.localPosition.y;
                    float collisionPosxy = collision.transform.localPosition.x + collision.transform.localPosition.y;
                    if(thisPosxy > collisionPosxy)
                    {
                        GameManager.GameManagerInstance.CombineNewFruit(fruitType, this.transform.localPosition, collision.transform.localPosition);
                        GameManager.GameManagerInstance.TotalScore += fuirtScore;
                        GameManager.GameManagerInstance.totalScore.text = "得分:" + GameManager.GameManagerInstance.TotalScore;
                        Destroy(this.gameObject);
                        Destroy(collision.gameObject);
                    }
                }
            }
        }
    }
}

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TopLine : MonoBehaviour
{

    public bool IsMove = false;
    public float speed = 0.1f;
    public float limit_y = -5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMove)
        {
            if (this.transform.position.y > limit_y)
            {
                this.transform.Translate(Vector3.down * speed);
            }
            else
            {
                IsMove = false;
                Invoke("YouName1", 1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag.Contains("Fruit"))
        {
            if ((int)GameManager.GameManagerInstance.gameState < (int)GameState.GameOver)
            {
                if (collider.gameObject.GetComponent<Fruit>().fruitState == FruitState.Collision)
                {
                    GameManager.GameManagerInstance.gameState = GameState.GameOver;
                    Invoke("ChangeMoveState", 0.5f);
                }

            }

            if (GameManager.GameManagerInstance.gameState == GameState.CalculateScore)
            {
                float currentScore = collider.GetComponent<Fruit>().fuirtScore;
                GameManager.GameManagerInstance.TotalScore += currentScore;
                GameManager.GameManagerInstance.totalScore.text = "�ܷ�:" + GameManager.GameManagerInstance.TotalScore;
                Destroy(collider.gameObject);

            }
        }
    }
    void ChangeMoveState()
    {
        IsMove = true;
        GameManager.GameManagerInstance.gameState = GameState.CalculateScore;
    }


    void YouName1()
    {
        GameManager.GameManagerInstance.youName.gameObject.SetActive(true);
    }

  
}
