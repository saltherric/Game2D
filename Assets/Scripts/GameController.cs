using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtResult;    // result text ("You Win!", "Draw", etc.)
    public TextMeshProUGUI txtScoreYou;  // score text for player
    public TextMeshProUGUI txtScoreCom;  // score text for computer
    public Image imgYou;                 // image that shows player's choice
    public Image imgCom;                 // image that shows computer's choice

    [Header("Sprites (assign in Inspector)")]
    public Sprite rockSprite;
    public Sprite paperSprite;
    public Sprite scissorSprite;

    [Header("Animation (optional)")]
    public Animator resultAnimator;      // optional: Animator with a pop/fade animation
    public string resultAnimStateName = "TxtResult_Pop"; // name of the animation state/clip
    public bool useAnimator = true;      // set false to use code-only pop

    [Header("Code-only pop settings")]
    public float popDuration = 0.35f;    // used when useAnimator = false
    public Vector3 popStartScale = new Vector3(0.5f, 0.5f, 1f);
    public Vector3 popPeakScale = new Vector3(1.25f, 1.25f, 1f);

    // internal score counters
    private int scoreYou = 0;
    private int scoreCom = 0;

    void Start()
    {
        // Initialize UI if not assigned
        if (txtResult != null) txtResult.text = "Make your choice!";
        UpdateScoreUI();
        if (imgYou != null) imgYou.sprite = null;
        if (imgCom != null) imgCom.sprite = null;
    }

    // This should be wired to the Button OnClick with a GameObject parameter (the button)
    public void OnButtonClick(GameObject buttonObject)
    {
        // safe-guard
        if (buttonObject == null)
        {
            Debug.LogWarning("OnButtonClick received null GameObject");
            return;
        }

        // Try to parse leading number from button name e.g. "1_Rock Button" -> 1
        int you = ParseButtonNumber(buttonObject.name);
        if (you < 1 || you > 3)
        {
            Debug.LogWarning("Button name not in expected format (1/2/3). Name: " + buttonObject.name);
            return;
        }

        PlayRound(you);
    }

    // Core round logic
    private void PlayRound(int you)
    {
        Debug.Log("Player choice: " + you);

        int com = Random.Range(1, 4); // 1..3
        Debug.Log("Computer choice: " + com);

        // Update art images
        UpdateChoiceImages(you, com);

        // Determine result
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

    // Set the image sprites for player and computer
    private void UpdateChoiceImages(int you, int com)
    {
        if (imgYou != null) imgYou.sprite = MapNumberToSprite(you);
        if (imgCom != null) imgCom.sprite = MapNumberToSprite(com);
    }

    // Map 1->rock, 2->paper, 3->scissor
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

    // Display result text and animate
    private void ShowResult(string message)
    {
        if (txtResult != null) txtResult.text = message;

        if (useAnimator && resultAnimator != null && !string.IsNullOrEmpty(resultAnimStateName))
        {
            // Play animator state from code. This assumes you created a state/clip with this name.
            resultAnimator.Play(resultAnimStateName, -1, 0f);
        }
        else if (!useAnimator && txtResult != null)
        {
            // Use code-only pop animation coroutine
            StopAllCoroutines();
            StartCoroutine(PopEffectCoroutine(txtResult.transform));
        }
    }

    // Parse leading digit safely
    private int ParseButtonNumber(string name)
    {
        if (string.IsNullOrEmpty(name)) return -1;
        // find first char that's a digit (robust to names like "1_Rock Button")
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

    // Code-only pop coroutine for UI element (scale bounce)
    private IEnumerator PopEffectCoroutine(Transform t)
    {
        if (t == null) yield break;

        Vector3 orig = t.localScale;
        t.localScale = popStartScale;

        float half = popDuration * 0.5f;
        float elapsed = 0f;
        // scale from start -> peak
        while (elapsed < half)
        {
            float p = elapsed / half;
            t.localScale = Vector3.Lerp(popStartScale, popPeakScale, Mathf.SmoothStep(0f, 1f, p));
            elapsed += Time.deltaTime;
            yield return null;
        }
        // scale from peak -> original
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

    // Public helper to reset scores (call from UI button if needed)
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
