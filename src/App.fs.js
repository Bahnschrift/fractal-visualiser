import { printf, toText } from "./.fable/fable-library.3.1.1/String.js";
import { findCookieValue } from "./Cookies.fs.js";
import { some, value as value_6 } from "./.fable/fable-library.3.1.1/Option.js";
import { createUniformLocation, createAttributeLocation, initBuffers, createShaderProgram, clear } from "./WebGLHelper.fs.js";
import { fsMandel, vsMandel } from "./Shaders.fs.js";
import { parse } from "./.fable/fable-library.3.1.1/Int32.js";
import { parse as parse_1 } from "./.fable/fable-library.3.1.1/Double.js";
import { int32ToString } from "./.fable/fable-library.3.1.1/Util.js";

export const WIDTH = 1280;

export const HEIGHT = 720;

export const juliaPresets = [[0, 0.8], [0.37, 0.1], [0.355, 0.355], [-0.54, 0.54], [-0.4, -0.59], [0.34, -0.05], [0.355534, 0.337292]];

export function getInputElement(id) {
    return document.querySelector(toText(printf("#%s"))(id));
}

export const divMandelbox = document.querySelector("#settingsmandelbox");

export const divJulia = document.querySelector("#settingsjulia");

export const fieldZoom = getInputElement("zoom");

export const fieldX = getInputElement("x");

export const fieldY = getInputElement("y");

export const fieldFractal = getInputElement("fractal");

export const fieldMandelbrot = getInputElement("mandelbrot");

export const fieldMandelbox = getInputElement("mandelbox");

export const fieldMandelboxScale = getInputElement("mandelboxscale");

export const fieldJulia = getInputElement("julia");

export const fieldJuliaX = getInputElement("juliax");

export const fieldJuliaY = getInputElement("juliay");

export const fieldJuliaPresets = document.querySelector("#juliapresets");

export const buttonCentre = document.querySelector("#centre");

export const buttonReset = document.querySelector("#reset");

export const cookieX = findCookieValue("x");

export const cookieY = findCookieValue("y");

export const cookieZoom = findCookieValue("zoom");

export const cookieGenerator = findCookieValue("generator");

export const cookieMandelboxScale = findCookieValue("mandelboxscale");

export const cookieJuliaX = findCookieValue("jx");

export const cookieJuliaY = findCookieValue("jy");

export const cookies = [cookieX, cookieY, cookieZoom, cookieGenerator, cookieMandelboxScale, cookieJuliaX, cookieJuliaY];

if (cookies.every((c) => (!(c == null)))) {
    fieldX.value = value_6(cookieX);
    fieldY.value = value_6(cookieY);
    fieldZoom.value = value_6(cookieZoom);
    const matchValue = value_6(cookieGenerator);
    switch (matchValue) {
        case "1": {
            fieldMandelbrot.checked = true;
            break;
        }
        case "2": {
            fieldJulia.checked = true;
            divJulia.hidden = false;
            break;
        }
        case "3": {
            fieldMandelbox.checked = true;
            divMandelbox.hidden = false;
            break;
        }
        default: {
        }
    }
    fieldMandelboxScale.value = value_6(cookieMandelboxScale);
    fieldJuliaX.value = value_6(cookieJuliaX);
    fieldJuliaY.value = value_6(cookieJuliaY);
}

export const canv = document.querySelector("#canv");

canv.width = WIDTH;

canv.height = HEIGHT;

export const gl = canv.getContext("webgl");

clear(gl);

export const shaderProgram = createShaderProgram(gl, vsMandel, fsMandel);

export function update() {
    const juliaPreset = parse(fieldJuliaPresets.value, 511, false, 32) | 0;
    if (juliaPreset !== -1) {
        console.log(some(juliaPresets[juliaPreset]));
        const juliaPresetCoords = juliaPresets[juliaPreset];
        fieldJuliaX.value = juliaPresetCoords[0].toString();
        fieldJuliaY.value = juliaPresetCoords[1].toString();
    }
    const zoom = parse_1(fieldZoom.value);
    let x = parse_1(fieldX.value);
    let y = parse_1(fieldY.value);
    const generator = (fieldMandelbrot.checked ? 1 : (fieldJulia.checked ? 2 : (fieldMandelbox.checked ? 3 : -1))) | 0;
    const mandelboxScale = parse_1(fieldMandelboxScale.value);
    const juliaX = parse_1(fieldJuliaX.value);
    const juliaY = parse_1(fieldJuliaY.value);
    document.cookie = toText(printf("zoom=%f;"))(zoom);
    const arg10_1 = x;
    document.cookie = toText(printf("x=%f;"))(arg10_1);
    const arg10_2 = y;
    document.cookie = toText(printf("y=%f;"))(arg10_2);
    document.cookie = toText(printf("generator=%i"))(generator);
    document.cookie = toText(printf("mandelboxscale=%f"))(mandelboxScale);
    document.cookie = toText(printf("jx=%f"))(juliaX);
    document.cookie = toText(printf("jy=%f"))(juliaY);
    fieldX.step = (0.1 * zoom).toString();
    fieldY.step = (0.1 * zoom).toString();
    fieldZoom.step = (0.1 * zoom).toString();
    const patternInput = initBuffers(gl);
    const positionBuffer = patternInput[0];
    const colourBuffer = patternInput[1];
    const vertexPositionAttr = createAttributeLocation(gl, shaderProgram, "aVertexPosition");
    const textureCoordAttr = createAttributeLocation(gl, shaderProgram, "aTextureCoord");
    const zoomUniform = createUniformLocation(gl, shaderProgram, "uZoom");
    const xcUniform = createUniformLocation(gl, shaderProgram, "xc");
    const ycUniform = createUniformLocation(gl, shaderProgram, "yc");
    const ratioUniform = createUniformLocation(gl, shaderProgram, "uRatio");
    const generatorUniform = createUniformLocation(gl, shaderProgram, "uGenerator");
    const mandelboxScaleUniform = createUniformLocation(gl, shaderProgram, "uMandelboxScale");
    const juliaXUniform = createUniformLocation(gl, shaderProgram, "uJuliaX");
    const juliaYUniform = createUniformLocation(gl, shaderProgram, "uJuliaY");
    const draw = (zoom_1, x_1, y_1, ratio) => {
        gl.useProgram(shaderProgram);
        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
        gl.vertexAttribPointer(vertexPositionAttr, 2, gl.FLOAT, false, 0, 0);
        gl.bindBuffer(gl.ARRAY_BUFFER, colourBuffer);
        gl.vertexAttribPointer(textureCoordAttr, 2, gl.FLOAT, false, 0, 0);
        gl.uniform1f(zoomUniform, zoom_1);
        gl.uniform1f(xcUniform, x_1);
        gl.uniform1f(ycUniform, y_1);
        gl.uniform1f(ratioUniform, ratio);
        gl.uniform1f(generatorUniform, generator);
        gl.uniform1f(mandelboxScaleUniform, mandelboxScale);
        gl.uniform1f(juliaXUniform, juliaX);
        gl.uniform1f(juliaYUniform, juliaY);
        gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
    };
    draw(zoom, x, y, WIDTH / HEIGHT);
    gl.useProgram(shaderProgram);
}

update();

fieldZoom.oninput = ((_arg1) => {
    update();
});

fieldX.oninput = ((_arg2) => {
    update();
});

fieldY.oninput = ((_arg3) => {
    update();
});

fieldMandelbrot.oninput = ((_arg4) => {
    divMandelbox.hidden = true;
    divJulia.hidden = true;
    update();
});

fieldJulia.oninput = ((_arg5) => {
    divJulia.hidden = false;
    divMandelbox.hidden = true;
    update();
});

fieldMandelbox.oninput = ((_arg6) => {
    divJulia.hidden = true;
    divMandelbox.hidden = false;
    update();
});

fieldMandelboxScale.oninput = ((_arg7) => {
    update();
});

fieldJuliaX.oninput = ((_arg8) => {
    update();
});

fieldJuliaY.oninput = ((_arg9) => {
    update();
});

fieldJuliaPresets.oninput = ((_arg10) => {
    update();
});

export function handleKeypress(e) {
    const scale = 0.1;
    const matchValue = e.key;
    switch (matchValue) {
        case "w": {
            fieldY.value = (parse_1(fieldY.value) + (parse_1(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "s": {
            fieldY.value = (parse_1(fieldY.value) - (parse_1(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "a": {
            fieldX.value = (parse_1(fieldX.value) - (parse_1(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "d": {
            fieldX.value = (parse_1(fieldX.value) + (parse_1(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "q": {
            fieldZoom.value = (parse_1(fieldZoom.value) + (parse_1(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "e": {
            fieldZoom.value = (parse_1(fieldZoom.value) - (parse_1(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        default: {
        }
    }
}

document.addEventListener("keydown", (e) => {
    handleKeypress(e);
});

export function handleMouseDown(e) {
    const zoom = parse_1(fieldZoom.value);
}

export function handleMouseUp(e) {
}

canv.addEventListener("mousedown", (e) => {
    handleMouseDown(e);
});

canv.addEventListener("mouseup", (e) => {
    handleMouseUp(e);
});

buttonCentre.onclick = ((_arg1) => {
    fieldX.value = int32ToString(0);
    fieldY.value = int32ToString(0);
    update();
});

buttonReset.onclick = ((_arg2) => {
    fieldX.value = int32ToString(0);
    fieldY.value = int32ToString(0);
    fieldZoom.value = (2.5).toString();
    fieldMandelboxScale.value = int32ToString(3);
    fieldJuliaX.value = int32ToString(0);
    fieldJuliaY.value = int32ToString(0);
    fieldMandelbrot.checked = true;
    divMandelbox.hidden = true;
    divJulia.hidden = true;
    update();
});

