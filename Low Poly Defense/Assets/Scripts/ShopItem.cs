using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItem : MonoBehaviour {

    public GameObject item;
    public int cost = 25;
    public string itemName;

    private Text priceText;
    private Text nameText;
    private Shop shop;

	// Use this for initialization
	void Start ()
    {
        if (!item)
            throw new System.NullReferenceException("No item for player to buy on shop item " + this.gameObject.name);

        shop = Shop.instance;
        priceText = transform.FindChild("Price").GetComponent<Text>();
        nameText = transform.FindChild("Name").GetComponent<Text>();

        if (item.tag == "Tower")
        {
            cost = item.GetComponent<Tower>().cost;
            priceText.text = cost.ToString();

            itemName = item.GetComponent<Tower>().title;
            nameText.text = itemName;
        } else
        {
            nameText.text = itemName;
            priceText.text = cost.ToString();
        }
	}
	
	public void Clicked()
    {
        shop.AttemptItemBuy(this);
    }

}
