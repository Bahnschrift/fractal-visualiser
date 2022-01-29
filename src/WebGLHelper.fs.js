import { printf, toText } from "./.fable/fable-library.3.1.1/String.js";
import { some } from "./.fable/fable-library.3.1.1/Option.js";

export function createShaderProgram(gl, vsSource, fsSource) {
    let arg10, arg10_1;
    const vertexShader = gl.createShader(gl.VERTEX_SHADER);
    gl.shaderSource(vertexShader, vsSource);
    gl.compileShader(vertexShader);
    if (gl.getShaderInfoLog(vertexShader) !== "") {
        window.alert(some((arg10 = gl.getShaderInfoLog(vertexShader), toText(printf("VERTEX SHADER LOG: %s"))(arg10))));
        console.error(some("VERTEX SHADER LOG:"), gl.getShaderInfoLog(vertexShader));
    }
    const fragShader = gl.createShader(gl.FRAGMENT_SHADER);
    gl.shaderSource(fragShader, fsSource);
    gl.compileShader(fragShader);
    if (gl.getShaderInfoLog(fragShader) !== "") {
        window.alert(some((arg10_1 = gl.getShaderInfoLog(fragShader), toText(printf("FRAGMENT SHADER LOG: %s"))(arg10_1))));
        console.error(some("FRAGMENT SHADER LOG:"), gl.getShaderInfoLog(fragShader));
    }
    const program = gl.createProgram();
    gl.attachShader(program, vertexShader);
    gl.attachShader(program, fragShader);
    gl.linkProgram(program);
    return program;
}

export function createBuffer(items, gl) {
    const buffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(items), gl.STATIC_DRAW);
    return buffer;
}

export function clear(gl) {
    gl.clearColor(0, 0, 0, 0);
    gl.clearDepth(1);
    gl.enable(gl.DEPTH_TEST);
    gl.depthFunc(gl.LEQUAL);
    gl.clear(gl.COLOR_BUFFER_BIT);
    gl.clear(gl.DEPTH_BUFFER_BIT);
}

export function initBuffers(gl) {
    return [createBuffer(new Float64Array([-1, -1, 1, -1, -1, 1, 1, 1]), gl), createBuffer(new Float64Array([0, 0, 1, 0, 0, 1, 1, 1]), gl)];
}

export function createAttributeLocation(gl, program, name) {
    const attributeLocation = gl.getAttribLocation(program, name);
    gl.enableVertexAttribArray(attributeLocation);
    return attributeLocation;
}

export function createUniformLocation(gl, program, name) {
    return gl.getUniformLocation(program, name);
}

