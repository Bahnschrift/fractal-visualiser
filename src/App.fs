module App

open System
open Shaders
open Cookies
open WebGLHelper
open Browser.Dom
open Browser.Types

let WIDTH = float <| 800
let HEIGHT = float <| 450

let divMandelbox = document.querySelector "#settingsmandelbox" :?> HTMLDivElement
let divJulia = document.querySelector "#settingsjulia" :?> HTMLDivElement

let fieldZoom = document.querySelector "#zoom" :?> HTMLInputElement
let fieldX = document.querySelector "#x" :?> HTMLInputElement
let fieldY = document.querySelector "#y" :?> HTMLInputElement
let fieldFractal = document.querySelector ".fractal" :?> HTMLInputElement
let buttonReset = document.querySelector "#reset" :?> HTMLButtonElement
let fieldMandelbrot = document.querySelector "#mandelbrot" :?> HTMLInputElement
let fieldMandelbox = document.querySelector "#mandelbox" :?> HTMLInputElement
let fieldMandelboxScale = document.querySelector "#mandelboxscale" :?> HTMLInputElement
let fieldJulia = document.querySelector "#julia" :?> HTMLInputElement
let fieldJuliaX = document.querySelector "#juliax" :?> HTMLInputElement
let fieldJuliaY = document.querySelector "#juliay" :?> HTMLInputElement

let cookieX = findCookieValue "x"
let cookieY = findCookieValue "y"
let cookieZoom = findCookieValue "zoom"
let cookieGenerator = findCookieValue "generator"
let cookieMandelboxScale = findCookieValue "mandelboxscale"
let cookieJuliaX = findCookieValue "jx"
let cookieJuliaY = findCookieValue "jy"

let cookies = [|cookieX; cookieY; cookieZoom; cookieGenerator; cookieMandelboxScale; cookieJuliaX; cookieJuliaY|]
if (Array.forall (fun (c: option<string>) -> not c.IsNone) cookies) then
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

let canv = document.querySelector ".canv" :?> HTMLCanvasElement
canv.width <- WIDTH
canv.height <- HEIGHT

let gl = canv.getContext "webgl" :?> GL
clear gl
let shaderProgram = createShaderProgram gl vsMandel fsMandel

let update () = 
    let zoom = float fieldZoom.value
    let x = float fieldX.value
    let y = float fieldY.value
    // 1: Mandelrot
    // 2: Julia
    // 3: Mandelbox
    let generator = if fieldMandelbrot.checked then 1 elif fieldJulia.checked then 2 elif fieldMandelbox.checked then 3 else -1
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

    draw zoom x y (WIDTH / HEIGHT)
    gl.useProgram(shaderProgram)
update()

fieldZoom.oninput <- fun _ -> update()
fieldX.oninput <- fun _ -> update()
fieldY.oninput <- fun _ -> update()
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
fieldMandelboxScale.oninput <- fun _ -> update()
fieldJuliaX.oninput <- fun _ -> update()
fieldJuliaY.oninput <- fun _ -> update()

let handleKeypress (e: KeyboardEvent) =
    let scale = 0.1  // if e.shiftKey then 0.01 else 0.1 
    match e.key with
    | "w" -> fieldY.value <- string <| float fieldY.value + float fieldZoom.value * scale; update()
    | "s" -> fieldY.value <- string <| float fieldY.value - float fieldZoom.value * scale; update()
    | "a" -> fieldX.value <- string <| float fieldX.value - float fieldZoom.value * scale; update()
    | "d" -> fieldX.value <- string <| float fieldX.value + float fieldZoom.value * scale; update()
    | "q" -> fieldZoom.value <- string <| float fieldZoom.value + float fieldZoom.value * scale; update()
    | "e" -> fieldZoom.value <- string <| float fieldZoom.value - float fieldZoom.value * scale; update()
    | _ -> ()
document.addEventListener("keydown", fun e -> handleKeypress (e :?> KeyboardEvent))

let handleMouseDown (e: MouseEvent) = 
    let zoom = float fieldZoom.value
    ()
let handleMouseUp (e: MouseEvent) = 
    ()
canv.addEventListener("mousedown", fun e -> handleMouseDown (e :?> MouseEvent))
canv.addEventListener("mouseup", fun e -> handleMouseUp (e :?> MouseEvent))

buttonReset.onclick <- fun _ ->
    fieldX.value <- string <| -0.75
    fieldY.value <- string <| 0.0
    fieldZoom.value <- string <| 2.5
    fieldMandelbrot.checked <- true
    divMandelbox.hidden <- true
    divJulia.hidden <- true
    update()