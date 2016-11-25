using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    public static Shop instance;

    public GameObject cancelPurchaseButton;
    public GameObject comfirmPurchaseButton;

    private TowerManager towerManager;
    private Player player;
    private Previewer previewer;
    private GameManager gameManager;
    private ShopItem currentItem;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = Player.instance;
        gameManager = GameManager.instance;
        towerManager = TowerManager.instance;
        previewer = Previewer.instance;
        ToggleButtons(false);
    }

    /// <summary>
    /// Attempt to buy an item from the shop, if the player has enough money
    /// </summary>
    /// <param name="itemToBuy"></param>
    public void AttemptItemBuy(ShopItem itemToBuy)
    {
        if (player.Coins < itemToBuy.cost)
            return;

        // Enables the cancel and comfirm purchase buttons for the user
        ToggleButtons(true);

        // If the current item is a tower, show preview
        if (currentItem == itemToBuy && !towerManager.selectedTower.IsPurchased())
        {
            CancelItemPurchase();
            currentItem = null;
        }
        else  
        {
            if(itemToBuy.item.GetComponent<Tower>())
                towerManager.BuildTower(itemToBuy.item);
          
            currentItem = itemToBuy;
        } 
    }

    /// <summary>
    /// Cancels item purchase, takes no money from the user
    /// </summary>
    public void CancelItemPurchase()
    {
        // Exits preview if the current item is a tower
        if (currentItem.item.GetComponent<Tower>())
        {
            towerManager.DestroyTower(towerManager.selectedTower.gameObject);
        }

        currentItem = null;

        ToggleButtons(false);
    }

    public void ComfirmItemPurchase()
    {
        if (!previewer.Overlapping)
        {
            if (currentItem.item.GetComponent<Tower>())
            {
                towerManager.PlaceTower(towerManager.selectedTower.gameObject);

                player.Coins += -currentItem.cost;
            }
            else if (!currentItem.item.GetComponent<Tower>())
            {
                player.Coins += -currentItem.cost;
            }

            ToggleButtons(false);
        } 
    }

    private void ToggleButtons(bool on)
    {
        cancelPurchaseButton.SetActive(on);
        comfirmPurchaseButton.SetActive(on);
    }

    public void UpdateButtons()
    {
        if (previewer.Previewing)
        {
            if (!previewer.Overlapping)
            {
                comfirmPurchaseButton.GetComponent<Button>().interactable = true;
                Color c = comfirmPurchaseButton.GetComponent<Image>().color;
                c.a = 1f;

                comfirmPurchaseButton.GetComponent<Image>().color = c;
            } else
            {
                comfirmPurchaseButton.GetComponent<Button>().interactable = false;

                Color c = comfirmPurchaseButton.GetComponent<Image>().color;
                c.a = 0.5f;

                comfirmPurchaseButton.GetComponent<Image>().color = c;
            }
        }
    }

}
