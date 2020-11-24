/*
 * Kacper Bazan
 * Connect4 Implementation
 * Game.cs
 * 11/20/2020
 */

/*Useful Links:
 * Algorithms Explained: https://www.youtube.com/watch?v=l-hh51ncgDI
 */

using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject mouseCursor;
    public GameObject hoverItem;
    [HideInInspector]
    public MouseCursor mouseCursorScript;
    public Text winText;

    public string[,] boardState = new string[6, 7]; //string representation of the board.
    public GameObject[,] boardObjects = new GameObject[6, 7]; //instantiated x's and o's
    public GameObject[] buttons = new GameObject[7]; //clickable buttons
    private KeyCode[] resetKeys = new KeyCode[] { KeyCode.R };
    private KeyCode[] switchKeys = new KeyCode[] { KeyCode.T };
    private int bestMove;

    public GameObject xGameObject;
    public GameObject oGameObject;
    private bool xMove = true;
    private bool doesPlayerStart = true;
    private bool playerTurn = true;
    public int maxDepth;
    private int winner = 0;

    void Start()
    {
        Cursor.visible = true;
        mouseCursorScript = FindObjectOfType<MouseCursor>();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i == 0)
                {
                    buttons[j] = GameObject.Find("Space" + j.ToString());
                    buttons[j].GetComponent<BoxCollider>().enabled = true;
                    //buttons[j].GetComponent<MeshRenderer>().enabled = true;
                }
                boardState[i, j] = "-";
            }
        }

    }

    private void ResetSpaces()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (i == 0)
                {
                    buttons[j].GetComponent<BoxCollider>().enabled = true;
                    //buttons[j].GetComponent<MeshRenderer>().enabled = true;
                }
                if (boardObjects[i, j] != null)
                {
                    Destroy(boardObjects[i, j].gameObject);
                }
                boardState[i, j] = "-";
            }
        }
        xMove = true;
        playerTurn = doesPlayerStart;
    }

    void DisableSpaces()
    {
        for (int i = 0; i < 7; i++)
        {
            buttons[i].gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    void PlayerMove()
    {
        if (Input.GetMouseButtonDown(0) && hoverItem != null)
        {
            for (int i = 0; i < 7; i++)
            {
                if (buttons[i] == hoverItem)
                {
                    XMove(xMove, i);
                }
            }
            //hoverItem = null;
            //mouseCursorScript.hoverObject = null;
            playerTurn = !playerTurn;
        }
    }

    void AIMove()
    {
        if (CheckWin(boardState) != 0 || CheckTie(boardState))
        {
            return;
        }
        else
        {
            MiniMax(boardState, maxDepth, -1000, 1000, xMove, maxDepth);
            XMove(xMove, bestMove);
            playerTurn = !playerTurn;
        }
    }

    int MiniMax(string[,] position, int depth, int alpha, int beta, bool maximizingPlayer, int startingDepth)
    {
        if (depth == 0 || CheckWin(position) != 0 || CheckTie(position))
        {
            return CheckWin(position);
        }

        if (maximizingPlayer)
        {
            int maxEval = -1000;
            for (int j = 0; j < 7; j++)
            {
                if (position[0, j] == "-")
                {
                    int change_i = 0;
                    for (int i = 5; i >= 0; i--)
                    {
                        if (position[i, j] == "-")
                        {
                            change_i = i;
                            i = -1;
                        }
                    }
                    position[change_i, j] = "X";
                    int eval = MiniMax(position, depth - 1, alpha, beta, false, startingDepth);
                    position[change_i, j] = "-";

                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        if (depth == startingDepth)
                        {
                            bestMove = j;
                        }
                    }
                    alpha = Mathf.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }

            return maxEval;
        }

        else
        {
            int minEval = 1000;
            for (int j = 0; j < 7; j++)
            {
                if (position[0, j] == "-")
                {
                    int change_i = 0;
                    for (int i = 5; i >= 0; i--)
                    {
                        if (position[i, j] == "-")
                        {
                            change_i = i;
                            i = -1;
                        }
                    }
                    position[change_i, j] = "O";
                    int eval = MiniMax(position, depth - 1, alpha, beta, true, startingDepth);
                    position[change_i, j] = "-";

                    if (eval < minEval)
                    {
                        minEval = eval;
                        if (depth == startingDepth)
                        {
                            bestMove = j;
                        }
                    }
                    beta = Mathf.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            return minEval;
        }
    }

    void XMove(bool areYouX, int col)
    {
        bool cont = true;
        for (int i = 0; i < 6; i++)
        {
            if (boardState[(5 - i), col] == "-" && cont)
            {
                if (areYouX)
                {
                    boardState[(5 - i), col] = "X";
                    boardObjects[(5 - i), col] = Instantiate(xGameObject, new Vector2(buttons[col].transform.position.x, 2.5f - (5 - i)), Quaternion.identity);
                    cont = false;
                    if ((5 - i) == 0)
                    {
                        buttons[col].gameObject.GetComponent<BoxCollider>().enabled = false;
                        //buttons[col].gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
                else
                {
                    boardState[(5 - i), col] = "O";
                    boardObjects[(5 - i), col] = Instantiate(oGameObject, new Vector2(buttons[col].transform.position.x, 2.5f - (5 - i)), Quaternion.identity);
                    cont = false;
                    if ((5 - i) == 0)
                    {
                        buttons[col].gameObject.GetComponent<BoxCollider>().enabled = false;
                        //buttons[col].gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }



        }
        xMove = !xMove;
    }

    int CheckWin(string[,] stringVal)
    {
        //Horizontally
        for (int i = 0; i <= 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (stringVal[(5 - i), j] != "-")
                {
                    int counter = 0;
                    for (int c = 1; c < 4; c++)
                    {
                        if (stringVal[(5 - i), j + c] == "-")
                        {
                            continue;
                        }
                        if (stringVal[(5 - i), j] == stringVal[(5 - i), j + c])
                        {
                            counter++;
                        }
                        if (counter == 3)
                        {
                            if (stringVal[(5 - i), j] == "X")
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }

                    }
                }
            }
        }

        //Vertically
        for (int i = 0; i <= 2; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (stringVal[(5 - i), j] != "-")
                {
                    int counter = 0;
                    for (int c = 1; c < 4; c++)
                    {
                        if (stringVal[(5 - i) - c, j] == "-")
                        {
                            continue;
                        }
                        if (stringVal[(5 - i), j] == stringVal[(5 - i) - c, j])
                        {
                            counter++;
                        }
                        if (counter == 3)
                        {
                            if (stringVal[(5 - i), j] == "X")
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }

                    }
                }
            }
        }

        //Up-Right
        for (int i = 0; i <= 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (stringVal[(5 - i), j] != "-")
                {
                    int counter = 0;
                    for (int c = 1; c < 4; c++)
                    {
                        if (stringVal[(5 - i) - c, j + c] == "-")
                        {
                            continue;
                        }
                        if (stringVal[(5 - i), j] == stringVal[(5 - i) - c, j + c])
                        {
                            counter++;
                        }
                        if (counter == 3)
                        {
                            if (stringVal[(5 - i), j] == "X")
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }

                    }
                }
            }
        }

        //Up-Left
        for (int i = 0; i <= 2; i++)
        {
            for (int j = 3; j < 7; j++)
            {
                if (stringVal[(5 - i), j] != "-")
                {
                    int counter = 0;
                    for (int c = 1; c < 4; c++)
                    {
                        if (stringVal[(5 - i) - c, j - c] == "-")
                        {
                            continue;
                        }
                        if (stringVal[(5 - i), j] == stringVal[(5 - i) - c, j - c])
                        {
                            counter++;
                        }
                        if (counter == 3)
                        {
                            if (stringVal[(5 - i), j] == "X")
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }

                    }
                }
            }
        }
        return 0;
    }
    bool CheckTie(string[,] stringVal)
    {
        if (CheckWin(stringVal) == 0)
        {
            for (int i = 0; i < 7; i++)
            {
                if (stringVal[0, i] == "-")
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
    void Update()
    {
        //checks each reset key and then performs resetspaces().
        foreach (KeyCode key in resetKeys)
        {
            if (Input.GetKeyDown(key))
            {
                ResetSpaces();
            }
        }
        foreach (KeyCode key in switchKeys)
        {
            if (Input.GetKeyDown(key))
            {
                doesPlayerStart = !doesPlayerStart;
                ResetSpaces();
            }
        }

        float x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        float y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        mouseCursor.transform.position = new Vector3(x, y, 0);
        hoverItem = mouseCursorScript.hoverObject;
        winner = CheckWin(boardState);

        if (winner == 0 && !CheckTie(boardState))
        {
            winText.text = "";
            if (playerTurn)
            {
                PlayerMove();
            }
            else
            {
                AIMove();
            }
        }
        else
        {
            DisableSpaces();
            switch (winner)
            {
                case -1:
                    winText.text = "O has won!";
                    break;
                case 0:
                    winText.text = "It is a tie!";
                    break;
                case 1:
                    winText.text = "X has won!";
                    break;

            }
        }
    }
}
