using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameLogic : MonoBehaviour
{
    Coroutine timer, lose;

    [Tooltip("Настройки игровой панели")]
    [Header("Question Panel")]
    [SerializeField] private GameObject _headPanel;
    [SerializeField] private QuestionList[] questions;
    [SerializeField] private Text[] answersText;
    [SerializeField] private GameObject[] _answerButtonsAnimator;
    [SerializeField] private Text _questionText;
    [SerializeField] private GameObject _projectName;
    [SerializeField] private GameObject _questionsPanel;
    [SerializeField] private Button[] _answerButtons = new Button[4];

    [Header("Win")]
    [SerializeField] private GameObject _winText;

    [Header("True Or False")]
    [SerializeField] private Sprite[] _trueOrFalseIcons;
    [SerializeField] private Image _trueOrFalseIcon;
    [SerializeField] private Text _trueOrFalseText;

    [Header("Timer")]
    [SerializeField] private float _time;
    [SerializeField] private Text _timerText;
    private float _timeLeft;

    List<object> questionList;
    QuestionList currentQuestion;
    int randomQuestion;

    public void OnClickPlay()
    {
        questionList = new List<object>(questions);
        QuestionGenerate();

        if (!_headPanel.GetComponent<Animator>().enabled)
        {
            _headPanel.GetComponent<Animator>().enabled = true;
        }
        else
        {
            _headPanel.GetComponent<Animator>().SetTrigger("Enter");
        }
    }

    void QuestionGenerate()
    {
        if (questionList.Count > 0)
        {
            randomQuestion = Random.Range(0, questionList.Count);

            currentQuestion = questionList[randomQuestion] as QuestionList;
            _questionText.text = currentQuestion.question;

            List<string> answers = new(currentQuestion.answers);

            for (int i = 0; i < currentQuestion.answers.Length; i++)
            {
                int random = Random.Range(0, answers.Count);
                answersText[i].text = answers[random];
                answers.RemoveAt(random);
            }

            StartCoroutine(AnimationButtons());
        }
        else
        {
            StartCoroutine(Win());
        }
    }

    IEnumerator AnimationButtons()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < _answerButtonsAnimator.Length; i++)
        {
            if (!_answerButtonsAnimator[i].GetComponent<Animator>().enabled)
            {
                _answerButtonsAnimator[i].GetComponent<Animator>().enabled = true;
            }
            else
            {
                _answerButtonsAnimator[i].GetComponent<Animator>().SetTrigger("Enter");
            }
        }

        for (int i = 0; i < _answerButtons.Length; i++)
        {
            _answerButtons[i].interactable = false;
        }

        int a = 0;

        while (a < _answerButtons.Length)
        {
            if (!_answerButtons[a].gameObject.activeSelf)
            {
                _answerButtons[a].gameObject.SetActive(true);
            }
            a++;
            yield return new WaitForSeconds(0.5f);
        }
        for (int i = 0; i < _answerButtons.Length; i++)
        {
            _answerButtons[i].interactable = true;
        }

        yield return new WaitForSeconds(0.25f);
        _timeLeft = _time;
        timer = StartCoroutine(Timer());
        yield break;
    }

    IEnumerator TrueOrFalseAnswer(bool check)
    {
        for (int i = 0; i < _answerButtons.Length; i++)
        {
            _answerButtons[i].interactable = false;
        }

        yield return new WaitForSeconds(0.5f);

        if (!_trueOrFalseIcon.gameObject.activeSelf)
        {
            _trueOrFalseIcon.gameObject.SetActive(true);
        }

        if (check)
        {
            _trueOrFalseIcon.sprite = _trueOrFalseIcons[0];
            _trueOrFalseText.text = "Правильно!";

            StopCoroutine(timer);
            yield return new WaitForSeconds(1f);

            _trueOrFalseIcon.gameObject.SetActive(false);
            for (int i = 0; i < _answerButtonsAnimator.Length; i++)
            {
                _answerButtonsAnimator[i].GetComponent<Animator>().SetTrigger("Out");
            }
            yield return new WaitForSeconds(0.5f);

            questionList.RemoveAt(randomQuestion);
            QuestionGenerate();
            _timeLeft = _time;

            for (int i = 0; i < _answerButtons.Length; i++)
            {
                _answerButtons[i].interactable = false;
            }
            yield break;
        }
        else
        {
            StopCoroutine(timer);
            _timerText.gameObject.SetActive(false);
            _trueOrFalseIcon.sprite = _trueOrFalseIcons[1];
            _trueOrFalseText.text = "Неправильно!";
            yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < _answerButtonsAnimator.Length; i++)
            {
                _answerButtonsAnimator[i].GetComponent<Animator>().SetTrigger("Out");
            }

            yield return new WaitForSeconds(1f);
            _trueOrFalseIcon.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _headPanel.GetComponent<Animator>().SetTrigger("Out");
            yield break;
        }
    }

    IEnumerator Timer()
    {
        while (_timeLeft > 0)
        {
            _timerText.gameObject.SetActive(true);
            _timeLeft -= Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }
    }

    IEnumerator Win()
    {
        _timerText.gameObject.SetActive(false);
        StopCoroutine(timer);

        for (int i = 0; i < _answerButtons.Length; i++)
        {
            _answerButtons[i].gameObject.SetActive(false);
        }
        _questionText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        _winText.SetActive(true);
        yield return new WaitForSeconds(10f);

        _winText.SetActive(false);
        _questionText.gameObject.SetActive(true);
        _headPanel.GetComponent<Animator>().SetTrigger("Out");

        yield break;
    }

    IEnumerator Lose(bool check)
    {
        if (!check)
        {
            for (int i = 0; i < _answerButtons.Length; i++)
            {
                _answerButtons[i].interactable = false;
            }

            for (int i = 0; i < _answerButtonsAnimator.Length; i++)
            {
                _answerButtonsAnimator[i].GetComponent<Animator>().SetTrigger("Out");
            }
            _questionText.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            _timerText.gameObject.SetActive(false);
            _trueOrFalseIcon.sprite = _trueOrFalseIcons[1];
            _trueOrFalseText.text = "Время вышло, вы проиграли!";

            if (!_trueOrFalseIcon.gameObject.activeSelf)
            {
                _trueOrFalseIcon.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(1f);

            _trueOrFalseIcon.gameObject.SetActive(false);
            _headPanel.GetComponent<Animator>().SetTrigger("Out");
            StopCoroutine(lose);

            yield break;
        }
    }

    public void AnswersButtons(int index)
    {
        if (answersText[index].text.ToString() == currentQuestion.answers[0])
        {
            StartCoroutine(TrueOrFalseAnswer(true));
        }
        else
        {
            StartCoroutine(TrueOrFalseAnswer(false));
            _timerText.gameObject.SetActive(false);
        }
    }

    public void UpdateTimerText()
    {
        if (_timeLeft < 1)
        {
            _timeLeft = 0;
            lose = StartCoroutine(Lose(false));
        }

        float seconds = Mathf.FloorToInt(_timeLeft % 60);
        _timerText.text = seconds.ToString();
    }
}

[System.Serializable]
public class QuestionList
{
    public string question;
    public string[] answers = new string[4];
}
