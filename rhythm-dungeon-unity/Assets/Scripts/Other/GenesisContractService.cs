using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using UnityEngine;
using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices;

public class GenesisContractService : MonoBehaviour
{
#if !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern string SendTransaction (string to, string data);
    [DllImport ("__Internal")]
    private static extern void CheckWeb3();
#endif

    public static GenesisContractService Instance;

    [FunctionOutput]
    public class CharacterDetailsDTO : IFunctionOutputDTO
    {
        [Parameter("string", "_name", 1)]
        public string _name { get; set; }
        [Parameter("uint", "_hp", 2)]
        public uint _hp { get; set; }
        [Parameter("uint", "_mp", 3)]
        public uint _mp { get; set; }
        [Parameter("uint", "_str", 4)]
        public uint _str { get; set; }
        [Parameter("uint", "_int", 5)]
        public uint _int { get; set; }
        [Parameter("uint", "_san", 6)]
        public uint _san { get; set; }
        [Parameter("uint", "_luck", 7)]
        public uint _luck { get; set; }
        [Parameter("uint", "_charm", 8)]
        public uint _charm { get; set; }
        [Parameter("uint", "_mt", 9)]
        public uint _mt { get; set; }
        [Parameter("string", "_optionalAttrs", 10)]
        public string _optionalAttrs { get; set; }
    }

    public CharacterDetailsDTO requestedCharacter = null;

    [HideInInspector]
    public string lastUploadTxnHash;

    private static string _url = ""; // your infura API address
    private static string ABI = @"[{""constant"":true,""inputs"":[],""name"":""getCharacterNo"",""outputs"":[{""name"":""_characterNo"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_hp"",""type"":""uint256""},{""name"":""_mp"",""type"":""uint256""},{""name"":""_str"",""type"":""uint256""},{""name"":""_intelli"",""type"":""uint256""},{""name"":""_san"",""type"":""uint256""},{""name"":""_luck"",""type"":""uint256""},{""name"":""_charm"",""type"":""uint256""},{""name"":""_mt"",""type"":""uint256""}],""name"":""checkLegal"",""outputs"":[{""name"":""_checkresult"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""version"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_id"",""type"":""uint256""},{""name"":""isPositiveEffect"",""type"":""uint256""}],""name"":""affectCharacter"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getRand"",""outputs"":[{""name"":""_rand"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_characterId"",""type"":""uint256""}],""name"":""getCharacterDetails"",""outputs"":[{""name"":""_name"",""type"":""string""},{""name"":""_hp"",""type"":""uint256""},{""name"":""_mp"",""type"":""uint256""},{""name"":""_str"",""type"":""uint256""},{""name"":""_int"",""type"":""uint256""},{""name"":""_san"",""type"":""uint256""},{""name"":""_luck"",""type"":""uint256""},{""name"":""_charm"",""type"":""uint256""},{""name"":""_mt"",""type"":""uint256""},{""name"":""_optionalAttrs"",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_name"",""type"":""string""},{""name"":""_hp"",""type"":""uint256""},{""name"":""_mp"",""type"":""uint256""},{""name"":""_str"",""type"":""uint256""},{""name"":""_intelli"",""type"":""uint256""},{""name"":""_san"",""type"":""uint256""},{""name"":""_luck"",""type"":""uint256""},{""name"":""_charm"",""type"":""uint256""},{""name"":""_mt"",""type"":""uint256""},{""name"":""_optionalAttrs"",""type"":""string""}],""name"":""insertCharacter"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""characterNo"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_id"",""type"":""uint256""},{""name"":""_hp"",""type"":""uint256""},{""name"":""_mp"",""type"":""uint256""},{""name"":""_str"",""type"":""uint256""},{""name"":""_intelli"",""type"":""uint256""},{""name"":""_san"",""type"":""uint256""},{""name"":""_luck"",""type"":""uint256""},{""name"":""_charm"",""type"":""uint256""},{""name"":""_optionalAttrs"",""type"":""string""}],""name"":""setCharacterAttributes"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""}]";
    private static string contractAddress = "0x8cae244D8E274058d1C10C8ED731994A10b7e7d2";
    private static string coinABI = @"[{""constant"":true,""inputs"":[{""internalType"":""uint256"",""name"":""level"",""type"":""uint256""}],""name"":""getLength"",""outputs"":[{""internalType"":""uint256"",""name"":""_length"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""withdraw"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""uint256"",""name"":""level"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""index"",""type"":""uint256""}],""name"":""getGenesisIndex"",""outputs"":[{""internalType"":""uint256"",""name"":""_index"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""ownerWithdraw"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""level"",""type"":""uint256""},{""internalType"":""string"",""name"":""_name"",""type"":""string""},{""internalType"":""uint256"",""name"":""_hp"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_mp"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_str"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_intelli"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_san"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_luck"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_charm"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_mt"",""type"":""uint256""},{""internalType"":""string"",""name"":""_optionalAttrs"",""type"":""string""}],""name"":""insertCharacter"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""isPriceAssigned"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":""tokenOwner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":""balance"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""owner"",""outputs"":[{""internalType"":""address payable"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""newPrice"",""type"":""uint256""}],""name"":""setPrice"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""index"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount"",""type"":""uint256""}],""name"":""reward"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""isGenesisSet"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""amount"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""aim"",""type"":""uint256""}],""name"":""useRevivalCoins"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":""tokenOwner"",""type"":""address""}],""name"":""coinBalanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":""balance"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""amount"",""type"":""uint256""}],""name"":""buy"",""outputs"":[],""payable"":true,""stateMutability"":""payable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""genesisAddress"",""type"":""address""}],""name"":""setGenesis"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""uint256"",""name"":""newPrice"",""type"":""uint256""}],""name"":""SetPrice"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""buyer"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""price"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount"",""type"":""uint256""}],""name"":""Buy"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""user"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""aim"",""type"":""uint256""}],""name"":""CoinBalanceInsufficient"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""user"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""aim"",""type"":""uint256""}],""name"":""SuccessfullyUse"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""receiver"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount"",""type"":""uint256""}],""name"":""Reward"",""type"":""event""}]";
    private static string coinContractAddress = "0x1B5269e48e16459F799Ff955019AcB7b68Ac24d9";
    private string accountAddress = ""; // your public key
    private string accountPrivateKey = ""; // your private key (it is bad design to expose the private key in the source code)

    private Contract contract = new Contract(null, ABI, contractAddress);
    private Contract coinContract = new Contract(null, coinABI, coinContractAddress);
    private int[] lengths;

    public int genesisIndex;

    private void Awake()
    {

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        if (GenesisContractService.Instance != null)
            Destroy(gameObject);
        else Instance = this;
        lengths = new int[9];
    }

    private void Start()
    {
#if !UNITY_EDITOR
        CheckWeb3();
#endif
    }

    public void RequestLengths()
    {
        StartCoroutine(RequestLengthsCoroutine());
    }

    IEnumerator RequestLengthsCoroutine()
    {
        for (int i = 0; i < 9; ++i)
        {
            StartCoroutine(GetLength(i));
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private IEnumerator GetLength(int level)
    {
        // We create a new pingsRequest as a new Eth Call Unity Request
        var lengthRequest = new EthCallUnityRequest(_url);
        if (coinContract == null) throw new System.Exception("fuck");
        var getLengthFunction = coinContract.GetFunction("getLength");
        var lengthCallInput = getLengthFunction.CreateCallInput(level);
        Debug.Log("Getting Length of level = " + level + "...");

        yield return lengthRequest.SendRequest(lengthCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        if (lengthRequest.Exception == null)
        {
            lengths[level] = getLengthFunction.DecodeSimpleTypeOutput<int>(lengthRequest.Result);
            //Debug.Log("Lengths (HEX): " + lengthsRequest.Result);
            Debug.Log("Length of level = " + level + "(INT):" + lengths[level]);
        }
        else
        {
            Debug.Log("Error submitting GetLengths tx: " + lengthRequest.Exception.Message);
        }
    }

    private IEnumerator GetGenesisIndex(int level, int index)
    {
        // We create a new pingsRequest as a new Eth Call Unity Request
        var genesisIndexRequest = new EthCallUnityRequest(_url);
        var getGenesisIndexFunction = coinContract.GetFunction("getGenesisIndex");
        var genesisIndexCallInput = getGenesisIndexFunction.CreateCallInput(level, index);
        Debug.Log("Getting genesis index of (level,index)=(" + level + "," + index + ")...");

        yield return genesisIndexRequest.SendRequest(genesisIndexCallInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        if (genesisIndexRequest.Exception == null)
        {
            genesisIndex = getGenesisIndexFunction.DecodeSimpleTypeOutput<int>(genesisIndexRequest.Result);
            //Debug.Log("Lengths (HEX): " + lengthsRequest.Result);
            Debug.Log("Genesis index of (level,index)=(" + level + "," + index + "):" + genesisIndex);
        }
        else
        {
            Debug.Log("Error submitting GetGenesisIndex tx: " + genesisIndexRequest.Exception.Message);
        }
    }


    public Coroutine RequestRandomCharacterCoroutine(int level)
    {
        if (lengths[level] == 0) throw new System.Exception("No Character Available.");

        return StartCoroutine(GetCharacterDetails(level, Random.Range(0, lengths[level])));
    }

    private IEnumerator GetCharacterDetails(int level, int index)
    {
        yield return StartCoroutine(GetGenesisIndex(level, index));

        Debug.Log("Fetching character " + genesisIndex);
        requestedCharacter = null;
        var function = contract.GetFunction("getCharacterDetails");
        var input = function.CreateCallInput(genesisIndex);
        var request = new EthCallUnityRequest(_url);
        yield return request.SendRequest(input, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        requestedCharacter = function.DecodeDTOTypeOutput<CharacterDetailsDTO>(request.Result);
        Debug.Log("Character fetched");

        Debug.Log("Requested Character Info: " + requestedCharacter._name + " " +
            requestedCharacter._hp + " " +
            requestedCharacter._str + " " +
            requestedCharacter._int + " " +
            requestedCharacter._luck + " " +
            requestedCharacter._optionalAttrs);
    }

    public Coroutine InsertCharacter(PlayerStats ch, string name, int level)
    {
        DataCollector.Instance.CodeAndSendEvent("upload_character", null);
        return StartCoroutine(InsertCharacterRequest(ch, name, level));
    }

    private TransactionInput CreateInsertCharacterInput(PlayerStats ch, string name, int level)
    {
        var optionalField = "[\"RD\":";
        foreach (Item item in ch.PropertyAffectingItems)
            optionalField += GameManager.Instance.nameToItemIndex[item.itemName].ToString() + ",";
        foreach (Item item in ch.ActionAffectingItems)
            optionalField += GameManager.Instance.nameToItemIndex[item.itemName].ToString() + ",";
        if (ch.SkillItems[0] != null)
            optionalField += GameManager.Instance.nameToItemIndex[ch.SkillItems[0].itemName].ToString() + ",";
        if (ch.SkillItems[1] != null)
            optionalField += GameManager.Instance.nameToItemIndex[ch.SkillItems[1].itemName].ToString() + ",";
        if (optionalField.EndsWith(","))
        {
            optionalField = optionalField.Substring(0, optionalField.Length - 1);
            optionalField += "]";
        }
        else optionalField = string.Empty;
        return coinContract.GetFunction("insertCharacter").CreateTransactionInput(
            accountAddress,
#if !UNITY_EDITOR
			new HexBigInteger(20000000),
			new HexBigInteger(200),
			new HexBigInteger(0),
#endif
            level,
            name,
            ch.MaxHealth / 10,
            50,
            ch.Strength / 10,
            50,
            50,
            ch.Luck / 10,
            50,
            0,
            optionalField
            );
    }

    private IEnumerator InsertCharacterRequest(PlayerStats ch, string name, int level)
    {

        Debug.Log("Inserting Character with luck of " + ch.Luck + " and name of " + name);
        var transactionInput = CreateInsertCharacterInput(ch, name, level);

#if !UNITY_EDITOR
        if(JSInterface.Instance.injectedWeb3Exists)
	        SendTransaction(transactionInput.To,transactionInput.Data);
        else UIManager.Instance.AfterInserting("No Injected Web3");
		yield break;
#else
        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey);
        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);
        if (transactionSignedRequest.Exception == null)
        {
            lastUploadTxnHash = transactionSignedRequest.Result;
            Debug.Log("Character Successfully Inserted!");
            UIManager.Instance.AfterInserting("Success!");
        }
        else
        {
            Debug.Log("Error inserting character: " + transactionSignedRequest.Exception.Message);
            UIManager.Instance.AfterInserting("Failed.");
        }
#endif
    }

    public Coroutine Reward(int index, int amount)
    {
        return StartCoroutine(RewardRequest(index, amount));
    }

    private TransactionInput CreateRewardInput(int index, int amount)
    {
        return coinContract.GetFunction("reward").CreateTransactionInput(
            accountAddress,
#if !UNITY_EDITOR
			new HexBigInteger(9999999),
			new HexBigInteger(200),
			new HexBigInteger(0),
#endif
            index,
            amount
            );
    }

    private IEnumerator RewardRequest(int index, int amount)
    {
        Debug.Log("Rewarding character " + index + " with " + amount + " revivial coin(s)");
        var transactionInput = CreateRewardInput(index, amount);

        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey);
        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);
        if (transactionSignedRequest.Exception == null)
        {
            lastUploadTxnHash = transactionSignedRequest.Result;
            Debug.Log("Character Successfully Rewarded!");
        }
        else
        {
            Debug.Log("Error rewarding character: " + transactionSignedRequest.Exception.Message);
        }
    }

    public Coroutine UseRevivalCoins(int amount, int aim)
    {
        return StartCoroutine(UseRevivalCoinsRequest(amount, aim));
    }

    private TransactionInput CreateUseRevivalCoinsInput(int amount, int aim)
    {
        return coinContract.GetFunction("useRevivalCoins").CreateTransactionInput(
            accountAddress,
#if !UNITY_EDITOR
			new HexBigInteger(20000000),
			new HexBigInteger(200),
			new HexBigInteger(0),
#endif
            amount,
            aim
            );
    }

    private IEnumerator UseRevivalCoinsRequest(int amount, int aim)
    {

        Debug.Log("Try using " + amount + " revival coins");
        var transactionInput = CreateUseRevivalCoinsInput(amount, aim);

#if !UNITY_EDITOR
        if(JSInterface.Instance.injectedWeb3Exists)
		    SendTransaction(transactionInput.To,transactionInput.Data);
        else UIManager.Instance.ReviveFailed("No Injected Web3");
		yield break;
#else
        var transactionSignedRequest = new TransactionSignedUnityRequest(_url, accountPrivateKey);
        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);
        if (transactionSignedRequest.Exception == null)
        {
            lastUploadTxnHash = transactionSignedRequest.Result;
            Debug.Log("Coins successfully used!");
            UIManager.Instance.ReviveSuccessfully();
        }
        else
        {
            Debug.Log("Error using coins: " + transactionSignedRequest.Exception.Message);
            UIManager.Instance.ReviveFailed("Failed to revive.");
        }
#endif
    }
}