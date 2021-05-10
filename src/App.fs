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
let divPallette = getDivElement "pallettemaker"
let divMandelbox = getDivElement "settingsmandelbox"
let divJulia = getDivElement "settingsjulia"

let getInputElement id = sprintf "#%s" id |> document.querySelector :?> HTMLInputElement
let fieldZoom = getInputElement "zoom"
let fieldX = getInputElement "x"
let fieldY = getInputElement "y"
let fieldPalletteOffset = getInputElement "palletteoffset"
let fieldFractal = getInputElement "fractal"
let fieldMandelbrot = getInputElement "mandelbrot"
let fieldBurningShip = getInputElement "burningship"
let fieldMandelbox = getInputElement "mandelbox"
let fieldMandelboxScale = getInputElement "mandelboxscale"
let fieldJulia = getInputElement "julia"
let fieldJuliaX = getInputElement "juliax"
let fieldJuliaY = getInputElement "juliay"
let fieldUseDoub = getInputElement "usedoub"
let fieldJuliaPresets = document.querySelector "#juliapresets" :?> HTMLSelectElement

let getButtonElement id = sprintf "#%s" id |> document.querySelector :?> HTMLButtonElement
let buttonFullscreen = getButtonElement "fullscreen"
let buttonPallette = getButtonElement "pallettebutton"
let buttonCentre = getButtonElement "centre"
let buttonReset = getButtonElement "reset"
let buttonSaveImage = getButtonElement "saveimage"

let cookieX = findCookieValue "x"
let cookieY = findCookieValue "y"
let cookieZoom = findCookieValue "zoom"
let cookiePalletteOffset = findCookieValue "palletteoffset"
let cookieGenerator = findCookieValue "generator"
let cookieMandelboxScale = findCookieValue "mandelboxscale"
let cookieJuliaX = findCookieValue "jx"
let cookieJuliaY = findCookieValue "jy"
let cookieUseDoub = findCookieValue "usedoub"

let setupCookies () = 
    let cookies = [|
        cookieX; 
        cookieY; 
        cookieZoom; 
        cookiePalletteOffset; 
        cookieGenerator; 
        cookieMandelboxScale; 
        cookieJuliaX; 
        cookieJuliaY; 
        cookieUseDoub;
        PalletteMaker.cookieP1X;
        PalletteMaker.cookieP1C;
        PalletteMaker.cookieP2X;
        PalletteMaker.cookieP2C;
        PalletteMaker.cookieP3X;
        PalletteMaker.cookieP3C;
        PalletteMaker.cookieP4X;
        PalletteMaker.cookieP4C;
        PalletteMaker.cookieP5X;
        PalletteMaker.cookieP5C;
    |]
    if (Array.forall (fun (c: option<string>) -> not c.IsNone) cookies) then
        try  // Prevents sneaky people from meddling with my cookies
            fieldX.value <- cookieX.Value
            fieldY.value <- cookieY.Value
            fieldZoom.value <- cookieZoom.Value
            fieldPalletteOffset.value <- cookiePalletteOffset.Value
            match cookieGenerator.Value with
            | "1" -> fieldMandelbrot.checked <- true
            | "2" -> fieldJulia.checked <- true; divJulia.hidden <- false
            | "3" -> fieldBurningShip.checked <- true
            | "4" -> fieldMandelbox.checked <- true; divMandelbox.hidden <- false
            | _ -> ()
            fieldMandelboxScale.value <- cookieMandelboxScale.Value
            fieldJuliaX.value <- cookieJuliaX.Value
            fieldJuliaY.value <- cookieJuliaY.Value
            fieldUseDoub.checked <- if cookieJuliaY.Value = "true" then true else false
            PalletteMaker.fieldP1X.value <- PalletteMaker.cookieP1X.Value
            PalletteMaker.fieldP1C.value <- PalletteMaker.cookieP1C.Value
            PalletteMaker.fieldP2X.value <- PalletteMaker.cookieP2X.Value
            PalletteMaker.fieldP2C.value <- PalletteMaker.cookieP2C.Value
            PalletteMaker.fieldP3X.value <- PalletteMaker.cookieP3X.Value
            PalletteMaker.fieldP3C.value <- PalletteMaker.cookieP3C.Value
            PalletteMaker.fieldP4X.value <- PalletteMaker.cookieP4X.Value
            PalletteMaker.fieldP4C.value <- PalletteMaker.cookieP4C.Value
            PalletteMaker.fieldP5X.value <- PalletteMaker.cookieP5X.Value
            PalletteMaker.fieldP5C.value <- PalletteMaker.cookieP5C.Value
        with
        | _ -> ()
setupCookies()

let mutable zoom = float fieldZoom.value
let mutable x = float fieldX.value
let mutable y = float fieldY.value
let mutable palletteOffset = float fieldPalletteOffset.value
let mutable generator = 
    if fieldMandelbrot.checked then 1  // 1: Mandelrot
    elif fieldJulia.checked then 2  // 2: Julia
    elif fieldBurningShip.checked then 3
    elif fieldMandelbox.checked then 4  // 3: Mandelbox
    else -1
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

PalletteMaker.init()
let mutable pallette = PalletteMaker.getColours 76

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
    document.cookie <- sprintf "palletteoffset=%f" palletteOffset
    document.cookie <- sprintf "generator=%i" generator
    document.cookie <- sprintf "mandelboxscale=%f" mandelboxScale
    document.cookie <- sprintf "jx=%f" juliaX
    document.cookie <- sprintf "jy=%f" juliaY
    document.cookie <- sprintf "usedoub=%b" useDoub

    fieldX.value <- string x
    fieldY.value <- string y
    fieldZoom.value <- string zoom
    fieldPalletteOffset.value <- string palletteOffset
    fieldUseDoub.checked <- useDoub
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
    let palletteOffsetUniform = uLoc "uPalletteOffset"
    let ratioUniform = uLoc "uRatio"
    let generatorUniform = uLoc "uGenerator"
    let mandelboxScaleUniform = uLoc "uMandelboxScale"
    let juliaXUniform = uLoc "uJuliaX"
    let juliaYUniform = uLoc "uJuliaY"
    let zoomDoubUniform = uLoc "uZoomDoub"
    let xcDoubUniform = uLoc "xcDoub"
    let ycDoubUniform = uLoc "ycDoub"
    let useDoubUniform = uLoc "uUseDoub"
    let palletteUniform = uLoc "uPallette"

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
    gl.uniform1f(palletteOffsetUniform, palletteOffset)
    gl.uniform1f(ratioUniform, ratio)
    gl.uniform1f(generatorUniform, float generator)
    gl.uniform1f(mandelboxScaleUniform, mandelboxScale)
    gl.uniform1f(juliaXUniform, juliaX)
    gl.uniform1f(juliaYUniform, juliaY)
    gl.uniform2fv(zoomDoubUniform, zoom |> SplitDouble.ofFloat |> SplitDouble.toUniform)
    gl.uniform2fv(xcDoubUniform, x |> SplitDouble.ofFloat |> SplitDouble.toUniform)
    gl.uniform2fv(ycDoubUniform, y |> SplitDouble.ofFloat |> SplitDouble.toUniform)
    gl.uniform1i(useDoubUniform, if useDoub then 1.0 else 0.0)

    gl.uniform3fv(palletteUniform, pallette |> Array.map (fun (r, g, b) -> [|r; g; b|]) |> Array.concat |> Fable.Core.JS.Constructors.Float32Array.Create)

    gl.drawArrays(gl.TRIANGLE_STRIP, 0., 4.)
    gl.useProgram(shaderProgram)
update()

fieldZoom.oninput <- fun _ -> zoom <- float fieldZoom.value; update()
fieldX.oninput <- fun _ -> x <- float fieldX.value; update()
fieldY.oninput <- fun _ -> y <- float fieldY.value; update()
fieldMandelboxScale.oninput <- fun _ -> mandelboxScale <- float fieldMandelboxScale.value; update()
fieldPalletteOffset.oninput <- fun _ -> palletteOffset <- float fieldPalletteOffset.value; update()
fieldJuliaX.oninput <- fun _ -> juliaX <- float fieldJuliaX.value; update()
fieldJuliaY.oninput <- fun _ -> juliaY <- float fieldJuliaY.value; update()
fieldMandelbrot.oninput <- fun _ -> 
    generator <- 1
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()
fieldJulia.oninput <- fun _ -> 
    generator <- 2
    divJulia.hidden <- false
    divMandelbox.hidden <- true
    update()
fieldBurningShip.oninput <- fun _ ->
    generator <- 3
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()
fieldMandelbox.oninput <- fun _ -> 
    generator <- 4
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

PalletteMaker.fieldP1X.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP1C.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP2X.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP2C.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP3X.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP3C.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP4X.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP4C.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP5X.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()
PalletteMaker.fieldP5C.oninput <- fun _ -> PalletteMaker.updatePoints(); PalletteMaker.drawPallette(); pallette <- PalletteMaker.getColours 76; update()

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

buttonPallette.onclick <- fun _ ->
    divPallette.hidden <- not divPallette.hidden

buttonCentre.onclick <- fun _ ->
    x <- 0.0
    y <- 0.0
    update()

buttonReset.onclick <- fun _ ->
    x <- 0.0
    y <- 0.0
    zoom <- 2.5
    palletteOffset <- 0.0
    useDoub <- false
    juliaX <- 0.0
    juliaY <- 0.0
    mandelboxScale <- 3.0
    generator <- 1
    fieldJuliaPresets.value <- string -1
    fieldMandelbrot.checked <- true
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()

PalletteMaker.buttonResetPallette.onclick <- fun _ -> 
    PalletteMaker.fieldP1X.value <- string 0.0
    PalletteMaker.fieldP1C.value <- string "#000764"
    PalletteMaker.fieldP2X.value <- string 0.16
    PalletteMaker.fieldP2C.value <- string "#206bcb"
    PalletteMaker.fieldP3X.value <- string 0.42
    PalletteMaker.fieldP3C.value <- string "#edffff"
    PalletteMaker.fieldP4X.value <- string 0.64
    PalletteMaker.fieldP4C.value <- string "#ffaa00"
    PalletteMaker.fieldP5X.value <- string 0.855
    PalletteMaker.fieldP5C.value <- string "#000200"
    PalletteMaker.init()
    pallette <- PalletteMaker.getColours 76
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
                let fname = sprintf "%s x=%s,y=%s,zoom=%s,offset=%s %sx%s.png" mode fieldX.value fieldY.value fieldZoom.value fieldPalletteOffset.value saveResWidth saveResHeight
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