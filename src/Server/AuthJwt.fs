module AuthJwt

//  Learn about JWT https://jwt.io/introduction/
//  This module uses the JOSE-JWT library https://github.com/dvsekhvalnov/jose-jwt

open System.IO
open Newtonsoft.Json

type UserRights =
   { UserName : string }

let internal createPassPhrase() =
    let crypto = System.Security.Cryptography.RandomNumberGenerator.Create()
    let randomNumber = Array.init 32 byte
    crypto.GetBytes(randomNumber)
    randomNumber

let private passPhrase =
    let fi = FileInfo("./temp/token.txt")
    if not fi.Exists then
        let passPhrase = createPassPhrase()
        if not fi.Directory.Exists then
            fi.Directory.Create()
        File.WriteAllBytes(fi.FullName,passPhrase)
    File.ReadAllBytes(fi.FullName)

let internal encodeString (payload:string) =
    Jose.JWT.Encode(payload, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)

let internal decodeString (jwt:string) =
    Jose.JWT.Decode(jwt, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)

let encode token =
    JsonConvert.SerializeObject token
    |> encodeString

let decode<'a> (jwt:string) : 'a =
    decodeString jwt
    |> JsonConvert.DeserializeObject<'a>

/// Returns true if the JSON Web Token is successfully decoded and the signature is verified.
let checkValid (jwt:string) : UserRights option =
    try
        let token = decode jwt
        Some token
    with
    | _ -> None