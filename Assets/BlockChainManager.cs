using System.Threading.Tasks;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class BlockChainManager : MonoBehaviour
{
    public string tokenAddress = "0xceC7c826F288583e348C5a0Df984026BA7699308";
    public string leaderboardAddress = "0xf0db1a6E52D5e5bF645c93C2c90756d3070C861f";
    public string leaderboardAbi = "[{\"type\":\"event\",\"name\":\"ScoreAdded\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"score\",\"indexed\":false,\"internalType\":\"uint256\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"function\",\"name\":\"_scores\",\"inputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getRank\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"rank\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"submitScore\",\"inputs\":[{\"type\":\"uint256\",\"name\":\"score\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"}]";
    public static BlockChainManager blockChainManager { private set; get; }
    public UnityEvent<string> OnLoggedIn;
    public CharacterManager characterController;
    private string address;
    public Button claimButton;
    public Button submitScore;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI balanceText;
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
        characterController.OnDeath.AddListener(PlayerDied);
    }
    public void PlayerDied(float amount)
    {
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(() => { Claim(amount); GetBalance(); });
        submitScore.onClick.RemoveAllListeners();
        submitScore.onClick.AddListener(async () => { await SubmitScore(amount); GetRank(); });
        GetRank();
        GetBalance();
    }
    public async void Login()
    {
        AuthProvider provider = AuthProvider.Google;
        WalletConnection connection = new WalletConnection(
            provider: WalletProvider.SmartWallet,
            chainId: 4002,
            personalWallet: WalletProvider.EmbeddedWallet,
            authOptions: new AuthOptions(authProvider: provider)
            );
        address = await ThirdwebManager.Instance.SDK.wallet.Connect(connection);

        OnLoggedIn?.Invoke(address);
    }
    internal async void GetRank()
    {
        Contract contract = ThirdwebManager.Instance.SDK.GetContract(
            leaderboardAddress,leaderboardAbi
            //"[{\"type\":\"event\",\"name\":\"ScoreAdded\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"score\",\"indexed\":false,\"internalType\":\"uint256\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"function\",\"name\":\"_scores\",\"inputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getRank\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"rank\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"submitScore\",\"inputs\":[{\"type\":\"uint256\",\"name\":\"score\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"}]"
        );
        int rank = await contract.Read<int>("getRank", address);
        rankText.text = "Global Rank : " + rank.ToString();
    }
    internal async Task SubmitScore(float distanceTravelled)
    {
        submitScore.interactable = false;
        Contract contract = ThirdwebManager.Instance.SDK.GetContract(leaderboardAddress, leaderboardAbi);
        await contract.Write("submitScore", (int)distanceTravelled, 123);
        submitScore.interactable = true;
    }
    internal async void Claim(float amount)
    {
        claimButton.interactable = false;
        Contract contract = ThirdwebManager.Instance.SDK.GetContract(tokenAddress);
        await contract.ERC20.ClaimTo(address, ((int)amount).ToString());
        claimButton.interactable = true;
    }
    internal async void GetBalance()
    {
        Contract contract = ThirdwebManager.Instance.SDK.GetContract(tokenAddress);
        CurrencyValue balance = await contract.ERC20.Balance();
        balanceText.text = "Balance :" + balance.displayValue.ToString();
    }
}
