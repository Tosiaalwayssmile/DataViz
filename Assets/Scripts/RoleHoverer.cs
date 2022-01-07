using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
