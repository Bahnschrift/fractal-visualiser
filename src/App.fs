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
let divMandelbox = getDivElement "settingsmandelbox"
let divJulia = getDivElement "settingsjulia"

let getInputElement id = sprintf "#%s" id |> document.querySelector :?> HTMLInputElement
let fieldZoom = getInputElement "zoom"
let fieldX = getInputElement "x"
let fieldY = getInputElement "y"
let fieldPalatteOffset = getInputElement "palatteoffset"
let fieldFractal = getInputElement "fractal"
let fieldMandelbrot = getInputElement "mandelbrot"
let fieldMandelbox = getInputElement "mandelbox"
let fieldMandelboxScale = getInputElement "mandelboxscale"
let fieldJulia = getInputElement "julia"
let fieldJuliaX = getInputElement "juliax"
let fieldJuliaY = getInputElement "juliay"
let fieldUseDoub = getInputElement "usedoub"
let fieldJuliaPresets = document.querySelector "#juliapresets" :?> HTMLSelectElement

let getButtonElement id = sprintf "#%s" id |> document.querySelector :?> HTMLButtonElement
let buttonFullscreen = getButtonElement "fullscreen"
let buttonCentre = getButtonElement "centre"
let buttonReset = getButtonElement "reset"
let buttonSaveImage = getButtonElement "saveimage"

let cookieX = findCookieValue "x"
let cookieY = findCookieValue "y"
let cookieZoom = findCookieValue "zoom"
let cookiePalatteOffset = findCookieValue "palatteoffset"
let cookieGenerator = findCookieValue "generator"
let cookieMandelboxScale = findCookieValue "mandelboxscale"
let cookieJuliaX = findCookieValue "jx"
let cookieJuliaY = findCookieValue "jy"
let cookieUseDoub = findCookieValue "usedoub"

let cookies = [|cookieX; cookieY; cookieZoom; cookiePalatteOffset; cookieGenerator; cookieMandelboxScale; cookieJuliaX; cookieJuliaY; cookieUseDoub|]
if (Array.forall (fun (c: option<string>) -> not c.IsNone) cookies) then
    try  // Prevents sneaky people from meddling with my cookies
        fieldX.value <- cookieX.Value
        fieldY.value <- cookieY.Value
        fieldZoom.value <- cookieZoom.Value
        fieldPalatteOffset.value <- cookiePalatteOffset.Value
        match cookieGenerator.Value with
        | "1" -> fieldMandelbrot.checked <- true
        | "2" -> fieldJulia.checked <- true; divJulia.hidden <- false;
        | "3" -> fieldMandelbox.checked <- true; divMandelbox.hidden <- false;
        | _ -> ()
        fieldMandelboxScale.value <- cookieMandelboxScale.Value
        fieldJuliaX.value <- cookieJuliaX.Value
        fieldJuliaY.value <- cookieJuliaY.Value
        fieldUseDoub.checked <- if cookieJuliaY.Value = "true" then true else false
    with
    | _ -> ()

let canv = document.querySelector "#canv" :?> HTMLCanvasElement
canv.width <- WIDTH
canv.height <- HEIGHT

let gl = canv.getContext "webgl" :?> GL
clear gl
let shaderProgram = createShaderProgram gl vsMandel fsMandel

let update() = 
    let resizeCanvas(canvas: HTMLCanvasElement) = 
        let displayWidth = canvas.clientWidth
        let displayHeight = canvas.clientHeight
        let needResize = 
            canvas.width <> displayWidth || 
            canvas.height <> displayHeight
        if needResize then
            // canvas.width <- displayWidth
            // canvas.height <- displayHeight
            canvas.width <- window.innerWidth * window.devicePixelRatio
            canvas.height <- window.innerHeight * window.devicePixelRatio
    
    let zoom = float fieldZoom.value
    let mutable x = float fieldX.value
    let mutable y = float fieldY.value
    let palatteOffset = float fieldPalatteOffset.value
    let generator = 
        if fieldMandelbrot.checked then 1  // 1: Mandelrot
        elif fieldJulia.checked then 2  // 2: Julia
        elif fieldMandelbox.checked then 3  // 3: Mandelbox
        else -1
    let mandelboxScale = float fieldMandelboxScale.value
    let juliaX = float fieldJuliaX.value
    let juliaY = float fieldJuliaY.value
    let useDoub = fieldUseDoub.checked
    document.cookie <- sprintf "zoom=%f;" zoom
    document.cookie <- sprintf "x=%f;" x
    document.cookie <- sprintf "y=%f;" y
    document.cookie <- sprintf "palatteoffset=%f" palatteOffset
    document.cookie <- sprintf "generator=%i" generator
    document.cookie <- sprintf "mandelboxscale=%f" mandelboxScale
    document.cookie <- sprintf "jx=%f" juliaX
    document.cookie <- sprintf "jy=%f" juliaY
    document.cookie <- sprintf "usedoub=%b" useDoub

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
    let palatteOffsetUniform = uLoc "uPalatteOffset"
    let ratioUniform = uLoc "uRatio"
    let generatorUniform = uLoc "uGenerator"
    let mandelboxScaleUniform = uLoc "uMandelboxScale"
    let juliaXUniform = uLoc "uJuliaX"
    let juliaYUniform = uLoc "uJuliaY"
    let zoomDoubUniform = uLoc "uZoomDoub"
    let xcDoubUniform = uLoc "xcDoub"
    let ycDoubUniform = uLoc "ycDoub"
    let useDoubUniform = uLoc "uUseDoub"

    let draw zoom x y (ratio: float) =
        resizeCanvas gl.canvas
        gl.viewport(0.0, 0.0, gl.canvas.width, gl.canvas.height)
        gl.useProgram(shaderProgram)

        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer)
        gl.vertexAttribPointer(vertexPositionAttr, 2., gl.FLOAT, false, 0., 0.)
        gl.bindBuffer(gl.ARRAY_BUFFER, colourBuffer)
        gl.vertexAttribPointer(textureCoordAttr, 2., gl.FLOAT, false, 0., 0.)

        gl.uniform1f(zoomUniform, zoom)
        gl.uniform1f(xcUniform, x)
        gl.uniform1f(ycUniform, y)
        gl.uniform1f(palatteOffsetUniform, palatteOffset)
        gl.uniform1f(ratioUniform, ratio)
        gl.uniform1f(generatorUniform, float generator)
        gl.uniform1f(mandelboxScaleUniform, mandelboxScale)
        gl.uniform1f(juliaXUniform, juliaX)
        gl.uniform1f(juliaYUniform, juliaY)
        gl.uniform2fv(zoomDoubUniform, zoom |> SplitDouble.ofFloat |> SplitDouble.toUniform)
        gl.uniform2fv(xcDoubUniform, x |> SplitDouble.ofFloat |> SplitDouble.toUniform)
        gl.uniform2fv(ycDoubUniform, y |> SplitDouble.ofFloat |> SplitDouble.toUniform)
        gl.uniform1i(useDoubUniform, if useDoub then 1.0 else 0.0)

        gl.drawArrays(gl.TRIANGLE_STRIP, 0., 4.)

    draw zoom x y (gl.canvas.width / gl.canvas.height)
    gl.useProgram(shaderProgram)
update()

fieldZoom.oninput <- fun _ -> update()
fieldX.oninput <- fun _ -> update()
fieldY.oninput <- fun _ -> update()
fieldMandelboxScale.oninput <- fun _ -> update()
fieldPalatteOffset.oninput <- fun _ -> update()
fieldJuliaX.oninput <- fun _ -> update()
fieldJuliaY.oninput <- fun _ -> update()
fieldMandelbrot.oninput <- fun _ -> 
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()
fieldJulia.oninput <- fun _ -> 
    divJulia.hidden <- false
    divMandelbox.hidden <- true
    update()
fieldMandelbox.oninput <- fun _ -> 
    divJulia.hidden <- true
    divMandelbox.hidden <- false
    update()
fieldJuliaPresets.oninput <- fun _ -> 
    let juliaPreset = int fieldJuliaPresets.value
    if juliaPreset <> -1 then
        let juliaPresetCoords = juliaPresets.[juliaPreset]
        fieldJuliaX.value <- string <| fst juliaPresetCoords
        fieldJuliaY.value <- string <| snd juliaPresetCoords
    update()
fieldUseDoub.oninput <- fun _ -> update()

// TODO: Fix this
let mutable keysDown = Set.empty
document.onkeydown <- fun e ->
    let scale = 0.075
    match e.key with
    | "w" | "s" | "a" | "d" | "q" | "e" -> keysDown <- keysDown.Add e.key
    | _ -> ()
    for key in keysDown do
        match key with
        | "w" -> fieldY.value <- string <| float fieldY.value + float fieldZoom.value * scale; update()
        | "s" -> fieldY.value <- string <| float fieldY.value - float fieldZoom.value * scale; update()
        | "a" -> fieldX.value <- string <| float fieldX.value - float fieldZoom.value * scale; update()
        | "d" -> fieldX.value <- string <| float fieldX.value + float fieldZoom.value * scale; update()
        | "q" -> fieldZoom.value <- string <| float fieldZoom.value + float fieldZoom.value * scale; update()
        | "e" -> fieldZoom.value <- string <| float fieldZoom.value - float fieldZoom.value * scale; update()
        | _ -> ()
document.onkeyup <- fun e ->
    let scale = 0.075
    match e.key with
    | "w" | "s" | "a" | "d" | "q" | "e" -> keysDown <- keysDown.Remove e.key
    | _ -> ()
    for key in keysDown do
        match key with
        | "w" -> fieldY.value <- string <| float fieldY.value + float fieldZoom.value * scale; update()
        | "s" -> fieldY.value <- string <| float fieldY.value - float fieldZoom.value * scale; update()
        | "a" -> fieldX.value <- string <| float fieldX.value - float fieldZoom.value * scale; update()
        | "d" -> fieldX.value <- string <| float fieldX.value + float fieldZoom.value * scale; update()
        | "q" -> fieldZoom.value <- string <| float fieldZoom.value + float fieldZoom.value * scale; update()
        | "e" -> fieldZoom.value <- string <| float fieldZoom.value - float fieldZoom.value * scale; update()
        | _ -> ()

buttonFullscreen.onclick <- fun _ ->
    canv.requestFullscreen()

buttonCentre.onclick <- fun _ ->
    fieldX.value <- string 0
    fieldY.value <- string 0
    update()

buttonReset.onclick <- fun _ ->
    fieldX.value <- string 0
    fieldY.value <- string 0
    fieldZoom.value <- string 2.5
    fieldPalatteOffset.value <- string 0
    fieldMandelboxScale.value <- string 3
    fieldJuliaX.value <- string 0
    fieldJuliaY.value <- string 0
    fieldJuliaPresets.value <- string -1
    fieldMandelbrot.checked <- true
    fieldUseDoub.checked <- false
    divMandelbox.hidden <- true
    divJulia.hidden <- true
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
                    else "Mandelbox"
                let fname = sprintf "%s x=%s,y=%s,zoom=%s,offset=%s %sx%s.png" mode fieldX.value fieldY.value fieldZoom.value fieldPalatteOffset.value saveResWidth saveResHeight
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