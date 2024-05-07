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
        claimAmount = characterController.DistanceTravelled.ToString();
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
        var Contract = ThirdwebManager.Instance.SDK.GetContract("0xF7Fa56BF7EA390C319c2501A0b9c364c96156B82");
        var result = await Contract.ERC20.ClaimTo(address, claimAmount);
    }
}
