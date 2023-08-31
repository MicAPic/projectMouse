using System.Collections;
using UnityEngine;

public class IntroManager : TextManager
{
    [Header("Intro")]
    [SerializeField]
    private GameObject[] introSlides;
    private int currentSlide;

    protected override IEnumerator WaitBeforeDisplayingText()
    {
        yield return new WaitForSeconds(0.625f);
        introSlides[currentSlide].SetActive(true);
        yield return new WaitForSeconds(initialDelay - 0.625f);
        ContinueStory();
    }

    protected override IEnumerator FinishDisplayingLine()
    {
        _currentDisplayLineCoroutine = null;
        
        isPlaying = false;
        isDisplayingRichText = false;
        if (canContinue)
        {
            canContinue = false;
            yield return new WaitForSecondsRealtime(autoModeWaitTime * 0.75f);
            currentSlide++;
            if (currentSlide < introSlides.Length)
            {
                introSlides[currentSlide].SetActive(true);
            }
            yield return new WaitForSecondsRealtime(autoModeWaitTime * 0.25f);
            ContinueStory();
        }
    }
}
