using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockChainManager : MonoBehaviour
{
    UnityEvent<string> OnLoggedIn;
    private string address;
    public static BlockChainManager blockChainManager { private set; get; }
    
}
