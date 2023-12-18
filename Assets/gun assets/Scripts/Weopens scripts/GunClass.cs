using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class GunClass : MonoBehaviour
{
    [Header("Input Type")]
    [SerializeField] bool mobileInput;
    [SerializeField] bool isAutomaticShooting;

    [Header("Gun Shooting Stats")]
    [SerializeField] int bulletDamage;
    [SerializeField] float timeBetweenInputShooting;
    [SerializeField] float gunBulletRange;
    [SerializeField] float bulletSpread;
    [SerializeField] public int GunMagazineSize;

    [Header("Gun Reloading Stats")]
    [SerializeField] bool allowedToReload;
    [SerializeField] float timeForReload;
    [SerializeField] bool isAutomaticReloading;

    [Header("Multiple Bullet Shooting Stats")]
    [SerializeField] bool allowToShootMultipleBulletsAtOnce;
    [SerializeField] float timeBetweenEachBulletShot;
    [SerializeField] int bulletsToShootPerClick;


    [Header("Debug Stats, DO NOT CHNAGE")]
    [SerializeField] public bool allowInputToShoot;
    [SerializeField] bool isReloading;
    [SerializeField] int bulletsLeftInMagazine;
    [SerializeField] string nameOfObjectHit;
    [SerializeField] int bulletsShotSoFar;
    Ray bulletRay;
    RaycastHit objectHit;

    [Header("References")]
    [SerializeField] Animator gunAnimator;


    [Header("Animations")]
    [SerializeField] AnimationClip shootAnimation;

    [Header("Weapon effects")]
    [SerializeField] GameObject gunCrossAir;
    [SerializeField] ParticleSystem gunMuzzleFlash;
    [SerializeField] ParticleSystem bulletImpactVfx;
    [SerializeField] GameObject furnitureBulletImpactDecal;
    [SerializeField] float effectDestroyTimer;

    [Header("Gun Sounds")]
    [SerializeField] AudioSource shootSound;
    [SerializeField] AudioSource reloadSound;
    [SerializeField] AudioSource denyShootSound;

    public static Action<int> currentGunAmmoHasBeenChanged;
    public static Action<int, int> BulletHasBeenShot;

    public int CurrentAmmo
    {
        get { return bulletsLeftInMagazine; }

        set
        {
            bulletsLeftInMagazine = value;
            currentGunAmmoHasBeenChanged?.Invoke(bulletsLeftInMagazine);
        }
    }
    public bool AllowInputToShoot
    {
        get
        {
            return allowInputToShoot;
        }
        set
        {
            allowInputToShoot = value;
            // cross air is disabled if shooting input is false else it will be enabled
            if (allowInputToShoot && gunCrossAir != null) { gunCrossAir?.SetActive(true); }
            else { gunCrossAir?.SetActive(false); }
        }
    }

    private void OnEnable()
    {
        GameManager.GameIsInPlayMode += EnableInputToShoot;
        GameManager.GameIsOver += EnableInputToShoot;
    }

    private void OnDisable()
    {
        GameManager.GameIsInPlayMode -= EnableInputToShoot;
        GameManager.GameIsOver -= EnableInputToShoot;
    }


    private void Start()
    {
        ReloadAmmoToGunMagazine();
        BulletHasBeenShot?.Invoke(bulletsLeftInMagazine, GunMagazineSize);
        allowInputToShoot = false;
    }
    private void Update()
    {
        CheckWhetherToReloadAmmo();
        DisableShootingWhenAmmoZero();
        CheckGunInputToStartShooting();
    }

    void CheckGunInputToStartShooting()
    {
        bool _shootingInput = ReceiveGunShootingInputType();

        //  check if gun is allowed to shoot and if it has ammo and if it is not reloading, then start shooting
        if (_shootingInput && allowInputToShoot && bulletsLeftInMagazine > 0 && !isReloading)
        {
            StartShootingProcess();
            PlayGunAnimation(shootAnimation);
            PlayGunVfx(gunMuzzleFlash);
            PlayAGunSoundAndChangePitch(shootSound, 0.75f, 1.3f);
        }
        WhenAmmoIsZeroDontAllowShooting();
    }

    private bool ReceiveGunShootingInputType()
    {
        bool _shootingInput = false;

        if (Input.touchCount > 0 && mobileInput)
        {
            // checking for touch count and if the player is tapping on the screen
            Touch _touch = Input.GetTouch(0);
            _shootingInput = _touch.phase == TouchPhase.Began;
        }
        else
        {
            // if not mobile then use pc input, check if it is automatic shooting or not
            _shootingInput = isAutomaticShooting ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);
        }
        return _shootingInput;
    }

    void WhenAmmoIsZeroDontAllowShooting()
    {
        // if there uis no ammo or not allowed to shoot then play a sound to notigy player that you have no ammo 

        if (Input.GetKeyDown(KeyCode.Mouse0) && !allowInputToShoot && bulletsLeftInMagazine <= 0)
        {
            PlayAGunSoundAndChangePitch(denyShootSound, 0.9f, 1.2f);
        }
    }

    void StartShootingProcess()
    {
        bulletsShotSoFar = allowToShootMultipleBulletsAtOnce ? bulletsToShootPerClick : 0; // to shoot multiple bullets at once ,  like shot gun or burst rifles
        allowInputToShoot = false;
        shootBullet();
        Invoke(nameof(EnableInputToShoot), timeBetweenInputShooting); // to make sure player can shoot again after am intervel of time 
    }
    void shootBullet()// this is where you would replace raycast with your own bullet prefab
    {
        CurrentAmmo--;
        ShootRaycastAndCauseDamage();

        currentGunAmmoHasBeenChanged?.Invoke(bulletsLeftInMagazine);
        BulletHasBeenShot?.Invoke(bulletsLeftInMagazine, GunMagazineSize);

        ShootMultipleBulletsAtOnceIfRequired();
        // bullets effects that spawn only after bullets are shot
        if (furnitureBulletImpactDecal == null) return;
        SpawnGunEffectAtRayNormalThendestroy(furnitureBulletImpactDecal, effectDestroyTimer, true);
        if (bulletImpactVfx == null) return;
        SpawnGunEffectAtRayNormalThendestroy(bulletImpactVfx.gameObject, effectDestroyTimer, false);
    }
    void ShootMultipleBulletsAtOnceIfRequired()
    {
        if (!allowToShootMultipleBulletsAtOnce) return;
        bulletsShotSoFar -= 1;
        // check to make sure it does not  keep shooting multiple bullets at once
        if (bulletsShotSoFar <= 0 || bulletsLeftInMagazine <= 0) return;
        // each bullet is shot after a short time interval, only for multiple bullets shooting
        Invoke(nameof(shootBullet), timeBetweenEachBulletShot);
    }

    void ShootRaycastAndCauseDamage()
    {
        Vector3 perShotBulletSpread = GetBulletSpreadDirection(); // bullet bloom, this prevents 100% accuracy iof each shot
        Vector3 bulletDirection = Camera.main.transform.forward + perShotBulletSpread;
        bulletRay = new Ray(Camera.main.transform.position, bulletDirection);

        if (Physics.Raycast(bulletRay, out objectHit, gunBulletRange))
        {
            nameOfObjectHit = objectHit.collider.name;
            InflictDamage(objectHit);
            Debug.DrawRay(Camera.main.transform.position, bulletDirection * gunBulletRange, Color.green, 10f);
        }
    }

    void InflictDamage(RaycastHit hit)
    {
        if (!hit.collider.TryGetComponent(out IDamagable damagable)) return;
        damagable?.TakeDamage(bulletDamage);
    }

    Vector3 GetBulletSpreadDirection()
    {
        float xSpread = Random.Range(-bulletSpread, bulletSpread);
        float ySpread = Random.Range(-bulletSpread, bulletSpread);
        return new Vector3(xSpread, ySpread);
    }

    void EnableInputToShoot()
    {
        // this makes sure if all bullets not shot then dont allow player to shoot, this prevents losing more bullets than required
        if (bulletsShotSoFar < 5 && bulletsShotSoFar != 0) // mainly for multiple bullets shooting
        {
            // this makes sure that you can shoot again if all bullets are shot
            Invoke(nameof(EnableInputToShoot), timeBetweenInputShooting);
            return;
        }
        allowInputToShoot = true;
    }

    void EnableInputToShoot(bool enable)
    {
        allowInputToShoot = enable;
    }

    private void CheckWhetherToReloadAmmo()
    {
        if (!allowedToReload) return;

        // whether to reload manually or automatically
        bool reloadingInput = isAutomaticReloading ? true : Input.GetKeyDown(KeyCode.R);

        if (reloadingInput && bulletsLeftInMagazine <= 0 && !isReloading)
        {
            StartReloadingProcess();
        }
    }

    public void StartReloadingProcess()
    {
        allowInputToShoot = false;
        isReloading = true;
        PlayAGunSoundAndChangePitch(reloadSound, 0.8f, 1.4f);
        Invoke(nameof(ReloadAmmoToGunMagazine), timeForReload);
    }

    void ReloadAmmoToGunMagazine()
    {
        allowInputToShoot = true;
        isReloading = false;
        CurrentAmmo = GunMagazineSize;

        currentGunAmmoHasBeenChanged?.Invoke(bulletsLeftInMagazine);
        BulletHasBeenShot?.Invoke(bulletsLeftInMagazine, GunMagazineSize);
    }

    void DisableShootingWhenAmmoZero()
    {
        if (bulletsLeftInMagazine <= 0) allowInputToShoot = false;
    }

    // Helper functions for spawning , playing or destroying gun effects

    void PlayGunAnimation(AnimationClip clip)
    {
        if (gunAnimator == null || clip == null) return; // prevents null reference exception
        gunAnimator.Play(clip.name, -1, 0);
    }

    void PlayGunVfx(ParticleSystem gunVfx)
    {
        if (gunVfx == null) return; // prevents null reference exception
        gunVfx.Play();
    }

    void SpawnGunEffectAtRayNormalThendestroy(GameObject effect, float timer, bool parenting)
    {
        if (objectHit.collider == null || effect == null) return;

        var decalDuplicate = Instantiate(effect, objectHit.point, Quaternion.LookRotation(objectHit.normal)); // spawn effect at raycast hit point
        // i dont understand the math behind this but it works, offsetting decal from its current position to prevent cliping
        Vector3 decalOffset = decalDuplicate.transform.forward / 1000;
        decalDuplicate.transform.position += decalOffset;

        decalDuplicate.transform.parent = parenting ? objectHit.collider.transform : null;
        Destroy(decalDuplicate, timer);
    }

    void PlayAGunSoundAndChangePitch(AudioSource source, float minPitch, float maxPitch)
    {
        if (source == null || source.clip == null) return; // prevents null reference exception
        source.pitch = Random.Range(minPitch, maxPitch);
        source.Play();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunBulletRange);
    }
}
