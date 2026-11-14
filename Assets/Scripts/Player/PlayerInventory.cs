using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private int pearlsCount;

    public int GetPearls()
    {
        return pearlsCount;
    }

    public void SetPearls(int count)
    {
        pearlsCount = count;
    }

    public void ChangePearls(int change)
    {
        pearlsCount += change;
        if (pearlsCount < 0)
        {
            pearlsCount = 0;
        }
    }
}
