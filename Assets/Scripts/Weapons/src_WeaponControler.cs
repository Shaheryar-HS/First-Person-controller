using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static scr_Models;

public class src_WeaponControler : MonoBehaviour
{
    private scr_CherectorController cherectorController;

    [Header("Refrences")]
    public Animator weaponAnimator;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Settings")]
    public WeaponSettingsModel weaponSettingsModel;

    bool isIntialized;

    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;

    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;

    Vector3 newWeaponMovementRotation;
    Vector3 newWeaponMovementRotationVelocity;

    Vector3 targetWeaponMovementRotation;
    Vector3 targetWeaponMovementRotationVelocity;

    private bool isGroundedTrigger;

    private float fallingDelay;

    [Header("Weapon Sway Breathing")]
    public Transform weaponSwayObject;

    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 600;
    public float swayLarpSpeed = 14;

    float swayTime;
    Vector3 swayPosition;

    [Header("Sights")]
    public Transform sightTarget;
    public float sightOffset;
    public float amingInTime;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;
    [HideInInspector]
    public bool isAmingIn;

    [Header("Refrences")]
    public float rateOfFire;
    public float currentFireRate;
    public List<WeaponFireType> allowerFireTypes;
    public WeaponFireType currentFireType;

    public bool isShooting;
    public float bulletForce = 20f;

    public ParticleSystem gunParticle;

    #region - Start/Update -

    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
        currentFireType = allowerFireTypes.First();
    }


    private void Update()
    {
        if (!isIntialized)
        {
            return;
        }

        CalculateWeaponRotation();
        SetWeaponAnimation();
        CalculateWeaponSway();
        CalculateAmingIn();
        CalculateShooting();
    }

    #endregion

    #region - Shooting -

    private void CalculateShooting()
    {
        if (isShooting)
        {
            Shoot();
            if (currentFireType == WeaponFireType.SamiAuto)
            {
                isShooting = false;
            }
        }
    }

    private void Shoot()
    {
        // // Play gunshot sound
        // // if (gunAudio != null)
        // // {
        // //     gunAudio.Play();
        // // }

        // // Instantiate bullet at fire point
        // GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        // // Apply forward force
        // Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // if (rb != null)
        // {
        //     rb.AddForce(bulletSpawn.forward * bulletForce);
        // }
        // // var bullet = Instantiate(bulletPrefab, bulletSpawn);


        //ObjectPooling concept
        GameObject bullet = ObjectPooler.Instance.GetPooledObject();

        if (bullet != null)
        {
            bullet.transform.position = bulletSpawn.position;
            bullet.transform.rotation = bulletSpawn.rotation;
            bullet.SetActive(true);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.linearVelocity = bulletSpawn.forward * bulletForce; // Reset velocity
            gunParticle.Play();
        }
    }

    #endregion

    #region - Intialize -
    public void Intitialize(scr_CherectorController CherectorController)
    {
        cherectorController = CherectorController;
        isIntialized = true;
    }
    #endregion

    #region - Aming In -

    private void CalculateAmingIn()
    {
        var targetPosition = transform.position;

        if (isAmingIn)
        {
            // weaponAnimator.SetBool("isAiming", true);
            targetPosition = cherectorController.Camera.transform.position + (weaponSwayObject.transform.position - sightTarget.position) + (cherectorController.Camera.transform.forward * sightOffset);
        }

        weaponSwayPosition = weaponSwayObject.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPosition, ref weaponSwayPositionVelocity, amingInTime);
        weaponSwayObject.transform.position = weaponSwayPosition + swayPosition;
    }

    #endregion

    #region - Jumping -
    public void TriggerJump()
    {
        isGroundedTrigger = false;
        weaponAnimator.SetTrigger("Jump");
    }

    #endregion

    #region - Rotation -

    private void CalculateWeaponRotation()
    {

        targetWeaponRotation.y += (isAmingIn ? weaponSettingsModel.SwayAmount / 3 : weaponSettingsModel.SwayAmount) * (weaponSettingsModel.SwayXInverted ? -cherectorController.Input_View.x : cherectorController.Input_View.x) * Time.deltaTime;
        targetWeaponRotation.x += (isAmingIn ? weaponSettingsModel.SwayAmount / 3 : weaponSettingsModel.SwayAmount) * (weaponSettingsModel.SwayYInverted ? cherectorController.Input_View.y : -cherectorController.Input_View.y) * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -weaponSettingsModel.SwayClampX, weaponSettingsModel.SwayClampX);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -weaponSettingsModel.SwayClampY, weaponSettingsModel.SwayClampY);
        targetWeaponRotation.z = isAmingIn ? 0 : targetWeaponRotation.y;

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, weaponSettingsModel.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, weaponSettingsModel.SwaySmoothing);



        targetWeaponMovementRotation.z = (isAmingIn ? weaponSettingsModel.MovementSwayX / 3 : weaponSettingsModel.MovementSwayX) * (weaponSettingsModel.MovementSwayXInverted ? -cherectorController.Input_Movemnt.x : cherectorController.Input_Movemnt.x);
        targetWeaponMovementRotation.x = (isAmingIn ? weaponSettingsModel.MovementSwayY / 3 : weaponSettingsModel.MovementSwayY) * (weaponSettingsModel.MovementSwayYInverted ? -cherectorController.Input_Movemnt.y : cherectorController.Input_Movemnt.y);

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, weaponSettingsModel.MovementSwaySmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, weaponSettingsModel.MovementSwaySmoothing);

        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }

    #endregion

    #region - Animation -
    private void SetWeaponAnimation()
    {

        if (isGroundedTrigger)
        {
            fallingDelay = 0f;
        }
        else
        {
            fallingDelay += Time.deltaTime;
        }

        if (cherectorController.isGrounded && !isGroundedTrigger && fallingDelay > 0.1f)
        {
            weaponAnimator.SetTrigger("Land");
            isGroundedTrigger = true;
        }
        else if (!cherectorController.isGrounded && isGroundedTrigger)
        {
            weaponAnimator.SetTrigger("Falling");
            isGroundedTrigger = false;
        }

        weaponAnimator.SetBool("isSprinting", cherectorController.isSprinting);
        weaponAnimator.SetFloat("WeaponAnimationSpeed", cherectorController.weaponAnimationSpeed);

    }

    #endregion

    #region - Sway -

    private void CalculateWeaponSway()
    {
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / (isAmingIn ? swayScale * 5 : swayScale);

        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLarpSpeed);

        swayTime += Time.deltaTime;
        if (swayTime > 6.3f)
        {
            swayTime = 0f;
        }

    }

    private Vector3 LissajousCurve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }

    #endregion
}
