using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private int priceOfAk47;
    [SerializeField] private int priceOfAk47Bullet;
    [SerializeField] private int priceOfShotgun;
    [SerializeField] private int priceOfShotgunBullet;
    [SerializeField] private int priceOfHealth;
    [SerializeField] private int priceOfStamina;
    [SerializeField] private Button BuyAk47Button;
    [SerializeField] private Image ak47Image;
    [SerializeField] private Image ak47BulletImage;
    [SerializeField] private Image shotgunImage;
    [SerializeField] private Image shotgunBulletImage;
    [SerializeField] private TMP_Text aK47Text;
    [SerializeField] private TMP_Text shotgunText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text staminaText;
    private PlayerInventory player;
    private bool isAk47Bought = false, isShotgunBought = false;
    void Start()
    {
        player = PlayerInventory.Instance;
        aK47Text.text = "Ak47\n" + priceOfAk47 + "$";
        shotgunText.text = "Shotgun\n" + priceOfShotgun + "$";
        healthText.text = "+10 Health\n" + priceOfHealth + "$";
        staminaText.text = "+10 Stamina\n" + priceOfStamina + "$";
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.E))
        {
            CloseMenu();
        }
    }
    public void BuyAk47()
    {
        if (!isAk47Bought)
        {
            if (player.UseMoney(priceOfAk47))
            {
                isAk47Bought = true;
                ak47Image.enabled = false;
                ak47BulletImage.enabled = true;
                aK47Text.text = "+30 Ak47 Ammunition\n" + priceOfAk47Bullet + "$";
                player.BuyAk47();
            }
        }
        else
        {
            if (player.UseMoney(priceOfAk47Bullet))
            {
                player.BuyAk47();
            }
        }
    }
    public void BuyShotgun()
    {
        if (!isShotgunBought)
        {
            if (player.UseMoney(priceOfShotgun))
            {
                isShotgunBought = true;
                shotgunImage.enabled = false;
                shotgunBulletImage.enabled = true;
                shotgunText.text = "+8 Shotgun Ammunition\n" + priceOfShotgunBullet + "$";
                player.BuyShotgun();
            }
        }
        else
        {
            if (player.UseMoney(priceOfShotgunBullet))
            {
                player.BuyShotgun();
            }
        }
    }
    public void BuyHealth()
    {
        if (player.GetHealth())
        {
            if (player.UseMoney(priceOfHealth))
            {
                player.BuyHealth();
            }
        }
    }
    public void BuyStamina()
    {
        if (player.UseMoney(priceOfStamina))
        {
            priceOfStamina *= 2;
            playerMovement.BuyStamina();
            staminaText.text = "+10 Stamina\n" + priceOfStamina + "$";
        }
    }
    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}
