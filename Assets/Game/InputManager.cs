using UnityEngine;

public class InputManager : MonoBehaviour
{
    public LayerMask toyCellLayer;
    private Camera cam;
    private GameManager gameManager;

    private void Awake()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CheckHit(Input.mousePosition);

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            CheckHit(Input.GetTouch(0).position);
#endif
    }

    void CheckHit(Vector2 screenPos)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0.1f, toyCellLayer);

        if (hit.collider != null)
        {
            ToyCell cell = hit.collider.GetComponent<ToyCell>();
            if (cell != null)
            {
                gameManager.OnToyCellClicked(cell);
            }
        }
    }
}
