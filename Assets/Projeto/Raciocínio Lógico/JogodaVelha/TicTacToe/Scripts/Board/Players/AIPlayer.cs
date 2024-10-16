using UnityEngine;
using System.Collections;
using Broniek.Stuff.Sounds;
using UnityEngine.InputSystem.HID;

namespace TicTacToeWithAI.Board
{
    // The player's artificial intelligence, regardless of whether they start first or second.

    public class AIPlayer : MonoBehaviour
    {
        public static int playerId = 1; // 0 - play first, 1 - play second

        private bool[,] ourBoard; // an array of our pawns
        private bool[,] foeBoard; // an opponent's pawn array

        private int continueAttack;
        private Transform boardTransform;

        private void Start() => PrepareNewGame();

        private void PrepareNewGame()
        {
            boardTransform = FindFirstObjectByType<BoardCreator>().transform;
            ourBoard = playerId == 0 ? BoardCreator.circleBoard : BoardCreator.crossBoard;
            foeBoard = playerId != 0 ? BoardCreator.circleBoard : BoardCreator.crossBoard;

            StartCoroutine(TurnMonitor());
        }

        private IEnumerator TurnMonitor()
        {
            while (true)
            {
                yield return null;

                if (GameController.round == playerId)
                {
                    yield return StartCoroutine(TakeCoroutineMove());
                    GameController.round = (++GameController.round) % 2;
                }
            }
        }

        private IEnumerator TakeCoroutineMove()
        {
            yield return new WaitForSeconds(0.5f);

            if (!GameController.gameOver)
            {
                GameController.moves++;

                if (CheckIfAction(0)) // check if AI wins
                {
                    GameController.gameOver = true;
                    SoundManager.GetSoundEffect(0, 1f, 0.5f);
                    yield break;
                }

                // Actions to check the state of the board in order of priority (importance).
                if (CheckIfAction(1)) yield break; // check type I forced motion
                if (CheckIfDefence()) yield break; // check type II forced motion
                if (CheckIfAttack()) yield break; // check if there is a possibility of creating a veiled threat
                if (CheckIfForced()) yield break; // check type III forced motion

                TakeFreeMove(HumanPlayer.row, HumanPlayer.col);
            }
        }

        private void TakeFreeMove(int row, int col) // make unforced movement
        {
            if (ourBoard[1, 1] == false) // if our pawn is not in the middle of the board
            {
                // if there is no opponent's pawn in the middle of the board, there are two possibilities: we start the game or the opponent has placed a pawn in a different place (he did not act optimally)
                if (foeBoard[1, 1] == false) // so we put our pawn in the center of the board there
                    PlacePattern(1, 1); // Debug.Log("We put in the middle of the board.");
                else // and if there is an opponent's pawn in the middle of the board, then the opponent started the game (otherwise our pawn would be there)
                {
                    if (ourBoard[0, 0] == false) // if our pawn is not in the upper left corner,
                        PlacePattern(0, 0); // we place our pawn in the upper left corner of the board
                    else // if we have already placed a pawn in the upper left corner
                    {
                        // forced our movement: we prevent the opponent from setting a three
                        if (GameController.moves == 4)
                            PlacePattern(0, 2);
                        else if (GameController.moves == 8)
                        {
                            if (ourBoard[1, 0] == false && foeBoard[1, 0] == false)
                                PlacePattern(1, 0);
                            else
                            {
                                if (!PlaceInCorner()) // if you don't manage to get in the corner,
                                    PlaceInSide(); // put it on the side
                            }
                        }
                    }
                }
            }
            else if (ourBoard[1, 1] == true) // if our pawn is in the middle of the board (our next move)
            {
                if (row != 1) row = (row + 2) % 4;
                if (col != 1) col = (col + 2) % 4;

                if (ourBoard[row, col] == true)
                    PlaceInCorner();
                else
                {
                    if (foeBoard[row, col] == true) PlaceInSide();
                    else PlacePattern(row, col);
                }
            }
        }

        private bool PlaceInSide() // put on the verge
        {
            if (ourBoard[0, 1] == false && foeBoard[0, 1] == false) return PlacePattern(0, 1);
            else if (ourBoard[1, 0] == false && foeBoard[1, 0] == false) return PlacePattern(1, 0);
            else if (ourBoard[1, 2] == false && foeBoard[1, 2] == false) return PlacePattern(1, 2);
            else if (ourBoard[2, 1] == false && foeBoard[2, 1] == false) return PlacePattern(2, 1);

            return false;
        }

        private bool PlaceInCorner() // put in the corner
        {
            if (ourBoard[2, 2] == false && foeBoard[2, 2] == false) return PlacePattern(2, 2);
            else if (ourBoard[0, 2] == false && foeBoard[0, 2] == false) return PlacePattern(0, 2);
            else if (ourBoard[2, 0] == false && foeBoard[2, 0] == false) return PlacePattern(2, 0);
            else if (ourBoard[0, 0] == false && foeBoard[0, 0] == false) return PlacePattern(0, 0);

            return false;
        }

        private bool CheckIfForced()
        {
            if (CheckA(0, 1, 2, 0, 0, 0, 1, 0)) return true;
            else if (CheckA(0, 1, 2, 2, 0, 2, 1, 2)) return true;
            else if (CheckA(1, 2, 2, 0, 2, 2, 2, 1)) return true;
            else if (CheckA(1, 2, 0, 0, 0, 2, 0, 1)) return true;
            else if (CheckA(2, 1, 0, 0, 2, 0, 1, 0)) return true;
            else if (CheckA(2, 1, 0, 2, 2, 2, 1, 2)) return true;
            else if (CheckA(1, 0, 0, 2, 0, 0, 0, 1)) return true;
            else if (CheckA(1, 0, 2, 2, 2, 0, 2, 1)) return true;

            return false;
        }

        private bool CheckA(int a, int b, int c, int d, int e, int f, int g, int h)
        {
            if (foeBoard[a, b] == true && foeBoard[c, d] == true)
                if (ourBoard[e, f] == false && ourBoard[g, h] == false)
                    return PlacePattern(e, f); //Debug.Log("CheckA");

            return false;
        }

        private bool CheckIfDefence()
        {
            if (CheckB(0, 1, 1, 0, 0, 0)) return true;
            else if (CheckB(1, 0, 2, 1, 2, 0)) return true;
            else if (CheckB(0, 1, 1, 2, 0, 2)) return true;
            else if (CheckB(1, 2, 2, 1, 2, 2)) return true;

            return false;
        }

        private bool CheckB(int a, int b, int c, int d, int e, int f)
        {
            if (foeBoard[a, b] == true && foeBoard[c, d] == true)
                if (ourBoard[e, f] == false)
                    return PlacePattern(e, f); //Debug.Log("CheckB");

            return false;
        }

        private bool CheckIfAction(int mode)
        {
            int m = 0, n = 0, r = 0, c = 0;

            for (int i = 0; i < 3; i++) // 3 horizontal lines
                if (PerformLoop(mode, 3, i, ref m, ref n, ref r, ref c))
                    return true;

            for (int i = 0; i < 3; i++) // 3 vertical lines
                if (PerformLoop(mode, 2, i, ref m, ref n, ref r, ref c))
                    return true;
            // 2 diagonal lines:
            if (PerformLoop(mode, 1, -1, ref m, ref n, ref r, ref c)) // first
                return true;
            if (PerformLoop(mode, 0, -1, ref m, ref n, ref r, ref c)) // second
                return true;

            return false;
        }

        private bool PerformLoop(int mode, int loop, int no, ref int m, ref int n, ref int r, ref int c)
        {
            m = n = 0;

            for (int i = 0; i < 3; i++)
                if (loop == 0) Detect(2 - i, i, ref m, ref n, ref r, ref c);
                else if (loop == 1) Detect(i, i, ref m, ref n, ref r, ref c);
                else if (loop == 2) Detect(i, no, ref m, ref n, ref r, ref c);
                else if (loop == 3) Detect(no, i, ref m, ref n, ref r, ref c);

            if (Check(mode, m, n, r, c))
                return true;

            return false;
        }

        private void Detect(int a, int b, ref int m, ref int n, ref int r, ref int c)
        {
            if (ourBoard[a, b] == true) m++;
            if (foeBoard[a, b] == true) n++;

            if (ourBoard[a, b] == false && foeBoard[a, b] == false)
            {
                r = a;
                c = b;
            }
        }

        private bool Check(int mode, int m, int n, int r, int c)
        {
            if (mode == 0)
            {
                if (m == 2 && n == 0) // win on the current move
                    return PlacePattern(r, c); //Debug.Log("Check: Win");
            }
            else if (mode == 1)
                if (m == 0 && n == 2) // We prevent the threat of Type I failing.
                    return PlacePattern(r, c); //Debug.Log("Check: Defense");

            return false;
        }

        private bool
            CheckIfAttack() // we can only attack when we are the first to start and the player's 1st move was suboptimal
        {
            if (GameController.moves == 3)
            {
                if (CheckC(1, 0, 0, 0, 1)) return true;
                else if (CheckC(1, 2, 2, 2, 2)) return true;
            }

            if (GameController.moves == 5)
            {
                if (continueAttack == 1)
                {
                    if (foeBoard[0, 1] == true) return PlacePattern(1, 0);
                    else return PlacePattern(0, 1);
                }
                else if (continueAttack == 2)
                {
                    if (foeBoard[1, 2] == true) return PlacePattern(2, 1);
                    else return PlacePattern(1, 2);
                }
            }

            return false;
        }

        private bool CheckC(int a, int b, int c, int d, int e)
        {
            if (ourBoard[1, 1] == true && (foeBoard[a, b] == true || foeBoard[b, a] == true))
            {
                continueAttack = e;
                return PlacePattern(c, d);
            }

            return false;
        }

        private bool PlacePattern(int r, int c)
        {
            SoundManager.GetSoundEffect(1, 0.5f);

            ourBoard[r, c] = true;
            boardTransform.GetChild(3 * r + c).GetChild(playerId).gameObject.SetActive(true);
            boardTransform.GetChild(3 * r + c).GetComponent<UnityEngine.UI.Button>().enabled = false;
            return true;
        }
    }
}