namespace Shared

module InputValidators =
    open System   
    open System.Text.RegularExpressions  
    
    let emailRules userName = 
        [   String.IsNullOrWhiteSpace(userName), "Please enter a valid email"
            // userName.Trim().Length < 5, "'Email' must at least have 5 characters"
            Regex("""^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$""").IsMatch(userName) |> not, "Incorrect email address"
             ]
    let passwordRules (password: string) = 
        [   //String.IsNullOrWhiteSpace(password), "Password cannot be empty"
            password.Trim().Length < 8, "Expected 8+ characters"
            Regex("""(?=.*[a-z])""").IsMatch(password) |> not, "Expected at least 1 lowercase character"
            Regex("""(?=.*[A-Z])""").IsMatch(password) |> not, "Expected at least 1 uppercase character"
            Regex("""(?=.*[\d])""").IsMatch(password) |> not, "Expected at least 1 digit character"
            Regex("""(?=.*[\W])""").IsMatch(password) |> not, "Expected at least 1 special character"
            ]

    let passwordConfRules confPassword password = 
        [ password <> confPassword, "Passwords don't match" ]
        @ passwordRules password

    let private processRules = List.filter fst >> List.map snd

    let emailValidation = emailRules >> processRules
    let passwordValidation = passwordRules >> processRules
    let passwordConfValidation confPassword = passwordConfRules confPassword >> processRules
