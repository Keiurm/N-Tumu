using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject[] ballPrefabs;
    private bool isDragged = false;

    private CommonBall firstBall = null;

    private List<GameObject> removableBallList = new List<GameObject>();

    private bool isPlaying = false;

    public TextMeshProUGUI timerText;

    public int TIME_LIMIT = 60;

    public int TIME_COUNT = 5;

    private int currentScore;

    public TextMeshProUGUI scoreText;
    void Start()
    {
        StartCoroutine(CountDown());
    }
    private IEnumerator CountDown()
    {
        float count = TIME_COUNT;
        while (count > 0)
        {
            timerText.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
        }
        timerText.text = "Start!";
        yield return new WaitForSeconds(1);

        isPlaying = true;
        StartCoroutine(DropBall());
        StartCoroutine(GameTimer());
    }

    private IEnumerator DropBall()
    {
        while (isPlaying)
        {
            int RANDOM_INDEX = Random.Range(0, ballPrefabs.Length);
            float RANDOM_X = Random.Range(-2.0f, 2.0f);
            Vector3 BALL_INITIAL_POSITION = new Vector3(RANDOM_X, 7.0f, 0.0f);
            GameObject cloneball = Instantiate(ballPrefabs[RANDOM_INDEX]);

            cloneball.transform.position = BALL_INITIAL_POSITION;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator GameTimer()
    {
        float count = TIME_LIMIT;
        while (count > 0)
        {
            timerText.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
        }
        timerText.text = "Finish!";
        isPlaying = false;

        foreach (GameObject ball in removableBallList)
        {
            ball.GetComponent<CommonBall>().ResetColor();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (isPlaying)
        {
            if (Input.GetMouseButtonDown(0) && isDragged == false)
            {
                isDragged = true;
                OnDragStart();
            }
            else if (Input.GetMouseButton(0) && isDragged == true)
            {
                OnDragging();
            }
            else
            {
                isDragged = false;
                OnDragEnd();
            }
        }
    }

    private GameObject GetCurrentTarget()
    {
        GameObject atDeleteTarget = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);

        if (hit2d)
        {
            atDeleteTarget = hit2d.transform.gameObject;
        }
        return atDeleteTarget;
    }

    private void ChangeColor(GameObject obj)
    {
        Material ballMaterial = obj.GetComponent<Renderer>().material;
        ballMaterial.SetFloat("_Metallic", 1.0f);
    }

    private void OnDragStart()
    {
        GameObject targetObject = GetCurrentTarget();
        removableBallList.Clear();
        if (targetObject)
        {
            if (targetObject.name.IndexOf("Ball") != -1)
            {
                firstBall = targetObject.GetComponent<CommonBall>();
                removableBallList.Add(targetObject);
                firstBall.isAdd = true;
                ChangeColor(targetObject);
            }
        }
    }

    private void OnDragging()
    {

        GameObject targetObject = GetCurrentTarget();

        if (targetObject != null && firstBall != null)
        {
            if (targetObject.name.IndexOf("Ball") != -1)
            {
                CommonBall targetBall = targetObject.transform.GetComponent<CommonBall>();
                if (targetBall.kindOfId == firstBall.kindOfId)
                {
                    if (targetBall.isAdd == false)
                    {
                        removableBallList.Add(targetObject);
                        targetBall.isAdd = true;
                        ChangeColor(targetObject);
                    }
                }
            }
        }
    }

    private void OnDragEnd()
    {
        firstBall = null;
        int length = removableBallList.Count;
        if (length >= 3)
        {
            foreach (GameObject ball in removableBallList)
            {
                currentScore += length;
                scoreText.text = $"Score: {currentScore}";
                Destroy(ball);
            }
            removableBallList.Clear();
        }
        else
        {
            foreach (GameObject ball in removableBallList)
            {
                ball.GetComponent<CommonBall>().isAdd = false;
                ball.GetComponent<CommonBall>().ResetColor();
            }
        }
    }
}
