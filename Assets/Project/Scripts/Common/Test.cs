using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour
{
    public UnityEvent ontest;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void testevent()
    {
        ontest.Invoke();
    }
}
