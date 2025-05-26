using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CreditsRoll : MonoBehaviour
{
    #region Global Attributes
    public float scrollSpeed = 75.0f;

    public UnityEvent creditsClose;
    public bool areCreditsFinished = false;
    public float currentScrollPosition = 0f;
    public float scrollDistance = 5000f; //TODO, make it auto

    public float timeBeforeTransition = 1f;

    private RectTransform rectTransform;

    #endregion

    #region Methods
    private void Start()
    {
        areCreditsFinished = false;
        //Invoke("End", 10);
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        //When credits are finished, just get it over with
        if (areCreditsFinished) return;


        currentScrollPosition += scrollSpeed * Time.deltaTime;
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        if(currentScrollPosition > scrollDistance)
        {
            areCreditsFinished = true;
            Debug.Log("Fin");
            StartCoroutine(End());
        }

        //TODO Check if last child is visible
    }//EndOf Unity method Update

    IEnumerator End()
    {
        yield return new WaitForSeconds(timeBeforeTransition);

        //SceneManager.LoadScene("MainMenu");
        //gameObject.SetActive(true);
        creditsClose.Invoke();
    }

    #endregion
}
