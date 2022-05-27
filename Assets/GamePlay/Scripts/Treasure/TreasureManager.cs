using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    public GameObject treasureOpen;
    public GameObject treasureClose;

    #region Singleton
    public static TreasureManager Ins;
    private void Awake()
    {
        Ins = this;
    }
    #endregion

    void Start()
    {
        treasureOpen.SetActive(false);
    }

    public void WaitToOpenTreasure(float time)
    {
        Invoke(nameof(OpenTreasure), time);
    }

    private void OpenTreasure()
    {
        treasureOpen.SetActive(true);
        treasureClose.SetActive(false);
    }
}
