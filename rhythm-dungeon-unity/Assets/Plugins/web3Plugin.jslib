//This file allows you to interop with Web3js / Metamask include it in your assets folder

mergeInto(LibraryManager.library, {
  SendTransaction: function (to, data) {
    var tostr = Pointer_stringify(to);
    var from = web3.eth.accounts[0];
    var datastr = Pointer_stringify(data);
    web3.eth.sendTransaction({ from: from, to: tostr, data: datastr }, function (error, hash) {
      if (error) {
        console.log(error);
      }
      else {
        console.log(hash);
      }
    }
    );
  },
  CheckWeb3: function () {
    if (typeof web3 != "undefined")
      gameInstance.SendMessage("JSInterface", "ThereIsInjectedWeb3");
  },
});
