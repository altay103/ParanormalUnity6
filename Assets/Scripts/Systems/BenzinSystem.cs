using UnityEngine;

public class BenzinSystem : MonoBehaviour
{
    [SerializeField] private Transform benzinHandPosPoint;

    public void BenzinYerlestir(GameObject benzinObj)
    {
        benzinObj.transform.SetParent(benzinHandPosPoint);
        benzinObj.transform.localPosition = Vector3.zero;
        benzinObj.transform.localRotation = Quaternion.Euler(-170.034f, 41.09599f, -263.656f);

        benzinObj.layer = LayerMask.NameToLayer("FPS1ARMS");
        benzinObj.SetActive(true);
    }
}
