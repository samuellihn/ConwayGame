using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TileManager : MonoBehaviour
{
    public Camera mainCamera;

    public GameObject rootTile;
    public Button stepButton;
    public Button clearButton;
    public GameObject playButton;

    Vector2Int[] mooreNeighborhood = {
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
    };

    public int borderSize;

    private List<GameObject> tileList = new List<GameObject>();
    private List<GameObject> aliveList = new List<GameObject>();
    private List<GameObject> searchList = new List<GameObject>();

    public int tilemapHeight;
    public int tilemapWidth;
    public Transform parent;

    public float simulationSpeed;
    private bool isPlaying = false;


    // Start is called before the first frame update
    void Start()
    {
        simulationSpeed = 1.0f;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

         GenerateNewTiles(bottomLeft, topRight);

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    onTileClick(hit.collider.gameObject);
                }
            }
        }

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        GenerateNewTiles(bottomLeft, topRight);
    }

    private void onTileClick(GameObject tile)
    {
        updateTrackedTiles(tile, tile.GetComponent<TileScript>().ChangeState());
    }

    public void GenerateNewTiles(Vector3 newCornerBL, Vector3 newCornerTR)
    {
        Vector2Int newBL = new Vector2Int(Mathf.RoundToInt(newCornerBL.x)-borderSize, Mathf.RoundToInt(newCornerBL.y)-borderSize);
        Vector2Int newTR = new Vector2Int(Mathf.RoundToInt(newCornerTR.x)+borderSize, Mathf.RoundToInt(newCornerTR.y)+borderSize);
        
        for (int x = newBL.x; x <= newTR.x; x++)
        {
            for (int y = newBL.y; y <= newTR.y; y++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, y), Vector2.zero);
                if (hit.collider == null)
                {
                    Vector3 tilePos = new Vector3(x, y, 0);
                    tileList.Add(Instantiate(rootTile, tilePos, Quaternion.identity, parent));
                }
            }
        }
    }

    private void updateTrackedTiles(GameObject tile,bool isAddCommand)
    {
        if (isAddCommand)
        {
            if (!aliveList.Contains(tile))
            {
                aliveList.Add(tile);
            }
        }
        else
        {
            aliveList.Remove(tile);
        }
    }

    private void updateSearchList()
    {
        searchList.Clear();
        foreach (GameObject tile in aliveList)
        {
            if (!searchList.Contains(tile))
            {
                searchList.Add(tile);
            }
            for (int i = 0; i < 8; i++)
            {
                Vector3 offset = new Vector3(mooreNeighborhood[i].x, mooreNeighborhood[i].y, 0);
                RaycastHit2D hit = Physics2D.Raycast(tile.transform.position + offset, Vector2.zero);
                if (hit.collider != null)
                {
                    if (!searchList.Contains(hit.collider.gameObject))
                    {
                        searchList.Add(hit.collider.gameObject);
                    }
                }

            }
        }
    }


    public void UpdateSimulation()
    {

        List<GameObject> deathList = new List<GameObject>();
        List<GameObject> lifeList = new List<GameObject>();
        int neighborsAlive;
        updateSearchList();
        foreach (GameObject currentTile in searchList)
        {
            neighborsAlive = 0;

            for (int i = 0; i < 8; i++)
            {

                Vector3 currentTilePos = currentTile.transform.position;
                Vector2 currentTilePos2D = new Vector2(currentTilePos.x, currentTilePos.y);
                RaycastHit2D hit = Physics2D.Raycast(currentTilePos2D + mooreNeighborhood[i], Vector2.zero);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<TileScript>().isAlive)
                    {
                        neighborsAlive++;
                    }
                }
            }

            if (neighborsAlive > 3 || neighborsAlive < 2)
            {
                deathList.Add(currentTile);
            }
            else if (neighborsAlive == 3)
            {
                lifeList.Add(currentTile);
            }
        }
        
        //Update the states with the lists of what to update
        foreach (GameObject cell in deathList)
        {
            cell.GetComponent<TileScript>().toDead();
            updateTrackedTiles(cell, false);
        }

        foreach (GameObject cell in lifeList)
        {
            cell.GetComponent<TileScript>().toAlive();
            updateTrackedTiles(cell, true);
        }

        Debug.Log(searchList.Count);
        Debug.Log(aliveList.Count);
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

    public void ResetBoard()
    {
        foreach (GameObject cell in aliveList)
        {
            cell.GetComponent<TileScript>().toDead();
        }
        aliveList.Clear();
        searchList.Clear();
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


}

