using UnityEngine;

namespace FuseboxSystem
{
    public class FBItem : MonoBehaviour
    {
        [Space(10)] [SerializeField] private ObjectType _objectType = ObjectType.None;

        private enum ObjectType { None, Fusebox, Fuse }

        [Header("Sound Effect Scriptables (Only for Fuse)")]
        [SerializeField] private Sound pickupSound;

        public void ObjectInteract()
        {
            if (_objectType == ObjectType.Fusebox)
            {
                GetComponent<FBController>().CheckFuseBox();
            }

            else if (_objectType == ObjectType.Fuse)
            {
                FBInventoryManager.instance.UpdateFuseUI();
                FBAudioManager.instance.Play(pickupSound.name);
                gameObject.SetActive(false);
            }
        }
    }
}
