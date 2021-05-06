module Cookies


open Browser.Dom

let findCookieValue (name: string): string option =
    let kvArrToPair (kvArr: string []): string * string =
        match kvArr with
        | [|k; v|] -> (k, v)
        | _ -> ("", "")

    let rawCookies: string = document.cookie
    rawCookies.Split ';'
    |> Array.map (fun (s: string) -> s.Trim().Split '=' |> kvArrToPair)
    |> Map.ofArray
    |> Map.tryFind name