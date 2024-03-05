using UnityEngine;

public class BillboardUI : BaseUI
{
    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
