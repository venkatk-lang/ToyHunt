using UnityEngine;

public class InputManager : MonoBehaviour
{
    public LayerMask toyCellLayer;
    private Camera cam;
    private GameManager gameManager;
    private ToyCell lastHoveredCell;
    private void Awake()
    {
        cam = Camera.main;

    }

    public void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDestroy()
    {
        if(gameManager  != null)
        gameManager.OnStateChanged -= HandleStateChanged;
    }
    private void Update()
    {
        HandleHover();
        HandleClick();
    }
    void HandleHover()
    {
#if UNITY_STANDALONE || UNITY_EDITOR || PLATFORM_WEBGL
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0.1f, toyCellLayer);

        ToyCell hitCell = hit.collider ? hit.collider.GetComponent<ToyCell>() : null;

        // if pointer moved off cell
        if (lastHoveredCell != null && hitCell != lastHoveredCell)
        {
            lastHoveredCell.SetHover(false);
            lastHoveredCell = null;
        }

        // if hovering a new cell
        if (hitCell != null && hitCell != lastHoveredCell)
        {
    
            if (hitCell.Toy != null)  // only highlight non-empty
            {

                hitCell.SetHover(true);
                lastHoveredCell = hitCell;
            }
        }
#endif
    }
    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
            CheckHit(Input.mousePosition);

//#if UNITY_ANDROID || UNITY_IOS
//        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
//            CheckHit(Input.GetTouch(0).position);
//#endif
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

    //Refresh hover
    private void HandleStateChanged(GameState state)
    {
   
        if (state == GameState.WaitForPlayer)
            RefreshHoverImmediately();

    }
    public void RefreshHoverImmediately()
    {
#if UNITY_STANDALONE || UNITY_EDITOR || PLATFORM_WEBGL
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0.1f, toyCellLayer);

        ToyCell hitCell = hit.collider ? hit.collider.GetComponent<ToyCell>() : null;

        // Clear previous hover
        if (lastHoveredCell != null && lastHoveredCell != hitCell)
        {
            lastHoveredCell.SetHover(false);
            lastHoveredCell = null;
        }

        // Apply new hover highlight
        if (hitCell != null && hitCell.Toy != null)
        {
            hitCell.SetHover(true);
            lastHoveredCell = hitCell;
        }
#endif
    }
}
