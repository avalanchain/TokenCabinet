module Client.Cabinet

type MenuPage =
    | PurchaseToken
    | Investments
    | ReferralProgram
    | Verification
    | Contacts
    // | Dashboard
    with static member Default = PurchaseToken  

