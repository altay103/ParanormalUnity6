using UnityEngine;

public class FenerSystem : MonoBehaviour
{
    [SerializeField] private Transform fenerHandPosPoint;

    public void FenerYerlestir(GameObject fenerobj)
    {
        fenerobj.transform.position = fenerHandPosPoint.position;
        fenerobj.transform.rotation = fenerHandPosPoint.rotation;
        fenerobj.transform.SetParent(fenerHandPosPoint.transform);
        gameObject.layer = LayerMask.NameToLayer("FPS1ARMS");
        fenerobj.SetActive(true);
    }
}
