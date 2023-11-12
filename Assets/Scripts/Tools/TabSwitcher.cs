using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [System.Serializable]
    private class Item
    {
        [SerializeField] private string tag;
        [SerializeField] private Image tab;
        [SerializeField] private GameObject panel;

        public void Show(string tag, Color colShow, Color colHide, float scale)
        {
            bool doShow = tag == this.tag;

            panel.gameObject.SetActive(doShow);
            tab.color = doShow ? colShow : colHide;
            tab.transform.localScale = (doShow ? scale : 1f) * Vector3.one;
        }
    }

    [SerializeField] private float tabShowScale = 1.2f;
    [SerializeField] private Color tabShowColor;
    [SerializeField] private Color tabHideColor;

    [SerializeField] private List<Item> items;

    public void SetTab(string tag)
    {
        foreach (var item in items)
            item.Show(tag, tabShowColor, tabHideColor, tabShowScale);
    }
}
