using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoleHoverer : MonoBehaviour
{
    public GameObject image;

    private void OnMouseEnter()
    {
        image.SetActive(true);
    }
    private void OnMouseExit()
    {
        image.SetActive(false);
    }
}
