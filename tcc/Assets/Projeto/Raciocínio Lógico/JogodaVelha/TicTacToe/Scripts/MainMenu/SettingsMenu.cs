using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TicTacToeWithAI.Board;

namespace TicTacToeWithAI
{
    // Setting essential game parameters:
    // - self play
    // - who's playing first

    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Toggle selfPlayToggle;
        [SerializeField] private Toggle playingFirstToggle;
        [SerializeField] private Button startPlayBtn;

        private void Awake()
        {
            startPlayBtn.onClick.AddListener(LoadGame);
            selfPlayToggle.onValueChanged.AddListener(SelfPlayChanged);
        }

        private void SelfPlayChanged(bool isOn)
        {
            playingFirstToggle.gameObject.SetActive(!isOn);
        }

        private void LoadGame()
        {
            if (selfPlayToggle.isOn)    // we play with ourselves
            {
                HumanPlayer.selfPlay = true;
                HumanPlayer.playerId = 0;
                AIPlayer.playerId = -1;
            }
            else
            {
                HumanPlayer.selfPlay = false;

                if (playingFirstToggle.isOn)
                {
                    HumanPlayer.playerId = 0;
                    AIPlayer.playerId = 1;
                }
                else
                {
                    HumanPlayer.playerId = 1;
                    AIPlayer.playerId = 0;
                }
            }

            SceneManager.LoadScene("Board");
        }
    }
}