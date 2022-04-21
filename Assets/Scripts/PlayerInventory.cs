using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject tutorialText;
    [SerializeField] private AudioClip gunShot;
    [SerializeField] private AudioClip diedSound;
    [SerializeField] private float offsetOpenShop;
    [SerializeField] private GameObject openShopText;
    [SerializeField] private TMP_Text healthCounterText;
    [SerializeField] private CanvasGroup deathCanvas;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private int maxHP = 100;
    [SerializeField] private GameObject shop;
    [SerializeField] private TMP_Text bulletCountText;
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private Transform[] attackingPoints;
    [SerializeField] private Transform[] lookingPoints;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletForce;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private int money = 0;
    private AudioSource audioSource;
    private int currentHP;
    private bool isInHouse = true;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private float fireCooldownCounter = 0;
    private Weapon[] weapons = new Weapon[4];
    public Weapon currentWeapon;
    private static PlayerInventory _player = null;
    private Animator animator;
    public static PlayerInventory Instance => _player;
    private void Awake()
    {
        if (_player == null)
        {
            _player = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        weapons[0] = new Knife();
        weapons[1] = new Pistol();

        currentHP = maxHP;

        moneyText.text = money + " $";

        WeaponSet(1);
    }
    void Update()
    {
        WeaponChange();

        Fire();

        ReloadAnimation();

        OpenShop();

        SetHealthBar();
    }
    private void SetHealthBar()
    {
        healthCounterText.text = (int)(((float)currentHP / maxHP) * 100) + "%";
        float fillAmount = (float)currentHP / maxHP;
        healthBarFill.fillAmount = fillAmount;
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0 && !isDead)
        {
            StartCoroutine("Die");
        }
    }
    private IEnumerator Die()
    {
        audioSource.PlayOneShot(diedSound);
        deathCanvas.alpha = 0;
        currentHP = 0;
        isDead = true;
        Time.timeScale = 0.3f;
        deathCanvas.gameObject.SetActive(true);
        deathCanvas.interactable = false;

        while (deathCanvas.alpha < 1)
        {
            deathCanvas.alpha += Time.deltaTime / Time.timeScale;
            yield return new WaitForSeconds(Time.deltaTime * Time.timeScale * Time.timeScale);
        }

        deathCanvas.interactable = true;
        Time.timeScale = 0;
    }
    private void WeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponSet(1);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponSet(2);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WeaponSet(3);
        }
    }

    private void WeaponSet(int position)
    {
        if (weapons[position] != null)
        {
            currentWeapon = weapons[position];
            pm.SetLookingPivot(lookingPoints[position]);
            currentWeapon.UpdateBulletCount(bulletCountText);
            animator.SetInteger("Weapon", position);
            animator.SetBool("isReloading", false);
        }
    }

    private void Fire()
    {
        fireCooldownCounter -= Time.deltaTime;

        if (Input.GetButton("Fire1") && !animator.GetBool("isReloading") && GameController.Instance.canFire() && !shop.activeSelf && !isDead)
        {
            currentWeapon.Fire(bulletPrefab, attackingPoints, bulletForce, ref fireCooldownCounter, audioSource, gunShot);
            currentWeapon.UpdateBulletCount(bulletCountText);
        }
    }
    private void ReloadAnimation()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.canReload())
        {
            animator.SetBool("isReloading", true);
        }
    }

    private void Reload()
    {
        currentWeapon.Reload();
        currentWeapon.UpdateBulletCount(bulletCountText);
        animator.SetBool("isReloading", false);
    }
    public void GainMoney(int amount)
    {
        money += amount;
        moneyText.text = money + " $";
    }
    public bool UseMoney(int price)
    {
        if (money < price)
        {
            return false;
        }

        money -= price;
        moneyText.text = money + " $";
        return true;
    }
    public void BuyAk47()
    {
        if (weapons[2] == null)
        {
            weapons[2] = new Ak47();
        }
        else
        {
            weapons[2].buyAmmo();
            weapons[2].UpdateBulletCount(bulletCountText);
        }
    }
    public void BuyShotgun()
    {
        if (weapons[3] == null)
        {
            weapons[3] = new Shotgun();
        }
        else
        {
            weapons[3].buyAmmo();
            weapons[3].UpdateBulletCount(bulletCountText);
        }
    }
    public bool GetHealth()
    {
        if (currentHP == 100)
        {
            return false;
        }

        return true;
    }
    public void BuyHealth()
    {
        currentHP += 10;
        if (currentHP > 100)
        {
            currentHP = 100;
        }
    }
    private void OpenShop()
    {
        Vector3 newPos = transform.position + new Vector3(0, offsetOpenShop, 0);
        openShopText.transform.position = newPos;

        openShopText.SetActive(isInHouse && GameController.Instance.IsFinished);

        if ((Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.E)) && isInHouse && GameController.Instance.IsFinished)
        {
            shop.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.P) && tutorialText != null)
        {
            tutorialText.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("House"))
        {
            isInHouse = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("House"))
        {
            isInHouse = false;
        }
    }
}
public abstract class Weapon
{
    protected float fireCooldown;
    public abstract void Fire(GameObject bulletPrefab, Transform[] attackingPoint, float bulletForce, ref float fireCooldownCounter, AudioSource audioSource, AudioClip gunShot);
    public abstract void UpdateBulletCount(TMP_Text bulletCountText);
    public abstract void Reload();
    public abstract bool canReload();
    public abstract void buyAmmo();
}

public abstract class Firearms : Weapon
{
    protected int ammoInClip;
    protected int ammoInBag = 1;
    protected int ammoClipMax;
    public override bool canReload()
    {
        if (ammoInClip < ammoClipMax && ammoInBag > 0)
        {
            return true;
        }

        return false;
    }
    public override void buyAmmo()
    {
        ammoInBag += ammoClipMax;
    }
}
public class Knife : Weapon
{
    public override void Fire(GameObject bulletPrefab, Transform[] attackingPoint, float bulletForce, ref float fireCooldownCounter, AudioSource audioSource, AudioClip gunShot) { }
    public override void UpdateBulletCount(TMP_Text bulletCountText)
    {
        bulletCountText.text = "";
    }
    public override void Reload() { }
    public override bool canReload() { return false; }
    public override void buyAmmo() { }
}

public class Pistol : Firearms
{
    public Pistol()
    {
        ammoInClip = 12;
        ammoClipMax = 12;
        fireCooldown = 0.6f;
    }
    public override void Fire(GameObject bulletPrefab, Transform[] attackingPoint, float bulletForce, ref float fireCooldownCounter, AudioSource audioSource, AudioClip gunShot)
    {
        if (fireCooldownCounter < 0)
        {
            if (ammoInClip > 0)
            {
                audioSource.PlayOneShot(gunShot, 0.5f);
                ammoInClip--;
                Quaternion bulletRot = Quaternion.Euler(attackingPoint[1].rotation.eulerAngles);
                GameObject bullet = Object.Instantiate(bulletPrefab, attackingPoint[1].position, bulletRot);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(bullet.transform.up * bulletForce * 5, ForceMode2D.Impulse);
                fireCooldownCounter = fireCooldown;
            }
        }
    }
    public override void UpdateBulletCount(TMP_Text bulletCountText)
    {
        bulletCountText.text = ammoInClip.ToString("F0");
    }
    public override void Reload()
    {
        ammoInClip = ammoClipMax;
    }
}

public class Ak47 : Firearms
{
    public Ak47()
    {
        ammoInClip = 30;
        ammoClipMax = 30;
        ammoInBag = 60;
        fireCooldown = 0.1f;
    }
    public override void Fire(GameObject bulletPrefab, Transform[] attackingPoint, float bulletForce, ref float fireCooldownCounter, AudioSource audioSource, AudioClip gunShot)
    {
        if (fireCooldownCounter < 0)
        {
            if (ammoInClip > 0)
            {
                audioSource.PlayOneShot(gunShot, 0.5f);
                ammoInClip--;
                Quaternion bulletRot = Quaternion.Euler(attackingPoint[2].rotation.eulerAngles);
                GameObject bullet = Object.Instantiate(bulletPrefab, attackingPoint[2].position, bulletRot);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(bullet.transform.up * bulletForce * 5, ForceMode2D.Impulse);
                fireCooldownCounter = fireCooldown;
            }
        }
    }
    public override void UpdateBulletCount(TMP_Text bulletCountText)
    {
        bulletCountText.text = ammoInClip + "/" + ammoInBag;
    }
    public override void Reload()
    {
        if (ammoInBag >= ammoClipMax)
        {
            ammoInBag -= ammoClipMax - ammoInClip;
            ammoInClip = ammoClipMax;
        }

        else if (ammoInBag > 0)
        {
            ammoInClip = ammoInBag;
            ammoInBag = 0;
        }
    }
}

public class Shotgun : Firearms
{
    public Shotgun()
    {
        ammoInClip = 8;
        ammoClipMax = 8;
        ammoInBag = 24;
        fireCooldown = 1f;
    }
    public override void Fire(GameObject bulletPrefab, Transform[] attackingPoint, float bulletForce, ref float fireCooldownCounter, AudioSource audioSource, AudioClip gunShot)
    {
        if (fireCooldownCounter < 0)
        {
            if (ammoInClip > 0)
            {
                audioSource.PlayOneShot(gunShot, 0.5f);
                ammoInClip--;
                float degree = -9f;

                for (int i = 1; i <= 5; i++)
                {
                    Vector3 shotgunScatter = new Vector3(0, 0, degree);
                    Quaternion bulletRot = Quaternion.Euler(attackingPoint[3].rotation.eulerAngles + shotgunScatter);
                    GameObject bullet = Object.Instantiate(bulletPrefab, attackingPoint[3].position, bulletRot);
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    rb.AddForce(bullet.transform.up * bulletForce * 5, ForceMode2D.Impulse);
                    degree += 4.5f;
                }

                fireCooldownCounter = fireCooldown;
            }
        }
    }
    public override void UpdateBulletCount(TMP_Text bulletCountText)
    {
        bulletCountText.text = ammoInClip + "/" + ammoInBag;
    }
    public override void Reload()
    {
        if (ammoInBag >= ammoClipMax)
        {
            ammoInBag -= ammoClipMax - ammoInClip;
            ammoInClip = ammoClipMax;
        }

        else if (ammoInBag > 0)
        {
            ammoInClip = ammoInBag;
            ammoInBag = 0;
        }
    }
}
