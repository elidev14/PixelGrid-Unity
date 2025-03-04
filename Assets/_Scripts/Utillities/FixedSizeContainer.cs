[System.Serializable]
public class FixedSizeContainer<T>
{
    public T level1;
    public T level2;
    public T level3;
    public T level4;
    public T level5;

    public int Length => 5;

    public ref T this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return ref level1;
                case 1: return ref level2;
                case 2: return ref level3;
                case 3: return ref level4;
                case 4: return ref level5;
                default: throw new System.ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}