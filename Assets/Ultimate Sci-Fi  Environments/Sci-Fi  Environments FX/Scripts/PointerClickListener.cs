using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace SciFiEnvironments.FX
{

    public class PointerClickListener : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public delegate void VoidDelegate(GameObject go);
        public VoidDelegate onClick;

        public VoidDelegate onEnter;
        public VoidDelegate onExit;

        static public PointerClickListener Get(GameObject go)
        {
            PointerClickListener listener = go.GetComponent<PointerClickListener>();
            if (listener == null) listener = go.AddComponent<PointerClickListener>();
            return listener;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
            {
                onClick(gameObject);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null) onEnter(gameObject);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null) onExit(gameObject);
        }
    }
}