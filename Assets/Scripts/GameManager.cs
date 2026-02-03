using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Button[,] board = new Button[3, 3];
    public string[,] boardState = new string[3, 3];

    private string playerSymbol;
    private string aiSymbol;
    private bool isPlayerTurn;

    void Start()
    {
        playerSymbol = PlayerPrefs.GetString("PlayerSide", "X");
        aiSymbol = (playerSymbol == "X") ? "O" : "X";
        isPlayerTurn = (playerSymbol == "X");

        SetupBoard();
    }

    void SetupBoard()
    {
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                string name = $"Button_{r}_{c}";
                Button btn = GameObject.Find(name).GetComponent<Button>();

                board[r, c] = btn;
                boardState[r, c] = "";

                int row = r, col = c;  // Capture loop vars
                btn.onClick.AddListener(() => OnCellClicked(row, col));

                // TMP label
                TextMeshProUGUI tmpText = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null) tmpText.text = "";
            }
        }

        if (!isPlayerTurn)
            Invoke("AIMove", 0.5f);
    }

    void OnCellClicked(int row, int col)
    {
        if (!isPlayerTurn || boardState[row, col] != "")
            return;

        MakeMove(row, col, playerSymbol);

        if (CheckGameOver()) return;

        isPlayerTurn = false;
        Invoke("AIMove", 0.5f);
    }

    void MakeMove(int row, int col, string symbol)
    {
        boardState[row, col] = symbol;

        TextMeshProUGUI tmpText = board[row, col].GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null) tmpText.text = symbol;

        board[row, col].interactable = false;
    }

    void AIMove()
    {
        int bestScore = int.MinValue;
        int moveRow = -1, moveCol = -1;

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                if (boardState[r, c] == "")
                {
                    boardState[r, c] = aiSymbol;
                    int score = Minimax(boardState, 0, false);
                    boardState[r, c] = "";

                    if (score > bestScore)
                    {
                        bestScore = score;
                        moveRow = r;
                        moveCol = c;
                    }
                }
            }
        }

        MakeMove(moveRow, moveCol, aiSymbol);

        if (!CheckGameOver())
            isPlayerTurn = true;
    }

    int Minimax(string[,] board, int depth, bool isMaximizing)
    {
        string result = GetWinner(board);
        if (result != null)
        {
            if (result == aiSymbol) return 10 - depth;
            else if (result == playerSymbol) return depth - 10;
            else return 0;
        }

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (board[r, c] == "")
                    {
                        board[r, c] = aiSymbol;
                        int eval = Minimax(board, depth + 1, false);
                        board[r, c] = "";
                        maxEval = Mathf.Max(maxEval, eval);
                    }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (board[r, c] == "")
                    {
                        board[r, c] = playerSymbol;
                        int eval = Minimax(board, depth + 1, true);
                        board[r, c] = "";
                        minEval = Mathf.Min(minEval, eval);
                    }
            return minEval;
        }
    }

    string GetWinner(string[,] board)
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] != "" && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                return board[i, 0];
            if (board[0, i] != "" && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                return board[0, i];
        }

        if (board[0, 0] != "" && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
            return board[0, 0];
        if (board[0, 2] != "" && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
            return board[0, 2];

        foreach (var cell in board)
            if (cell == "") return null;

        return "Draw";
    }

    bool CheckGameOver()
    {
        string result = GetWinner(boardState);
        if (result != null)
        {
            PlayerPrefs.SetString("Result", result);
            SceneManager.LoadScene("GameOver");
            return true;
        }
        return false;
    }
}