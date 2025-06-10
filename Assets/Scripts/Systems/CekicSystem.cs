using UnityEngine;

public class CekicSystem : MonoBehaviour
{
    [SerializeField] private Transform cekicHandPosPoint;

    public void CekicYerlestir(GameObject cekicobj)
    {
        cekicobj.transform.SetParent(cekicHandPosPoint.transform);
        cekicobj.transform.localPosition = Vector3.zero;
        cekicobj.transform.localRotation = Quaternion.identity;
        cekicobj.transform.localScale = new Vector3(1.27662003f,1.825634f,1.41287196f);
        gameObject.layer = LayerMask.NameToLayer("FPS1ARMS");
        cekicobj.SetActive(true);
    }
}
