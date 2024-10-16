using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
    public void VoltarMenu()
    {
        var menuLoading = SceneManager.LoadSceneAsync(0);
        StartCoroutine(LoadScene(menuLoading, 0));
    }

    public void JogarNovamente()
    {
        var actualLevel = SceneManager.GetActiveScene().buildIndex;

        switch (actualLevel)
        {
            case 1:
                var easyLoading = SceneManager.LoadSceneAsync(1);
                StartCoroutine(LoadScene(easyLoading, 1));
                break;
            case 2:
                var mediumLoading = SceneManager.LoadSceneAsync(2);
                StartCoroutine(LoadScene(mediumLoading, 2));
                break;
            case 3:
                var hardLoading = SceneManager.LoadSceneAsync(3);
                StartCoroutine(LoadScene(hardLoading, 3));
                break;
            default:
                break;
        }
    }

    private static IEnumerator LoadScene(AsyncOperation loading, int buildIndex)
    {
        
        yield return new WaitUntil(() => loading.isDone);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex));
    }
}