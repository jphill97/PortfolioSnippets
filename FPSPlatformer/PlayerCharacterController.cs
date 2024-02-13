using System;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Gameplay
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
    public class PlayerCharacterController : MonoBehaviour
    {
        #region Default Variables
        [Header("References")] [Tooltip("Reference to the main camera used for the player")]
        public Camera PlayerCamera;

        [Tooltip("Audio source for footsteps, jump, etc...")]
        public AudioSource AudioSource;

        [Header("General")] [Tooltip("Force applied downward when in the air")]
        public float GravityDownForce = 20f;

        [Tooltip("Physic layers checked to consider the player grounded")]
        public LayerMask GroundCheckLayers = -1;

        [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
        public float GroundCheckDistance = 0.05f;

        [Header("Movement")] [Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float MaxSpeedOnGround = 10f;

        [Header("Movement")]
        public float GroundFriction = 10f;

        [Tooltip(
            "Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float MovementSharpnessOnGround = 15;

        [Tooltip("Max movement speed when crouching")] [Range(0, 1)]
        public float MaxSpeedCrouchedRatio = 0.5f;

        [Tooltip("Max movement speed when not grounded")]
        public float MaxSpeedInAir = 10f;

        [Tooltip("Acceleration speed when in the air")]
        public float AccelerationSpeedInAir = 25f;

        [Tooltip("Acceleration speed when on the ground")]
        public float AccelerationSpeedOnGround = 25f;

        [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
        public float SprintSpeedModifier = 2f;

        [Tooltip("Height at which the player dies instantly when falling off the map")]
        public float KillHeight = -50f;

        [Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 200f;

        [Range(0.1f, 1f)] [Tooltip("Rotation speed multiplier when aiming")]
        public float AimingRotationMultiplier = 0.4f;

        [Header("Jump")] [Tooltip("Force applied upward when jumping")]
        public float JumpForce = 9f;

        [Header("Stance")] [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        public float CameraHeightRatio = 0.9f;

        [Tooltip("Height of character when standing")]
        public float CapsuleHeightStanding = 1.8f;

        [Tooltip("Height of character when crouching")]
        public float CapsuleHeightCrouching = 0.9f;

        [Tooltip("Speed of crouching transitions")]
        public float CrouchingSharpness = 10f;

        [Header("Audio")] [Tooltip("Amount of footstep sounds played when moving one meter")]
        public float FootstepSfxFrequency = 1f;

        [Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
        public float FootstepSfxFrequencyWhileSprinting = 1f;

        [Tooltip("Sound played for footsteps")]
        public AudioClip FootstepSfx;

        [Tooltip("Sound played when jumping")] public AudioClip JumpSfx;
        [Tooltip("Sound played when landing")] public AudioClip LandSfx;

        [Tooltip("Sound played when taking damage froma fall")]
        public AudioClip FallDamageSfx;

        [Header("Fall Damage")]
        [Tooltip("Whether the player will recieve damage when hitting the ground at high speed")]
        public bool RecievesFallDamage;

        [Tooltip("Minimun fall speed for recieving fall damage")]
        public float MinSpeedForFallDamage = 10f;

        [Tooltip("Fall speed for recieving th emaximum amount of fall damage")]
        public float MaxSpeedForFallDamage = 30f;

        [Tooltip("Damage recieved when falling at the mimimum speed")]
        public float FallDamageAtMinSpeed = 10f;

        [Tooltip("Damage recieved when falling at the maximum speed")]
        public float FallDamageAtMaxSpeed = 50f;

        public UnityAction<bool> OnStanceChanged;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }
        public bool HasJumpedThisFrame { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsCrouching { get; private set; }

        public float RotationMultiplier
        {
            get
            {
                if (m_WeaponsManager.IsAiming)
                {
                    return AimingRotationMultiplier;
                }

                return 1f;
            }
        }

        Health m_Health;
        PlayerInputHandler m_InputHandler;
        CharacterController m_Controller;
        PlayerWeaponsManager m_WeaponsManager;
        Actor m_Actor;
        Vector3 m_GroundNormal;
        Vector3 m_CharacterVelocity;
        Vector3 m_LatestImpactSpeed;
        float m_LastTimeJumped = 0f;
        float m_CameraVerticalAngle = 0f;
        float m_FootstepDistanceCounter;
        float m_TargetCharacterHeight;

        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;

        #endregion

        #region My Variables
        public Vector4 inputVec;
        public float grapplePullRate;
        public float knockbackModifier = .15f;

        Vector3 grapplePos;
        Vector3 grapplePivot = Vector3.zero;
        Vector3 prevPivot;

        float grappleLength;

        bool usePivot = false;
        bool isGrappling = false;
        Transform grappleMuzzle;

        Vector3 prevTargetVel = Vector3.zero;

        #endregion

        void Awake()
        {
            ActorsManager actorsManager = FindObjectOfType<ActorsManager>();
            if (actorsManager != null)
                actorsManager.SetPlayer(gameObject);
        }

        void Start()
        {
            // fetch components on the same gameObject
            m_Controller = GetComponent<CharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<CharacterController, PlayerCharacterController>(m_Controller,
                this, gameObject);

            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerCharacterController>(m_InputHandler,
                this, gameObject);

            m_WeaponsManager = GetComponent<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, PlayerCharacterController>(
                m_WeaponsManager, this, gameObject);

            m_Health = GetComponent<Health>();
            DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerCharacterController>(m_Health, this, gameObject);

            m_Actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, PlayerCharacterController>(m_Actor, this, gameObject);

            m_Controller.enableOverlapRecovery = true;

            m_Health.OnDie += OnDie;
            m_Health.onExplosion += ReceiveKnockback;

            // force the crouch state to false when starting
            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);
        }

        void Update()
        {
            // check for Y kill
            if (!IsDead && transform.position.y < KillHeight)
            {
                m_Health.Kill();
            }

            HasJumpedThisFrame = false;

            bool wasGrounded = IsGrounded;
            GroundCheck();

            // landing
            if (IsGrounded && !wasGrounded)
            {
                // Fall damage
                float fallSpeed = -Mathf.Min(CharacterVelocity.y, m_LatestImpactSpeed.y);
                float fallSpeedRatio = (fallSpeed - MinSpeedForFallDamage) /
                                       (MaxSpeedForFallDamage - MinSpeedForFallDamage);
                if (RecievesFallDamage && fallSpeedRatio > 0f)
                {
                    float dmgFromFall = Mathf.Lerp(FallDamageAtMinSpeed, FallDamageAtMaxSpeed, fallSpeedRatio);
                    m_Health.TakeDamage(dmgFromFall, null);

                    // fall damage SFX
                    AudioSource.PlayOneShot(FallDamageSfx);
                }
                else
                {
                    // land SFX
                    AudioSource.PlayOneShot(LandSfx);
                }
            }

            // crouching
            if (m_InputHandler.GetCrouchInputDown())
            {
                SetCrouchingState(!IsCrouching, false);
            }

            UpdateCharacterHeight(false);

            HandleCharacterMovement();
        }

        void OnDie()
        {
            IsDead = true;

            // Tell the weapons manager to switch to a non-existing weapon in order to lower the weapon
            m_WeaponsManager.SwitchToWeaponIndex(-1, true);

            EventManager.Broadcast(Events.PlayerDeathEvent);
        }

        void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            float chosenGroundCheckDistance =
                IsGrounded ? (m_Controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

            // reset values before the ground check
            IsGrounded = false;
            m_GroundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height),
                    m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    m_GroundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(m_GroundNormal))
                    {
                        IsGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > m_Controller.skinWidth)
                        {
                            m_Controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        void HandleCharacterMovement()
        {
            // horizontal character rotation
            {
                // rotate the transform with the input speed around its local Y axis
                transform.Rotate(
                    new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * RotationSpeed * RotationMultiplier),
                        0f), Space.Self);
            }

            // vertical camera rotation
            {
                // add vertical inputs to the camera's vertical angle
                m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed * RotationMultiplier;

                // limit the camera's vertical angle to min/max
                m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            }

            // character movement handling
            bool isSprinting = m_InputHandler.GetSprintInputHeld();
            {
                if (isSprinting)
                {
                    isSprinting = SetCrouchingState(false, false);
                }

                float speedModifier = isSprinting ? SprintSpeedModifier : 1f;

                // converts move input to a worldspace vector based on our character's transform orientation
                inputVec = m_InputHandler.GetMoveInput();

                Vector3 tempInp = new Vector3(inputVec.w - inputVec.y, 0, inputVec.x - inputVec.z );
                Vector3 worldspaceMoveInput = transform.TransformVector(tempInp);


                // handle grounded movement
                if (IsGrounded)
                {
                    // jumping
                    if (IsGrounded && m_InputHandler.GetJumpInputHeld())
                    {
                        // force the crouch state to false
                        if (SetCrouchingState(false, false))
                        {
                            // start by canceling out the vertical component of our velocity
                            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                            // then, add the jumpSpeed value upwards
                            CharacterVelocity += Vector3.up * JumpForce;

                            // play sound
                            AudioSource.PlayOneShot(JumpSfx);

                            // remember last time we jumped because we need to prevent snapping to ground for a short time
                            m_LastTimeJumped = Time.time;
                            HasJumpedThisFrame = true;

                            // Force grounding to false
                            IsGrounded = false;
                            m_GroundNormal = Vector3.up;
                        }
                    }
                    else
                    {
                        Friction();
                        WalkMove();
                    }

                    // footsteps sound
                    float chosenFootstepSfxFrequency =
                        (isSprinting ? FootstepSfxFrequencyWhileSprinting : FootstepSfxFrequency);
                    if (m_FootstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
                    {
                        m_FootstepDistanceCounter = 0f;
                        AudioSource.PlayOneShot(FootstepSfx);
                    }

                    // keep track of distance traveled for footsteps sound
                    m_FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
                }
                // handle air movement
                else
                {
                    AirMove();
                    CharacterVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
                }

                if (isGrappling)
                {
                    GrappleMove();
                }
            }

            


            // apply the final calculated velocity value as a character movement
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);

           /* if (isGrappling && Vector3.Distance((transform.position + CharacterVelocity), grapplePivot) > grappleLength)
            {
                Vector3 vertProj = Vector3.Project(CharacterVelocity, (grapplePivot - transform.position).normalized);
                CharacterVelocity -= vertProj;
            }*/


            m_Controller.Move(CharacterVelocity * Time.deltaTime);

            // detect obstructions to adjust velocity accordingly
            m_LatestImpactSpeed = Vector3.zero;
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius,
                CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
                QueryTriggerInteraction.Ignore))
            {
                // We remember the last impact speed because the fall damage logic might need it
                m_LatestImpactSpeed = CharacterVelocity;

                CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
            }

        }

        // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * m_Controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - m_Controller.radius));
        }

        // Gets a reoriented direction that is tangent to a given slope
        public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        void UpdateCharacterHeight(bool force)
        {
            // Update height instantly
            if (force)
            {
                m_Controller.height = m_TargetCharacterHeight;
                m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
                PlayerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
                m_Actor.AimPoint.transform.localPosition = m_Controller.center;
            }
            // Update smooth height
            else if (m_Controller.height != m_TargetCharacterHeight)
            {
                // resize the capsule and adjust camera position
                m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight,
                    CrouchingSharpness * Time.deltaTime);
                m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
                PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                    Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
                m_Actor.AimPoint.transform.localPosition = m_Controller.center;
            }
        }

        // returns false if there was an obstruction
        bool SetCrouchingState(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            if (crouched)
            {
                m_TargetCharacterHeight = CapsuleHeightCrouching;
            }
            else
            {
                // Detect obstructions
                if (!ignoreObstructions)
                {
                    Collider[] standingOverlaps = Physics.OverlapCapsule(
                        GetCapsuleBottomHemisphere(),
                        GetCapsuleTopHemisphere(CapsuleHeightStanding),
                        m_Controller.radius,
                        -1,
                        QueryTriggerInteraction.Ignore);
                    foreach (Collider c in standingOverlaps)
                    {
                        if (c != m_Controller)
                        {
                            return false;
                        }
                    }
                }

                m_TargetCharacterHeight = CapsuleHeightStanding;
            }

            if (OnStanceChanged != null)
            {
                OnStanceChanged.Invoke(crouched);
            }

            IsCrouching = crouched;
            return true;
        }

        void AirAccelerate(Vector3 wishdir, float wishspeed, float accel)
        {
            float addspeed, accelspeed, currentspeed, wishspd = wishspeed;

            // Cap speed
            //wishspd = VectorNormalize (pmove->wishveloc);

            if (wishspd > 1.1f)
                wishspd = 1.1f;
            // Determine veer amount
            currentspeed = Vector3.Dot(CharacterVelocity, wishdir);
            // See how much to add
            addspeed = wishspd - currentspeed;

            // If not adding any, done.
            if (addspeed <= 0)
                return;
            // Determine acceleration speed after acceleration

            accelspeed = accel * wishspeed * Time.deltaTime;
            // Cap it
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            

            CharacterVelocity += accelspeed * wishdir;
        }

        void GrappleMove()
        {
            float dist = Vector3.Distance(transform.position, grapplePivot);

            if (m_InputHandler.GetSprintInputHeld() && grappleLength > 0.5f)
            {
                grappleLength -= grapplePullRate * Time.deltaTime;
            }

            if (dist >= grappleLength)
            {
                Vector3 wishvel;
                Vector3 wishdir;
                float wishspeed;

                wishvel = CharacterVelocity;
                wishdir = wishvel.normalized;

                Vector3 grappleVec = grapplePivot - transform.position;
                Vector3 newDir = Vector3.RotateTowards(grappleVec.normalized, wishdir, Mathf.Deg2Rad * 90, 0f);

                CharacterVelocity = Vector3.RotateTowards(CharacterVelocity, newDir, Mathf.Deg2Rad * 180, 0.0f) + grappleVec.normalized * (dist - grappleLength); 
            }
        }

        void AirMove()
        {
            Vector3 wishvel;
            float fmove, smove;
            Vector3 wishdir;
            float wishspeed;

            inputVec = m_InputHandler.GetMoveInput();

            fmove = (MaxSpeedOnGround * 0.8f) * inputVec.x - (MaxSpeedOnGround * 0.8f) * inputVec.z;
            smove = (MaxSpeedOnGround * 0.8f) * inputVec.w - (MaxSpeedOnGround * 0.8f) * inputVec.y;

            Vector3 fwd = transform.forward;
            fwd.y = 0;

            Vector3 side = transform.right;
            side.y = 0;

            wishvel = fwd * fmove + side * smove;

            wishdir = wishvel.normalized;
            wishspeed = wishvel.magnitude;

            AirAccelerate(wishdir, wishspeed, AccelerationSpeedInAir);
        }

        void Accelerate(Vector3 wishdir, float wishspeed, float accel)
        {
            float addspeed, accelspeed, currentspeed;

            // See if we are changing direction a bit
            currentspeed = Vector3.Dot(CharacterVelocity, wishdir);

            // Reduce wishspeed by the amount of veer.
            addspeed = wishspeed - currentspeed;

            // If not going to add any speed, done.
            if (addspeed <= 0)
                return;

            // Determine amount of accleration.
            accelspeed = accel * Time.deltaTime * wishspeed * 1f;

            // Cap at addspeed
            if (accelspeed > addspeed)
                accelspeed = addspeed;

             CharacterVelocity += accelspeed * wishdir;
        }


        void WalkMove()
        {
            float forward;
            float side;
            float desiredSpeed;

            Vector3 desiredDirection;
            Vector3 desiredVelocity;

            inputVec = m_InputHandler.GetMoveInput();

            forward = (MaxSpeedOnGround * 0.8f) * inputVec.x - (MaxSpeedOnGround * 0.8f) * inputVec.z;
            side = (MaxSpeedOnGround * 0.8f) * inputVec.w - (MaxSpeedOnGround * 0.8f) * inputVec.y;

            Vector3 fwd = transform.forward;
            Vector3 sd = transform.right;

            fwd.y = 0;
            sd.y = 0;

            desiredVelocity = fwd * forward + side * sd;
            desiredVelocity.y = 0;

            desiredDirection = desiredVelocity;
            desiredSpeed = desiredDirection.magnitude;
            desiredDirection = desiredDirection.normalized;

            if (desiredSpeed > MaxSpeedOnGround)
            {
                desiredVelocity.x *= (MaxSpeedOnGround / desiredSpeed);
                desiredVelocity.y *= (MaxSpeedOnGround / desiredSpeed);
                desiredVelocity.z *= (MaxSpeedOnGround / desiredSpeed);

                desiredSpeed = MaxSpeedOnGround;
            }

            CharacterVelocity = new Vector3(CharacterVelocity.x, 0, CharacterVelocity.z);
            Accelerate(desiredDirection, desiredSpeed, 1.09375f * MaxSpeedOnGround);
            CharacterVelocity = new Vector3(CharacterVelocity.x, 0, CharacterVelocity.z);
        }

        void Friction()
        {
            Vector3 vel;
            float currentSpeed;
            float newSpeed;
            float control;
            float drop;
            Vector3 newVel;

            
            vel = CharacterVelocity;

            currentSpeed = vel.magnitude;

            if (currentSpeed < 0.001f)
            {
                return;
            }

            drop = 0;

            if (IsGrounded)  
            {
                
                if(currentSpeed < 1f)
                {
                    control = 1f;
                }
                else
                {
                    control = currentSpeed;
                }

                drop += control * GroundFriction * Time.deltaTime;
            }

           
            newSpeed = currentSpeed - drop;

            if (newSpeed < 0)
                newSpeed = 0;

            newSpeed /= currentSpeed;

            newVel = vel * newSpeed;

            CharacterVelocity = newVel;
        }

        public void StartGrapple(Vector3 GrapplePos, Transform muzzle)
        {
            grapplePos = GrapplePos;
            grapplePivot = grapplePos;
            grappleMuzzle = muzzle;
            grappleLength = (grapplePivot - transform.position).magnitude;
            isGrappling = true;
        }

        public void GrappleRelease()
        {
            isGrappling = false;
        }

        public void UpdateGrapplePoint(Vector3 newPoint)
        {
            grapplePivot = newPoint;
            grappleLength = (grapplePivot - transform.position).magnitude;
        }

        public void ReceiveKnockback(Vector3 knockbackVector, float damage,bool useModifier = true)
        {
            Debug.Log("Receive Knockback");

            Vector3 myVel = CharacterVelocity;
            if (useModifier)
            {
                if (Mathf.Sign(myVel.y) != Mathf.Sign(knockbackVector.y))
                {
                    myVel.y = knockbackVector.y + (knockbackVector.y * (CharacterVelocity.magnitude / 3) * knockbackModifier);
                }
                else
                {
                    myVel.y += knockbackVector.y + (knockbackVector.y * (CharacterVelocity.magnitude / 3) * knockbackModifier);
                }

                myVel.x += knockbackVector.x + (knockbackVector.x * (CharacterVelocity.magnitude / 3) * knockbackModifier);
                myVel.z += knockbackVector.z + (knockbackVector.z * (CharacterVelocity.magnitude / 3) * knockbackModifier);
            }
            else
            {
                if (Mathf.Sign(myVel.y) != Mathf.Sign(knockbackVector.y))
                {
                    myVel.y = 0;
                }

                myVel.y += knockbackVector.y;
                myVel.x += knockbackVector.x;
                myVel.z += knockbackVector.z;
            }
            

            CharacterVelocity = myVel;
            m_Health.TakeDamage(damage);
        }

    }
}