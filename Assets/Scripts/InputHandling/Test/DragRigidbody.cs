using System.Collections;
using UnityEngine;
namespace InputHandling.Test
{
    public class DragRigidbody : MonoBehaviour
    {
        public Rigidbody draggingBody => _springJoint?.connectedBody;

        const float Spring = 100.0f;
        const float Damper = 5.0f;
        const float Drag = 5.0f;
        const float AngularDrag = 2.0f;
        const float Distance = 0.1f;
        const bool AttachToCenterOfMass = false;

        [SerializeField] InputReader inputReader;

        SpringJoint _springJoint;
        bool _isInteracting;
        Vector2 _mousePosition;

        void Start()
        {
            inputReader.LookEvent += OnMousePosition;
            inputReader.InteractEvent += OnInteract;
            inputReader.EnablePlayerActions();
        }

        void OnDisable()
        {
            inputReader.LookEvent -= OnMousePosition;
            inputReader.InteractEvent -= OnInteract;
            inputReader.DisablePlayerActions();
        }

        void Update()
        {
            if (!_isInteracting)
            {
                return;
            }

            var mainCamera = FindCamera();

            // We need to actually hit an object
            var hit = new RaycastHit();
            var ray = mainCamera.ScreenPointToRay(_mousePosition);
            if (!Physics.Raycast(ray, out hit, 100, Physics.DefaultRaycastLayers))
            {
                return;
            }

            // We need to hit a rigidbody that is not kinematic
            if (!hit.rigidbody || hit.rigidbody.isKinematic) return;
            if (!_springJoint)
            {
                var gameObject = new GameObject("Rigidbody dragger");
                var body = gameObject.AddComponent<Rigidbody>();
                _springJoint = gameObject.AddComponent<SpringJoint>();
                body.isKinematic = true;
            }

            _springJoint.transform.position = hit.point;
            _springJoint.anchor = Vector3.zero;

            _springJoint.spring = Spring;
            _springJoint.damper = Damper;
            _springJoint.maxDistance = Distance;
            _springJoint.connectedBody = hit.rigidbody;

            StartCoroutine(nameof(DragObject), hit.distance);
        }

        void OnInteract(bool isInteracting)
        {
            _isInteracting = isInteracting;
        }

        void OnMousePosition(Vector2 mousePosition)
        {
            _mousePosition = mousePosition;
        }

        IEnumerator DragObject(float distance)
        {
            float oldDrag = _springJoint.connectedBody.linearDamping;
            float oldAngularDrag = _springJoint.connectedBody.angularDamping;
            _springJoint.connectedBody.linearDamping = Drag;
            _springJoint.connectedBody.angularDamping = AngularDrag;
            var mainCamera = FindCamera();
            while (_isInteracting)
            {
                var ray = mainCamera.ScreenPointToRay(_mousePosition);
                _springJoint.transform.position = ray.GetPoint(distance);
                yield return null;
            }

            if (!_springJoint.connectedBody) yield break;
            _springJoint.connectedBody.linearDamping = oldDrag;
            _springJoint.connectedBody.angularDamping = oldAngularDrag;
            _springJoint.connectedBody = null;
        }

        Camera FindCamera()
        {
            return GetComponent<Camera>() ? GetComponent<Camera>() : Camera.main;
        }
    }
}
