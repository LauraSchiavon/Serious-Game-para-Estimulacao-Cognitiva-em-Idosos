using UnityEngine;

namespace TicTacToeWithAI.Board
{
    // Board creration.

    public class BoardCreator : MonoBehaviour
    {
        public static bool[,] circleBoard; // tic-tac-toe bit arrays
        public static bool[,] crossBoard;

        public static BoardCreator Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            circleBoard = new bool[3, 3];
            crossBoard = new bool[3, 3];
        }
    }
}