using UnityEngine;
using System.Collections.Generic;

public class GameOfLifeBehaviour : MonoBehaviour
{
    [SerializeField]
    [Range(20, 70)]
    private int MapWidth = 30;

    [SerializeField]
    [Range(20, 70)]
    private int MapHeight = 30;

    [SerializeField]
    private GameObject TilePrefab;

    [SerializeField]
    private float GenerationDelay = 1.2f;

    [SerializeField]
    private int solitude;

    [SerializeField]
    private int overPopulation;

    [SerializeField]
    private int revive;

    private Tile[,] map;
    private float timer = 0;
    private bool isRunning;
    private List<Tile> selected = new List<Tile>();

    void Start()
    {
        this.map = new Tile[MapWidth, MapHeight];
        initMap();
    }

    void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;

            if (timer >= GenerationDelay)
            {
                timer = 0;
                processGenerations();
            }
        }
        else
        {
            selectTiles();
        }
    }

    private void initMap()
    {
        setupMap();
        setNeighbors();
        buildMap();
    }

    private void setupMap()
    {
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                this.map[x, y] = new Tile(x, y, CellState.Dead);
            }
        }
    }

    private void setNeighbors()
    {
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                Tile temp = this.map[x, y];

                for (int yOffset = -1; yOffset < 2; yOffset++)
                {
                    for (int xOffset = -1; xOffset < 2; xOffset++)
                    {
                        if (!(xOffset == 0 && yOffset == 0))
                        {
                            int xPos = x + xOffset;
                            int yPos = y + yOffset;

                            if (xPos < 0)
                            {
                                xPos += MapWidth;
                            }
                            else
                            {
                                xPos %= MapWidth;
                            }

                            if (yPos < 0)
                            {
                                yPos += MapHeight;
                            }
                            else
                            {
                                yPos %= MapHeight;
                            }

                            if (map[xPos, yPos] != null)
                            {
                                temp.Neighbors.Add(map[xPos, yPos]);
                            }
                        }
                    }
                }
            }
        }
    }

    private void buildMap()
    {
        foreach (var cell in map)
        {
            if (cell != null)
            {
                Vector3 instantiatePos = new Vector3(cell.Position.x, 0, cell.Position.y);
                cell.TileObject = (GameObject)Instantiate(TilePrefab, instantiatePos, Quaternion.identity);
                cell.TileObject.GetComponent<Transform>().SetParent(this.gameObject.GetComponent<Transform>());
            }
        }
    }

    private void processGenerations()
    {
        foreach (var cell in map)
        {
            if (cell != null)
            {
                int aliveCells = cell.GetAliveCells();

                if (cell.CurrentState == CellState.Alive)
                {
                    if (aliveCells < solitude)
                    {
                        cell.NextState = CellState.Dead;
                    }
                    if (aliveCells > overPopulation)
                    {
                        cell.NextState = CellState.Dead;
                    }
                }
                else
                {
                    if (aliveCells == revive)
                    {
                        cell.NextState = CellState.Alive;
                    }
                }
            }
        }

        foreach (var cell in map)
        {
            if (cell != null)
            {
                cell.CurrentState = cell.NextState;

                switch (cell.CurrentState)
                {
                    case CellState.Alive:
                        cell.TileObject.GetComponent<Renderer>().material.color = Color.black;
                        break;
                    case CellState.Dead:
                        cell.TileObject.GetComponent<Renderer>().material.color = Color.white;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void selectTiles()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit))
            {
                if (rayHit.collider.gameObject.tag == TilePrefab.tag)
                {
                    Vector3 pos = rayHit.collider.gameObject.GetComponent<Transform>().position;
                    Tile temp = map[(int)pos.x, (int)pos.z];

                    if (!selected.Contains(temp))
                    {
                        selected.Add(temp);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (selected.Count > 0)
            {
                foreach (var cell in selected)
                {
                    switch (cell.CurrentState)
                    {
                        case CellState.Alive:
                            cell.CurrentState = CellState.Dead;
                            cell.NextState = CellState.Dead;
                            cell.TileObject.GetComponent<Renderer>().material.color = Color.white;
                            break;

                        case CellState.Dead:
                            cell.CurrentState = CellState.Alive;
                            cell.NextState = CellState.Alive;
                            cell.TileObject.GetComponent<Renderer>().material.color = Color.black;
                            break;
                        default:
                            break;
                    }
                }
                selected.Clear();
            }
        }
    }

    public void ToggleRunning()
    {
        isRunning = !isRunning;
    }
}
