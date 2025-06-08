using InputHandling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ragdoll
{

    public class CharacterController : PortalTraveller
    {
        [SerializeField] InputReader input;

        //---------------------------------
        [Header("Head Physics")] [SerializeField]
        ConstantForce headConstantForce;

        [Header("Foot Physics")] [SerializeField]
        float legForce;

        [SerializeField] float legCooldownTime = 0.2f; // Cooldown time between leg movements
        [SerializeField] Rigidbody rightFoot, leftFoot;
        [SerializeField] ConstantForce rightFootConstantForce, leftFootConstantForce;

        [Header("Arm Physics")] [SerializeField]
        float armAttackForce;

        [SerializeField] Rigidbody rightArm, leftArm;

        //---------------------------------
        [Header("Movement Settings")] [SerializeField]
        Vector3 bodyTorque;

        [SerializeField] Vector3 movementDirectionForce;
        [SerializeField] Rigidbody body;
        //---------------------------------

        Vector3 _movementInput;
        Vector3 _initialHeadForce, _initialLeftFootForce, _initialRightFootForce;

        float _nextLegMoveTime;

        bool _wasLastLegRight;
        bool _leftAttackTriggered;
        bool _rightAttackTriggered;
        
        StoreCharacterJoints _characterJoints;

        void Awake()
        {
            _initialHeadForce = headConstantForce.force; // Store the initial force
            _initialLeftFootForce = leftFootConstantForce.force; // Store the initial force
            _initialRightFootForce = rightFootConstantForce.force; // Store the initial force
            _characterJoints = GetComponent<StoreCharacterJoints>();
        }

        void Start()
        {
            // Initialize the input reader
            //TODO: Implement OnEnable and OnDisable methods to enable/disable input and unsubscribe from events (to avoid memory leaks)
            input.MoveEvent += direction => _movementInput = direction;
            input.JumpEvent += Jump;
            input.LeftArmAttackEvent += isLeftArmAttackPressed => { _leftAttackTriggered = isLeftArmAttackPressed; };
            input.RightArmAttackEvent += isRightArmAttackPressed =>
            {
                _rightAttackTriggered = isRightArmAttackPressed;
            };
            input.EnablePlayerActions();
        }

        void Update()
        {
            _movementInput = NormalizeInput(_movementInput);
        }

        void FixedUpdate()
        {
            Move(_movementInput);

            if (_leftAttackTriggered)
            {
                LeftArmAttack();
            }

            if (_rightAttackTriggered)
            {
                RightArmAttack();
            }
        }

        #region Teleport Logic
        public override void EnterPortalThreshold () {
            base.EnterPortalThreshold ();
            _characterJoints.ChangeCharacterActiveState(true);
        }

        // Called once no longer touching portal (excluding when teleporting)
        public override void ExitPortalThreshold () {
            base.ExitPortalThreshold ();
            _characterJoints.ChangeCharacterActiveState(false);
        }
        

        #endregion
        public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
            base.Teleport (fromPortal, toPortal, pos, rot);
            body.linearVelocity = toPortal.TransformVector (fromPortal.InverseTransformVector (body.linearVelocity));
            body.angularVelocity = toPortal.TransformVector (fromPortal.InverseTransformVector (body.angularVelocity));
        }

        #region Handling

        Vector3 NormalizeInput(Vector3 moveInput)
        {
            // Normalize the input to ensure consistent movement magnitude for rotation
            //TODO: add camera direction into calculation
            return moveInput.normalized;
        }

        //TODO: Refactor this to use the chain of responsibility pattern (too mitigate all the if statements).
        void Move(Vector3 moveInputNormalized)
        {
            if (Time.time >= _nextLegMoveTime) // Check if cooldown has elapsed
            {
                if (moveInputNormalized.y != 0) // Forward or backward movement
                {
                    int directionMultiplier = moveInputNormalized.y > 0 ? 1 : -1; // Determine direction
                    ApplyLegForce(directionMultiplier);
                }
            }

            if (moveInputNormalized.x == 0)
                return;

            // Rotate the body
            Vector3 rotation;
            if (moveInputNormalized.x > 0)
                rotation = bodyTorque;
            else
                rotation = -bodyTorque;
            body.transform.eulerAngles += rotation;
        }

        void ApplyLegForce(int directionMultiplier)
        {
            var calculatedLegForce = movementDirectionForce * (legForce * directionMultiplier);

            if (_wasLastLegRight)
            {
                rightFoot.AddRelativeForce(calculatedLegForce, ForceMode.Impulse);
            }
            else
            {
                leftFoot.AddRelativeForce(calculatedLegForce, ForceMode.Impulse);
            }

            _wasLastLegRight = !_wasLastLegRight; // Toggle the leg
            _nextLegMoveTime = Time.time + legCooldownTime; // Reset cooldown
        }

        void Jump(bool isJumpPressed)
        {
            if (isJumpPressed)
            {
                headConstantForce.force = Vector3.zero;
                leftFootConstantForce.force = Vector3.zero;
                rightFootConstantForce.force = Vector3.zero;
            }
            else
            {
                // On Jump Released: Restore initial force
                headConstantForce.force = _initialHeadForce; // Restore the initial force
                leftFootConstantForce.force = _initialLeftFootForce; // Restore the initial force
                rightFootConstantForce.force = _initialRightFootForce; // Restore the initial force
            }
        }

        void RightArmAttack()
        {
            var attack = Vector3.forward * Random.value;
            rightArm.AddRelativeForce(attack * armAttackForce, ForceMode.Impulse);
        }

        void LeftArmAttack()
        {
            var attack = Vector3.forward * Random.value;
            leftArm.AddRelativeForce(attack * armAttackForce, ForceMode.Impulse);
        }

        #endregion

    }

}