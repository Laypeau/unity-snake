using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    public GameObject segmentPrefab;
    public GameObject foodPrefab;
    public Camera gameCamera;

    private Vector3 moveDirection = Vector3.right;
    private Vector3 faceDirection = Vector3.right;
    private Vector3 foodPosition;
    private GameObject food;

    private bool gameRunning = true;

    private IList<Vector3> segmentPositions = new List<Vector3>();

    void Start()
    {
        segmentPositions.Add(Vector3.left);
        segmentPositions.Add(Vector3.zero);
        segmentPositions.Add(Vector3.right);

        SpawnFood();

        StartCoroutine(MoveSnake());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            faceDirection = Vector3.forward;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            faceDirection = Vector3.back;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            faceDirection = Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            faceDirection = Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            faceDirection = Vector3.up;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            faceDirection = Vector3.down;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameCamera.orthographic)
            {
                gameCamera.orthographic = false;
            }
            else
            {
                gameCamera.orthographic = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void SpawnFood()
    {
        foodPosition = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));

        while (segmentPositions.Contains(foodPosition))
        {
            foodPosition = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));
        }

        food = Instantiate(foodPrefab);
        food.transform.position = foodPosition;
    }

    public void Die(Vector3 _explosionOrigin)
    {
        gameRunning = false;
        Debug.Log("Game End");
        GameObject[] tokill = GameObject.FindGameObjectsWithTag("segment");
        for (int i = 0; i < tokill.Length; i++)
        {
            //Destroy(tokill[i]);
            tokill[i].AddComponent<Rigidbody>();
            tokill[i].GetComponent<Rigidbody>().AddExplosionForce(3f, Vector3.zero, 0, 1, ForceMode.Impulse);
        }
    }

    public IEnumerator MoveSnake()
    {
        while(true)
        {
            if (gameRunning)
            {
                yield return new WaitForSeconds(0.5f);

                //eat food
                if (segmentPositions[segmentPositions.Count - 1] == foodPosition)
                {
                    Destroy(food);
                    SpawnFood();
                }
                else
                {
                    segmentPositions.RemoveAt(0);
                }

                //movement
                if (-1 * faceDirection != moveDirection)
                {
                    moveDirection = faceDirection;
                }

                Vector3 _nextPos = segmentPositions[segmentPositions.Count - 1] + moveDirection;
                if (segmentPositions.Contains(_nextPos))
                {
                    Die(_nextPos);
                    break;
                }
                else
                {
                    if (_nextPos.x < 0 || _nextPos.x > 9 || _nextPos.y < 0 || _nextPos.y > 9 || _nextPos.z < 0 || _nextPos.z > 9)
                    {
                        Die(_nextPos);
                        break;
                    }
                    else
                    {
                        segmentPositions.Add(segmentPositions[segmentPositions.Count - 1] + moveDirection);
                    }
                }

                //keeping track of every existing segment in a list is too much work
                GameObject[] todestroy = GameObject.FindGameObjectsWithTag("segment");
                for (int i = 0; i < todestroy.Length; i++)
                {
                    Destroy(todestroy[i]);
                }

                for (int i = 0; i < segmentPositions.Count; i++)
                {
                    Transform _temp = Instantiate(segmentPrefab).transform;
                    _temp.tag = "segment";
                    _temp.position = segmentPositions[i];
                }

            }
        }
    }
}
