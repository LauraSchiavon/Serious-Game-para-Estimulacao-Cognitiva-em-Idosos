using System;
using UnityEngine;
using Broniek.Stuff.Sounds;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TicTacToeWithAI.Board
{
    // Support for actions taken by a live player.

    public class HumanPlayer : MonoBehaviour
    {
        public static int playerId = 0;
        public static bool selfPlay; // working when PlayerAI has playerId = -1, and PlayerHuman has playerId = 0

        public static int row; // coordinates of the player's last move
        public static int col;

        private bool[,] board;
        private Transform boardTransform;
        private bool canClick;


        private void Start()
        {
            boardTransform = BoardCreator.Instance.transform;
            board = (playerId == 0) ? BoardCreator.circleBoard : BoardCreator.crossBoard;
        }

        private void Update()
        {
            if (!GameController.gameOver)
            {
                canClick = GameController.round == playerId;
            }
        }


        public void PlacePattern(GameObject tile)
        {
            if (!canClick) return;
            col = tile.GetComponent<JogoVelhaClicar>().col;
            row = tile.GetComponent<JogoVelhaClicar>().row;
            board[row, col] = true;

            GameController.moves++;

            GameController.round = selfPlay ? playerId : (++GameController.round) % 2;
            int no = selfPlay ? GameController.moves % 2 : playerId;
            tile.transform.GetChild(no).gameObject.SetActive(true);
            tile.GetComponent<Button>().enabled = false;

            SoundManager.GetSoundEffect(1, 0.5f);

            if (selfPlay) // only playing alone with ourself
                if (CheckIfOver()) // check if AI wins
                {
                    Debug.Log("player ganhou");
                    GameController.gameOver = true;
                    GameController.player = selfPlay;
                }
        }

        private bool CheckIfOver()
        {
            for (int k = 0; k < 2; k++) // for each symbol
            {
                int amount3 = 0;
                int amount4 = 0;

                for (int i = 0; i < 3; i++)
                {
                    int amount1 = 0;
                    int amount2 = 0;

                    for (int j = 0; j < 3; j++)
                    {
                        if (boardTransform.GetChild(j + 3 * i).GetChild(k).gameObject.activeSelf) // horizontally
                            amount1++;

                        if (boardTransform.GetChild(i + 3 * j).GetChild(k).gameObject.activeSelf) // vertically
                            amount2++;
                    }

                    if (boardTransform.GetChild(4 * i).GetChild(k).gameObject.activeSelf) // diagonally
                        amount3++;

                    if (boardTransform.GetChild(2 * i + 2).GetChild(k).gameObject.activeSelf) // diagonally
                        amount4++;

                    if (amount1 == 3 || amount2 == 3)
                        return true;
                }

                if (amount3 == 3 || amount4 == 3)
                    return true;
            }

            return false;
        }
    }
}