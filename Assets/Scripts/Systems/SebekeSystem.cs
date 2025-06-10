using UnityEngine;

public class SebekeSystem : MonoBehaviour
{
    [SerializeField] private Transform sebekeHandPosPoint;

    public void SebekeYerlestir(GameObject sebekeobj)
    {
        sebekeobj.transform.SetParent(sebekeHandPosPoint.transform);
        sebekeobj.transform.position = sebekeHandPosPoint.position;
        sebekeobj.transform.localScale = new Vector3(2.2905f, 2.2905f, 2.2905f);
        sebekeobj.transform.localRotation = Quaternion.Euler(180f, 180f, 0f);
        
        gameObject.layer = LayerMask.NameToLayer("FPS1ARMS");
        sebekeobj.SetActive(true);
    }
}
