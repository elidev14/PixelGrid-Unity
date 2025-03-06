using UnityEngine;

public abstract class StaticInstances<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; set; }

    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    { 
        Instance = null; 
        Destroy(gameObject);
    }
}

public abstract class Singleton<T> : StaticInstances<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance == null)
        {

            base.Awake();
        }
        else
        {
            Destroy(gameObject);
        }


    }
}

public abstract class PersistenceSingleton<T> : StaticInstances<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        if(Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
