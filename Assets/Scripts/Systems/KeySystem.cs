using UnityEngine;

public class KeySystem : MonoBehaviour
{
    [SerializeField] private Transform keyHandPosPoint;
    public string keyID;
    public void KeyYerlestir(GameObject keyobj)
    {
        keyobj.transform.SetParent(keyHandPosPoint.transform); 
        keyobj.transform.localPosition = Vector3.zero;
        keyobj.transform.localRotation = Quaternion.identity;
        keyobj.transform.localScale = new Vector3(1.008009f, 0.9503155f, 0.9528691f);

        gameObject.layer = LayerMask.NameToLayer("FPS1ARMS");
        keyobj.SetActive(true);
    }
}
