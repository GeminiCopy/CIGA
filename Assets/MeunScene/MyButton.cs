using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MyButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] Image touchImage;
    public void OnPointerEnter(PointerEventData eventData)
    {
        touchImage.rectTransform.position = new Vector3();
    }
}
