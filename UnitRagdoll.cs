using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;
    [SerializeField] private float explosionRange = 10f;
    [SerializeField] private float explosionForce = 500f;

    public void Setup(Transform originalRootBone) 
    {
        MatchAllChildTransform(originalRootBone, ragdollRootBone);
        Vector3 randomDir = new Vector3(Random.Range(-1f, +1f), 0, Random.Range(-1f, +1f));
        ApplyExplosionToRagdoll(ragdollRootBone, explosionForce, transform.position+randomDir, explosionRange);
    }

    private void MatchAllChildTransform(Transform root, Transform clone) 
    {
        foreach (Transform child in root) 
        {
            Transform cloneChild = clone.Find(child.name);

            if (cloneChild != null) 
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;         
                
                MatchAllChildTransform(child, cloneChild);

                

            }

        }
    
    }

    private void ApplyExplosionToRagdoll(Transform root,float explosionForce, Vector3 explosionPosition, float explosionRange) 
    {
        foreach (Transform child in root) 
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody)) 
            { 
            childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }
            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    
    
    }
}
