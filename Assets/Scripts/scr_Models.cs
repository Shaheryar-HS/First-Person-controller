using System;
using UnityEngine;

public static class scr_Models
{
    #region - Player -

    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone
    }

    [Serializable]
    public class playerSettingModels
    {
        [Header("View Settings")]
        public float viewXSensitivity;
        public float viewYSensitivity;

        public float AmingSensitivityEfector;

        public bool viewXInverted;
        public bool viewYInverted;

        [Header("Movement Setting")]
        public bool SprintingHold;
        public float MovementSmoothing;

        [Header("Movement - Running")]
        public float RunningForwardSpeed;
        public float RunningStafeSpeed;

        [Header("Movement - Walking")]
        public float walkingForwardSpeed;
        public float walkingBackwardSpeed;
        public float walkingStafeSpeed;

        [Header("Jumping")]
        public float jumpingHeight;
        public float jumpingFalloff;
        public float FallingSmoothing;

        [Header("Speed Effectors")]
        public float SpeedEffector = 1;
        public float CrouchSpeedEffector;
        public float ProneSpeedEffector;
        public float FallingSpeedEffector;
        public float AmingSpeedEffector;

        [Header("Is Grounded / Falling")]
        public float isGroundedRadius;
        public float isFallingSpeed;
    }


    [Serializable]
    public class CherecterStance
    {
        public float CameraHeight;
        public CapsuleCollider StanceCollider;
    }
    #endregion

    #region - Weapons -

    public enum WeaponFireType
    {
        SamiAuto,
        FullyAuto
    }

    [Serializable]
    public class WeaponSettingsModel
    {
        [Header("Weapon Sway")]
        public float SwayAmount;
        public float SwaySmoothing;
        public float SwayResetSmoothing;
        public float SwayClampX;
        public float SwayClampY;
        public bool SwayYInverted;
        public bool SwayXInverted;

        [Header("Weapon Movement Sway")]
        public float MovementSwayX;
        public float MovementSwayY;
        public float MovementSwaySmoothing;
        public bool MovementSwayYInverted;
        public bool MovementSwayXInverted;

    }


    #endregion

}
