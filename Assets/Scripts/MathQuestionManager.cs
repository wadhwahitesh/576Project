using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MathQuestionManager : MonoBehaviour
{
    public static MathQuestionManager Instance;

    public GameObject questionPanel;
    public GameObject livesPanel;
    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public TMP_Text feedbackText;

    private GameObject starObject;

    private void Awake()
    {
        Instance = this;
        questionPanel.SetActive(false);
    }

    public void ShowRandomQuestion(GameObject star)
    {
        
        starObject = star;
        questionPanel.SetActive(true);
        livesPanel.SetActive(false);

        string randomQuestion = GenerateRandomQuestion(2);
        questionText.text = randomQuestion;

        feedbackText.text = "";
    }

    public void CheckAnswer()
    {

        string playerAnswer = answerInput.text;

        bool isCorrect = ValidateAnswer(playerAnswer,questionText.text);
        if (isCorrect)
        {
            feedbackText.text = "Correct!";
            NewBehaviourScript.Instance.IncreaseLives();
        }
        else
        {
            feedbackText.text = "Incorrect!";
        }
        if (starObject != null)
        {
            Destroy(starObject);
        }

        Time.timeScale = 1f;
        Invoke("DeactivateQuestionPanel", 1.5f);
        answerInput.text = "";
    }

    void DeactivateQuestionPanel()
    {
        questionPanel.SetActive(false);
        livesPanel.SetActive(true);
    }

    bool ValidateAnswer(string answer, string question)
    {
        try
        {
            double result = EvaluateExpression(question);

            return Mathf.Approximately((float)result, float.Parse(answer));
        }
        catch
        {
            return false;
        }
    }


    string GenerateRandomQuestion(int difficultyLevel)
    {
        
        char[] operators = { '+', '-', '*' };

        int numTerms = 0;
        int maxNumber = 0;

        switch (difficultyLevel)
        {
            case 1: // Very Easy
                numTerms = Random.Range(2, 3);
                maxNumber = 5;
                break;
            case 2: // Easy
                numTerms = Random.Range(2, 4);
                maxNumber = 10;
                break;
            case 3: // Medium
                numTerms = Random.Range(3, 5);
                maxNumber = 20;
                break;
            case 4: // Hard
                numTerms = Random.Range(3, 5);
                maxNumber = 30;
                break;
            case 5: // Very Hard
                numTerms = Random.Range(4, 6);
                maxNumber = 40;
                break;
            default:
                Debug.LogError("Invalid difficulty level.");
                break;
        }

        string expression = BuildExpression(numTerms, maxNumber, operators);

        return expression;
    }

    string BuildExpression(int numTerms, int maxNumber, char[] operators)
    {
        if (numTerms == 1)
        {
            return Random.Range(1, maxNumber + 1).ToString();
        }

        
        string expression = "";
        int remainingTerms = numTerms;

        while (remainingTerms > 0)
        {
            
            if (expression.Length > 0)
            {
                expression += operators[Random.Range(0, operators.Length)];
            }

            
            int randomNumber = Random.Range(1, maxNumber + 1);
            expression += randomNumber.ToString();
            remainingTerms--;
        }

        return expression;
    }


    double EvaluateExpression(string expression)
    {
        return System.Convert.ToDouble(new System.Data.DataTable().Compute(expression, ""));
    }
}
