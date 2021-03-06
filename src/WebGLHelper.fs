module WebGLHelper

open Fetch
// open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types
open Browser.Dom

type GL = WebGLRenderingContext

let createShaderProgram (gl: GL) vsSource fsSource =
    let vertexShader = gl.createShader(gl.VERTEX_SHADER)
    gl.shaderSource(vertexShader, vsSource)
    gl.compileShader(vertexShader)
    if gl.getShaderInfoLog(vertexShader) <> "" then
        window.alert(sprintf "VERTEX SHADER LOG: %s" (gl.getShaderInfoLog(vertexShader)))
        console.error("VERTEX SHADER LOG:", gl.getShaderInfoLog(vertexShader))

    let fragShader = gl.createShader(gl.FRAGMENT_SHADER)
    gl.shaderSource(fragShader, fsSource)
    gl.compileShader(fragShader)
    if gl.getShaderInfoLog(fragShader) <> "" then
        window.alert(sprintf "FRAGMENT SHADER LOG: %s" (gl.getShaderInfoLog(fragShader)))
        console.error("FRAGMENT SHADER LOG:", gl.getShaderInfoLog(fragShader))

    let program = gl.createProgram()
    gl.attachShader(program, vertexShader)
    gl.attachShader(program, fragShader)
    gl.linkProgram(program)

    program

let createBuffer (items: float[]) (gl: GL) =
    let buffer = gl.createBuffer()

    gl.bindBuffer(gl.ARRAY_BUFFER, buffer)
    gl.bufferData(
        gl.ARRAY_BUFFER, 
        createNew Fable.Core.JS.Constructors.Float32Array items 
        |> unbox, 
        gl.STATIC_DRAW
    )

    buffer

let clear (gl: GL) =
    gl.clearColor(0., 0., 0., 0.)
    gl.clearDepth(1.)
    gl.enable(gl.DEPTH_TEST)
    gl.depthFunc(gl.LEQUAL)

    gl.clear(gl.COLOR_BUFFER_BIT)
    gl.clear(gl.DEPTH_BUFFER_BIT)

let initBuffers (gl: GL) =
    let positions =
        createBuffer
            [|
                -1.0; -1.0;
                1.0; -1.0;
                -1.0;  1.0;
                1.0;  1.0
            |] gl
    let textureCoords =
        createBuffer
            [|
                0.0; 0.0;
                1.0; 0.0;
                0.0; 1.0;
                1.0; 1.0
            |] gl
    positions, textureCoords

let createAttributeLocation (gl: GL) program name =
    let attributeLocation = gl.getAttribLocation(program, name)
    gl.enableVertexAttribArray(attributeLocation)

    attributeLocation

let createUniformLocation (gl: GL) program name =
    gl.getUniformLocation(program, name)
    