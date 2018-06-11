// ts2fable 0.6.1
module rec W3
open System
open Fable.Core
open Fable.Import.JS

type JsonRPCRequest = __jsonrpc.JsonRPCRequest
type JsonRPCResponse = __jsonrpc.JsonRPCResponse
type BigNumber = Bignumber_js.BigNumber

type [<AllowNullLiteral>] IExports =
    /// Convert number or hex string to BigNumber 
    abstract toBN: value: float -> BigNumber
    abstract W3: W3Static

/// Strongly-types wrapper over web3.js with additional helper methods.
type [<AllowNullLiteral>] W3 =
    abstract _counter: obj with get, set
    abstract _default: obj with get, set
    /// Default W3 instance that is used as a fallback when such an instance is not provided to a construct constructor.
    /// You must set it explicitly via W3.Default setter. Use an empty `new W3()` constructor to get an instance that
    /// automatically resolves to a global web3 instance `window['web3']` provided by e.g. MIST/Metamask or connects
    /// to the default 8545 port if
    /// no global instance is present.
    abstract Default: W3 with get, set
    abstract providers: W3.Providers with get, set
    abstract givenProvider: W3.Provider with get, set
    abstract modules: obj with get, set
    abstract currentProvider: W3.Provider
    abstract eth: W3.Eth
    abstract version: W3.Version0
    abstract utils: W3.Utils
    /// Convert number or hex string to BigNumber 
    abstract toBigNumber: value: U2<float, string> -> BigNumber
    abstract fromDecimal: value: U2<float, string> -> string
    /// web3.js untyped instance created with a resolved or given in ctor provider, if any.
    abstract web3: obj option with get, set
    abstract globalWeb3: obj with get, set
    abstract netId: obj with get, set
    abstract netNode: obj with get, set
    abstract _defaultAccount: obj with get, set
    abstract defaultAccount: string with get, set
    /// Request netid string from ctor or after provider change 
    abstract updateNetworkInfo: ?netid: obj -> unit
    abstract accounts: Promise<ResizeArray<string>>
    abstract web3API: string
    abstract isPre1API: bool
    abstract isTestRPC: Promise<bool>
    /// Get network ID as a promise. 
    abstract networkId: Promise<string>
    abstract setProvider: provider: W3.Provider -> unit
    abstract sendRPC: payload: JsonRPCRequest -> Promise<JsonRPCResponse>
    /// Returns the time of the last mined block in seconds. 
    abstract latestTime: Promise<float>
    abstract blockNumber: Promise<float>
    /// Async unlock while web3.js only has sync version 
    abstract unlockAccount: address: string * password: string * ?duration: float -> Promise<bool>
    abstract getBalance: address: string -> Promise<BigNumber>
    /// Sign using eth.sign but with prefix as personal_sign 
    abstract ethSignRaw: hex: string * account: string -> Promise<string>
    /// Sign a message 
    abstract sign: message: string * account: string * ?password: string -> Promise<string>
    /// Message already as hex 
    abstract signRaw: message: obj option * account: string * ?password: string -> Promise<string>
    /// Recover signature address 
    abstract ecRecover: message: string * signature: string -> Promise<string>
    abstract ecRecoverRaw: message: obj option * signature: string -> Promise<string>
    abstract isMetaMask: bool
    abstract getTransactionCount: ?account: string * ?defaultBlock: U2<float, string> -> Promise<float>
    /// <summary>Sends a raw signed transaction and returns tx hash. Use waitTransactionReceipt method on w3 or a contract to get a tx receipt.</summary>
    /// <param name="to">Target contract address or zero address (W3.zeroAddress) to deploy a new contract.</param>
    /// <param name="privateKey">Private key hex string prefixed with 0x.</param>
    /// <param name="data">Payload data.</param>
    /// <param name="txParams">Tx parameters.</param>
    /// <param name="nonce">Nonce override if needed to replace a pending transaction.</param>
    abstract sendSignedTransaction: ``to``: string * privateKey: string * ?data: string * ?txParams: W3.TX.TxParams * ?nonce: float -> Promise<string>
    abstract waitTransactionReceipt: hashString: string -> Promise<W3.TransactionReceipt>

/// Strongly-types wrapper over web3.js with additional helper methods.
type [<AllowNullLiteral>] W3Static =
    abstract NextCounter: unit -> float
    /// Create a default Web3 instance - resolves to a global window['web3'] injected my MIST, MetaMask, etc
    /// or to `localhost:8545` if not running on https.
    [<Emit "new $0($1...)">] abstract Create: unit -> W3
    /// <summary>Create a W3 instance with a given provider.</summary>
    /// <param name="provider">web3.js provider.</param>
    [<Emit "new $0($1...)">] abstract Create: ?provider: W3.Provider -> W3

module W3 =
    let [<Import("TX","W3/W3")>] tX: TX.IExports = jsNative

    type [<AllowNullLiteral>] IExports =
        abstract zeroAddress: string
        abstract isValidAddress: addr: address -> bool
        abstract Utf8: obj option
        /// <summary>Convert value to hex with optional left padding. If a string is already a hex it will be converted to lower case.</summary>
        /// <param name="value">Value to convert to hex</param>
        /// <param name="size">Size of number in bits (8 for int8, 16 for uint16, etc)</param>
        abstract toHex: value: U3<float, string, BigNumber> * ?stripPrefix: bool * ?size: float -> string
        abstract leftPad: str: string * len: float * ch: obj option -> string
        abstract utf8ToHex: str: string * ?stripPrefix: bool -> string
        abstract EthUtils: W3.EthUtils
        abstract sha3: a: U4<Buffer, Array<obj option>, string, float> * ?bits: float -> string
        abstract sha256: a: U4<Buffer, Array<obj option>, string, float> -> string
        abstract sign: message: obj option * privateKey: string -> bytes
        abstract ecrecover: message: string * signature: string -> address
        /// https://github.com/ethereumjs/keythereum
        abstract getKeythereum: unit -> obj option
        abstract duration: obj

    type address =
        string

    type bytes =
        string

    module TX =

        type [<AllowNullLiteral>] IExports =
            /// 4500000 gas @ 2 Gwei 
            abstract txParamsDefaultDeploy: from: address * ?gas: float * ?gasPrice: float -> TxParams
            /// 50000 gas @ 2 Gwei 
            abstract txParamsDefaultSend: from: address * ?gas: float * ?gasPrice: float -> TxParams
            abstract getEthereumjsTx: unit -> obj option

        type [<AllowNullLiteral>] TxParams =
            abstract from: address with get, set
            abstract gas: U2<float, BigNumber> with get, set
            abstract gasPrice: U2<float, BigNumber> with get, set
            abstract value: U2<float, BigNumber> with get, set

        type ContractDataType =
            U7<BigNumber, float, string, bool, ResizeArray<BigNumber>, ResizeArray<float>, ResizeArray<string>>

        [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module ContractDataType =
            let ofBigNumber v: ContractDataType = v |> U7.Case1
            let isBigNumber (v: ContractDataType) = match v with U7.Case1 _ -> true | _ -> false
            let asBigNumber (v: ContractDataType) = match v with U7.Case1 o -> Some o | _ -> None
            let ofFloat v: ContractDataType = v |> U7.Case2
            let isFloat (v: ContractDataType) = match v with U7.Case2 _ -> true | _ -> false
            let asFloat (v: ContractDataType) = match v with U7.Case2 o -> Some o | _ -> None
            let ofString v: ContractDataType = v |> U7.Case3
            let isString (v: ContractDataType) = match v with U7.Case3 _ -> true | _ -> false
            let asString (v: ContractDataType) = match v with U7.Case3 o -> Some o | _ -> None
            let ofBool v: ContractDataType = v |> U7.Case4
            let isBool (v: ContractDataType) = match v with U7.Case4 _ -> true | _ -> false
            let asBool (v: ContractDataType) = match v with U7.Case4 o -> Some o | _ -> None
            let ofBigNumberArray v: ContractDataType = v |> U7.Case5
            let isBigNumberArray (v: ContractDataType) = match v with U7.Case5 _ -> true | _ -> false
            let asBigNumberArray (v: ContractDataType) = match v with U7.Case5 o -> Some o | _ -> None
            let ofFloatArray v: ContractDataType = v |> U7.Case6
            let isFloatArray (v: ContractDataType) = match v with U7.Case6 _ -> true | _ -> false
            let asFloatArray (v: ContractDataType) = match v with U7.Case6 o -> Some o | _ -> None
            let ofStringArray v: ContractDataType = v |> U7.Case7
            let isStringArray (v: ContractDataType) = match v with U7.Case7 _ -> true | _ -> false
            let asStringArray (v: ContractDataType) = match v with U7.Case7 o -> Some o | _ -> None

        type [<AllowNullLiteral>] TransactionResult =
            /// Transaction hash. 
            abstract tx: string with get, set
            abstract receipt: TransactionReceipt with get, set
            /// This array has decoded events, while reseipt.logs has raw logs when returned from TC transaction 
            abstract logs: ResizeArray<Log> with get, set

    type [<AllowNullLiteral>] Provider =
        abstract sendAsync: payload: JsonRPCRequest * callback: (Error -> JsonRPCResponse -> unit) -> unit

    type [<AllowNullLiteral>] WebsocketProvider =
        inherit Provider

    type [<AllowNullLiteral>] HttpProvider =
        inherit Provider

    type [<AllowNullLiteral>] IpcProvider =
        inherit Provider

    type [<AllowNullLiteral>] Providers =
        abstract WebsocketProvider: obj with get, set
        abstract HttpProvider: obj with get, set
        abstract IpcProvider: obj with get, set

    type [<StringEnum>] [<RequireQualifiedAccess>] Unit =
        | Kwei
        | Femtoether
        | Babbage
        | Mwei
        | Picoether
        | Lovelace
        | Qwei
        | Nanoether
        | Shannon
        | Microether
        | Szabo
        | Nano
        | Micro
        | Milliether
        | Finney
        | Milli
        | Ether
        | Kether
        | Grand
        | Mether
        | Gether
        | Tether

    type BlockType =
        U4<string, string, string, float>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module BlockType =
        let ofCase1 v: BlockType = v |> U4.Case1
        let isCase1 (v: BlockType) = match v with U4.Case1 _ -> true | _ -> false
        let asCase1 (v: BlockType) = match v with U4.Case1 o -> Some o | _ -> None
        let ofCase2 v: BlockType = v |> U4.Case2
        let isCase2 (v: BlockType) = match v with U4.Case2 _ -> true | _ -> false
        let asCase2 (v: BlockType) = match v with U4.Case2 o -> Some o | _ -> None
        let ofCase3 v: BlockType = v |> U4.Case3
        let isCase3 (v: BlockType) = match v with U4.Case3 _ -> true | _ -> false
        let asCase3 (v: BlockType) = match v with U4.Case3 o -> Some o | _ -> None
        let ofFloat v: BlockType = v |> U4.Case4
        let isFloat (v: BlockType) = match v with U4.Case4 _ -> true | _ -> false
        let asFloat (v: BlockType) = match v with U4.Case4 o -> Some o | _ -> None

    type [<AllowNullLiteral>] BatchRequest =
        abstract add: request: Request -> unit
        abstract execute: unit -> unit

    type [<AllowNullLiteral>] Iban =
        interface end

    type [<AllowNullLiteral>] Utils =
        abstract BN: BigNumber with get, set
        abstract isBN: obj: obj option -> bool
        abstract isBigNumber: obj: obj option -> bool
        abstract isAddress: obj: obj option -> bool
        abstract isHex: obj: obj option -> bool
        abstract asciiToHex: ``val``: string -> string
        abstract hexToAscii: ``val``: string -> string
        abstract bytesToHex: ``val``: ResizeArray<float> -> string
        abstract numberToHex: ``val``: U2<float, BigNumber> -> string
        abstract checkAddressChecksum: address: string -> bool
        abstract fromAscii: ``val``: string -> string
        abstract fromDecimal: ``val``: U3<string, float, BigNumber> -> string
        abstract fromUtf8: ``val``: string -> string
        abstract fromWei: ``val``: U3<string, float, BigNumber> * unit: Unit -> U2<string, BigNumber>
        abstract hexToBytes: ``val``: string -> ResizeArray<float>
        abstract hexToNumber: ``val``: U3<string, float, BigNumber> -> float
        abstract hexToNumberString: ``val``: U3<string, float, BigNumber> -> string
        abstract hexToString: ``val``: string -> string
        abstract hexToUtf8: ``val``: string -> string
        abstract keccak256: ``val``: string -> string
        abstract leftPad: str: string * chars: float * sign: string -> string
        abstract padLeft: str: string * chars: float * sign: string -> string
        abstract rightPad: str: string * chars: float * sign: string -> string
        abstract padRight: str: string * chars: float * sign: string -> string
        abstract sha3: ``val``: string * ?val2: string * ?val3: string * ?val4: string * ?val5: string -> string
        abstract soliditySha3: ``val``: string -> string
        abstract randomHex: bytes: float -> string
        abstract stringToHex: ``val``: string -> string
        abstract toAscii: hex: string -> string
        abstract toBN: obj: obj option -> BigNumber
        abstract toChecksumAddress: ``val``: string -> string
        abstract toDecimal: ``val``: obj option -> float
        abstract toHex: ``val``: obj option -> string
        abstract toUtf8: ``val``: obj option -> string
        abstract toWei: ``val``: U3<string, float, BigNumber> * unit: Unit -> U2<string, BigNumber>
        abstract unitMap: obj option with get, set

    /// https://github.com/ethereumjs/ethereumjs-util/blob/master/docs/index.md
    type [<AllowNullLiteral>] EthUtils =
        abstract BN: BigNumber with get, set
        abstract addHexPrefix: str: string -> string
        abstract baToJSON: ba: U2<Buffer, Array<obj option>> -> obj option
        abstract bufferToHex: buf: Buffer -> string
        abstract bufferToInt: buf: Buffer -> float
        abstract ecrecover: msgHash: Buffer * v: float * r: Buffer * s: Buffer -> Buffer
        abstract ecsign: msgHash: Buffer * privateKey: Buffer -> obj option
        abstract fromRpcSig: ``sig``: string -> obj option
        abstract fromSigned: num: Buffer -> obj option
        abstract generateAddress: from: Buffer * nonce: Buffer -> Buffer
        abstract hashPersonalMessage: message: Buffer -> Buffer
        abstract importPublic: publicKey: Buffer -> Buffer
        abstract isValidAddress: address: string -> bool
        abstract isValidChecksumAddress: address: Buffer -> bool
        abstract isValidPrivate: privateKey: Buffer -> bool
        abstract isValidPublic: privateKey: Buffer * sanitize: bool -> obj option
        abstract isValidSignature: v: float * r: Buffer * s: Buffer * ?homestead: bool -> obj option
        abstract privateToAddress: privateKey: Buffer -> Buffer
        abstract pubToAddress: privateKey: Buffer * ?sanitize: bool -> Buffer
        abstract sha256: a: U4<Buffer, Array<obj option>, string, float> -> Buffer
        /// Keccak[bits] 
        abstract sha3: a: U4<Buffer, Array<obj option>, string, float> * ?bits: float -> Buffer
        abstract SHA3_NULL: Buffer with get, set
        abstract SHA3_NULL_S: string with get, set
        abstract toBuffer: v: obj option -> Buffer
        abstract toChecksumAddress: address: string -> string
        abstract toRpcSig: v: float * r: Buffer * s: Buffer -> string
        abstract privateToPublic: privateKey: Buffer -> Buffer
        abstract zeros: bytes: float -> Buffer

    type [<AllowNullLiteral>] Callback<'T> =
        [<Emit "$0($1...)">] abstract Invoke: error: Error * result: 'T -> unit

    type ABIDataTypes =
        U5<string, string, string, string, string>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module ABIDataTypes =
        let ofCase1 v: ABIDataTypes = v |> U5.Case1
        let isCase1 (v: ABIDataTypes) = match v with U5.Case1 _ -> true | _ -> false
        let asCase1 (v: ABIDataTypes) = match v with U5.Case1 o -> Some o | _ -> None
        let ofCase2 v: ABIDataTypes = v |> U5.Case2
        let isCase2 (v: ABIDataTypes) = match v with U5.Case2 _ -> true | _ -> false
        let asCase2 (v: ABIDataTypes) = match v with U5.Case2 o -> Some o | _ -> None
        let ofCase3 v: ABIDataTypes = v |> U5.Case3
        let isCase3 (v: ABIDataTypes) = match v with U5.Case3 _ -> true | _ -> false
        let asCase3 (v: ABIDataTypes) = match v with U5.Case3 o -> Some o | _ -> None
        let ofCase4 v: ABIDataTypes = v |> U5.Case4
        let isCase4 (v: ABIDataTypes) = match v with U5.Case4 _ -> true | _ -> false
        let asCase4 (v: ABIDataTypes) = match v with U5.Case4 o -> Some o | _ -> None
        let ofString v: ABIDataTypes = v |> U5.Case5
        let isString (v: ABIDataTypes) = match v with U5.Case5 _ -> true | _ -> false
        let asString (v: ABIDataTypes) = match v with U5.Case5 o -> Some o | _ -> None

    type [<AllowNullLiteral>] ABIDefinition =
        abstract constant: bool option with get, set
        abstract payable: bool option with get, set
        abstract anonymous: bool option with get, set
        abstract inputs: Array<obj> option with get, set
        abstract name: string option with get, set
        abstract outputs: Array<obj> option with get, set
        abstract ``type``: U4<string, string, string, string> with get, set

    type [<AllowNullLiteral>] CompileResult =
        abstract code: string with get, set
        abstract info: obj with get, set
        abstract userDoc: obj with get, set
        abstract developerDoc: obj with get, set

    type [<AllowNullLiteral>] Transaction =
        abstract hash: string with get, set
        abstract nonce: float with get, set
        abstract blockHash: string with get, set
        abstract blockNumber: float with get, set
        abstract transactionIndex: float with get, set
        abstract from: string with get, set
        abstract ``to``: string with get, set
        abstract value: string with get, set
        abstract gasPrice: string with get, set
        abstract gas: float with get, set
        abstract input: string with get, set
        abstract v: string option with get, set
        abstract r: string option with get, set
        abstract s: string option with get, set

    type [<AllowNullLiteral>] EventLog =
        abstract ``event``: string with get, set
        abstract address: string with get, set
        abstract returnValues: obj with get, set
        abstract logIndex: float with get, set
        abstract transactionIndex: float with get, set
        abstract transactionHash: string with get, set
        abstract blockHash: string with get, set
        abstract blockNumber: float with get, set
        abstract raw: obj option with get, set

    type [<AllowNullLiteral>] TransactionReceipt =
        abstract transactionHash: string with get, set
        abstract transactionIndex: float with get, set
        abstract blockHash: string with get, set
        abstract blockNumber: float with get, set
        abstract from: string with get, set
        abstract ``to``: string with get, set
        abstract contractAddress: string with get, set
        abstract cumulativeGasUsed: float with get, set
        abstract gasUsed: float with get, set
        abstract logs: ResizeArray<Log> option with get, set
        abstract events: obj option with get, set

    type [<AllowNullLiteral>] BlockHeader =
        abstract number: float with get, set
        abstract hash: string with get, set
        abstract parentHash: string with get, set
        abstract nonce: string with get, set
        abstract sha3Uncles: string with get, set
        abstract logsBloom: string with get, set
        abstract transactionRoot: string with get, set
        abstract stateRoot: string with get, set
        abstract receiptRoot: string with get, set
        abstract miner: string with get, set
        abstract extraData: string with get, set
        abstract gasLimit: float with get, set
        abstract gasUsed: float with get, set
        abstract timestamp: float with get, set

    type [<AllowNullLiteral>] Block =
        inherit BlockHeader
        abstract transactions: Array<Transaction> with get, set
        abstract size: float with get, set
        abstract difficulty: float with get, set
        abstract totalDifficulty: float with get, set
        abstract uncles: Array<string> with get, set

    type [<AllowNullLiteral>] Logs =
        abstract fromBlock: float option with get, set
        abstract address: string option with get, set
        abstract topics: Array<U2<string, ResizeArray<string>>> option with get, set

    type [<AllowNullLiteral>] Log =
        /// true when the log was removed, due to a chain reorganization. false if its a valid log. 
        abstract removed: bool option with get, set
        abstract logIndex: float with get, set
        abstract transactionIndex: float with get, set
        abstract transactionHash: string with get, set
        abstract blockHash: string with get, set
        abstract blockNumber: float with get, set
        abstract address: string with get, set
        abstract data: string option with get, set
        abstract topics: Array<string> option with get, set
        /// Truffle-contract returns this as 'mined' 
        abstract ``type``: string option with get, set
        /// Event name decoded by Truffle-contract 
        abstract ``event``: string option with get, set
        /// Args passed to a Truffle-contract method 
        abstract args: obj option with get, set

    type [<AllowNullLiteral>] Subscribe<'T> =
        abstract subscription: obj with get, set
        [<Emit "$0.on('data',$1)">] abstract on_data: handler: ('T -> unit) -> unit
        [<Emit "$0.on('changed',$1)">] abstract on_changed: handler: ('T -> unit) -> unit
        [<Emit "$0.on('error',$1)">] abstract on_error: handler: (Error -> unit) -> unit

    type [<AllowNullLiteral>] Account =
        abstract address: string with get, set
        abstract privateKey: string with get, set
        abstract publicKey: string with get, set

    type [<AllowNullLiteral>] PrivateKey =
        abstract address: string with get, set
        abstract Crypto: obj with get, set
        abstract id: string with get, set
        abstract version: float with get, set

    type [<AllowNullLiteral>] Signature =
        abstract message: string with get, set
        abstract hash: string with get, set
        abstract r: string with get, set
        abstract s: string with get, set
        abstract v: string with get, set

    type [<AllowNullLiteral>] Tx =
        abstract nonce: U2<string, float> option with get, set
        abstract chainId: U2<string, float> option with get, set
        abstract from: string option with get, set
        abstract ``to``: string option with get, set
        abstract data: string option with get, set
        abstract value: U2<string, float> option with get, set
        abstract gas: U2<string, float> option with get, set
        abstract gasPrice: U2<string, float> option with get, set

    type [<AllowNullLiteral>] ContractOptions =
        abstract address: string with get, set
        abstract jsonInterface: ResizeArray<ABIDefinition> with get, set
        abstract from: string option with get, set
        abstract gas: U3<string, float, BigNumber> option with get, set
        abstract gasPrice: float option with get, set
        abstract data: string option with get, set

    type [<StringEnum>] [<RequireQualifiedAccess>] PromiEventType =
        | TransactionHash
        | Receipt
        | Confirmation
        | Error

    type [<AllowNullLiteral>] PromiEvent<'T> =
        inherit Promise<'T>
        [<Emit "$0.once('transactionHash',$1)">] abstract once_transactionHash: handler: (string -> unit) -> PromiEvent<'T>
        [<Emit "$0.once('receipt',$1)">] abstract once_receipt: handler: (TransactionReceipt -> unit) -> PromiEvent<'T>
        [<Emit "$0.once('confirmation',$1)">] abstract once_confirmation: handler: (float -> TransactionReceipt -> unit) -> PromiEvent<'T>
        [<Emit "$0.once('error',$1)">] abstract once_error: handler: (Error -> unit) -> PromiEvent<'T>
        abstract once: ``type``: U4<string, string, string, string> * handler: (U3<Error, TransactionReceipt, string> -> unit) -> PromiEvent<'T>
        [<Emit "$0.on('transactionHash',$1)">] abstract on_transactionHash: handler: (string -> unit) -> PromiEvent<'T>
        [<Emit "$0.on('receipt',$1)">] abstract on_receipt: handler: (TransactionReceipt -> unit) -> PromiEvent<'T>
        [<Emit "$0.on('confirmation',$1)">] abstract on_confirmation: handler: (float -> TransactionReceipt -> unit) -> PromiEvent<'T>
        [<Emit "$0.on('error',$1)">] abstract on_error: handler: (Error -> unit) -> PromiEvent<'T>
        abstract on: ``type``: U4<string, string, string, string> * handler: (U3<Error, TransactionReceipt, string> -> unit) -> PromiEvent<'T>

    type [<AllowNullLiteral>] EventEmitter =
        [<Emit "$0.on('data',$1)">] abstract on_data: handler: (EventLog -> unit) -> EventEmitter
        [<Emit "$0.on('changed',$1)">] abstract on_changed: handler: (EventLog -> unit) -> EventEmitter
        [<Emit "$0.on('error',$1)">] abstract on_error: handler: (Error -> unit) -> EventEmitter
        abstract on: ``type``: U3<string, string, string> * handler: (U3<Error, TransactionReceipt, string> -> unit) -> EventEmitter

    type [<AllowNullLiteral>] TransactionObject<'T> =
        abstract arguments: ResizeArray<obj option> with get, set
        abstract call: ?tx: Tx -> Promise<'T>
        abstract send: ?tx: Tx -> PromiEvent<'T>
        abstract estimateGas: ?tx: Tx -> Promise<float>
        abstract encodeABI: unit -> string

    type [<AllowNullLiteral>] Contract =
        abstract options: ContractOptions with get, set
        abstract methods: obj with get, set
        abstract deploy: options: ContractDeployOptions -> TransactionObject<Contract>
        abstract events: obj with get, set

    type [<AllowNullLiteral>] ContractDeployOptions =
        abstract data: string with get, set
        abstract arguments: ResizeArray<obj option> with get, set

    type [<AllowNullLiteral>] Eth =
        abstract defaultAccount: string
        abstract defaultBlock: BlockType
        abstract BatchRequest: obj with get, set
        abstract Iban: obj with get, set
        abstract Contract: obj with get, set
        abstract abi: obj with get, set
        abstract accounts: obj with get, set
        abstract call: callObject: Tx * ?defaultBloc: BlockType * ?callBack: Callback<string> -> Promise<string>
        abstract clearSubscriptions: unit -> bool
        [<Emit "$0.subscribe('logs',$1,$2)">] abstract subscribe_logs: ?options: Logs * ?callback: Callback<Subscribe<Log>> -> Promise<Subscribe<Log>>
        [<Emit "$0.subscribe('syncing',$1)">] abstract subscribe_syncing: ?callback: Callback<Subscribe<obj option>> -> Promise<Subscribe<obj option>>
        [<Emit "$0.subscribe('newBlockHeaders',$1)">] abstract subscribe_newBlockHeaders: ?callback: Callback<Subscribe<BlockHeader>> -> Promise<Subscribe<BlockHeader>>
        [<Emit "$0.subscribe('pendingTransactions',$1)">] abstract subscribe_pendingTransactions: ?callback: Callback<Subscribe<Transaction>> -> Promise<Subscribe<Transaction>>
        abstract subscribe: ``type``: U4<string, string, string, string> * ?options: Logs * ?callback: Callback<Subscribe<U3<Transaction, BlockHeader, obj option>>> -> Promise<Subscribe<U3<Transaction, BlockHeader, obj option>>>
        abstract unsubscribe: callBack: Callback<bool> -> U2<unit, bool>
        abstract compile: obj with get, set
        abstract currentProvider: Provider with get, set
        abstract estimateGas: tx: Tx * ?callback: Callback<float> -> Promise<float>
        abstract getAccounts: ?cb: Callback<Array<string>> -> Promise<Array<string>>
        abstract getBalance: address: string * ?defaultBlock: BlockType * ?cb: Callback<float> -> Promise<float>
        abstract getBlock: number: BlockType * ?returnTransactionObjects: bool * ?cb: Callback<Block> -> Promise<Block>
        abstract getBlockNumber: ?callback: Callback<float> -> Promise<float>
        abstract getBlockTransactionCount: number: U2<BlockType, string> * ?cb: Callback<float> -> Promise<float>
        abstract getBlockUncleCount: number: U2<BlockType, string> * ?cb: Callback<float> -> Promise<float>
        abstract getCode: address: string * ?defaultBlock: BlockType * ?cb: Callback<string> -> Promise<string>
        abstract getCoinbase: ?cb: Callback<string> -> Promise<string>
        abstract getCompilers: ?cb: Callback<ResizeArray<string>> -> Promise<ResizeArray<string>>
        abstract getGasPrice: ?cb: Callback<float> -> Promise<float>
        abstract getHashrate: ?cb: Callback<float> -> Promise<float>
        abstract getPastLogs: options: EthGetPastLogsOptions * ?cb: Callback<Array<Log>> -> Promise<Array<Log>>
        abstract getProtocolVersion: ?cb: Callback<string> -> Promise<string>
        abstract getStorageAt: address: string * ?defaultBlock: BlockType * ?cb: Callback<string> -> Promise<string>
        abstract getTransactionReceipt: hash: string * ?cb: Callback<TransactionReceipt> -> Promise<TransactionReceipt>
        abstract getTransaction: hash: string * ?cb: Callback<Transaction> -> Promise<Transaction>
        abstract getTransactionCount: address: string * ?defaultBlock: BlockType * ?cb: Callback<float> -> Promise<float>
        abstract getTransactionFromBlock: block: BlockType * index: float * ?cb: Callback<Transaction> -> Promise<Transaction>
        abstract getUncle: blockHashOrBlockNumber: U2<BlockType, string> * uncleIndex: float * ?returnTransactionObjects: bool * ?cb: Callback<Block> -> Promise<Block>
        abstract getWork: ?cb: Callback<Array<string>> -> Promise<Array<string>>
        abstract givenProvider: Provider with get, set
        abstract isMining: ?cb: Callback<bool> -> Promise<bool>
        abstract isSyncing: ?cb: Callback<bool> -> Promise<bool>
        abstract net: Net with get, set
        abstract personal: Personal with get, set
        abstract sendSignedTransaction: data: string * ?cb: Callback<string> -> PromiEvent<TransactionReceipt>
        abstract sendTransaction: tx: Tx * ?cb: Callback<string> -> PromiEvent<TransactionReceipt>
        abstract submitWork: nonce: string * powHash: string * digest: string * ?cb: Callback<bool> -> Promise<bool>
        abstract sign: address: string * dataToSign: string * ?cb: Callback<string> -> Promise<string>

    type [<AllowNullLiteral>] EthGetPastLogsOptions =
        abstract fromBlock: BlockType option with get, set
        abstract toBlock: BlockType option with get, set
        abstract address: string with get, set
        abstract topics: Array<U2<string, Array<string>>> option with get, set

    type [<AllowNullLiteral>] SyncingState =
        abstract startingBlock: float with get, set
        abstract currentBlock: float with get, set
        abstract highestBlock: float with get, set

    type SyncingResult =
        U2<obj, SyncingState>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module SyncingResult =
        let ofObj v: SyncingResult = v |> U2.Case1
        let isObj (v: SyncingResult) = match v with U2.Case1 _ -> true | _ -> false
        let asObj (v: SyncingResult) = match v with U2.Case1 o -> Some o | _ -> None
        let ofSyncingState v: SyncingResult = v |> U2.Case2
        let isSyncingState (v: SyncingResult) = match v with U2.Case2 _ -> true | _ -> false
        let asSyncingState (v: SyncingResult) = match v with U2.Case2 o -> Some o | _ -> None

    type [<AllowNullLiteral>] Version0 =
        abstract api: string with get, set
        abstract network: string with get, set
        abstract node: string with get, set
        abstract ethereum: string with get, set
        abstract whisper: string with get, set
        abstract getNetwork: callback: (Error -> string -> unit) -> unit
        abstract getNode: callback: (Error -> string -> unit) -> unit
        abstract getEthereum: callback: (Error -> string -> unit) -> unit
        abstract getWhisper: callback: (Error -> string -> unit) -> unit

    type [<AllowNullLiteral>] Net =
        interface end

    type [<AllowNullLiteral>] Personal =
        abstract newAccount: password: string * ?cb: Callback<bool> -> Promise<bool>
        abstract getAccounts: ?cb: Callback<Array<string>> -> Promise<Array<string>>
        abstract importRawKey: unit -> obj option
        abstract lockAccount: unit -> obj option
        abstract unlockAccount: unit -> obj option
        abstract sign: unit -> obj option

    type [<AllowNullLiteral>] Shh =
        interface end

    type [<AllowNullLiteral>] Bzz =
        interface end
