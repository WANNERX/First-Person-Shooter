using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public const string SHOOT = "Shoot";
    public const string RELOAD = "Reload";

    AudioSource audioSource;
    Animator weaponAnimator;

    public Transform muzzle;
    public WeaponData weaponData;

    public PlayerController myController { get; set; }
    public int ammoCount { get; private set; }

    bool reloading = false;
    bool shooting = false;
    bool readyToShoot = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        weaponAnimator = GetComponentInChildren<Animator>();

        // ammoCount = weaponData.maxAmmo;
        //UserInterface.Singleton.UpdateBulletCounter(ammoCount, weaponData.maxAmmo);
        if (PlayerPrefs.HasKey("SavedAmmo"))
        {
            ammoCount = PlayerPrefs.GetInt("SavedAmmo");
            PlayerPrefs.DeleteKey("SavedAmmo");
            PlayerPrefs.Save();
        }
        else
        {
            ammoCount = weaponData.maxAmmo;
        }
        UserInterface.Singleton.UpdateBulletCounter(ammoCount, weaponData.maxAmmo);
    }

    public void Shoot()
    {
        if(!readyToShoot || shooting || reloading || weaponData == null) return;

        if(ammoCount <= 0) { Reload(); return; }

        readyToShoot = false;
        shooting = true;
        
        UseAmmo();

        Invoke(nameof(ResetAttack), weaponData.fireRate);
        AttackRaycast();

        Instantiate(weaponData.fireEffect, muzzle);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(weaponData.fireSound);
    
        weaponAnimator.Play(SHOOT);
        myController.PlayAnimation(SHOOT);
    }

    void UseAmmo()
    {
        ammoCount--;
        UserInterface.Singleton.UpdateBulletCounter(ammoCount, weaponData.maxAmmo);
    }

    public void Reload()
    {
        if(ammoCount == weaponData.maxAmmo || reloading) return;

        reloading = true;

        myController.PlayAnimation(RELOAD);
        weaponAnimator.Play(RELOAD);
        Invoke(nameof(ResetReload), weaponData.reloadTime);
    }

    void ResetReload()
    { 
        reloading = false;
        ammoCount = weaponData.maxAmmo;

        UserInterface.Singleton.UpdateBulletCounter(ammoCount, weaponData.maxAmmo);
    }

    void ResetAttack()
    {
        shooting = false;
        readyToShoot = true;
    }

    void AttackRaycast()
    {
        RaycastHit hit;
        RaycastHit[] hits;

        switch (weaponData.type)
        {
            case WeaponType.Bullet:
                if(Physics.Raycast(myController.cam.transform.position, myController.cam.transform.forward, out hit, weaponData.weaponRange))
                { HitTarget(hit); }
                break;
            case WeaponType.Piercing:
                hits = Physics.RaycastAll(myController.cam.transform.position, myController.cam.transform.forward, weaponData.weaponRange);

                if(hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    { HitTarget(hits[i]); }
                }
                break;
                case WeaponType.Explosive:
                    if(Physics.Raycast(myController.cam.transform.position, myController.cam.transform.forward, out hit, weaponData.weaponRange))
                    { HitTarget(hit); }
                break;
        }
    }

    void HitTarget(RaycastHit hit)
    {
        Actor target = hit.transform.GetComponent<Actor>();
        if (target == null && hit.transform.parent != null)
            hit.transform.parent.TryGetComponent(out target);

        if (target != null)
        {
            if (hit.transform.CompareTag("Head"))
                target.TakeDamage(Mathf.RoundToInt(weaponData.attackDamage * weaponData.headshotMultiplier));
            else
            { 
                target.TakeDamage(weaponData.attackDamage);
            }
        }

        if (weaponData.explosive)
        {
            Explode(hit.point);
        }
        else
        {
            GameObject GO = Instantiate(weaponData.hitEffect, hit.point, Quaternion.identity);
            Destroy(GO, 4);
        }
    }

    void Explode(Vector3 point)
{
    Collider[] colliders = Physics.OverlapSphere(point, weaponData.explosionRadius);
    for (int i = 0; i < colliders.Length; i++)
    {
        if (colliders[i].transform.parent != null &&
            colliders[i].transform.parent.TryGetComponent<Actor>(out Actor A))
        {
            A.TakeDamage(weaponData.explosionDamage);
        }
    }

    if (weaponData.explosionEffect != null)
    {
        GameObject explosion = Instantiate(weaponData.explosionEffect, point, Quaternion.identity);
        Destroy(explosion, 4);
    }

    // หน่วง 0.5 วินาทีแล้วค่อยเล่นเสียง
    StartCoroutine(PlayExplosionSoundDelayed(point, 0.02f));
}

    IEnumerator PlayExplosionSoundDelayed(Vector3 point, float delaySec)
    {
        yield return new WaitForSeconds(delaySec);          // ใช้ WaitForSecondsRealtime ถ้าไม่อยากให้โดน Time.timeScale
        if (weaponData.explosionSound == null) yield break;

        // ถ้าอยากให้เสียงออกที่ตำแหน่งระเบิดจริง ๆ:
        // AudioSource.PlayClipAtPoint(weaponData.explosionSound, point, 1f);

        // หรือใช้ AudioSource เดิมที่ผูกกับออบเจกต์นี้:
        audioSource.PlayOneShot(weaponData.explosionSound);
    }
 
}
