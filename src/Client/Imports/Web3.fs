namespace Fable.Import
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.JS

open Fable.Import.BigNumber

module web3Impl =

    module Web3Types =
        type ContractAbi = ResizeArray<AbiDefinition>

        and AbiDefinition = U4<string, obj, FunctionDescription, EventDescription>

        and [<AllowNullLiteral>] FunctionDescription =
            abstract ``type``: (* TODO StringEnum function | constructor | fallback *) string with get, set
            abstract name: string option with get, set
            abstract inputs: ResizeArray<FunctionParameter> with get, set
            abstract outputs: ResizeArray<FunctionParameter> option with get, set
            abstract constant: bool option with get, set
            abstract payable: bool option with get, set

        and [<AllowNullLiteral>] EventParameter =
            abstract name: string with get, set
            abstract ``type``: string with get, set
            abstract indexed: bool with get, set

        and [<AllowNullLiteral>] EventDescription =
            abstract ``type``: obj with get, set
            abstract name: string with get, set
            abstract inputs: ResizeArray<EventParameter> with get, set
            abstract anonymous: bool with get, set

        and [<AllowNullLiteral>] FunctionParameter =
            abstract name: string with get, set
            abstract ``type``: string with get, set

        and [<AllowNullLiteral>] Contract<'A> = 
            abstract at: address: string -> 'A

        and [<AllowNullLiteral>] FilterObject =
            abstract fromBlock: U2<float, string> with get, set
            abstract toBlock: U2<float, string> with get, set
            abstract address: string with get, set
            abstract topics: ResizeArray<string> with get, set

        and [<AllowNullLiteral>] SolidityEvent<'A> =
            abstract ``event``: string with get, set
            abstract address: string with get, set
            abstract args: 'A with get, set

        and [<AllowNullLiteral>] FilterResult =
            abstract get: callback: Func<unit, unit> -> unit
            abstract watch: callback: Func<Error, SolidityEvent<'A>, unit> -> unit
            abstract stopWatching: callback: Func<unit, unit> -> unit

        and [<AllowNullLiteral>] Provider =
            interface end

        and [<AllowNullLiteral>] Sha3Options =
            abstract encoding: obj with get, set

        and [<AllowNullLiteral>] EthApi =
            abstract coinbase: string with get, set
            abstract mining: bool with get, set
            abstract hashrate: float with get, set
            abstract gasPrice: BigNumber with get, set
            abstract accounts: ResizeArray<string> with get, set
            abstract blockNumber: float with get, set
            abstract defaultAccount: string with get, set
            abstract defaultBlock: BlockParam with get, set
            abstract syncing: SyncingResult with get, set
            abstract compile: obj with get, set
            abstract getMining: cd: Func<Error, bool, unit> -> unit
            abstract getHashrate: cd: Func<Error, float, unit> -> unit
            abstract getGasPrice: cd: Func<Error, BigNumber, unit> -> unit
            abstract getAccounts: cd: Func<Error, ResizeArray<string>, unit> -> unit
            abstract getBlockNumber: callback: Func<Error, float, unit> -> unit
            abstract getSyncing: cd: Func<Error, SyncingResult, unit> -> unit
            abstract isSyncing: cb: Func<Error, bool, SyncingState, unit> -> IsSyncing
            abstract getBlock: hashStringOrBlockNumber: U2<string, BlockParam> -> BlockWithoutTransactionData
            abstract getBlock: hashStringOrBlockNumber: U2<string, BlockParam> * callback: Func<Error, BlockWithoutTransactionData, unit> -> unit
            abstract getBlock: hashStringOrBlockNumber: U2<string, BlockParam> * returnTransactionObjects: obj -> BlockWithTransactionData
            abstract getBlock: hashStringOrBlockNumber: U2<string, BlockParam> * returnTransactionObjects: obj * callback: Func<Error, BlockWithTransactionData, unit> -> unit
            abstract getBlockTransactionCount: hashStringOrBlockNumber: U2<string, BlockParam> -> float
            abstract getBlockTransactionCount: hashStringOrBlockNumber: U2<string, BlockParam> * callback: Func<Error, float, unit> -> unit
            abstract getUncle: hashStringOrBlockNumber: U2<string, BlockParam> * uncleNumber: float -> BlockWithoutTransactionData
            abstract getUncle: hashStringOrBlockNumber: U2<string, BlockParam> * uncleNumber: float * callback: Func<Error, BlockWithoutTransactionData, unit> -> unit
            abstract getTransaction: transactionHash: string -> Transaction
            abstract getTransaction: transactionHash: string * callback: Func<Error, Transaction, unit> -> unit
            abstract getTransactionFromBlock: hashStringOrBlockNumber: U2<string, BlockParam> * indexNumber: float -> Transaction
            abstract getTransactionFromBlock: hashStringOrBlockNumber: U2<string, BlockParam> * indexNumber: float * callback: Func<Error, Transaction, unit> -> unit
            abstract contract: abi: AbiDefinition[] -> Contract<obj>
            abstract getBalance: addressHexString: string -> BigNumber
            abstract getBalance: addressHexString: string * callback: Func<Error, BigNumber, unit> -> unit
            abstract getStorageAt: address: string * position: float -> string
            abstract getStorageAt: address: string * position: float * callback: Func<Error, string, unit> -> unit
            abstract getCode: addressHexString: string -> string
            abstract getCode: addressHexString: string * callback: Func<Error, string, unit> -> unit
            abstract filter: value: U2<string, FilterObject> -> FilterResult
            abstract sendTransaction: txData: TxData -> string
            abstract sendTransaction: txData: TxData * callback: Func<Error, string, unit> -> unit
            abstract sendRawTransaction: rawTxData: string -> string
            abstract sendRawTransaction: rawTxData: string * callback: Func<Error, string, unit> -> unit
            abstract sign: address: string * data: string -> string
            abstract sign: address: string * data: string * callback: Func<Error, string, unit> -> unit
            abstract getTransactionReceipt: txHash: string -> TransactionReceipt
            abstract getTransactionReceipt: txHash: string * callback: Func<Error, TransactionReceipt, unit> -> unit
            abstract call: callData: CallData -> string
            abstract call: callData: CallData * callback: Func<Error, string, unit> -> unit
            abstract estimateGas: callData: CallData -> float
            abstract estimateGas: callData: CallData * callback: Func<Error, float, unit> -> unit
            abstract getTransactionCount: address: string -> float
            abstract getTransactionCount: address: string * callback: Func<Error, float, unit> -> unit

        and [<AllowNullLiteral>] VersionApi =
            abstract api: string with get, set
            abstract network: string with get, set
            abstract node: string with get, set
            abstract ethereum: string with get, set
            abstract whisper: string with get, set
            abstract getNetwork: cd: Func<Error, string, unit> -> unit
            abstract getNode: cd: Func<Error, string, unit> -> unit
            abstract getEthereum: cd: Func<Error, string, unit> -> unit
            abstract getWhisper: cd: Func<Error, string, unit> -> unit

        and [<AllowNullLiteral>] PersonalApi =
            abstract listAccounts: U2<ResizeArray<string>, obj> with get, set
            abstract newAccount: ?password: string -> string
            abstract unlockAccount: address: string * ?password: string * ?duration: float -> bool
            abstract lockAccount: address: string -> bool
            abstract sign: message: string * account: string * password: string -> string

        and [<AllowNullLiteral>] NetApi =
            abstract listening: bool with get, set
            abstract peerCount: bool with get, set
            abstract getListening: cd: Func<Error, bool, unit> -> unit
            abstract getPeerCount: cd: Func<Error, float, unit> -> unit

        and BlockParam = U4<float, obj, obj, obj>

        and [<StringEnum>] Unit =
                | Kwei | Ada | Mwei | Babbage | Gwei | Shannon | Szabo | Finney | Ether | Kether | Grand | Einstein | Mether | Gether | Tether

        and [<AllowNullLiteral>] SyncingState =
            abstract startingBlock: float with get, set
            abstract currentBlock: float with get, set
            abstract highestBlock: float with get, set

        and SyncingResult = obj

        and [<AllowNullLiteral>] IsSyncing =
            abstract addCallback: cb: Func<Error, bool, SyncingState, unit> -> unit
            abstract stopWatching: unit -> unit

        and [<AllowNullLiteral>] AbstractBlock =
            abstract number: U2<float, obj> with get, set
            abstract hash: U2<string, obj> with get, set
            abstract parentHash: string with get, set
            abstract nonce: U2<string, obj> with get, set
            abstract sha3Uncles: string with get, set
            abstract logsBloom: U2<string, obj> with get, set
            abstract transactionsRoot: string with get, set
            abstract stateRoot: string with get, set
            abstract miner: string with get, set
            abstract difficulty: BigNumber with get, set
            abstract totalDifficulty: BigNumber with get, set
            abstract extraData: string with get, set
            abstract size: float with get, set
            abstract gasLimit: float with get, set
            abstract gasUser: float with get, set
            abstract timestamp: float with get, set
            abstract uncles: ResizeArray<string> with get, set

        and [<AllowNullLiteral>] BlockWithoutTransactionData =
            inherit AbstractBlock
            abstract transactions: ResizeArray<string> with get, set

        and [<AllowNullLiteral>] BlockWithTransactionData =
            inherit AbstractBlock
            abstract transactions: ResizeArray<Transaction> with get, set

        and [<AllowNullLiteral>] Transaction =
            abstract hash: string with get, set
            abstract nonce: float with get, set
            abstract blockHash: U2<string, obj> with get, set
            abstract blockNumber: U2<float, obj> with get, set
            abstract transactionIndex: U2<float, obj> with get, set
            abstract from: string with get, set
            abstract ``to``: U2<string, obj> with get, set
            abstract value: BigNumber with get, set
            abstract gasPrice: BigNumber with get, set
            abstract gas: float with get, set
            abstract input: string with get, set

        and [<AllowNullLiteral>] CallTxDataBase =
            abstract ``to``: string option with get, set
            abstract value: U3<float, string, BigNumber> option with get, set
            abstract gas: U3<float, string, BigNumber> option with get, set
            abstract gasPrice: U3<float, string, BigNumber> option with get, set
            abstract data: string option with get, set
            abstract nonce: float option with get, set

        and [<AllowNullLiteral>] TxData =
            inherit CallTxDataBase
            abstract from: string with get, set

        and [<AllowNullLiteral>] CallData =
            inherit CallTxDataBase
            abstract from: string option with get, set

        and [<AllowNullLiteral>] TransactionReceipt =
            abstract blockHash: string with get, set
            abstract blockNumber: float with get, set
            abstract transactionHash: string with get, set
            abstract transactionIndex: float with get, set
            abstract from: string with get, set
            abstract ``to``: string with get, set
            abstract cumulativeGasUsed: float with get, set
            abstract gasUsed: float with get, set
            abstract contractAddress: U2<string, obj> with get, set
            abstract ``null``: obj with get, set
            abstract logs: ResizeArray<LogEntry> with get, set

        and [<AllowNullLiteral>] LogEntry =
            abstract logIndex: U2<float, obj> with get, set
            abstract transactionIndex: float with get, set
            abstract transactionHash: string with get, set
            abstract blockHash: U2<string, obj> with get, set
            abstract blockNumber: U2<float, obj> with get, set
            abstract address: string with get, set
            abstract data: string with get, set
            abstract topics: ResizeArray<string> with get, set

    type MixedData = obj

    and [<AllowNullLiteral>] [<Import("*","web3")>] Web3Impl(?provider: Web3Types.Provider) =
        member __.providers with get(): obj = jsNative and set(v: obj): unit = jsNative
        member __.currentProvider with get(): Web3Types.Provider = jsNative and set(v: Web3Types.Provider): unit = jsNative
        member __.eth with get(): Web3Types.EthApi = jsNative and set(v: Web3Types.EthApi): unit = jsNative
        member __.personal with get(): Web3Types.PersonalApi = jsNative and set(v: Web3Types.PersonalApi): unit = jsNative
        member __.version with get(): Web3Types.VersionApi = jsNative and set(v: Web3Types.VersionApi): unit = jsNative
        member __.net with get(): Web3Types.NetApi = jsNative and set(v: Web3Types.NetApi): unit = jsNative
        member __.isConnected(): bool = jsNative
        member __.setProvider(provider: Web3Types.Provider): unit = jsNative
        member __.reset(keepIsSyncing: bool): unit = jsNative
        member __.toHex(data: MixedData): string = jsNative
        member __.toAscii(hex: string): string = jsNative
        member __.fromAscii(ascii: string, ?padding: float): string = jsNative
        member __.toDecimal(hex: string): float = jsNative
        member __.fromDecimal(value: U2<float, string>): string = jsNative
        member __.fromWei(value: U2<float, string>, unit: Unit): string = jsNative
        member __.fromWei(value: BigNumber, unit: Unit): BigNumber = jsNative
        member __.toWei(amount: U2<float, string>, unit: Unit): string = jsNative
        member __.toWei(amount: BigNumber, unit: Unit): BigNumber = jsNative
        member __.toBigNumber(value: U2<float, string>): BigNumber = jsNative
        member __.isAddress(address: string): bool = jsNative
        member __.sha3(value: string, ?options: Web3Types.Sha3Options): string = jsNative

    module providers =
        type [<AllowNullLiteral>] [<Import("providers.HttpProvider","web3")>] HttpProvider(?url: string, ?timeout: float, ?username: string, ?password: string) =
            interface Web3Types.Provider


    let web3 = importDefault<Web3Impl> "web3"
    Browser.console.log "Imported web3:"
    Browser.console.log web3
    let httpProvider = import<providers.HttpProvider> "providers.HttpProvider" "web3"
    Browser.console.log "Imported httpProvider:"
    Browser.console.log httpProvider