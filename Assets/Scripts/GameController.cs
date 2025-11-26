using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtResult;
    public TextMeshProUGUI txtScoreYou;
    public TextMeshProUGUI txtScoreCom;
    public Image imgYou;
    public Image imgCom;

    [Header("Sprites (assign in Inspector)")]
    public Sprite rockSprite;
    public Sprite paperSprite;
    public Sprite scissorSprite;

    [Header("Animation (optional)")]
    public Animator resultAnimator;
    public string resultAnimStateName = "TxtResult_Pop";
    public bool useAnimator = true;

    [Header("Code-only pop settings")]
    public float popDuration = 0.35f;
    public Vector3 popStartScale = new Vector3(0.5f, 0.5f, 1f);
    public Vector3 popPeakScale = new Vector3(1.25f, 1.25f, 1f);

    private int scoreYou = 0;
    private int scoreCom = 0;

    void Start()
    {
        if (txtResult != null) txtResult.text = "Make your choice!";
        UpdateScoreUI();
        if (imgYou != null) imgYou.sprite = null;
        if (imgCom != null) imgCom.sprite = null;
    }

    public void OnButtonClick(GameObject buttonObject)
    {
        if (buttonObject == null) return;

        int you = ParseButtonNumber(buttonObject.name);
        if (you < 1 || you > 3) return;

        PlayRound(you);
    }

    private void PlayRound(int you)
    {
        int com = Random.Range(1, 4);

        UpdateChoiceImages(you, com);

        int k = you - com;
        if (k == 0)
        {
            ShowResult("It's a Draw!");
        }
        else if (k == 1 || k == -2)
        {
            scoreYou++;
            ShowResult("You WIN!");
        }
        else
        {
            scoreCom++;
            ShowResult("Computer WINS!");
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (txtScoreYou != null) txtScoreYou.text = scoreYou.ToString();
        if (txtScoreCom != null) txtScoreCom.text = scoreCom.ToString();
    }

    private void UpdateChoiceImages(int you, int com)
    {
        if (imgYou != null) imgYou.sprite = MapNumberToSprite(you);
        if (imgCom != null) imgCom.sprite = MapNumberToSprite(com);
    }

    private Sprite MapNumberToSprite(int n)
    {
        switch (n)
        {
            case 1: return rockSprite;
            case 2: return paperSprite;
            case 3: return scissorSprite;
            default: return null;
        }
    }

    private void ShowResult(string message)
    {
        if (txtResult != null) txtResult.text = message;

        if (useAnimator && resultAnimator != null && !string.IsNullOrEmpty(resultAnimStateName))
        {
            resultAnimator.Play(resultAnimStateName, -1, 0f);
        }
        else if (!useAnimator && txtResult != null)
        {
            StopAllCoroutines();
            StartCoroutine(PopEffectCoroutine(txtResult.transform));
        }
    }

    private int ParseButtonNumber(string name)
    {
        if (string.IsNullOrEmpty(name)) return -1;

        for (int i = 0; i < name.Length; i++)
        {
            if (char.IsDigit(name[i]))
            {
                int val;
                if (int.TryParse(name[i].ToString(), out val)) return val;
            }
        }
        return -1;
    }

    private IEnumerator PopEffectCoroutine(Transform t)
    {
        if (t == null) yield break;

        Vector3 orig = t.localScale;
        t.localScale = popStartScale;

        float half = popDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < half)
        {
            float p = elapsed / half;
            t.localScale = Vector3.Lerp(popStartScale, popPeakScale, Mathf.SmoothStep(0f, 1f, p));
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            float p = elapsed / half;
            t.localScale = Vector3.Lerp(popPeakScale, orig, Mathf.SmoothStep(0f, 1f, p));
            elapsed += Time.deltaTime;
            yield return null;
        }

        t.localScale = orig;
    }

    public void ResetScores()
    {
        scoreYou = 0;
        scoreCom = 0;
        UpdateScoreUI();
        if (txtResult != null) txtResult.text = "Make your choice!";
        if (imgYou != null) imgYou.sprite = null;
        if (imgCom != null) imgCom.sprite = null;
    }
}
