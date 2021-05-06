import { findCookieValue } from "./Cookies.fs.js";
import { some, value as value_6 } from "./.fable/fable-library.3.1.1/Option.js";
import { createUniformLocation, createAttributeLocation, initBuffers, createShaderProgram, clear } from "./WebGLHelper.fs.js";
import { fsMandel, vsMandel } from "./Shaders.fs.js";
import { parse } from "./.fable/fable-library.3.1.1/Double.js";
import { printf, toText } from "./.fable/fable-library.3.1.1/String.js";

export const WIDTH = 1280;

export const HEIGHT = 720;

export const fieldZoom = document.querySelector("#zoom");

export const fieldX = document.querySelector("#x");

export const fieldY = document.querySelector("#y");

export const buttonReset = document.querySelector("#reset");

const patternInput$004018 = [findCookieValue("x"), findCookieValue("y"), findCookieValue("zoom")];

export const zoom = patternInput$004018[2];

export const y = patternInput$004018[1];

export const x = patternInput$004018[0];

if (((!(x == null)) ? (!(y == null)) : false) ? (!(zoom == null)) : false) {
    fieldX.value = value_6(x);
    fieldY.value = value_6(y);
    fieldZoom.value = value_6(zoom);
}

console.log(some(document.cookie));

export const canv = document.querySelector(".canv");

canv.width = WIDTH;

canv.height = HEIGHT;

export const gl = canv.getContext("webgl");

clear(gl);

export const shaderProgram = createShaderProgram(gl, vsMandel, fsMandel);

export function update() {
    const zoom_1 = parse(fieldZoom.value);
    const x_1 = parse(fieldX.value);
    const y_1 = parse(fieldY.value);
    document.cookie = toText(printf("zoom=%f;"))(zoom_1);
    document.cookie = toText(printf("x=%f;"))(x_1);
    document.cookie = toText(printf("y=%f;"))(y_1);
    document.cookie = toText(printf("expiry=2038-01-19 04:14:07;"));
    fieldX.step = (0.1 * zoom_1).toString();
    fieldY.step = (0.1 * zoom_1).toString();
    fieldZoom.step = (0.1 * zoom_1).toString();
    const patternInput = initBuffers(gl);
    const positionBuffer = patternInput[0];
    const colourBuffer = patternInput[1];
    const vertexPositionAttr = createAttributeLocation(gl, shaderProgram, "aVertexPosition");
    const textureCoordAttr = createAttributeLocation(gl, shaderProgram, "aTextureCoord");
    const zoomUniform = createUniformLocation(gl, shaderProgram, "uZoom");
    const xcUniform = createUniformLocation(gl, shaderProgram, "xc");
    const ycUniform = createUniformLocation(gl, shaderProgram, "yc");
    const ratioUniform = createUniformLocation(gl, shaderProgram, "uRatio");
    const draw = (zoom_2, x_2, y_2, ratio) => {
        gl.useProgram(shaderProgram);
        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
        gl.vertexAttribPointer(vertexPositionAttr, 2, gl.FLOAT, false, 0, 0);
        gl.bindBuffer(gl.ARRAY_BUFFER, colourBuffer);
        gl.vertexAttribPointer(textureCoordAttr, 2, gl.FLOAT, false, 0, 0);
        gl.uniform1f(zoomUniform, zoom_2);
        gl.uniform1f(xcUniform, x_2);
        gl.uniform1f(ycUniform, y_2);
        gl.uniform1f(ratioUniform, ratio);
        gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
    };
    draw(zoom_1, x_1, y_1, WIDTH / HEIGHT);
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

export function handleKeypress(e) {
    const scale = 0.1;
    const matchValue = e.key;
    switch (matchValue) {
        case "w": {
            fieldY.value = (parse(fieldY.value) + (parse(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "s": {
            fieldY.value = (parse(fieldY.value) - (parse(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "a": {
            fieldX.value = (parse(fieldX.value) - (parse(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "d": {
            fieldX.value = (parse(fieldX.value) + (parse(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "q": {
            fieldZoom.value = (parse(fieldZoom.value) + (parse(fieldZoom.value) * scale)).toString();
            update();
            break;
        }
        case "e": {
            fieldZoom.value = (parse(fieldZoom.value) - (parse(fieldZoom.value) * scale)).toString();
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
    const zoom_1 = parse(fieldZoom.value);
    console.log(some(e.offsetX), e.offsetY);
}

export function handleMouseUp(e) {
    console.log(some("mouse up"));
}

canv.addEventListener("mousedown", (e) => {
    handleMouseDown(e);
});

canv.addEventListener("mouseup", (e) => {
    handleMouseUp(e);
});

buttonReset.onclick = ((_arg1) => {
    fieldX.value = (-0.75).toString();
    fieldY.value = (0).toString();
    fieldZoom.value = (2.5).toString();
    update();
});

