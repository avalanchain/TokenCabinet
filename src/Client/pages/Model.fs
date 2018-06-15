module Client.Model

open ServerCode.Domain
open ServerCode.Commodities

open Client
open Client.Messages
open Client.Entity
open Client.Login
open Client.Statics

[<RequireQualifiedAccess>]
type MenuPage = 
  | Home 
  | Admin
  | Login
  | Static of Statics.Page
  | Trading of Trading.Page
  with static member Default = Home

type LoginState =
| LoggedOut
| LoggedIn of JWT

// Model

type SubModel =
  | NoSubModel
  | LoginModel of Login.Model
  | StaticModel of Statics.Model

type Model =
  { Page : MenuPage
    User: UserData option
    Trading: Trading.Model
    SubModel: SubModel
    EthConnection: EthConnection option
    Loading: bool }
