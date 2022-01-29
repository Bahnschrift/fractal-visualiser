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
let divpalette = getDivElement "palettemaker"
let divMandelbrot = getDivElement "settingsmandelbrot"
let divMandelbox = getDivElement "settingsmandelbox"
let divJulia = getDivElement "settingsjulia"

let getInputElement id = sprintf "#%s" id |> document.querySelector :?> HTMLInputElement
let fieldZoom = getInputElement "zoom"
let fieldX = getInputElement "x"
let fieldY = getInputElement "y"
let fieldpaletteOffset = getInputElement "paletteoffset"
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
let buttonpalette = getButtonElement "palettebutton"
let buttonCentre = getButtonElement "centre"
let buttonReset = getButtonElement "reset"
let buttonSaveImage = getButtonElement "saveimage"

let cookieX = findCookieValue "x"
let cookieY = findCookieValue "y"
let cookieZoom = findCookieValue "zoom"
let cookiepaletteOffset = findCookieValue "paletteoffset"
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
        cookiepaletteOffset; 
        cookieGenerator; 
        cookieMandelbrotPower;
        cookieMandelboxScale; 
        cookieJuliaX; 
        cookieJuliaY; 
        cookieUseDoub;
        paletteMaker.cookieP1X;
        paletteMaker.cookieP1C;
        paletteMaker.cookieP2X;
        paletteMaker.cookieP2C;
        paletteMaker.cookieP3X;
        paletteMaker.cookieP3C;
        paletteMaker.cookieP4X;
        paletteMaker.cookieP4C;
        paletteMaker.cookieP5X;
        paletteMaker.cookieP5C;
    |]
    if (Array.forall (fun (c: option<string>) -> not c.IsNone) cookies) then
        try  // Prevents sneaky people from meddling with my cookies
            fieldX.value <- cookieX.Value
            fieldY.value <- cookieY.Value
            fieldZoom.value <- cookieZoom.Value
            fieldpaletteOffset.value <- cookiepaletteOffset.Value
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
            paletteMaker.fieldP1X.value <- paletteMaker.cookieP1X.Value
            paletteMaker.fieldP1C.value <- paletteMaker.cookieP1C.Value
            paletteMaker.fieldP2X.value <- paletteMaker.cookieP2X.Value
            paletteMaker.fieldP2C.value <- paletteMaker.cookieP2C.Value
            paletteMaker.fieldP3X.value <- paletteMaker.cookieP3X.Value
            paletteMaker.fieldP3C.value <- paletteMaker.cookieP3C.Value
            paletteMaker.fieldP4X.value <- paletteMaker.cookieP4X.Value
            paletteMaker.fieldP4C.value <- paletteMaker.cookieP4C.Value
            paletteMaker.fieldP5X.value <- paletteMaker.cookieP5X.Value
            paletteMaker.fieldP5C.value <- paletteMaker.cookieP5C.Value
        with
        | _ -> ()
setupCookies()

let mutable zoom = float fieldZoom.value
let mutable x = float fieldX.value
let mutable y = float fieldY.value
let mutable paletteOffset = float fieldpaletteOffset.value
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

paletteMaker.init()
let mutable palette = paletteMaker.getColours 76

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
    fieldpaletteOffset.value <- string paletteOffset
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
    let paletteOffsetUniform = uLoc "upaletteOffset"
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
    let paletteUniform = uLoc "upalette"

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
fieldpaletteOffset.oninput <- fun _ -> paletteOffset <- float fieldpaletteOffset.value; update()
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

paletteMaker.fieldP1X.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP1C.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP2X.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP2C.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP3X.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP3C.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP4X.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP4C.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP5X.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()
paletteMaker.fieldP5C.oninput <- fun _ -> paletteMaker.updatePoints(); paletteMaker.drawpalette(); palette <- paletteMaker.getColours 76; update()

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

buttonpalette.onclick <- fun _ ->
    divpalette.hidden <- not divpalette.hidden

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

paletteMaker.buttonResetpalette.onclick <- fun _ -> 
    paletteMaker.fieldP1X.value <- string 0.0
    paletteMaker.fieldP1C.value <- string "#000764"
    paletteMaker.fieldP2X.value <- string 0.16
    paletteMaker.fieldP2C.value <- string "#206bcb"
    paletteMaker.fieldP3X.value <- string 0.42
    paletteMaker.fieldP3C.value <- string "#edffff"
    paletteMaker.fieldP4X.value <- string 0.64
    paletteMaker.fieldP4C.value <- string "#ffaa00"
    paletteMaker.fieldP5X.value <- string 0.855
    paletteMaker.fieldP5C.value <- string "#000200"
    paletteMaker.init()
    palette <- paletteMaker.getColours 76
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
                let fname = sprintf "%s x=%s,y=%s,zoom=%s,offset=%s %sx%s.png" mode fieldX.value fieldY.value fieldZoom.value fieldpaletteOffset.value saveResWidth saveResHeight
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