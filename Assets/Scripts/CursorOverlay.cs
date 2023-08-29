using DG.Tweening;
using UnityEngine;

public class CursorOverlay : MonoBehaviour
{
    void OnEnable()
    {
        GetComponentInParent<CanvasGroup>().DOFade(0.0f, 1.0f);
    }
}
