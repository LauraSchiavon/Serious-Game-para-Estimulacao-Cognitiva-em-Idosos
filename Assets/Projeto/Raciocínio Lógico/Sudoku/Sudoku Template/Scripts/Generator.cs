using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

/* Used to generate a game of sudoku with custom settings. */
public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject[] tilesPosition;

    public GameObject telaVitoria;
    public GameObject telaDerrota;

    //Self-reference
    public static Generator generator;

    [Tooltip("The size of the sudoku grid. Increasing the grid size may result into very slow performance.")]
    public Vector2 gridSize = new(9, 9);

    [Tooltip("The scale of the grid on the X and Y axis.")]
    public Vector2 neighborhoodSize = new(3, 3);

    [Tooltip("Specify the size of your custom tiles.")]
    public Vector2 tileSize = new(30, 30);

    [Tooltip("All tiles will be automatically generated under the parent gameObject.")]
    public GameObject tileParent;

    [Tooltip("Maximum allowed number of iterations while generating the matrix.")]
    public int failSafe = 10000;

    [Tooltip("The margin of individual tiles.")]
    public float tileMargin = 5;

    [Tooltip("The maximum number of allowed mistakes before the game ends.")]
    public int maxMistakes = 5;

    private int mistakes;

    [Tooltip("The maximum number of allowed hints before the game ends.")]
    public int maxHints = 3;

    //Current number of used hints
    private int hints;

    //Time passed since timer has initiated
    private float timePassed;

    //Game difficulty
    public DifficultyConfig difficultyConfig;

    //Button configuration
    public ButtonConfig buttonConfig;

    //Tile configuration
    public TileConfig tileConfig;

    //Object used to display mistakes made
    public TMP_Text mistakesObject;

    //Object used to display remaining hints
    public TMP_Text hintsObject;

    //Selected value, usually 1 -> 9
    private int selectedValue;

    private int[,] currentMatrix;
    private int[,] answerMatrix;

    private GameObject[,] currentGameObjectMatrix;
    private Vector2 defaultPosition;

    //Modes
    private bool eraseOn, notesOn = false;

    [Tooltip("Enabling this mode will allow players to select tiles before answering")]
    public bool selectMode = false;

    //Timer coroutine
    private Coroutine timerRoutine;

    //Is the game paused?
    private bool paused;

    //Selected tile data
    private GameObject selectedTile = null;
    private Vector2 selectedTileCoordinates;

    //Initiating the game
    private void Start()
    {
        int sizeX = (int)gridSize.x;
        int sizeY = (int)gridSize.y;

        //Initiating matrices
        currentGameObjectMatrix = new GameObject[sizeX, sizeY];
        currentMatrix = new int[sizeX, sizeY];
        answerMatrix = new int[sizeX, sizeY];

        changeDifficulty(PlayerPrefs.HasKey("difficulty") ? PlayerPrefs.GetInt("difficulty") : 0);

        if (!selectMode) setSelectedValue(1);
        updateHints(0);
        updateMistakes(0);
        gen();
        populateGrid();
    }

    public void newGame()
    {
        if (!selectMode) setSelectedValue(1);
        updateHints(0);
        updateMistakes(0);
        gen();
        populateGrid();
    }

    private void checkProgress()
    {
        if (mistakes >= maxMistakes)
        {
            telaDerrota.SetActive(true);
        }
        else if (checkAnswer())
        {
            telaVitoria.SetActive(true);
        }
    }

    //Updating mistake data
    private void updateMistakes(int i)
    {
        mistakes = i == 0 ? 0 : mistakes + i;
        mistakesObject.text = "ERROS: " + mistakes.ToString() + "/" + maxMistakes.ToString();
    }

    //Updating hits
    private void updateHints(int i)
    {
        hints = i == 0 ? 0 : hints + i;
        hintsObject.text = "DICAS: " + hints.ToString() + "/" + maxHints.ToString();
    }

    //Displaying hint
    public void showHint()
    {
        if (hints < maxHints)
        {
            if (notesOn) toggleNotes();

            //Displaying random tiles
            int x = Random.Range(0, (int)gridSize.x);
            int y = Random.Range(0, (int)gridSize.y);

            bool found = true;

            int counter = 0;
            while (answerMatrix[x, y] != -1)
            {
                x = Random.Range(0, (int)gridSize.x);
                y = Random.Range(0, (int)gridSize.y);
                counter++;

                if (counter >= gridSize.x * gridSize.y)
                {
                    found = false;
                    break;
                }

                ;
            }

            if (found)
            {
                if (eraseOn) toggleErase();
                if (selectMode)
                {
                    selectedTile = currentGameObjectMatrix[x, y];
                    selectedTileCoordinates = new Vector2(x, y);
                }

                setElement(currentGameObjectMatrix[x, y], currentMatrix[x, y], x, y);
                updateHints(1);
            }
        }
    }

    public void changeDifficulty(int i)
    {
        var difficulty = i < difficultyConfig.difficulties.Count ? i : 0;
        difficultyConfig.currentDifficulty = difficulty;

        PlayerPrefs.SetInt("difficulty", difficulty);
        difficultyConfig.difficultyButtonText.text = difficultyConfig.difficulties[difficulty].name;
        difficultyConfig.difficultyMenu.SetActive(false);
    }

    //Returns true only if the puzzle has been solved successfully
    private bool checkAnswer()
    {
        if (arraysEqual(answerMatrix, currentMatrix))
        {
            return true;
        }

        return false;
    }

    private bool arraysEqual(int[,] a, int[,] b)
    {
        if (a.GetLength(0) == b.GetLength(0) && b.GetLength(1) == a.GetLength(1))
        {
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (a[i, j] != b[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        return false;
    }

    //Setting answer
    private void setCurrentMatrixElement(GameObject g, int v, int x, int y, bool setEmpty)
    {
        //If set empty is false (meaning that we are simply highlighting a tile), do not check any answers
        if (!setEmpty)
        {
            //Storing the answer the user gave into the answers matrix
            answerMatrix[x, y] = v;

            //Highlighting the tile based on the answer given
            if (v == currentMatrix[x, y]) g.GetComponent<Image>().color = tileConfig.correctTileColor;
            else
            {
                //If v is -1, it means that we are erasing the answer, meaning the tile should be set to its default color
                //Otherwise we must set the tile to the color corresponding to a wrong answer
                if (v == -1 && !selectMode) g.GetComponent<Image>().color = tileConfig.defaultTileColor;
                else if (v == -1 && selectMode) g.GetComponent<Image>().color = tileConfig.selectedTileColor;
                else g.GetComponent<Image>().color = tileConfig.wrongTileColor;

                if (v != -1) updateMistakes(1);
            }

            if (v != -1) checkProgress();
        }
        else
        {
            //Setting "selected" tile color
            g.GetComponent<Image>().color = tileConfig.selectedTileColor;
        }


        //Highlighting all instances of the tile value and connected tiles (rows, columns, and neighborhood)
        for (int i = 0; i < currentGameObjectMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < currentGameObjectMatrix.GetLength(1); j++)
            {
                Color c = currentGameObjectMatrix[i, j].GetComponent<Image>().color;
                if (c != tileConfig.correctTileColor && c != tileConfig.wrongTileColor && !(i == x && j == y))
                {
                    if (((!notesOn && !eraseOn) || selectMode) && (i == x || j == y || isInNeighborhood(i, j, x, y)))
                        currentGameObjectMatrix[i, j].GetComponent<Image>().color = tileConfig.highlightTileColor;
                    else if ((i != x && j != y) || notesOn || eraseOn)
                        currentGameObjectMatrix[i, j].GetComponent<Image>().color = tileConfig.defaultTileColor;
                }
            }
        }
    }

    //Toggles erase mode
    public void toggleErase()
    {
        eraseOn = !eraseOn;

        //Chaning text color based on erase mode status
        buttonConfig.eraseButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color =
            eraseOn ? buttonConfig.mainButtonsSelectedColor : buttonConfig.mainButtonsDefaultColor;

        //If you are using icons instead of text buttons, please enable the following line instead.
        //buttonConfig.eraseButton.GetComponent<Image>().color = eraseOn ? buttonConfig.mainButtonsSelectedColor : buttonConfig.mainButtonsDefaultColor;
    }

    //Toggles notes mode
    public void toggleNotes()
    {
        notesOn = !notesOn;

        //Chaning text color based on notes mode status
        buttonConfig.notesButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color =
            notesOn ? buttonConfig.mainButtonsSelectedColor : buttonConfig.mainButtonsDefaultColor;

        //If you are using icons instead of text buttons, please enable the following line instead.
        //buttonConfig.notesButton.GetComponent<Image>().color = notesOn ? buttonConfig.mainButtonsSelectedColor : buttonConfig.mainButtonsDefaultColor;
    }

    //Updating the selected value variable. The newValue parameter corresponds to the value to be set.
    public void setSelectedValue(int newValue)
    {
        selectedValue = newValue;

        if (!selectMode)
        {
            for (int i = 0; i < buttonConfig.numberButtons.Count; i++)
            {
                if (i + 1 != selectedValue)
                {
                    buttonConfig.numberButtons[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color =
                        buttonConfig.numberButtonsDefaultColor;

                    //buttonConfig.numberButtons[i].GetComponent<Image>().color = buttonConfig.numberButtonsDefaultColor;
                }
                else
                {
                    buttonConfig.numberButtons[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color =
                        buttonConfig.numberButtonsSelectedColor;
                }
            }
        }
        else
        {
            if (selectedTile != null && selectMode)
                setElement(selectedTile, -1, (int)selectedTileCoordinates.x, (int)selectedTileCoordinates.y);
        }
    }

    //Setting element value based on selected mode
    private void setElement(GameObject tile, int c, int x, int y)
    {
        int value = c != -1 ? c : selectedValue;

        //Getting default text object
        GameObject g = tile.transform.GetChild(0).gameObject;

        //Are we taking nodes?
        if (notesOn)
        {
            //Disabling answer if taking notes
            g.SetActive(false);

            //Erasing current content and updating answer matrix
            g.GetComponent<Text>().text = "";
            setCurrentMatrixElement(tile, -1, x, y, false);

            //Getting tile child relative to the selected value
            g = tile.transform.GetChild(value).gameObject;
        }
        else
        {
            //If we are answering, we need to disable notes
            for (int i = 1; i < gridSize.x + 1; i++)
            {
                tile.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        //Are we erasing ?
        if (eraseOn)
        {
            //Erasing value from previously determined field
            g.GetComponent<Text>().text = "";

            //If we are not taking notes, we need to update our answer in the corresponding matrix 
            if (!notesOn) setCurrentMatrixElement(tile, -1, x, y, false);
            else g.SetActive(false);
        }
        else
        {
            if (answerMatrix[x, y] != selectedValue)
            {
                g.GetComponent<Text>().text = value.ToString();
                if (!notesOn) setCurrentMatrixElement(tile, value, x, y, false);
            }

            //Setting selected element as active
            g.SetActive(true);
        }
    }

    //Algorithmicaly generating a sudoku matrix
    private void gen()
    {
        int[,] matrix = new int[(int)gridSize.x, (int)gridSize.y];

        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);

        bool restart = true;

        int iterations = 0;

        //The range x -> y of sudoku digits
        //RangeY must always be larger than the number of columns of the matrix
        int rangeX = 1;
        int rangeY = columns + 1;

        while (restart)
        {
            if (iterations > failSafe)
            {
                Debug.Log("Failed to generate matrix. Please select different settings or increase the failsafe.");
                break;
            }

            iterations++;

            restart = false;
            matrix = new int[(int)gridSize.x, (int)gridSize.y];

            //For each row
            for (int i = 0; i < rows; i++)
            {
                List<int> temp = new List<int>();
                for (int k = (int)rangeX; k < (int)rangeY; k++) temp.Add(k);

                //For each column in the row
                for (int j = 0; j < columns; j++)
                {
                    //Starting with a random integer
                    int indx = Random.Range(0, temp.Count);
                    int rand = temp[indx];

                    List<int> tracker = new List<int>();

                    //While either the row, column, or neighborhood contains that integer, generate a new one 
                    while (checkColumn(matrix, j, rand) || checkRow(matrix, i, rand) ||
                           checkNeighborhood(matrix, i, j, rand))
                    {
                        indx = Random.Range(0, temp.Count);
                        rand = temp[indx];

                        restart = true;

                        //Are we out of options?
                        foreach (int z in temp)
                        {
                            if (!tracker.Contains(z))
                            {
                                restart = false;
                                break;
                            }
                        }

                        if (restart) break;

                        tracker.Add(rand);
                    }

                    if (restart) break;

                    //Adding element to matrix
                    matrix[i, j] = rand;
                }

                if (restart) break;
            }
        }

        currentMatrix = matrix;
        genGrid();
    }

    //Automatically generating greed
    //Please note that the pivot of the tile parent object should be set to 0,0
    private void genGrid()
    {
        int k = 0;

        for (int i = 0; i < currentMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < currentMatrix.GetLength(1); j++)
            {
                currentGameObjectMatrix[i, j] = tilesPosition[k];
                k++;
            }
        }
    }

    private void populateGrid()
    {
        var tsx = tileSize.x + tileMargin;
        var tsy = tileSize.y + tileMargin;

        List<Vector2> occupied = new List<Vector2>();

        for (int i = 0; i < currentMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < currentMatrix.GetLength(1); j++)
            {
                //Getting new tile position
                Vector2 coordinates = tileParent.transform.position;
                coordinates.x += tileMargin + i * tsx;
                coordinates.y += tileMargin + j * tsy;

                var neighborhoodX = Mathf.Ceil((i + 1) / neighborhoodSize.x);
                var neighborhoodY = Mathf.Ceil((j + 1) / neighborhoodSize.y);

                //Determining percentage of tiles to be displayed
                var probability = difficultyConfig.difficulties[difficultyConfig.currentDifficulty].probability;

                //Getting tile object
                var tile = currentGameObjectMatrix[i, j];
                //Changing tile color to default
                tile.GetComponent<Image>().color = tileConfig.defaultTileColor;

                //Each neighborhood must contain at least one value
                if (Random.value < probability || (!occupied.Contains(new Vector2(neighborhoodX, neighborhoodY)) &&
                                                   (i == neighborhoodX * neighborhoodSize.x - 1 &&
                                                    j == neighborhoodY * neighborhoodSize.y - 1)))
                {
                    tile.transform.GetChild(0).gameObject.GetComponent<Text>().text = currentMatrix[i, j].ToString();
                    tile.GetComponent<Image>().color = new Color(223, 223, 223);
                    occupied.Add(new Vector2(neighborhoodX, neighborhoodY));
                    answerMatrix[i, j] = currentMatrix[i, j];
                    tile.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                else
                {
                    answerMatrix[i, j] = -1;
                    tile.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";

                    var x = i;
                    var y = j;

                    tile.GetComponent<Button>().onClick.RemoveAllListeners();
                    tile.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (!selectMode) setElement(tile, -1, x, y);
                        else
                        {
                            if (tile != selectedTile)
                            {
                                selectedTile = tile;
                                selectedTileCoordinates = new Vector2(x, y);
                                setCurrentMatrixElement(tile, -1, x, y, true);
                            }
                        }
                    });
                }
            }
        }
    }

    private bool checkRow(int[,] m, int row, int value)
    {
        for (int i = 0; i < m.GetLength(1); i++)
        {
            if (m[row, i] == value)
            {
                return true;
            }
        }

        return false;
    }

    private bool checkColumn(int[,] m, int column, int value)
    {
        for (int i = 0; i < m.GetLength(0); i++)
        {
            if (m[i, column] == value)
            {
                return true;
            }
        }

        return false;
    }

    private bool checkNeighborhood(int[,] m, int row, int column, int value)
    {
        row++;
        column++;

        //Detecting the neighborouhood to which the value belongs
        Vector2 neighborhood =
            new Vector2(Mathf.Ceil(row / neighborhoodSize.x), Mathf.Ceil(column / neighborhoodSize.y));

        //Getting the starting location of the neighborhood within the matrix
        Vector2 neighborhoodStart = new Vector2((neighborhood.x * neighborhoodSize.x) - neighborhoodSize.x,
            (neighborhood.y * neighborhoodSize.y) - neighborhoodSize.y);

        //Getting the ending location of the neighborhood within the matrix
        Vector2 neighborhoodEnd = new Vector2(neighborhood.x * neighborhoodSize.x, neighborhood.y * neighborhoodSize.y);


        //Checking whether the value is unique in its neighborhood
        for (int i = (int)neighborhoodStart.x; i < (int)neighborhoodEnd.x; i++)
        {
            for (int j = (int)neighborhoodStart.y; j < (int)neighborhoodEnd.y; j++)
            {
                if (m[i, j] == value)
                {
                    return true;
                }
            }
        }

        return false;
    }

    //Check if a tile in within a neighborhood of another tile
    private bool isInNeighborhood(int x, int y, int row, int column)
    {
        row++;
        column++;

        //Detecting the neighborouhood to which the value belongs
        Vector2 neighborhood =
            new Vector2(Mathf.Ceil(row / neighborhoodSize.x), Mathf.Ceil(column / neighborhoodSize.y));

        //Getting the starting location of the neighborhood within the matrix
        Vector2 neighborhoodStart = new Vector2((neighborhood.x * neighborhoodSize.x) - neighborhoodSize.x,
            (neighborhood.y * neighborhoodSize.y) - neighborhoodSize.y);

        //Getting the ending location of the neighborhood within the matrix
        Vector2 neighborhoodEnd = new Vector2(neighborhood.x * neighborhoodSize.x, neighborhood.y * neighborhoodSize.y);


        if (x < neighborhoodEnd.x && x >= neighborhoodStart.x && y >= neighborhoodStart.y &&
            y < neighborhoodEnd.y) return true;
        return false;
    }

    public void toggle(GameObject g)
    {
        g.SetActive(!g.activeSelf);
    }
}

[System.Serializable]
public class DifficultyConfig
{
    [Tooltip("A list of game difficulties.")]
    public List<CustomDifficulty> difficulties = new List<CustomDifficulty>();

    [HideInInspector] public int currentDifficulty;

    public GameObject difficultyMenu;

    public Text difficultyButtonText;
}

[System.Serializable]
public class CustomDifficulty
{
    [Tooltip("The name off the difficulty.")]
    public string name;

    [Tooltip("The probability of ann answer being dispalyed under this difficulty, when generating the grid.")]
    public float probability;
}

[System.Serializable]
public class ButtonConfig
{
    public Color mainButtonsDefaultColor;
    public Color mainButtonsSelectedColor;
    public Color numberButtonsDefaultColor;
    public Color numberButtonsSelectedColor;

    public GameObject notesButton;
    public GameObject eraseButton;

    public List<GameObject> numberButtons = new List<GameObject>();
}

[System.Serializable]
public class TileConfig
{
    public Color defaultTileColor;
    public Color selectedTileColor;
    public Color wrongTileColor;
    public Color correctTileColor;
    public Color highlightTileColor;
}