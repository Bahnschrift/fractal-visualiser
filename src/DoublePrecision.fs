module DoublePrecision

open Fable.Core.JsInterop


let toDouble (num: float) = 
    (num?toPrecision(32): string) |> float

let toFloat (num: float) = 
    (num?toPrecision(16): string) |> float

let splitDouble (num: float) = 
    let upper = toFloat num
    let lower = num - toDouble upper |> toFloat
    upper, lower

type SplitDouble = 
    | SplitDouble of float * float
    static member ofFloat (num: double) = 
        splitDouble num |> SplitDouble
    static member Upper (SplitDouble(hi, _)) = 
        hi
    static member Lower (SplitDouble(_, lo)) = 
        lo
    static member toUniform (SplitDouble(hi, lo)) = 
        Fable.Core.JS.Constructors.Float32Array.Create [|hi; lo|]
    member this.upper = 
        SplitDouble.Upper this
    member this.lower = 
        SplitDouble.Lower this
    member this.toFloat =
        this.upper

    static member (+) (SplitDouble(u1, l1), SplitDouble(u2, l2)) = 
        let t1 = u1 + u2
        let e = t1 - u1
        let t2 = ((u2 - e) + (u1 - (t1 - e))) + l1 + l2
        let u3 = t1 + t2
        let l3 = t2 - (u3 - t1)
        SplitDouble(u3, l3)

    static member (*) (SplitDouble(u1, l1), SplitDouble(u2, l2)) = 
        let split = 8193.0
        let cona = u1 * split
        let conb = u2 * split
        let a1 = cona - (cona - u1)
        let b1 = conb - (conb - u2)
        let a2 = u1 - a1
        let b2 = u2 - b1

        let c11 = u1 * u2
        let c21 = a2 * b2 + (a2 * b1 + (a1 * b2 + (a1 * b1 - c11)))

        let c2 = u1 * l2 + l1 * u2

        let t1 = c11 + c2
        let e = t1 - c11
        let t2 = l1 * l2 + ((c2 - e) + (c11 - (t1 - e))) + c21

        let u3 = t1 + t2
        let l3 = t2 - (u3 - t1)
        SplitDouble(u3, l3)