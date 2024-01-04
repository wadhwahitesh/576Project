using System.Collections;
using UnityEngine;

public class StarCollision : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Please");
    }
    
    void OnTriggerEnter(Collider other)
    {

        
        Debug.Log("Check here");
        if (other.CompareTag("Claire"))
        {

            StartCoroutine(AnimateStar());
            Time.timeScale = 0f;
            Debug.Log("I have reached here");
            
        }
    }
    IEnumerator AnimateStar()
    {
        float duration = 1.0f; 
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;
        

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.unscaledDeltaTime; 
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Debug.Log("Star animation complete");
        

        if (MathQuestionManager.Instance != null)
        {
            Debug.Log("Showing question");
            MathQuestionManager.Instance.ShowRandomQuestion(gameObject);
        }
        else
        {
            Debug.LogWarning("MathQuestionManager.Instance is null");
        }
        
    }
}