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
        var connection = new WalletConnection(provider: WalletProvider.SmartWallet, chainId: 421614, personalWallet: WalletProvider.EmbeddedWallet, authOptions: new AuthOptions(provider));
        address = await ThirdwebManager.Instance.SDK.wallet.Connect(connection);
        OnLoggedIn?.Invoke(address);
    }
    public async void ClaimToken()
    {
        claimButton.interactable = false;
        var Contract = ThirdwebManager.Instance.SDK.GetContract("0xceC7c826F288583e348C5a0Df984026BA7699308");
        CurrencyValue currencyValue =await Contract.ERC20.BalanceOf(address);
        Debug.Log(currencyValue.displayValue + " " + currencyValue.value + " " + currencyValue.symbol + " " + currencyValue.name + " " + currencyValue.decimals);
        var result = await Contract.ERC20.ClaimTo(address,claimAmount);
        currencyValue =await Contract.ERC20.BalanceOf(address);
        Debug.Log(currencyValue.displayValue+" "+currencyValue.value+" "+currencyValue.symbol+" "+currencyValue.name+" "+currencyValue.decimals);
    }
}
