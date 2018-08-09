module Config

open System.Threading.Tasks
open Shared.Auth

type ResetPasswordEmailer = ForgotPasswordInfo -> PwdResetToken -> Task<Result<unit, exn>>  

type Config = {
    connectionString    : string
    resetPasswordEmailer: ResetPasswordEmailer
}