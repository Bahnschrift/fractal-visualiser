
export function createShaderProgram(gl, vsSource, fsSource) {
    const vertexShader = gl.createShader(gl.VERTEX_SHADER);
    gl.shaderSource(vertexShader, vsSource);
    gl.compileShader(vertexShader);
    const fragShader = gl.createShader(gl.FRAGMENT_SHADER);
    gl.shaderSource(fragShader, fsSource);
    gl.compileShader(fragShader);
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
    gl.clearColor(0, 0, 0, 1);
    gl.clearDepth(1);
    gl.enable(gl.DEPTH_TEST);
    gl.depthFunc(gl.LEQUAL);
    gl.clear((~(~gl.COLOR_BUFFER_BIT)) | (~(~gl.DEPTH_BUFFER_BIT)));
}

export function initBuffers(gl) {
    const positions = createBuffer(new Float64Array([-1, -1, 1, -1, -1, 1, 1, 1]), gl);
    const textureCoords = createBuffer(new Float64Array([0, 0, 1, 0, 0, 1, 1, 1]), gl);
    return [positions, textureCoords];
}

export function createAttributeLocation(gl, program, name) {
    const attributeLocation = gl.getAttribLocation(program, name);
    gl.enableVertexAttribArray(attributeLocation);
    return attributeLocation;
}

export function createUniformLocation(gl, program, name) {
    return gl.getUniformLocation(program, name);
}

