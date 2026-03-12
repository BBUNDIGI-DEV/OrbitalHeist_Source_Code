using System.Collections;
using UnityEngine;

public class ItemElement : MonoBehaviour
{
    [SerializeField] private eItemNameID sfItemNameID;
    private ItemData mItemData;

    public void EnableItem()
    {
        GetComponent<Collider>().enabled = false;
        gameObject.SetActive(true);
        if(mItemData == null)
        {
            mItemData = ItemDataUtil.sAllDatas[(int)sfItemNameID];
        }
        StartCoroutine(delayEnable());
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        PlayerManager.Instance.CurrentPlayer.Value.OnItemReceived(mItemData);
        Destroy(gameObject);
    }

    private IEnumerator delayEnable()
    {
        yield return new WaitForSeconds(1.5f);
        GetComponent<Collider>().enabled = true;
    }
}
