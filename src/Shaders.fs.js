
export function glsl(str) {
    return str;
}

export const vsMandel = glsl("\r\n    precision highp float;\r\n    precision highp int;\r\n\r\n    attribute vec4 aVertexPosition;\r\n    attribute vec2 aTextureCoord;\r\n    varying vec2 vTextureCoord;\r\n    void main() {\r\n        gl_Position = aVertexPosition;\r\n        vTextureCoord = aTextureCoord;\r\n    }\r\n    ");

export const fsMandel = glsl("\r\n    precision highp float;\r\n    precision highp int;\r\n    \r\n    varying vec2 vTextureCoord;\r\n    uniform float uZoom;\r\n    uniform float xc;\r\n    uniform float yc;\r\n    uniform float uPalletteOffset;\r\n    uniform float uRatio;\r\n    uniform float uGenerator;\r\n    uniform float uMandelbrotPower;\r\n    uniform float uMandelboxScale;\r\n    uniform float uJuliaX;\r\n    uniform float uJuliaY;\r\n\r\n    uniform int uUseDoub;\r\n    uniform vec2 uZoomDoub;\r\n    uniform vec2 xcDoub;\r\n    uniform vec2 ycDoub;\r\n\r\n    uniform vec3 uPallette[76];\r\n\r\n    const float MAX = 1000.;\r\n\r\n    vec3 getPallette(int index) {\r\n        if (index == 0) { return uPallette[0] / 255.0; }\r\n        else if (index == 1) { return uPallette[1] / 255.0; }\r\n        else if (index == 2) { return uPallette[2] / 255.0; }\r\n        else if (index == 3) { return uPallette[3] / 255.0; }\r\n        else if (index == 4) { return uPallette[4] / 255.0; }\r\n        else if (index == 5) { return uPallette[5] / 255.0; }\r\n        else if (index == 6) { return uPallette[6] / 255.0; }\r\n        else if (index == 7) { return uPallette[7] / 255.0; }\r\n        else if (index == 8) { return uPallette[8] / 255.0; }\r\n        else if (index == 9) { return uPallette[9] / 255.0; }\r\n        else if (index == 10) { return uPallette[10] / 255.0; }\r\n        else if (index == 11) { return uPallette[11] / 255.0; }\r\n        else if (index == 12) { return uPallette[12] / 255.0; }\r\n        else if (index == 13) { return uPallette[13] / 255.0; }\r\n        else if (index == 14) { return uPallette[14] / 255.0; }\r\n        else if (index == 15) { return uPallette[15] / 255.0; }\r\n        else if (index == 16) { return uPallette[16] / 255.0; }\r\n        else if (index == 17) { return uPallette[17] / 255.0; }\r\n        else if (index == 18) { return uPallette[18] / 255.0; }\r\n        else if (index == 19) { return uPallette[19] / 255.0; }\r\n        else if (index == 20) { return uPallette[20] / 255.0; }\r\n        else if (index == 21) { return uPallette[21] / 255.0; }\r\n        else if (index == 22) { return uPallette[22] / 255.0; }\r\n        else if (index == 23) { return uPallette[23] / 255.0; }\r\n        else if (index == 24) { return uPallette[24] / 255.0; }\r\n        else if (index == 25) { return uPallette[25] / 255.0; }\r\n        else if (index == 26) { return uPallette[26] / 255.0; }\r\n        else if (index == 27) { return uPallette[27] / 255.0; }\r\n        else if (index == 28) { return uPallette[28] / 255.0; }\r\n        else if (index == 29) { return uPallette[29] / 255.0; }\r\n        else if (index == 30) { return uPallette[30] / 255.0; }\r\n        else if (index == 31) { return uPallette[31] / 255.0; }\r\n        else if (index == 32) { return uPallette[32] / 255.0; }\r\n        else if (index == 33) { return uPallette[33] / 255.0; }\r\n        else if (index == 34) { return uPallette[34] / 255.0; }\r\n        else if (index == 35) { return uPallette[35] / 255.0; }\r\n        else if (index == 36) { return uPallette[36] / 255.0; }\r\n        else if (index == 37) { return uPallette[37] / 255.0; }\r\n        else if (index == 38) { return uPallette[38] / 255.0; }\r\n        else if (index == 39) { return uPallette[39] / 255.0; }\r\n        else if (index == 40) { return uPallette[40] / 255.0; }\r\n        else if (index == 41) { return uPallette[41] / 255.0; }\r\n        else if (index == 42) { return uPallette[42] / 255.0; }\r\n        else if (index == 43) { return uPallette[43] / 255.0; }\r\n        else if (index == 44) { return uPallette[44] / 255.0; }\r\n        else if (index == 45) { return uPallette[45] / 255.0; }\r\n        else if (index == 46) { return uPallette[46] / 255.0; }\r\n        else if (index == 47) { return uPallette[47] / 255.0; }\r\n        else if (index == 48) { return uPallette[48] / 255.0; }\r\n        else if (index == 49) { return uPallette[49] / 255.0; }\r\n        else if (index == 50) { return uPallette[50] / 255.0; }\r\n        else if (index == 51) { return uPallette[51] / 255.0; }\r\n        else if (index == 52) { return uPallette[52] / 255.0; }\r\n        else if (index == 53) { return uPallette[53] / 255.0; }\r\n        else if (index == 54) { return uPallette[54] / 255.0; }\r\n        else if (index == 55) { return uPallette[55] / 255.0; }\r\n        else if (index == 56) { return uPallette[56] / 255.0; }\r\n        else if (index == 57) { return uPallette[57] / 255.0; }\r\n        else if (index == 58) { return uPallette[58] / 255.0; }\r\n        else if (index == 59) { return uPallette[59] / 255.0; }\r\n        else if (index == 60) { return uPallette[60] / 255.0; }\r\n        else if (index == 61) { return uPallette[61] / 255.0; }\r\n        else if (index == 62) { return uPallette[62] / 255.0; }\r\n        else if (index == 63) { return uPallette[63] / 255.0; }\r\n        else if (index == 64) { return uPallette[64] / 255.0; }\r\n        else if (index == 65) { return uPallette[65] / 255.0; }\r\n        else if (index == 66) { return uPallette[66] / 255.0; }\r\n        else if (index == 67) { return uPallette[67] / 255.0; }\r\n        else if (index == 68) { return uPallette[68] / 255.0; }\r\n        else if (index == 69) { return uPallette[69] / 255.0; }\r\n        else if (index == 70) { return uPallette[70] / 255.0; }\r\n        else if (index == 71) { return uPallette[71] / 255.0; }\r\n        else if (index == 72) { return uPallette[72] / 255.0; }\r\n        else if (index == 73) { return uPallette[73] / 255.0; }\r\n        else if (index == 74) { return uPallette[74] / 255.0; }\r\n        else if (index == 75) { return uPallette[75] / 255.0; }\r\n    }\r\n\r\n    vec3 getPalletteSmall(int index) {\r\n        if (index == 0) { return vec3(0.01253, 0.08452, 0.51573); }\r\n        else if (index == 1) { return vec3(0.06352, 0.26197, 0.69032); }\r\n        else if (index == 2) { return vec3(0.13598, 0.43791, 0.80556); }\r\n        else if (index == 3) { return vec3(0.30453, 0.60977, 0.87876); }\r\n        else if (index == 4) { return vec3(0.55496, 0.79061, 0.94006); }\r\n        else if (index == 5) { return vec3(0.79353, 0.93555, 0.98271); }\r\n        else if (index == 6) { return vec3(0.92651, 0.99970, 0.99992); }\r\n        else if (index == 7) { return vec3(0.95905, 0.96668, 0.83293); }\r\n        else if (index == 8) { return vec3(0.98314, 0.87426, 0.44273); }\r\n        else if (index == 9) { return vec3(0.99720, 0.75086, 0.08820); }\r\n        else if (index == 10) { return vec3(0.97248, 0.61366, 0.00000); }\r\n        else if (index == 11) { return vec3(0.66576, 0.38345, 0.00000); }\r\n        else if (index == 12) { return vec3(0.24680, 0.13969, 0.00000); }\r\n        else if (index == 13) { return vec3(0.00368, 0.00974, 0.00000); }\r\n        else if (index == 14) { return vec3(0.00000, 0.01095, 0.09399); }\r\n        else if (index == 15) { return vec3(0.00000, 0.02126, 0.31171); }\r\n    }\r\n\r\n    vec3 linearInterpolate(vec3 v1, vec3 v2, float f) {\r\n        return v1 * (1.0 - f) + v2 * f;\r\n    }\r\n\r\n    vec3 cosineInterpolate(vec3 v1, vec3 v2, float f) {\r\n        float f2 = (1.0 - cos(f * 3.15159)) / 2.0;\r\n        return v1 * (1.0 - f2) + v2 * f2;\r\n    }\r\n\r\n    vec2 doubOfFloat(float upper) {\r\n        return vec2(upper, 0.0);\r\n    }\r\n\r\n    vec2 doubAddDoub(vec2 doubA, vec2 doubB) {\r\n        vec2 doubC;\r\n        float t1, t2, e;\r\n\r\n        t1 = doubA.x + doubB.x;\r\n        e = t1 - doubA.x;\r\n        t2 = ((doubB.x - e) + (doubA.x - (t1 - e))) + doubA.y + doubB.y;\r\n\r\n        doubC.x = t1 + t2;\r\n        doubC.y = t2 - (doubC.x - t1);\r\n        return doubC;\r\n    }\r\n\r\n    vec2 doubAddFloat(vec2 doub, float f) {\r\n        return doubAddDoub(doub, doubOfFloat(f));\r\n    }\r\n\r\n    vec2 doubMulDoub(vec2 doubA, vec2 doubB) {\r\n        vec2 doubC;\r\n        float c11, c21, c2, e, t1, t2;\r\n        float a1, a2, b1, b2, cona, conb, split = 8193.;\r\n\r\n        cona = doubA.x * split;\r\n        conb = doubB.x * split;\r\n        a1 = cona - (cona - doubA.x);\r\n        b1 = conb - (conb - doubB.x);\r\n        a2 = doubA.x - a1;\r\n        b2 = doubB.x - b1;\r\n\r\n        c11 = doubA.x * doubB.x;\r\n        c21 = a2 * b2 + (a2 * b1 + (a1 * b2 + (a1 * b1 - c11)));\r\n\r\n        c2 = doubA.x * doubB.y + doubA.y * doubB.x;\r\n\r\n        t1 = c11 + c2;\r\n        e = t1 - c11;\r\n        t2 = doubA.y * doubB.y + ((c2 - e) + (c11 - (t1 - e))) + c21;\r\n\r\n        doubC.x = t1 + t2;\r\n        doubC.y = t2 - (doubC.x - t1);\r\n        return doubC;\r\n    }\r\n\r\n    vec2 doubMulFloat(vec2 doub, float f) {\r\n        return doubMulDoub(doub, doubOfFloat(f));\r\n    }\r\n\r\n    vec2 doubDot(vec2 x, vec2 y) {\r\n        return doubAddDoub(doubMulDoub(x, x), doubMulDoub(y, y));\r\n    }\r\n\r\n    vec2 compPower(vec2 z, float n) {\r\n        return vec2(\r\n            pow(z.x * z.x + z.y * z.y, n / 2.0) * cos(n * atan(z.y, z.x)),\r\n            pow(z.x * z.x + z.y * z.y, n / 2.0) * sin(n * atan(z.y, z.x))\r\n        );\r\n    }\r\n\r\n    float mandelbrot(float x, float y) {\r\n        vec2 c = vec2(x, y);\r\n        vec2 z = c;\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (length(z) \u003e sqrt(8.0)) {\r\n                float l = log(abs(uMandelbrotPower));\r\n                return float(i) - log(log(dot(z, z)) / l) / l + 4.0;\r\n            }\r\n            if (uMandelbrotPower == 2.0) {\r\n                z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);\r\n            } else {\r\n                z = compPower(z, uMandelbrotPower) + c;\r\n            }\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float mandelbrotDoub(vec2 x, vec2 y) {\r\n        vec2 zx = x;\r\n        vec2 zy = y;\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x \u003e 4096.0) {\r\n                return float(i) - log2(log2(doubDot(zx, zy).x)) + 4.0;\r\n            }\r\n            vec2 zxTemp = doubAddDoub(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(doubOfFloat(-1.0), doubMulDoub(zy, zy))), x);\r\n            zy = doubAddDoub(doubMulFloat(doubMulDoub(zx, zy), 2.0), y);\r\n            zx = zxTemp;\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float julia(float x, float y) {\r\n        vec2 c = vec2(uJuliaX, uJuliaY);\r\n        vec2 z = vec2(x, y);\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (length(z) \u003e 4096.) {\r\n                return float(i) - log2(log(dot(z, z)) / log(10.0)) + 2.0;\r\n            }\r\n            z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float juliaDoub(vec2 x, vec2 y) {\r\n        vec2 zx = x;\r\n        vec2 zy = y;\r\n        vec2 cx = doubOfFloat(uJuliaX);\r\n        vec2 cy = doubOfFloat(uJuliaY);\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x \u003e 4096.0) {\r\n                return float(i) + 1.0 - log2(log(sqrt(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x)));\r\n            }\r\n            vec2 zxTemp = doubAddDoub(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(doubOfFloat(-1.0), doubMulDoub(zy, zy))), cx);\r\n            zy = doubAddDoub(doubMulFloat(doubMulDoub(zx, zy), 2.0), cy);\r\n            zx = zxTemp;\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float burningShip(float x, float y) {\r\n        vec2 c = vec2(x, -y);\r\n        vec2 z = c;\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (length(z) \u003e 4096.) {\r\n                return float(i) - log2(log2(dot(z, z))) + 4.0;\r\n            }\r\n            z = vec2(z.x*z.x - z.y*z.y + c.x, abs(2.0*z.x*z.y) + c.y);\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float mandelbox(float x, float y) {\r\n        vec2 c = vec2(x, y);\r\n        vec2 z = c;\r\n        \r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (length(z) \u003e 4096.) {\r\n                return float(i) - log2(log2(dot(z, z))) + 4.0;\r\n            }\r\n\r\n            if (z.x \u003e 1.) {\r\n                z.x = 2. - z.x;\r\n            } else if (z.x \u003c -1.) {\r\n                z.x = -2. - z.x;\r\n            }\r\n            if (z.y \u003e 1.) {\r\n                z.y = 2. - z.y;\r\n            } else if (z.y \u003c -1.) {\r\n                z.y = -2. - z.y;\r\n            }\r\n\r\n            float mag = length(z);\r\n            if (mag \u003c .5) {\r\n                z *= 4.;\r\n            } else if (mag \u003c 1.) {\r\n                z /= mag * mag;\r\n            }\r\n\r\n            z = uMandelboxScale * z + c;\r\n        }\r\n        return 0.;\r\n    }\r\n\r\n    void main() {\r\n        bool useDoub = uUseDoub == 1 ? true : false;\r\n        \r\n        float m = 0.0;\r\n        if (useDoub) {\r\n            vec2 x = doubAddDoub(xcDoub, doubMulFloat(uZoomDoub, uRatio * (vTextureCoord.x - 0.5)));\r\n            vec2 y = doubAddDoub(ycDoub, doubMulFloat(uZoomDoub, vTextureCoord.y - 0.5));\r\n            if (uGenerator == 1.0) {\r\n                m = mandelbrotDoub(x, y);\r\n            } else if (uGenerator == 2.0) {\r\n                m = juliaDoub(x, y);\r\n            } else if (uGenerator == 3.0) {\r\n                m = burningShip(x.x, y.x);\r\n            } else if (uGenerator == 4.0) {\r\n                m = mandelbox(x.x, y.x);\r\n            } else {\r\n                gl_FragColor = vec4(vTextureCoord.x, vTextureCoord.y, 0.0, 1.);\r\n                return;\r\n            }\r\n        } else {\r\n            float x = xc + (vTextureCoord.x - 0.5) * uZoom * uRatio;\r\n            float y = yc + (vTextureCoord.y - 0.5) * uZoom;\r\n            if (uGenerator == 1.0) {\r\n                m = mandelbrot(x, y);\r\n            } else if (uGenerator == 2.0) {\r\n                m = julia(x, y);\r\n            } else if (uGenerator == 3.0) {\r\n                m = burningShip(x, y);\r\n            } else if (uGenerator == 4.0) {\r\n                m = mandelbox(x, y);\r\n            } else {\r\n                gl_FragColor = vec4(vTextureCoord.x, vTextureCoord.y, 0.0, 1.);\r\n                return;\r\n            }\r\n        }\r\n\r\n        // Colouring\r\n        vec3 col = vec3(0., 0., 0.);\r\n        if (m != MAX) {\r\n            if (uGenerator == 4.0) {  // Mandelbox\r\n                m = mod(m + uPalletteOffset, 16.0);\r\n                col = getPalletteSmall(int(m));\r\n            } else {\r\n                m = mod(m + uPalletteOffset, 75.0);\r\n                vec3 col1 = getPallette(int(m));\r\n                vec3 col2 = getPallette(int(m) + 1);\r\n                float f = mod(m, 1.0);\r\n                col = linearInterpolate(col1, col2, f);\r\n            }\r\n        }\r\n        gl_FragColor = vec4(col.xyz, 1.0);\r\n    }\r\n    ");

