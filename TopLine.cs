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
                GameManager.GameManagerInstance.totalScore.text = "总分:" + GameManager.GameManagerInstance.TotalScore;
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
