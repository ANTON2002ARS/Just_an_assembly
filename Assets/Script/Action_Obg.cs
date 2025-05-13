using UnityEngine;

public class Action_Obg : MonoBehaviour
{
    void OnMouseUpAsButton()
    {
        GameManager.gameManager.Ckick_Obg(this.gameObject);
    }
}
