module App

open System
open Shaders
open Cookies
open WebGLHelper
open DoublePrecision
open Browser.Dom
open Browser.Types


let WIDTH = float 1280
let HEIGHT = float 720

let juliaPresets = [|
    0.0, 0.8;
    0.37, 0.1;
    0.355, 0.355;
    -0.54, 0.54;
    -0.4, -0.59;
    0.34, -0.05;
    -0.687, 0.312;
    -0.673, 0.312;
    -0.75, -0.15;
    0.355534, 0.337292
|]

let getDivElement id = sprintf "#%s" id |> document.querySelector :?> HTMLDivElement
let divPalette = getDivElement "palettemaker"
let divMandelbrot = getDivElement "settingsmandelbrot"
let divMandelbox = getDivElement "settingsmandelbox"
let divJulia = getDivElement "settingsjulia"

let getInputElement id = sprintf "#%s" id |> document.querySelector :?> HTMLInputElement
let fieldZoom = getInputElement "zoom"
let fieldX = getInputElement "x"
let fieldY = getInputElement "y"
let fieldPaletteOffset = getInputElement "paletteoffset"
let fieldFractal = getInputElement "fractal"
let fieldMandelbrot = getInputElement "mandelbrot"
let fieldBurningShip = getInputElement "burningship"
let fieldMandelbox = getInputElement "mandelbox"
let fieldMandelbrotPower = getInputElement "mandelbrotpower"
let fieldMandelboxScale = getInputElement "mandelboxscale"
let fieldJulia = getInputElement "julia"
let fieldJuliaX = getInputElement "juliax"
let fieldJuliaY = getInputElement "juliay"
let fieldUseDoub = getInputElement "usedoub"
let fieldJuliaPresets = document.querySelector "#juliapresets" :?> HTMLSelectElement

let getButtonElement id = sprintf "#%s" id |> document.querySelector :?> HTMLButtonElement
let buttonFullscreen = getButtonElement "fullscreen"
let buttonPalette = getButtonElement "palettebutton"
let buttonCentre = getButtonElement "centre"
let buttonReset = getButtonElement "reset"
let buttonSaveImage = getButtonElement "saveimage"

let cookieX = findCookieValue "x"
let cookieY = findCookieValue "y"
let cookieZoom = findCookieValue "zoom"
let cookiePaletteOffset = findCookieValue "paletteoffset"
let cookieGenerator = findCookieValue "generator"
let cookieMandelbrotPower = findCookieValue "mandelbrotpower"
let cookieMandelboxScale = findCookieValue "mandelboxscale"
let cookieJuliaX = findCookieValue "jx"
let cookieJuliaY = findCookieValue "jy"
let cookieUseDoub = findCookieValue "usedoub"

let setupCookies () = 
    let cookies = [|
        cookieX; 
        cookieY; 
        cookieZoom; 
        cookiePaletteOffset; 
        cookieGenerator; 
        cookieMandelbrotPower;
        cookieMandelboxScale; 
        cookieJuliaX; 
        cookieJuliaY; 
        cookieUseDoub;
        PaletteMaker.cookieP1X;
        PaletteMaker.cookieP1C;
        PaletteMaker.cookieP2X;
        PaletteMaker.cookieP2C;
        PaletteMaker.cookieP3X;
        PaletteMaker.cookieP3C;
        PaletteMaker.cookieP4X;
        PaletteMaker.cookieP4C;
        PaletteMaker.cookieP5X;
        PaletteMaker.cookieP5C;
    |]
    if (Array.forall (fun (c: option<string>) -> not c.IsNone) cookies) then
        try  // Prevents sneaky people from meddling with my cookies
            fieldX.value <- cookieX.Value
            fieldY.value <- cookieY.Value
            fieldZoom.value <- cookieZoom.Value
            fieldPaletteOffset.value <- cookiePaletteOffset.Value
            match cookieGenerator.Value with
            | "1" -> fieldMandelbrot.checked <- true; divMandelbrot.hidden <- false
            | "2" -> fieldJulia.checked <- true; divJulia.hidden <- false
            | "3" -> fieldBurningShip.checked <- true
            | "4" -> fieldMandelbox.checked <- true; divMandelbox.hidden <- false
            | _ -> ()
            fieldMandelbrotPower.value <- cookieMandelbrotPower.Value
            fieldMandelboxScale.value <- cookieMandelboxScale.Value
            fieldJuliaX.value <- cookieJuliaX.Value
            fieldJuliaY.value <- cookieJuliaY.Value
            fieldUseDoub.checked <- if cookieJuliaY.Value = "true" then true else false
            PaletteMaker.fieldP1X.value <- PaletteMaker.cookieP1X.Value
            PaletteMaker.fieldP1C.value <- PaletteMaker.cookieP1C.Value
            PaletteMaker.fieldP2X.value <- PaletteMaker.cookieP2X.Value
            PaletteMaker.fieldP2C.value <- PaletteMaker.cookieP2C.Value
            PaletteMaker.fieldP3X.value <- PaletteMaker.cookieP3X.Value
            PaletteMaker.fieldP3C.value <- PaletteMaker.cookieP3C.Value
            PaletteMaker.fieldP4X.value <- PaletteMaker.cookieP4X.Value
            PaletteMaker.fieldP4C.value <- PaletteMaker.cookieP4C.Value
            PaletteMaker.fieldP5X.value <- PaletteMaker.cookieP5X.Value
            PaletteMaker.fieldP5C.value <- PaletteMaker.cookieP5C.Value
        with
        | _ -> ()
setupCookies()

let mutable zoom = float fieldZoom.value
let mutable x = float fieldX.value
let mutable y = float fieldY.value
let mutable paletteOffset = float fieldPaletteOffset.value
let mutable generator = 
    if fieldMandelbrot.checked then 1  // 1: Mandelrot
    elif fieldJulia.checked then 2  // 2: Julia
    elif fieldBurningShip.checked then 3
    elif fieldMandelbox.checked then 4  // 3: Mandelbox
    else -1
let mutable mandelbrotPower = float fieldMandelbrotPower.value
let mutable mandelboxScale = float fieldMandelboxScale.value
let mutable juliaX = float fieldJuliaX.value
let mutable juliaY = float fieldJuliaY.value
let mutable useDoub = fieldUseDoub.checked

let canv = document.querySelector "#canv" :?> HTMLCanvasElement
canv.width <- WIDTH
canv.height <- HEIGHT

let gl = canv.getContext "webgl" :?> GL
clear gl
let shaderProgram = createShaderProgram gl vsMandel fsMandel

PaletteMaker.init()
let mutable palette = PaletteMaker.getColours 76

let update () = 
    let resizeCanvas(canvas: HTMLCanvasElement) = 
        let displayWidth = canvas.clientWidth
        let displayHeight = canvas.clientHeight
        let needResize = 
            canvas.width <> displayWidth || 
            canvas.height <> displayHeight
        if needResize then
            canvas.width <- window.innerWidth * window.devicePixelRatio
            canvas.height <- window.innerHeight * window.devicePixelRatio
    
    document.cookie <- sprintf "zoom=%f;" zoom
    document.cookie <- sprintf "x=%f;" x
    document.cookie <- sprintf "y=%f;" y
    document.cookie <- sprintf "paletteoffset=%f" paletteOffset
    document.cookie <- sprintf "generator=%i" generator
    document.cookie <- sprintf "mandelbrotpower=%f" mandelbrotPower
    document.cookie <- sprintf "mandelboxscale=%f" mandelboxScale
    document.cookie <- sprintf "jx=%f" juliaX
    document.cookie <- sprintf "jy=%f" juliaY
    document.cookie <- sprintf "usedoub=%b" useDoub

    fieldX.value <- string x
    fieldY.value <- string y
    fieldZoom.value <- string zoom
    fieldPaletteOffset.value <- string paletteOffset
    fieldUseDoub.checked <- useDoub
    fieldMandelbrotPower.value <- string mandelbrotPower
    fieldJuliaX.value <- string juliaX
    fieldJuliaY.value <- string juliaY
    fieldMandelboxScale.value <- string mandelboxScale

    fieldX.step <- 0.1 * zoom |> string
    fieldY.step <- 0.1 * zoom |> string
    fieldZoom.step <- 0.1 * zoom |> string

    let positionBuffer, colourBuffer = initBuffers gl

    let vertexPositionAttr = createAttributeLocation gl shaderProgram "aVertexPosition"
    let textureCoordAttr = createAttributeLocation gl shaderProgram "aTextureCoord"

    let uLoc loc = createUniformLocation gl shaderProgram loc
    let zoomUniform = uLoc "uZoom"
    let xcUniform = uLoc "xc"
    let ycUniform = uLoc "yc"
    let paletteOffsetUniform = uLoc "uPaletteOffset"
    let ratioUniform = uLoc "uRatio"
    let generatorUniform = uLoc "uGenerator"
    let mandelbrotPowerUniform = uLoc "uMandelbrotPower"
    let mandelboxScaleUniform = uLoc "uMandelboxScale"
    let juliaXUniform = uLoc "uJuliaX"
    let juliaYUniform = uLoc "uJuliaY"
    let zoomDoubUniform = uLoc "uZoomDoub"
    let xcDoubUniform = uLoc "xcDoub"
    let ycDoubUniform = uLoc "ycDoub"
    let useDoubUniform = uLoc "uUseDoub"
    let paletteUniform = uLoc "uPalette"

    resizeCanvas gl.canvas
    gl.viewport(0.0, 0.0, gl.canvas.width, gl.canvas.height)
    gl.useProgram(shaderProgram)

    gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer)
    gl.vertexAttribPointer(vertexPositionAttr, 2., gl.FLOAT, false, 0., 0.)
    gl.bindBuffer(gl.ARRAY_BUFFER, colourBuffer)
    gl.vertexAttribPointer(textureCoordAttr, 2., gl.FLOAT, false, 0., 0.)
    
    let ratio = (gl.canvas.width / gl.canvas.height)
    gl.uniform1f(zoomUniform, zoom)
    gl.uniform1f(xcUniform, x)
    gl.uniform1f(ycUniform, y)
    gl.uniform1f(paletteOffsetUniform, paletteOffset)
    gl.uniform1f(ratioUniform, ratio)
    gl.uniform1f(generatorUniform, float generator)
    gl.uniform1f(mandelbrotPowerUniform, mandelbrotPower)
    gl.uniform1f(mandelboxScaleUniform, mandelboxScale)
    gl.uniform1f(juliaXUniform, juliaX)
    gl.uniform1f(juliaYUniform, juliaY)
    // gl.uniform2fv(zoomDoubUniform, zoom |> SplitDouble.ofFloat |> SplitDouble.toUniform)
    // gl.uniform2fv(xcDoubUniform, x |> SplitDouble.ofFloat |> SplitDouble.toUniform)
    // gl.uniform2fv(ycDoubUniform, y |> SplitDouble.ofFloat |> SplitDouble.toUniform)
    // gl.uniform1i(useDoubUniform, if useDoub then 1.0 else 0.0)
    gl.uniform3fv(paletteUniform, palette |> Array.collect (fun (r, g, b) -> [|r; g; b|]) |> Fable.Core.JS.Constructors.Float32Array.Create)

    gl.drawArrays(gl.TRIANGLE_STRIP, 0., 4.)
    gl.useProgram(shaderProgram)
update()

fieldZoom.oninput <- fun _ -> zoom <- float fieldZoom.value; update()
fieldX.oninput <- fun _ -> x <- float fieldX.value; update()
fieldY.oninput <- fun _ -> y <- float fieldY.value; update()
fieldMandelbrotPower.oninput <- fun _ -> mandelbrotPower <- float fieldMandelbrotPower.value; update()
fieldMandelboxScale.oninput <- fun _ -> mandelboxScale <- float fieldMandelboxScale.value; update()
fieldPaletteOffset.oninput <- fun _ -> paletteOffset <- float fieldPaletteOffset.value; update()
fieldJuliaX.oninput <- fun _ -> juliaX <- float fieldJuliaX.value; update()
fieldJuliaY.oninput <- fun _ -> juliaY <- float fieldJuliaY.value; update()
fieldMandelbrot.oninput <- fun _ -> 
    generator <- 1
    divMandelbrot.hidden <- false
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()
fieldJulia.oninput <- fun _ -> 
    generator <- 2
    divMandelbrot.hidden <- true
    divJulia.hidden <- false
    divMandelbox.hidden <- true
    update()
fieldBurningShip.oninput <- fun _ ->
    generator <- 3
    divMandelbrot.hidden <- true
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()
fieldMandelbox.oninput <- fun _ -> 
    generator <- 4
    divMandelbrot.hidden <- true
    divJulia.hidden <- true
    divMandelbox.hidden <- false
    update()
fieldJuliaPresets.oninput <- fun _ -> 
    let juliaPreset = int fieldJuliaPresets.value
    if juliaPreset <> -1 then
        let juliaPresetCoords = juliaPresets.[juliaPreset]
        fieldJuliaX.value <- string <| fst juliaPresetCoords
        fieldJuliaY.value <- string <| snd juliaPresetCoords
        juliaX <- fst juliaPresetCoords
        juliaY <- snd juliaPresetCoords
    update()
fieldUseDoub.oninput <- fun _ -> useDoub <- fieldUseDoub.checked; update()

PaletteMaker.fieldP1X.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP1C.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP2X.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP2C.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP3X.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP3C.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP4X.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP4C.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP5X.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()
PaletteMaker.fieldP5C.oninput <- fun _ -> PaletteMaker.updatePoints(); PaletteMaker.drawPalette(); palette <- PaletteMaker.getColours 76; update()

// TODO: Fix this
let mutable keysDown = Set.empty
document.onkeydown <- fun e ->
    let mutable scale = 0.075
    match e.key with
    | "w" | "s" | "a" | "d" | "q" | "e" -> keysDown <- keysDown.Add e.key
    | _ -> ()
    for key in keysDown do
        match key with
        | "w" -> y <- y + zoom * scale; update()
        | "s" -> y <- y - zoom * scale; update()
        | "a" -> x <- x - zoom * scale; update()
        | "d" -> x <- x + zoom * scale; update()
        | "q" -> zoom <- zoom + zoom * scale; update()
        | "e" -> zoom <- zoom - zoom * scale; update()
        | _ -> ()
document.onkeyup <- fun e ->
    let scale = 0.075
    match e.key with
    | "w" | "s" | "a" | "d" | "q" | "e" -> keysDown <- keysDown.Remove e.key
    | _ -> ()
    for key in keysDown do
        match key with
        | "w" -> y <- y + zoom * scale; update()
        | "s" -> y <- y - zoom * scale; update()
        | "a" -> x <- x - zoom * scale; update()
        | "d" -> x <- x + zoom * scale; update()
        | "q" -> zoom <- zoom + zoom * scale; update()
        | "e" -> zoom <- zoom - zoom * scale; update()
        | _ -> ()

buttonFullscreen.onclick <- fun _ ->
    canv.requestFullscreen()

buttonPalette.onclick <- fun _ ->
    divPalette.hidden <- not divPalette.hidden

buttonCentre.onclick <- fun _ ->
    x <- 0.0
    y <- 0.0
    update()

buttonReset.onclick <- fun _ ->
    x <- 0.0
    y <- 0.0
    zoom <- 2.5
    paletteOffset <- 0.0
    useDoub <- false
    juliaX <- 0.0
    juliaY <- 0.0
    mandelbrotPower <- 2.0
    mandelboxScale <- 3.0
    generator <- 1
    fieldJuliaPresets.value <- string -1
    fieldMandelbrot.checked <- true
    divMandelbrot.hidden <- false
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()

PaletteMaker.buttonResetPalette.onclick <- fun _ -> 
    PaletteMaker.fieldP1X.value <- string 0.0
    PaletteMaker.fieldP1C.value <- string "#000764"
    PaletteMaker.fieldP2X.value <- string 0.16
    PaletteMaker.fieldP2C.value <- string "#206bcb"
    PaletteMaker.fieldP3X.value <- string 0.42
    PaletteMaker.fieldP3C.value <- string "#edffff"
    PaletteMaker.fieldP4X.value <- string 0.64
    PaletteMaker.fieldP4C.value <- string "#ffaa00"
    PaletteMaker.fieldP5X.value <- string 0.855
    PaletteMaker.fieldP5C.value <- string "#000200"
    PaletteMaker.init()
    palette <- PaletteMaker.getColours 76
    update()

buttonSaveImage.onclick <- fun _ ->
    let saveResWidth = window.prompt("Save resolution width:")
    if not (isNull saveResWidth) then
        let saveResHeight = window.prompt("Save resolution height:")
        if not (isNull saveResHeight) then
            if Seq.forall Char.IsDigit saveResHeight && Seq.forall Char.IsDigit saveResWidth then
                canv.width <- float saveResWidth
                canv.height <- float saveResHeight
                update()
                update()

                let mode = 
                    if fieldMandelbrot.checked then "Mandelbrot"
                    elif fieldJulia.checked then "Julia"
                    elif fieldBurningShip.checked then "BurningShip"
                    else "Mandelbox"
                let fname = sprintf "%s x=%s,y=%s,zoom=%s,offset=%s %sx%s.png" mode fieldX.value fieldY.value fieldZoom.value fieldPaletteOffset.value saveResWidth saveResHeight
                let link = document.querySelector "#link" :?> HTMLLinkElement
                link.setAttribute("download", fname)
                link.setAttribute("href", canv.toDataURL("png").Replace("image/png", "image/octet-stream"))
                link.click()
                canv.width <- float WIDTH
                canv.height <- float HEIGHT

document.onfullscreenchange <- fun _ ->
    if isNull document.fullscreenElement then
        canv.width <- WIDTH
        canv.height <- HEIGHT
        update()

// TODO: make this actually working so I don't have to just call it twice
window.onresize <- fun _ -> update(); update()