using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TileManager : MonoBehaviour
{

    public GameObject rootTile;
    public Button stepButton;
    public Button clearButton;
    public GameObject playButton;

    public int tilemapHeight;
    public int tilemapWidth;
    public float simulationSpeed;
    public Transform parent;
    private bool isPlaying = false;
    private List<List<GameObject>> tileList = new List<List<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < tilemapWidth; x++)
        {

            tileList.Add(new List<GameObject>());
            for (int y = 0; y < tilemapHeight; y++)
            {
                tileList[x].Add(Instantiate(rootTile, new Vector3(x - Mathf.FloorToInt(tilemapHeight / 2.0f), y - Mathf.FloorToInt(tilemapWidth / 2.0f), 0), Quaternion.identity, parent));
            }
        }
        simulationSpeed = 1.0f;


    }

    // Update is called once per frame
    void Update()
    {   
        
        if (Input.GetMouseButtonDown(0)) {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                hit.collider.gameObject.GetComponent<TileScript>().ChangeState();
            }
        }

    }

    public void ResetBoard()
    {
        foreach (List<GameObject> col in tileList)
        {
            foreach (GameObject cell in col)
            {
                cell.GetComponent<TileScript>().toDead();
            }
        }
    }


    public void StartSimulation()
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            InvokeRepeating("UpdateSimulation", 0.0f, simulationSpeed);
            playButton.GetComponent<Image>().color = Color.green;
            playButton.transform.GetChild(0).GetComponent<Text>().text = "Stop";
            stepButton.enabled = false;
            clearButton.enabled = false;
        }
        else
        {
            CancelInvoke("UpdateSimulation");
            playButton.GetComponent<Image>().color = Color.red;
            playButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
            stepButton.enabled = true;
            clearButton.enabled = true;
        }
    }

    public void UpdateSpeed(string speed)
    {
        if (isPlaying)
        {
            CancelInvoke("UpdateSimulation");
            simulationSpeed = float.Parse(speed);
            InvokeRepeating("UpdateSimulation", 0.0f, simulationSpeed);
        }
        else
        {
            simulationSpeed = float.Parse(speed);
        }


    }

    public void UpdateSimulation()
    {

        List<Vector2Int> deathList = new List<Vector2Int>();
        List<Vector2Int> lifeList = new List<Vector2Int>();
        int neighborsAlive;
        for (int x = 1; x < tilemapWidth - 1; x++)
        {
            for (int y = 1; y < tilemapHeight - 1; y++)
            {
                neighborsAlive = 0;

                int[] xOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
                int[] yOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };
                for (int i = 0; i < 8; i++)
                {
                    if (tileList[x + xOffsets[i]][y + yOffsets[i]].GetComponent<TileScript>().isAlive)
                    {
                        neighborsAlive++;

                    }
                }

                if (neighborsAlive > 3 || neighborsAlive < 2)
                {
                    deathList.Add(new Vector2Int(x, y));
                }
                else if (neighborsAlive == 3)
                {
                    lifeList.Add(new Vector2Int(x, y));
                }

            }
        }

        foreach (Vector2Int cell in deathList)
        {
            tileList[cell.x][cell.y].GetComponent<TileScript>().toDead();
        }

        foreach (Vector2Int cell in lifeList)
        {
            tileList[cell.x][cell.y].GetComponent<TileScript>().toAlive();
        }
    }
}

