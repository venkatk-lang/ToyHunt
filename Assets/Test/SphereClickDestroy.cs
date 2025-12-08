using UnityEngine;

public class SphereClickDestroy : MonoBehaviour
{
    private SphereSpawner spawner;

    public void Init(SphereSpawner s) => spawner = s;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    spawner.RemoveSphere(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}