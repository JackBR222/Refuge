using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FinalScene : MonoBehaviour
{
    public GameObject resumoText;
    public GameObject fimText;
    public GameObject botaoSair;
    public GameObject botaoResetar;

    private CanvasGroup resumoCanvasGroup;
    private CanvasGroup fimCanvasGroup;
    private CanvasGroup sairCanvasGroup;
    private CanvasGroup resetarCanvasGroup;

    public float tempoResumo = 3f;
    public float tempoFim = 1f;
    public float tempoBotoes = 1f;

    public string cenaParaCarregar = "CenaInicial";

    private void Start()
    {
        resumoCanvasGroup = resumoText.GetComponent<CanvasGroup>();
        fimCanvasGroup = fimText.GetComponent<CanvasGroup>();
        sairCanvasGroup = botaoSair.GetComponent<CanvasGroup>();
        resetarCanvasGroup = botaoResetar.GetComponent<CanvasGroup>();

        resumoCanvasGroup.alpha = 0f;
        fimCanvasGroup.alpha = 0f;
        sairCanvasGroup.alpha = 0f;
        resetarCanvasGroup.alpha = 0f;

        StartCoroutine(FadeIn(resumoCanvasGroup));

        StartCoroutine(ControleResumo());
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float tempo = 1f;
        float t = 0f;

        while (t < tempo)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / tempo);
            yield return null;
        }
    }

    IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float tempo = 1f;
        float t = 0f;

        while (t < tempo)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - t / tempo);
            yield return null;
        }
    }

    IEnumerator ControleResumo()
    {
        yield return StartCoroutine(FadeIn(resumoCanvasGroup));

        yield return new WaitForSeconds(tempoResumo);
        yield return StartCoroutine(FadeOut(resumoCanvasGroup));

        yield return StartCoroutine(FadeIn(fimCanvasGroup));

        yield return new WaitForSeconds(tempoFim);

        yield return new WaitForSeconds(tempoBotoes);
        yield return StartCoroutine(FadeIn(sairCanvasGroup));
        yield return StartCoroutine(FadeIn(resetarCanvasGroup));
    }

    public void CarregarCena()
    {
        SceneManager.LoadScene(cenaParaCarregar);
    }

    public void SairJogo()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
