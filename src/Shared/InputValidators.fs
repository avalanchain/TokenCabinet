namespace Shared

module InputValidators =
    open System   
    open System.Text.RegularExpressions  
    
    let emailRules userName = 
        [   String.IsNullOrWhiteSpace(userName), "'Email' cannot be empty"
            userName.Trim().Length < 5, "'Email' must at least have 5 characters"
            Regex("""^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$""").IsMatch(userName) |> not, "'Email' must be a valid email address"
             ]
    let passwordRules password = 
        [   String.IsNullOrWhiteSpace(password), "'Password' cannot be empty"
            password.Trim().Length < 8, "'Password' must be at least have 8 characters"
            Regex("""(?=.*[a-z])""").IsMatch(password) |> not, "'Password' must have at least 1 lowercase character"
            Regex("""(?=.*[A-Z])""").IsMatch(password) |> not, "'Password' must have at least 1 uppercase character"
            Regex("""(?=.*[\d])""").IsMatch(password) |> not, "'Password' must have at least 1 digit character"
            Regex("""(?=.*[\W])""").IsMatch(password) |> not, "'Password' must have at least 1 special character"
            ]

    let passwordConfRules confPassword password = 
        [ password <> confPassword, "Passwords don't match" ]
        @ passwordRules password

    let private processRules = List.filter fst >> List.map snd

    let emailValidation = emailRules >> processRules
    let passwordValidation = passwordRules >> processRules
    let passwordConfValidation confPassword = passwordConfRules confPassword >> processRules
