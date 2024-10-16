using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TicTacToeWithAI.Board
{
    // Operation of the menu present in TicTacToe scene and support for the most important game parameters.

    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject fimJogoObj;
        [SerializeField] private GameObject vocePerdeuObj;

        public static int round;
        public static bool player;

        public static int
            moves; // the number of moves made in the game by both players in total (number of pieces on the board)

        public static bool gameOver;

        private void Start()
        {
            NewGame();
        }

        public void NewGame()
        {
            gameOver = false;
            fimJogoObj.SetActive(false);
            vocePerdeuObj.SetActive(false);
        }

        public void ExitGame()
        {
            moves = 0;
            round = 0;
            gameOver = false;
        }

        private void Update()
        {
            if (gameOver || moves >= 9)
                PrepareExit();
        }

        private void PrepareExit()
        {
            gameOver = true;
            moves = 0;
            round = 0;
            if (player) fimJogoObj.SetActive(true);
            else vocePerdeuObj.SetActive(true);
        }
    }
}