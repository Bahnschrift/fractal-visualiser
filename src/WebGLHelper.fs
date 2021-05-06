module WebGLHelper

open Fetch
// open Fable.Core.JS
open Fable.Core.JsInterop
open Browser.Types


type GL = WebGLRenderingContext

let createShaderProgram (gl: GL) vsSource fsSource =
    let vertexShader = gl.createShader(gl.VERTEX_SHADER)
    gl.shaderSource(vertexShader, vsSource)
    gl.compileShader(vertexShader)
    Fable.Core.JS.console.log("VERTEX SHADER LOG:", gl.getShaderInfoLog(vertexShader))

    let fragShader = gl.createShader(gl.FRAGMENT_SHADER)
    gl.shaderSource(fragShader, fsSource)
    gl.compileShader(fragShader)
    Fable.Core.JS.console.log("FRAGMENT SHADER LOG:", gl.getShaderInfoLog(fragShader))

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
    gl.clearColor(0., 0., 0., 1.)
    gl.clearDepth(1.)
    gl.enable(gl.DEPTH_TEST)
    gl.depthFunc(gl.LEQUAL)

    gl.clear(float(int gl.COLOR_BUFFER_BIT ||| int gl.DEPTH_BUFFER_BIT))


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
    