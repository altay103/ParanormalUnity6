using UnityEngine;

namespace FuseboxSystem
{
    public class FBRaycast : MonoBehaviour
    {
        [Header("Raycast Distance")]
        [SerializeField] private int rayLength = 5;
        private FBItem raycastedObj;
        private Camera _camera;

        private const string interactiveObjectTag = "InteractiveObject";

        private void Awake()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            RaycastHit hit;
            if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), transform.forward, out hit, rayLength))
            {
                if (hit.collider.CompareTag(interactiveObjectTag))
                {
                    var selectedItem = hit.collider.GetComponent<FBItem>();
                    if (selectedItem != null)
                    {
                        raycastedObj = selectedItem;
                        CrosshairChange(true);
                    }
                    else
                    {
                        ClearExaminable();
                    }
                }
                else
                {
                    ClearExaminable();
                }

                if (raycastedObj != null)
                {
                    if (Input.GetKeyDown(FBInputManager.instance.interactKey))
                    {
                        raycastedObj.ObjectInteract();
                    }
                }
            }
        }
        private void ClearExaminable()
        {
            if (raycastedObj != null)
            {
                CrosshairChange(false);
                raycastedObj = null;
            }
        }

        void CrosshairChange(bool on)
        {
            FBUIManager.instance.CrosshairChange(on);
        }
    }
}
