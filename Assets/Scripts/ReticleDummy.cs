using UnityEngine;

public class ReticleDummy : MonoBehaviour
{
    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = (Vector2) _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}
