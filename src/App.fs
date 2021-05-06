module App

open Shaders
open Cookies
open WebGLHelper
open Browser.Dom
open Browser.Types

let WIDTH = float <| 1280
let HEIGHT = float <| 720


let fieldZoom = document.querySelector "#zoom" :?> HTMLInputElement
let fieldX = document.querySelector "#x" :?> HTMLInputElement
let fieldY = document.querySelector "#y" :?> HTMLInputElement
let buttonReset = document.querySelector "#reset" :?> HTMLButtonElement

let x, y, zoom = findCookieValue "x", findCookieValue "y", findCookieValue "zoom"
if (not x.IsNone && not y.IsNone && not zoom.IsNone) then
    fieldX.value <- x.Value
    fieldY.value <- y.Value
    fieldZoom.value <- zoom.Value

console.log(document.cookie)

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
    document.cookie <- sprintf "zoom=%f;" zoom
    document.cookie <- sprintf "x=%f;" x
    document.cookie <- sprintf "y=%f;" y
    document.cookie <- sprintf "expiry=2038-01-19 04:14:07;"
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

        gl.drawArrays(gl.TRIANGLE_STRIP, 0., 4.)

    draw zoom x y (WIDTH / HEIGHT)
    gl.useProgram(shaderProgram)
update()

fieldZoom.oninput <- fun _ -> update()
fieldX.oninput <- fun _ -> update()
fieldY.oninput <- fun _ -> update()

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
    console.log(e.offsetX, e.offsetY)
    ()
let handleMouseUp (e: MouseEvent) = 
    console.log("mouse up")
canv.addEventListener("mousedown", fun e -> handleMouseDown (e :?> MouseEvent))
canv.addEventListener("mouseup", fun e -> handleMouseUp (e :?> MouseEvent))

buttonReset.onclick <- fun _ ->
    fieldX.value <- string <| -0.75
    fieldY.value <- string <| 0.0
    fieldZoom.value <- string <| 2.5
    update()