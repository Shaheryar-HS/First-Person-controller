using UnityEngine;
using static scr_Models;
using UnityEngine.InputSystem;
using Terresquall;

public class scr_CherectorController : MonoBehaviour
{


    public VirtualJoystick movementJoystick;  // Your virtual joystick for movement
    public VirtualJoystick viewJoystick;      // Your virtual joystick for camera look


    private CharacterController characterController;
    private DefaultInput defaultInput;

    [HideInInspector]
    public Vector2 Input_Movemnt;
    [HideInInspector]
    public Vector2 Input_View;

    private Vector3 newCameraRotation;
    private Vector3 newCherecterRotation;

    [Header("Refrences")]
    public Transform CameraHolder;
    public Transform Camera;
    public Transform feetTransform;

    [Header("Settings")]
    public playerSettingModels playerSettings;
    public float viewClampMin = -70;
    public float viewClampMax = 80;
    public LayerMask playerMask;
    public LayerMask groundMask;

    [Header("Gravity")]
    public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

    public Vector3 jumpingForce;
    public Vector3 jumpingForceVelocity;

    [Header("Stance")]
    public PlayerStance playerStance;
    public float playerStanceSmoothing;
    public CherecterStance playerStandStance;
    public CherecterStance playerCrouchStance;
    public CherecterStance playerProneStance;

    private float stanceCheckErrorMargin = 0.05f;

    private float camerHeight;
    private float camerHeightVelocity;

    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;

    [HideInInspector]
    public bool isSprinting;

    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;

    [Header("Weapon")]
    public src_WeaponControler currentWeapon;

    public float weaponAnimationSpeed;

    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isFalling;

    [Header("Leaning")]
    public Transform LeanPivot;
    private float currentLean;
    private float targetLean;
    public float leanAngle;
    public float leanSmoothing;
    private float leanVelocity;

    private bool isLeaningLeft;
    private bool isLeaningRight;

    [Header("Aming In")]
    public bool isAmingIn;

    //for spint-lock
    private bool isSprintLocked = false;
    private float sprintThreshold = 0.95f;

    #region - Awake -

    private void Awake()
    {
        defaultInput = new DefaultInput();

        defaultInput.Character.Movement.performed += e => Input_Movemnt = e.ReadValue<Vector2>();
        defaultInput.Character.View.performed += e => Input_View = e.ReadValue<Vector2>();
        defaultInput.Character.Jump.performed += e => Jump();
        defaultInput.Character.Crounh.performed += e => Crounh();
        defaultInput.Character.Proun.performed += e => Proun();
        defaultInput.Character.Sprint.performed += e => ToggleSprint();
        defaultInput.Character.SprintReliease.performed += e => StopSprint();

        defaultInput.Character.LeanRightPressed.performed += e => isLeaningRight = true;
        defaultInput.Character.LeanRightRelised.performed += e => isLeaningRight = false;

        defaultInput.Character.LeanLeftPressed.performed += e => isLeaningLeft = true;
        defaultInput.Character.LeanLeftRelised.performed += e => isLeaningLeft = false;

        defaultInput.Wepon.Fire2Pressed.performed += e => AmingInPressed();
        defaultInput.Wepon.Fire2Relised.performed += e => AmingInRelised();

        // Shooting through mouse clicks
        defaultInput.Wepon.Fire1Pressed.performed += e => ShootPressed();
        defaultInput.Wepon.Fire1Relised.performed += e => ShootRelised();

        defaultInput.Enable();

        newCameraRotation = CameraHolder.localRotation.eulerAngles;
        newCherecterRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        camerHeight = CameraHolder.localPosition.y;

        if (currentWeapon)
        {
            currentWeapon.Intitialize(this);
        }
    }

    #endregion

    #region - Update - 

    private void Update()
    {
        
        //Update Joystick Inputs

        // if (movementJoystick != null)
        // {
        //     if (!isSprintLocked)
        //     {
        //         Vector2 moveInput = movementJoystick.InputDirection;
        //         Input_Movemnt = moveInput;
        //         HandleJoystickSprinting();
        //     }
        // }

        // if (viewJoystick != null)
        // {
        //     Vector2 viewInput = viewJoystick.InputDirection;
        //     Input_View = viewInput;
        // }


        SetIsGrounder();
        SetIsFalling();
        Calculate_Movement();
        Calculate_View();
        Clculate_Jump();
        CalculateCameraStance();
        ClculateLeaning();
        CalculateAmingIn();


        double str = 10.52349f; ;
        float f = str.SafeCustomFloat();
    }

    #endregion

    #region - Shooting -

    private void ShootPressed()
    {
        if (currentWeapon)
        {
            currentWeapon.isShooting = true;
        }
    }
    private void ShootRelised()
    {
        if (currentWeapon)
        {
            currentWeapon.isShooting = false;
        }
    }

    #endregion

    #region - Aming In -

    private void AmingInPressed()
    {
        isAmingIn = true;
    }

    private void AmingInRelised()
    {
        isAmingIn = false;
    }

    private void CalculateAmingIn()
    {
        if (!currentWeapon)
        {
            return;
        }

        currentWeapon.isAmingIn = isAmingIn;
    }

    #endregion

    #region - isFalling / isGrounder -

    private void SetIsGrounder()
    {
        isGrounded = Physics.CheckSphere(feetTransform.position, playerSettings.isGroundedRadius, groundMask);
    }

    private void SetIsFalling()
    {
        isFalling = (!isGrounded && characterController.velocity.magnitude > playerSettings.isFallingSpeed);
    }

    #endregion

    #region - View/Movement -

    private void Calculate_Movement()
    {
        if (Input_Movemnt.y <= 0.2f)
        {
            isSprinting = false;
        }

        var verticalSpeed = playerSettings.walkingForwardSpeed;
        var horizontalSpeed = playerSettings.walkingStafeSpeed;

        if (isSprinting)
        {
            verticalSpeed = playerSettings.RunningForwardSpeed;
            horizontalSpeed = playerSettings.RunningStafeSpeed;
        }

        //Effectors

        if (!isGrounded)
        {
            playerSettings.SpeedEffector = playerSettings.FallingSpeedEffector;
        }
        else if (playerStance == PlayerStance.Crouch)
        {
            playerSettings.SpeedEffector = playerSettings.CrouchSpeedEffector;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            playerSettings.SpeedEffector = playerSettings.ProneSpeedEffector;
        }
        else if (isAmingIn)
        {
            playerSettings.SpeedEffector = playerSettings.AmingSpeedEffector;
        }
        else
        {
            playerSettings.SpeedEffector = 1;
        }


        if (isAmingIn)
        {
            weaponAnimationSpeed = 0;
        }
        else
        {
            weaponAnimationSpeed = characterController.velocity.magnitude / (playerSettings.walkingForwardSpeed * playerSettings.SpeedEffector);
        }

        if (weaponAnimationSpeed > 1)
        {
            weaponAnimationSpeed = 1;
        }

        verticalSpeed *= playerSettings.SpeedEffector;
        horizontalSpeed *= playerSettings.SpeedEffector;


        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * Input_Movemnt.x * Time.deltaTime, 0, verticalSpeed * Input_Movemnt.y * Time.deltaTime), ref newMovementSpeedVelocity, isGrounded ? playerSettings.MovementSmoothing : playerSettings.FallingSmoothing);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);

        if (playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        if (playerGravity < -0.1f && isGrounded)
        {
            playerGravity = -0.1f;
        }

        movementSpeed.y += playerGravity;
        movementSpeed += jumpingForce * Time.deltaTime;

        characterController.Move(movementSpeed);
    }

    private void Calculate_View()
    {
        // Accumulate horizontal rotation (yaw)
        newCherecterRotation.y += (isAmingIn ? playerSettings.viewXSensitivity * playerSettings.AmingSensitivityEfector : playerSettings.viewXSensitivity) *
                                  (playerSettings.viewXInverted ? -Input_View.x : Input_View.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, newCherecterRotation.y, 0);

        // Accumulate vertical rotation (pitch)

        newCameraRotation.x += (isAmingIn ? playerSettings.viewYSensitivity * playerSettings.AmingSensitivityEfector : playerSettings.viewYSensitivity) *
                               (playerSettings.viewYInverted ? Input_View.y : -Input_View.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, viewClampMin, viewClampMax);
        CameraHolder.localRotation = Quaternion.Euler(newCameraRotation.x, 0, 0);
    }

    #endregion

    #region - Leaning -

    private void LeanRight()
    {

    }
    private void LeanLeft()
    {

    }

    private void ClculateLeaning()
    {
        if (isLeaningLeft)
        {
            targetLean = leanAngle;
        }
        else if (isLeaningRight)
        {
            targetLean = -leanAngle;
        }
        else
        {
            targetLean = 0;
        }
        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);

        LeanPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, currentLean));
    }

    #endregion

    #region - Jump -

    private void Clculate_Jump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playerSettings.jumpingFalloff);
    }

    private void Jump()
    {
        if (!isGrounded || playerStance == PlayerStance.Prone)
        {
            return;
        }
        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }
            playerStance = PlayerStance.Stand;
            return;
        }

        //jump
        jumpingForce = Vector3.up * playerSettings.jumpingHeight;
        playerGravity = 0;
        currentWeapon.TriggerJump();
    }

    #endregion

    #region - Stance -

    private void CalculateCameraStance()
    {
        var currentStance = playerStandStance;

        if (playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }

        camerHeight = Mathf.SmoothDamp(CameraHolder.localPosition.y, currentStance.CameraHeight, ref camerHeightVelocity, playerStanceSmoothing);
        CameraHolder.localPosition = new Vector3(CameraHolder.localPosition.x, camerHeight, CameraHolder.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }

    private void Crounh()
    {
        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }
            playerStance = PlayerStance.Stand;
            return;
        }

        if (StanceCheck(playerCrouchStance.StanceCollider.height))
        {
            return;
        }

        playerStance = PlayerStance.Crouch;
    }

    private void Proun()
    {
        playerStance = PlayerStance.Prone;
    }

    private bool StanceCheck(float stanceCheckHeight)
    {
        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y - characterController.radius - stanceCheckErrorMargin + stanceCheckHeight, feetTransform.position.z);



        return Physics.CheckCapsule(start, end, characterController.radius, playerMask);
    }

    #endregion

    #region - Sprinting -

    private void ToggleSprint()
    {
        if (Input_Movemnt.y <= 0.2f)
        {
            isSprinting = false;
            return;
        }
        isSprinting = !isSprinting;
    }

    private void StopSprint()
    {
        if (playerSettings.SprintingHold)
        {
            isSprinting = false;
        }
    }

    #endregion

    #region - Gizmos -

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetTransform.position, playerSettings.isGroundedRadius);
    }

    #endregion



    public void OnJumpButton() => Jump();
    public void OnCrouchButton() => Crounh();
    public void OnProneButton() => Proun();
    public void OnSprintButton(bool sprinting)
    {
        if (sprinting) ToggleSprint();
        else StopSprint();
    }
    public void OnLeanRight(bool leaning) => isLeaningRight = leaning;
    public void OnLeanLeft(bool leaning) => isLeaningLeft = leaning;
    public void OnAim(bool aiming)
    {
        if (aiming) AmingInPressed();
        else AmingInRelised();
    }
    public void OnFire(bool firing)
    {
        if (firing) ShootPressed();
        else ShootRelised();
    }
    private void HandleJoystickSprinting()
    {
        if (!isGrounded || playerStance != PlayerStance.Stand)
        {
            isSprinting = false;
            isSprintLocked = false;
            return;
        }

        if (Input_Movemnt.y >= sprintThreshold)
        {
            isSprinting = true;
            isSprintLocked = true;  // Lock sprint when fully forward
            movementJoystick.Lock(); // <<< Freeze joystick here
        }
        else
        {
            isSprinting = false;
        }
    }
    public void OnJoystickClicked()
    {
        if (isSprintLocked)
        {
            isSprintLocked = false;
            isSprinting = false;
            movementJoystick.Unlock(); // <<< Unfreeze joystick
        }
    }


}
