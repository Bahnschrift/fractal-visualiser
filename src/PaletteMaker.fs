module paletteMaker

open System
open Cookies
open Fable.Core.JsInterop
open Browser.Dom
open Browser.Types


let WIDTH = 1250.0
let HEIGHT = 150.0

let mutable points = [||]
let mutable scaledPoints = [||]

let interpolateLinear y1 y2 mu = 
    y1 * (1.0 - mu) + y2 * mu

let interpolateCosine y1 y2 mu = 
    let mu2 = (1.0 - Math.Cos(mu * Math.PI)) / 2.0
    y1 * (1.0 - mu2) + y2 * mu2

let interpolateCubic y0 y1 y2 y3 mu = 
    let mu2 = mu * mu
    let a0 = y3 - y2 - y0 + y1
    let a1 = y0 - y1 - a0
    let a2 = y2 - y0
    let a3 = y1
    (a0 * mu * mu2) + (a1 * mu2) + (a2 * mu) + (a3)

let interpolateCatmull y0 y1 y2 y3 mu = 
    let mu2 = mu * mu
    let a0 = -0.5 * y0 + 1.5 * y1 - 1.5 * y2 + 0.5 * y3
    let a1 = y0 - 2.5 * y1 + 2.0 * y2 - 0.5 * y3
    let a2 = -0.5 * y0 + 0.5 * y2
    let a3 = y1
    (a0 * mu * mu2) + (a1 * mu2) + (a2 * mu) + (a3)

let private TENSION = 1.0 // -1 <= TENSION <= 1
let private BIAS = -0.0
let interpolateHermite y0 y1 y2 y3 mu = 
    let mu2 = mu * mu
    let mu3 = mu2 * mu
    let m0 = (y1 - y0) * (1.0 + BIAS) * (1.0 - TENSION) / 2.0 + (y2 - y1) * (1.0 - BIAS) * (1.0 - TENSION) / 2.0
    let m1 = (y2 - y1) * (1.0 + BIAS) * (1.0 - TENSION) / 2.0 + (y3 - y2) * (1.0 - BIAS) * (1.0 - TENSION) / 2.0
    let a0 = 2.0 * mu3 - 3.0 * mu2 + 1.0
    let a1 = mu3 - 2.0 * mu2 + mu
    let a2 = mu3 - mu2
    let a3 = -2.0 * mu3 + 3.0 * mu2
    (a0 * y1) + (a1 * m0) + (a2 * m1) + (a3 * y2)


let hexToRGB (hex: string): float * float * float = 
    let mutable hex = hex
    if hex.[0] = '#' then
        hex <- hex.[1 .. ]
    let r = Convert.ToInt32(hex.[0 .. 1], 16) |> float |> fun a -> a / 255.0
    let g = Convert.ToInt32(hex.[2 .. 3], 16) |> float |> fun a -> a / 255.0
    let b = Convert.ToInt32(hex.[4 .. 5], 16) |> float |> fun a -> a / 255.0
    r, g, b

let fieldP1X = document.getElementById "p1x" :?> HTMLInputElement
let fieldP1C = document.getElementById "p1c" :?> HTMLInputElement
let fieldP2X = document.getElementById "p2x" :?> HTMLInputElement
let fieldP2C = document.getElementById "p2c" :?> HTMLInputElement
let fieldP3X = document.getElementById "p3x" :?> HTMLInputElement
let fieldP3C = document.getElementById "p3c" :?> HTMLInputElement
let fieldP4X = document.getElementById "p4x" :?> HTMLInputElement
let fieldP4C = document.getElementById "p4c" :?> HTMLInputElement
let fieldP5X = document.getElementById "p5x" :?> HTMLInputElement
let fieldP5C = document.getElementById "p5c" :?> HTMLInputElement
let buttonResetpalette = document.getElementById "resetpalette" :?> HTMLButtonElement

let cookieP1X  = findCookieValue "p1x"
let cookieP1C  = findCookieValue "p1c"
let cookieP2X  = findCookieValue "p2x"
let cookieP2C  = findCookieValue "p2c"
let cookieP3X  = findCookieValue "p3x"
let cookieP3C  = findCookieValue "p3c"
let cookieP4X  = findCookieValue "p4x"
let cookieP4C  = findCookieValue "p4c"
let cookieP5X  = findCookieValue "p5x"
let cookieP5C  = findCookieValue "p5c"

let updatePoints () = 
    let (p1R, p1G, p1B) = hexToRGB fieldP1C.value
    let (p2R, p2G, p2B) = hexToRGB fieldP2C.value
    let (p3R, p3G, p3B) = hexToRGB fieldP3C.value
    let (p4R, p4G, p4B) = hexToRGB fieldP4C.value
    let (p5R, p5G, p5B) = hexToRGB fieldP5C.value
    points <- [|
        (float fieldP1X.value, p1R, p1G, p1B);
        (float fieldP2X.value, p2R, p2G, p2B);
        (float fieldP3X.value, p3R, p3G, p3B);
        (float fieldP4X.value, p4R, p4G, p4B);
        (float fieldP5X.value, p5R, p5G, p5B);
        (1.0, p1R, p1G, p1B)
    |]
    scaledPoints <- points |> Array.map (fun (x, r, g, b) -> (x * WIDTH, r * HEIGHT, g * HEIGHT, b * HEIGHT))
    
    fieldP2X.min <- fieldP1X.value
    fieldP2X.max <- fieldP3X.value
    fieldP3X.min <- fieldP2X.value
    fieldP3X.max <- fieldP4X.value
    fieldP4X.min <- fieldP3X.value
    fieldP4X.max <- fieldP5X.value
    fieldP5X.min <- fieldP4X.value

    document.cookie <- sprintf "p1x=%s" fieldP1X.value
    document.cookie <- sprintf "p1c=%s" fieldP1C.value
    document.cookie <- sprintf "p2x=%s" fieldP2X.value
    document.cookie <- sprintf "p2c=%s" fieldP2C.value
    document.cookie <- sprintf "p3x=%s" fieldP3X.value
    document.cookie <- sprintf "p3c=%s" fieldP3C.value
    document.cookie <- sprintf "p4x=%s" fieldP4X.value
    document.cookie <- sprintf "p4c=%s" fieldP4C.value
    document.cookie <- sprintf "p5x=%s" fieldP5X.value
    document.cookie <- sprintf "p5c=%s" fieldP5C.value

let canv = document.getElementById "palettecanv" :?> HTMLCanvasElement
canv.width <- WIDTH
canv.height <- HEIGHT
let ctx = canv.getContext_2d()

let private set = [|[4; 0; 1; 2]; [0; 1; 2; 3]; [1; 2; 3; 4]; [2; 3; 4; 0]; [3; 4; 0; 1];|]
let private getNeighbouring x = 
    let mutable minpoint = 0
    while x > (scaledPoints.[minpoint + 1] |> fun (x, _, _, _) -> x) do
        minpoint <- minpoint + 1
    if minpoint = scaledPoints.Length - 1 then
        minpoint <- minpoint - 1
    let p = set.[minpoint]
    let a = scaledPoints.[p.[0]]
    let b = scaledPoints.[p.[1]]
    let c = if minpoint < 4 then scaledPoints.[p.[2]] else scaledPoints.[p.[2]] |> fun (a, b, c, d) -> (a + WIDTH, b, c, d)
    let d = scaledPoints.[p.[3]]
    a, b, c, d

let drawpalette () = 
    for x in {0.0 .. WIDTH} do
        let ((_, p0r, p0g, p0b), (p1x, p1r, p1g, p1b),  (p2x, p2r, p2g, p2b), (_, p3r, p3g, p3b)) = getNeighbouring x
        let mul = (x - p1x) / (p2x - p1x)
        let yInterpR = interpolateHermite p0r p1r p2r p3r mul
        let yInterpG = interpolateHermite p0g p1g p2g p3g mul
        let yInterpB = interpolateHermite p0b p1b p2b p3b mul
        ctx.fillStyle <- !^ (sprintf "rgb(%f, %f, %f)" (yInterpR / HEIGHT * 255.0) (yInterpG / HEIGHT * 255.0) (yInterpB / HEIGHT * 255.0))
        ctx.fillRect(x, 0.0, 1.0, HEIGHT)

        ctx.fillStyle <- !^ "rgb(255, 0, 0)"
        ctx.fillRect(x, HEIGHT - yInterpR - 1.0, 1.0, 1.0)
        ctx.fillStyle <- !^ "rgb(0, 255, 0)"
        ctx.fillRect(x, HEIGHT - yInterpG - 1.0, 1.0, 1.0)
        ctx.fillStyle <- !^ "rgb(0, 0, 255)"
        ctx.fillRect(x, HEIGHT - yInterpB - 1.0, 1.0, 1.0)

    for point in scaledPoints do
        let (x, r, g, b) = point
        ctx.fillStyle <- !^ "rgb(255, 0, 0)"
        ctx.fillRect(x - 3.0, HEIGHT - r - 3.0, 5.0, 5.0)
        ctx.fillStyle <- !^ "rgb(0, 255, 0)"
        ctx.fillRect(x - 3.0, HEIGHT - g - 3.0, 5.0, 5.0)
        ctx.fillStyle <- !^ "rgb(0, 0, 255)"
        ctx.fillRect(x - 3.0, HEIGHT - b - 3.0, 5.0, 5.0)

let getColour (x: float): float * float * float = 
    let ((_, p0r, p0g, p0b), (p1x, p1r, p1g, p1b),  (p2x, p2r, p2g, p2b), (_, p3r, p3g, p3b)) = getNeighbouring x
    let mul = (x - p1x) / (p2x - p1x)
    let yInterpR = (interpolateHermite p0r p1r p2r p3r mul) / HEIGHT * 255.0
    let yInterpG = (interpolateHermite p0g p1g p2g p3g mul) / HEIGHT * 255.0
    let yInterpB = (interpolateHermite p0b p1b p2b p3b mul) / HEIGHT * 255.0
    (yInterpR, yInterpG, yInterpB)

let getColours (n: int): (float * float * float)[] =
    let inc = WIDTH / float n
    let mutable cols = [||]
    for x in {0.0 .. inc .. WIDTH - 1.0} do
        cols <- Array.append cols [|getColour x|]
    cols

let init () = 
    updatePoints()
    drawpalette()