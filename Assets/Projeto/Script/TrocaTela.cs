using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TrocaTela : MonoBehaviour
{
    public void trocarParaMemoria()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaMemoria");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaRaciocinio()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaRacioc");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaMemoriaAud()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaMemAud");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaTemasMem()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaTemasMem");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaJogoMemAlimento()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("MemoriaAlimento");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaJogoMemEsporte()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("MemoriaEsporte");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaJogoMemMaterial()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("MemoriaMaterial");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaRegrasMem()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaRegrasMemoria");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaTelaInicial()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaBuild");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaTemasMemAud()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaTemasMemAud");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaTemasMemAudAnimais()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("Animais");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaTemasMemAudCidade()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("Cidade");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaRegrasMemAud()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaRegrasMemAud");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaCenaSudoku()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("CenaSudoku");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaRegrasSudoku()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("CenaRegrasSudoku");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaJogoSudoku()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("LevelSudoku");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaCenaJogoDaVelha()
    {
        var start = SceneManager.GetActiveScene();
        var sceneLoad = SceneManager.LoadSceneAsync("CenaJogodaVelha");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaRegrasJogoDaVelha()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("CenaRegrasJogodaVelha");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaJogoJogoDaVelha()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("MainMenu");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaMenuJogoDaVelha()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("MainMenu");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaCenaCampoMinado()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("CenaCampoMinado");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaRegrasCampoMinado()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("CenaRegrasCampMin");
        sceneLoad.allowSceneActivation = true;
    }

    public void trocarParaJogoCampoMinado()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("CampoMinadoJogo");
        sceneLoad.allowSceneActivation = true;
    }

        public void trocarParaCredito()
    {
        var sceneLoad = SceneManager.LoadSceneAsync("Credito");
        sceneLoad.allowSceneActivation = true;
    }
}