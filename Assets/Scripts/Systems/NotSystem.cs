using UnityEngine;

public class NotSystem : MonoBehaviour
{
    [SerializeField] private Transform notHandPosPoint;

    public void NotYerlestir(GameObject notobj)
    {
        notobj.transform.position = notHandPosPoint.position;
        notobj.transform.rotation = notHandPosPoint.rotation;
        notobj.transform.SetParent(notHandPosPoint.transform);
        gameObject.layer = LayerMask.NameToLayer("FPS1ARMS");
        notobj.SetActive(true);
    }
}
