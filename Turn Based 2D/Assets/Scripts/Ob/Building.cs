using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    public Transform[] GetAllPoints()
    {
        return points;
    }
}
