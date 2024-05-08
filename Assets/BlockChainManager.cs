using Thirdweb;
using UnityEngine;
using UnityEngine.Events;

public class BlockChainManager : MonoBehaviour
{
    public UnityEvent<string> OnLoggedIn;
    private string address;
    public static BlockChainManager blockChainManager { private set; get; }
    public string claimAmount;
    public UnityEngine.UI.Button claimButton;
    public CharacterManager characterController;
    private void Awake()
    {
        if (blockChainManager == null)
        {
            blockChainManager = this;
        }
        else
        {
            Destroy(this);
        }
        claimButton.onClick.AddListener(ClaimToken);
    }
    private void Update()
    {
        claimAmount = ((int)characterController.DistanceTravelled).ToString();
    }
    public async void Login(string authProvider)
    {
        AuthProvider provider = AuthProvider.Google;
        switch (authProvider)
        {
            case "Google":
                provider = AuthProvider.Google;
                break;
            case "Apple":
                provider = AuthProvider.Apple;
                break;
        }
        var connection = new WalletConnection(
            provider: WalletProvider.SmartWallet, 
            chainId: 4002, 
            personalWallet: WalletProvider.EmbeddedWallet, 
            authOptions: new AuthOptions(authProvider : provider)
            );
        address = await ThirdwebManager.Instance.SDK.wallet.Connect(connection);
        Debug.Log(address);
        OnLoggedIn?.Invoke(address);
    }
    public async void ClaimToken()
    {
        claimButton.interactable = false;
        var Contract = ThirdwebManager.Instance.SDK.GetContract("0x7D77a4De30e7c07Fa43d8A0B3A574591f6e07EeF");
        CurrencyValue currencyValue =await Contract.ERC20.BalanceOf(address);
        Debug.Log(currencyValue.displayValue + " " + currencyValue.value + " " + currencyValue.symbol + " " + currencyValue.name + " " + currencyValue.decimals);
        //var a = await Contract.ERC20.Transfer("0x030C9BbD80d5b940438B53bA32E6392555998d24", claimAmount);
        //currencyValue = await Contract.ERC20.BalanceOf(address);
        //Debug.Log(currencyValue.displayValue + " " + currencyValue.value + " " + currencyValue.symbol + " " + currencyValue.name + " " + currencyValue.decimals);
        //var a = await Contract.ERC20.Claim("1");
        //Debug.Log(await a.ToJToken());
        var result = await Contract.ERC20.Claim(claimAmount);
        currencyValue =await Contract.ERC20.BalanceOf(address);

        Debug.Log(currencyValue.displayValue+" "+currencyValue.value+" "+currencyValue.symbol+" "+currencyValue.name+" "+currencyValue.decimals);
    }
}
