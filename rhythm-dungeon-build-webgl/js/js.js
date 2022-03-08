let gameInstance = UnityLoader.instantiate("gameContainer", "Build/rhythm-dungeon-build-webgl.json", { onProgress: UnityProgress });
let initSize = {width: undefined, height:undefined};
let resizeHandler = null,resizeHandlerMobile = null;
let mobileMode = 0,tpFlag = 0,QQbrowser = 0;;
let canvas = document.getElementsByTagName("canvas")[0];
let container = document.getElementById("gameContainer");
let webgl = document.getElementsByClassName("gameouter")[0];
let main = document.getElementsByTagName("main")[0];
let mainContainer = document.getElementsByClassName("mainContainer")[0];
let body = document.getElementsByTagName("body")[0];
let footer = document.getElementsByTagName("footer")[0];
let bottom = document.getElementsByClassName("bottom")[0];
let button = document.getElementsByClassName("buttomGroup")[0];
let modal = document.getElementsByClassName("modal")[0];
let fullScreen = document.getElementById("mobileFullscreen");
let iphone = 0;


async function lockOrientation() {
  if(screen.lockOrientation)
  await  screen.lockOrientation("landscape");
  else if(screen.mozLockOrientation)
  await  screen.mozLockOrientation("landscape");
  else if(screen.msLockOrientation)
  await  screen.msLockOrientation("landscape");
  else if(screen.orientation.lock)
  await  screen.orientation.lock("landscape");
}

async function Fullscreen(ele) {
  if (ele.requestFullscreen) {
      await ele.requestFullscreen();
  } else if (ele.mozRequestFullScreen) {
      await ele.mozRequestFullScreen();
  } else if (ele.msRequestFullscreen) {
      await ele.msRequestFullscreen();
  } else if (ele.webkitRequestFullscreen) {
      await ele.webkitRequestFullScreen();
  }
}



(()=>{
  var userAgent = navigator.userAgent;

  if(userAgent.indexOf('Android') != -1 || userAgent.indexOf('Mobile') != -1 || userAgent.indexOf('iPhone') != -1){
    mobileMode = 1;
  }
  if(userAgent.indexOf('TokenPocket_Android') != -1||userAgent.indexOf('TokenPocket_iOS') != -1){
    tp.setMenubar({
      flag: 1
    });
    
    tp.fullScreen({
      fullScreen: 1
    });
    
    tp.rollHorizontal({
      horizontal: true
    });
    tpFlag = 1;
  }
  if(userAgent.indexOf('QQ/') == -1 && userAgent.indexOf('MQQBrowser') != -1){
    QQbrowser = 1;
  }
  if(userAgent.indexOf("UCBrowser") != -1){
    QQbrowser = 1;
  }
  if(userAgent.indexOf("iPhone") != -1){
    iphone = 1;
  }
})()

window.addEventListener('load', function () {
  //钱包接口
  // Checking if Web3 has been injected by the browser (Mist/MetaMask)
  if (typeof web3 !== 'undefined') {
    web3js = new Web3(web3.currentProvider);
    web3js.version.getNetwork(async (err, netId) => {
      if (netId != 1) {
        document.getElementById("metamaskWarning").innerText = 'Please connect to mainnet to submit your character and revive.';
        web3js = undefined;
      } else {
        document.getElementById("metamaskWarning").innerText = '';
        try {
          await ethereum.enable();
        } catch (error) {
          web3js = undefined;
          document.getElementById("metamaskWarning").innerText = 'You denied the access to your wallet.';
        }
      }
    });
  } else {
    document.getElementById("metamaskWarning").innerText = 'Please install Metamask and connect to mainnet to upload your character and revive.';
  }
    //修改尺寸  
  if(!mobileMode) 
    changeMainSize();
  else if(tpFlag == 1 || QQbrowser == 1)
    mobilePreChange();
  else
    normalMoblie();
  body.style.display = "block";

});



if (typeof web3js != "undefined") {
  let abi = [{ "constant": false, "inputs": [], "name": "withdraw", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": false, "inputs": [], "name": "ownerWithdraw", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [], "name": "isPriceAssigned", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": true, "inputs": [{ "internalType": "address", "name": "tokenOwner", "type": "address" }], "name": "balanceOf", "outputs": [{ "internalType": "uint256", "name": "balance", "type": "uint256" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": true, "inputs": [], "name": "owner", "outputs": [{ "internalType": "address payable", "name": "", "type": "address" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "internalType": "uint256", "name": "newPrice", "type": "uint256" }], "name": "setPrice", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [], "name": "price", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "internalType": "uint256", "name": "index", "type": "uint256" }, { "internalType": "uint256", "name": "amount", "type": "uint256" }], "name": "reward", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": false, "inputs": [{ "internalType": "string", "name": "_name", "type": "string" }, { "internalType": "uint256", "name": "_hp", "type": "uint256" }, { "internalType": "uint256", "name": "_mp", "type": "uint256" }, { "internalType": "uint256", "name": "_str", "type": "uint256" }, { "internalType": "uint256", "name": "_intelli", "type": "uint256" }, { "internalType": "uint256", "name": "_san", "type": "uint256" }, { "internalType": "uint256", "name": "_luck", "type": "uint256" }, { "internalType": "uint256", "name": "_charm", "type": "uint256" }, { "internalType": "uint256", "name": "_mt", "type": "uint256" }, { "internalType": "string", "name": "_optionalAttrs", "type": "string" }], "name": "insertCharacter", "outputs": [{ "internalType": "uint256", "name": "", "type": "uint256" }], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [], "name": "isGenesisSet", "outputs": [{ "internalType": "bool", "name": "", "type": "bool" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "internalType": "uint256", "name": "amount", "type": "uint256" }, { "internalType": "uint256", "name": "aim", "type": "uint256" }], "name": "useRevivalCoins", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [{ "internalType": "address", "name": "tokenOwner", "type": "address" }], "name": "coinBalanceOf", "outputs": [{ "internalType": "uint256", "name": "balance", "type": "uint256" }], "payable": false, "stateMutability": "view", "type": "function" }, { "constant": false, "inputs": [{ "internalType": "uint256", "name": "amount", "type": "uint256" }], "name": "buy", "outputs": [], "payable": true, "stateMutability": "payable", "type": "function" }, { "constant": false, "inputs": [{ "internalType": "address", "name": "genesisAddress", "type": "address" }], "name": "setGenesis", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "inputs": [], "payable": false, "stateMutability": "nonpayable", "type": "constructor" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "uint256", "name": "newPrice", "type": "uint256" }], "name": "SetPrice", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "address", "name": "buyer", "type": "address" }, { "indexed": false, "internalType": "uint256", "name": "price", "type": "uint256" }, { "indexed": false, "internalType": "uint256", "name": "amount", "type": "uint256" }], "name": "Buy", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "address", "name": "user", "type": "address" }, { "indexed": false, "internalType": "uint256", "name": "amount", "type": "uint256" }, { "indexed": false, "internalType": "uint256", "name": "aim", "type": "uint256" }], "name": "CoinBalanceInsufficient", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "address", "name": "user", "type": "address" }, { "indexed": false, "internalType": "uint256", "name": "amount", "type": "uint256" }, { "indexed": false, "internalType": "uint256", "name": "aim", "type": "uint256" }], "name": "SuccessfullyUse", "type": "event" }, { "anonymous": false, "inputs": [{ "indexed": false, "internalType": "address", "name": "receiver", "type": "address" }, { "indexed": false, "internalType": "uint256", "name": "amount", "type": "uint256" }], "name": "Reward", "type": "event" }];
  let revivalCoinContract = web3js.eth.contract(abi);
  let revivalCoin = revivalCoinContract.at('0x1FB12429900de812942a8baea2f26E34b69fAAB0');
  let successfullyUseEvent = revivalCoin.SuccessfullyUse();
  successfullyUseEvent.watch(function (error, result) {
    if (!error) {
      if (result.args.user == ethereum.selectedAddress)
        gameInstance.SendMessage("JSInterface", "SuccessfullyRevival", result.args.amount.toString());
      } else { console.log(error); }      
    });    
}
//钱包接口结束


function safariAddressBar(){
  if(resizeHandlerMobile) clearTimeout(resizeHandlerMobile);
  else{
    resizeHandlerMobile = setTimeout(fullSize,300)
  } 
}

function normalMoblie(){
  fullscreenChange();
  displaySwitcher([modal],"flex")
  mobileBackground();
  displaySwitcher([webgl]);
  if(iphone && !tpFlag){
    Full();
    window.onresize = safariAddressBar;
    window.addEventListener("orientationchange",()=>{setTimeout(safariDirection,100);});
    safariDirection();
  }
  fullScreen.addEventListener("click",()=>{
    Full();
  })  
}

function safariDirection(){
  if(window.innerHeight>window.innerWidth){    
    modal.style.display = "flex"
    webgl.style.display = "none"
    displaySwitcher([modal.children[2]],"none");
    modal.children[0].innerText = "Please rotate your screen.";
  }else {
    modal.style.display = "none"
    webgl.style.display = "flex"
    setTimeout(fullSize,1000);
  }
}

async function Full() {
  await Fullscreen(container)
  await lockOrientation()
  displaySwitcher([webgl]);
  displaySwitcher([modal],"none");
  setTimeout(fullSize,1000);
}

function mobilePreChange() {
    mobileBackground()
    body.style.backgroundImage = "url()";
    setTimeout(fullSize,1000);
}

function fullscreenChange() {
  document.onfullscreenchange = ()=>{
    if(!document.fullscreenElement){
      displaySwitcher([webgl]);
      displaySwitcher([modal],"flex");
    }
  }
}

function mobileBackground(){
  displaySwitcher([footer,bottom,button],"none");
  webgl.style.marginTop = "0";
  webgl.style.paddingBottom = "0";
  webgl.style.backgroundImage = "url()";
//  body.style.backgroundImage = "url()";
}

async function fullSize() {
    let width = Math.max(window.innerHeight,window.innerWidth);
    let height = Math.min(window.innerHeight,window.innerWidth);
    container.style.width = width + 'px';
    container.style.height = height + 'px';
    canvas.style.width = width + 'px';
    canvas.style.height = height + 'px'; 
    canvas.style.position = "fixed";
}



function responsibleSize(){
  if(resizeHandler) clearTimeout(resizeHandler);
    resizeHandler = setTimeout(() => {
      let container = document.getElementById("gameContainer");
      if(initSize.width > parseInt(document.body.clientWidth)){
        let Width = document.body.clientWidth;
        mainContainer.style.width = Width + 'px';
        canvas.style.width =  (Width) + 'px';
        canvas.style.height = (Width/7)*4 + 'px';
        webgl.style.backgroundSize = 2940*(Width/1820) + 'px'; 
        main.style.paddingTop = 420*(Width/1820) + 'px'; 
        container.style.height = (Width/7)*4 + 'px';
        container.style.width = Width + 'px';

      }else{
        let Width = initSize.width;
        mainContainer.style.width = Width + 'px';
        canvas.style.width =  (Width) + 'px';
        canvas.style.height = (Width/7)*4 + 'px';
        webgl.style.backgroundSize = 2940*(Width/1820) + 'px'; 
        main.style.paddingTop = 420*(Width/1820) + 'px'; 
        container.style.height = (Width/7)*4 + 'px';
        container.style.width = Width + 'px';

      }
    }, 500);
}


function changeMainSize() {
  window.onresize = responsibleSize;
  let scale = 0.6; 
  const scaleFlag = (window.screen.width/window.screen.height) - (16/9);
  if(scaleFlag<0){
    let Height = window.screen.height*scale;
    container.style.width = (Height/4)*7 + 'px';
    container.style.height = Height + 'px';
    mainContainer.style.width = (Height/4)*7 + 'px';
    webgl.style.backgroundSize = 2940*(Height/1040) + 'px'; 
    main.style.paddingTop = 420*(Height/1040) + 'px'; 
  }else{
    let Width = window.screen.width*scale;
    container.style.width = Width + 'px';
    container.style.height = (Width/7)*4 + 'px';
    mainContainer.style.width = Width + 'px';
    webgl.style.backgroundSize = 2940*(Width/1820) + 'px'; 
    main.style.paddingTop = 420*(Width/1820) + 'px'; 
  }
  initSize.width = parseInt(container.style.width);
  initSize.height = parseInt(container.style.height);
}

function changeSize(width,scale) {
  canvas.style.width = width + 'px';
  canvas.style.height = width * scale + 'px'; 
  container.style.width = width + 'px';
  container.style.height = width * scale + 'px';
}


function sizeChanger(doms,width,height,scale,unit){
  if(!scale) height = width*scale;
  for(dom of doms){
    dom.style.width = width + unit;
    dom.style.height = height + unit;
  }
}

function displaySwitcher(doms,display = undefined) {
  if(display == undefined)
    for(dom of doms){
      console.log(dom)
      console.log(dom.style.display)
      dom.style.display = (dom.style.display == ""|| dom.style.display == "block"|| dom.style.display == undefined) ? "none" : "block";
      console.log(dom.style.display)
    }
  else
    for(dom of doms)
      dom.style.display = (dom.style.display == display ||  dom.style.display == '') ? "none" : display;
}