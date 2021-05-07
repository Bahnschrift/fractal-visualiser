module App

open Shaders
open Cookies
open WebGLHelper
open Browser.Dom
open Browser.Types

let WIDTH = float <| 1280
let HEIGHT = float <| 720

let juliaPresets = [|
    0.0, 0.8;
    0.37, 0.1;
    0.355, 0.355;
    -0.54, 0.54;
    -0.4, -0.59;
    0.34, -0.05;
    -0.687, 0.312;
    -0.673, 0.312;
    0.355534, 0.337292
|]

let getInputElement id = sprintf "#%s" id |> document.querySelector :?> HTMLInputElement

let divMandelbox = document.querySelector "#settingsmandelbox" :?> HTMLDivElement
let divJulia = document.querySelector "#settingsjulia" :?> HTMLDivElement

let fieldZoom = getInputElement "zoom"
let fieldX = getInputElement "x"
let fieldY = getInputElement "y"
let fieldFractal = getInputElement "fractal"
let fieldMandelbrot = getInputElement "mandelbrot"
let fieldMandelbox = getInputElement "mandelbox"
let fieldMandelboxScale = getInputElement "mandelboxscale"
let fieldJulia = getInputElement "julia"
let fieldJuliaX = getInputElement "juliax"
let fieldJuliaY = getInputElement "juliay"
let fieldJuliaPresets = document.querySelector "#juliapresets" :?> HTMLSelectElement

let buttonFullscreen = document.querySelector "#fullscreen" :?> HTMLButtonElement
let buttonCentre = document.querySelector "#centre" :?> HTMLButtonElement
let buttonReset = document.querySelector "#reset" :?> HTMLButtonElement

let cookieX = findCookieValue "x"
let cookieY = findCookieValue "y"
let cookieZoom = findCookieValue "zoom"
let cookieGenerator = findCookieValue "generator"
let cookieMandelboxScale = findCookieValue "mandelboxscale"
let cookieJuliaX = findCookieValue "jx"
let cookieJuliaY = findCookieValue "jy"

let cookies = [|cookieX; cookieY; cookieZoom; cookieGenerator; cookieMandelboxScale; cookieJuliaX; cookieJuliaY|]
if (Array.forall (fun (c: option<string>) -> not c.IsNone) cookies) then
    try  // Prevents sneaky people from meddling with my cookies
        fieldX.value <- cookieX.Value
        fieldY.value <- cookieY.Value
        fieldZoom.value <- cookieZoom.Value
        match cookieGenerator.Value with
        | "1" -> fieldMandelbrot.checked <- true
        | "2" -> fieldJulia.checked <- true; divJulia.hidden <- false;
        | "3" -> fieldMandelbox.checked <- true; divMandelbox.hidden <- false;
        | _ -> ()
        fieldMandelboxScale.value <- cookieMandelboxScale.Value
        fieldJuliaX.value <- cookieJuliaX.Value
        fieldJuliaY.value <- cookieJuliaY.Value
    with
    | _ -> ()

let canv = document.querySelector "#canv" :?> HTMLCanvasElement
canv.width <- WIDTH
canv.height <- HEIGHT

let gl = canv.getContext "webgl" :?> GL
clear gl
let shaderProgram = createShaderProgram gl vsMandel fsMandel

let update () = 
    let resizeCanvas(canvas: HTMLCanvasElement) = 
        let displayWidth = canvas.clientWidth
        let displayHeight = canvas.clientHeight
        let needResize = 
            canvas.width <> displayWidth || 
            canvas.height <> displayHeight
        if needResize then
            canvas.width <- displayWidth
            canvas.height <- displayHeight
    
    let zoom = float fieldZoom.value
    let mutable x = float fieldX.value
    let mutable y = float fieldY.value
    let generator = 
        if fieldMandelbrot.checked then 1  // 1: Mandelrot
        elif fieldJulia.checked then 2  // 2: Julia
        elif fieldMandelbox.checked then 3  // 3: Mandelbox
        else -1
    let mandelboxScale = float fieldMandelboxScale.value
    let juliaX = float fieldJuliaX.value
    let juliaY = float fieldJuliaY.value
    document.cookie <- sprintf "zoom=%f;" zoom
    document.cookie <- sprintf "x=%f;" x
    document.cookie <- sprintf "y=%f;" y
    document.cookie <- sprintf "generator=%i" generator
    document.cookie <- sprintf "mandelboxscale=%f" mandelboxScale
    document.cookie <- sprintf "jx=%f" juliaX
    document.cookie <- sprintf "jy=%f" juliaY

    fieldX.step <- 0.1 * zoom |> string
    fieldY.step <- 0.1 * zoom |> string
    fieldZoom.step <- 0.1 * zoom |> string

    let positionBuffer, colourBuffer = initBuffers gl
    let vertexPositionAttr = createAttributeLocation gl shaderProgram "aVertexPosition"
    let textureCoordAttr = createAttributeLocation gl shaderProgram "aTextureCoord"
    let zoomUniform = createUniformLocation gl shaderProgram "uZoom"
    let xcUniform = createUniformLocation gl shaderProgram "xc"
    let ycUniform = createUniformLocation gl shaderProgram "yc"
    let ratioUniform = createUniformLocation gl shaderProgram "uRatio"
    let generatorUniform = createUniformLocation gl shaderProgram "uGenerator"
    let mandelboxScaleUniform = createUniformLocation gl shaderProgram "uMandelboxScale"
    let juliaXUniform = createUniformLocation gl shaderProgram "uJuliaX"
    let juliaYUniform = createUniformLocation gl shaderProgram "uJuliaY"

    let draw zoom x y (ratio: float) =
        resizeCanvas canv
        gl.viewport(0.0, 0.0, canv.width, canv.height)
        gl.useProgram(shaderProgram)

        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer)
        gl.vertexAttribPointer(vertexPositionAttr, 2., gl.FLOAT, false, 0., 0.)
        gl.bindBuffer(gl.ARRAY_BUFFER, colourBuffer)
        gl.vertexAttribPointer(textureCoordAttr, 2., gl.FLOAT, false, 0., 0.)

        gl.uniform1f(zoomUniform, zoom)
        gl.uniform1f(xcUniform, x)
        gl.uniform1f(ycUniform, y)
        gl.uniform1f(ratioUniform, ratio)
        gl.uniform1f(generatorUniform, float generator)
        gl.uniform1f(mandelboxScaleUniform, mandelboxScale)
        gl.uniform1f(juliaXUniform, juliaX)
        gl.uniform1f(juliaYUniform, juliaY)

        gl.drawArrays(gl.TRIANGLE_STRIP, 0., 4.)

    draw zoom x y (canv.width / canv.height)
    gl.useProgram(shaderProgram)
update()

fieldZoom.oninput <- fun _ -> update()
fieldX.oninput <- fun _ -> update()
fieldY.oninput <- fun _ -> update()
fieldMandelboxScale.oninput <- fun _ -> update()
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
    clear gl
    canv.requestFullscreen()
    update()

buttonCentre.onclick <- fun _ ->
    fieldX.value <- string <| 0
    fieldY.value <- string <| 0
    update()

buttonReset.onclick <- fun _ ->
    fieldX.value <- string <| 0
    fieldY.value <- string <| 0
    fieldZoom.value <- string <| 2.5
    fieldMandelboxScale.value <- string <| 3
    fieldJuliaX.value <- string <| 0
    fieldJuliaY.value <- string <| 0
    fieldJuliaPresets.value <- string <| -1
    fieldMandelbrot.checked <- true
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()

document.onfullscreenchange <- fun _ ->
    if isNull document.fullscreenElement then
        canv.width <- WIDTH
        canv.height <- HEIGHT
        update()