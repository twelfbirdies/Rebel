using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LookAtCamera : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] private bool invert;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void LateUpdate()
    {
        if (invert) 
        { 
            Vector3 dirToCamera = (cameraTransform.position - transform.position).normalized;
            transform.LookAt(transform.position +dirToCamera*-1);
        } 
        else 
        { 
        transform.LookAt(cameraTransform);
        }
    }

    private void UpdateHealthBar() 
    {
      healthBarImage.fillAmount =  healthSystem.GetHealthNormalized();
    
    }
}
