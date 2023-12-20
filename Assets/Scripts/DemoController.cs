using UnityEngine;

public class DemoController : MonoBehaviour
{
    private MetalonController metalonController;

    void Start()
    {
        metalonController = GetComponent<MetalonController>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            metalonController.ChangeState(MetalonState.Roaming);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            metalonController.ChangeState(MetalonState.Attack);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            metalonController.ChangeState(MetalonState.Defend);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            metalonController.ChangeState(MetalonState.Flee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            metalonController.ChangeState(MetalonState.Die);
        }
    }
}
