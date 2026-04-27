using UnityEngine;

public class ItemSpinning : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    void Start()
    {
        
    }


    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
