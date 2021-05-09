
export function glsl(str) {
    return str;
}

export const vsMandel = glsl("\r\n    precision highp float;\r\n    precision highp int;\r\n\r\n    attribute vec4 aVertexPosition;\r\n    attribute vec2 aTextureCoord;\r\n    varying vec2 vTextureCoord;\r\n    void main() {\r\n        gl_Position = aVertexPosition;\r\n        vTextureCoord = aTextureCoord;\r\n    }\r\n    ");

export const fsMandel = glsl("\r\n    precision highp float;\r\n    precision highp int;\r\n    \r\n    varying vec2 vTextureCoord;\r\n    uniform float uZoom;\r\n    uniform float xc;\r\n    uniform float yc;\r\n    uniform float uPalatteOffset;\r\n    uniform float uRatio;\r\n    uniform float uGenerator;\r\n    uniform float uMandelboxScale;\r\n    uniform float uJuliaX;\r\n    uniform float uJuliaY;\r\n\r\n    uniform int uUseDoub;\r\n    uniform vec2 uZoomDoub;\r\n    uniform vec2 xcDoub;\r\n    uniform vec2 ycDoub;\r\n\r\n    const float MAX = 1000.;\r\n\r\n    vec3 getPalatte(int index) {\r\n        if (index == 0) { return vec3(0.00005, 0.02816, 0.39805); }\r\n        else if (index == 1) { return vec3(0.00166, 0.03771, 0.43408); }\r\n        else if (index == 2) { return vec3(0.00555, 0.05555, 0.47167); }\r\n        else if (index == 3) { return vec3(0.01152, 0.08046, 0.51019); }\r\n        else if (index == 4) { return vec3(0.01937, 0.1112, 0.54901); }\r\n        else if (index == 5) { return vec3(0.02888, 0.14654, 0.58753); }\r\n        else if (index == 6) { return vec3(0.03987, 0.18527, 0.62511); }\r\n        else if (index == 7) { return vec3(0.05214, 0.22615, 0.66114); }\r\n        else if (index == 8) { return vec3(0.06548, 0.26795, 0.69499); }\r\n        else if (index == 9) { return vec3(0.0797, 0.30945, 0.72605); }\r\n        else if (index == 10) { return vec3(0.09459, 0.34942, 0.75368); }\r\n        else if (index == 11) { return vec3(0.10996, 0.38663, 0.77727); }\r\n        else if (index == 12) { return vec3(0.12561, 0.41984, 0.7962); }\r\n        else if (index == 13) { return vec3(0.14545, 0.45198, 0.81253); }\r\n        else if (index == 14) { return vec3(0.17281, 0.48636, 0.82859); }\r\n        else if (index == 15) { return vec3(0.20679, 0.52255, 0.84433); }\r\n        else if (index == 16) { return vec3(0.2465, 0.56012, 0.85969); }\r\n        else if (index == 17) { return vec3(0.29104, 0.59865, 0.87459); }\r\n        else if (index == 18) { return vec3(0.33953, 0.6377, 0.88898); }\r\n        else if (index == 19) { return vec3(0.39105, 0.67686, 0.90278); }\r\n        else if (index == 20) { return vec3(0.44473, 0.71568, 0.91594); }\r\n        else if (index == 21) { return vec3(0.49967, 0.75374, 0.92839); }\r\n        else if (index == 22) { return vec3(0.55496, 0.79061, 0.94006); }\r\n        else if (index == 23) { return vec3(0.60972, 0.82587, 0.9509); }\r\n        else if (index == 24) { return vec3(0.66305, 0.85909, 0.96083); }\r\n        else if (index == 25) { return vec3(0.71406, 0.88983, 0.96979); }\r\n        else if (index == 26) { return vec3(0.76185, 0.91767, 0.97771); }\r\n        else if (index == 27) { return vec3(0.80553, 0.94219, 0.98454); }\r\n        else if (index == 28) { return vec3(0.84421, 0.96294, 0.99021); }\r\n        else if (index == 29) { return vec3(0.87698, 0.97951, 0.99465); }\r\n        else if (index == 30) { return vec3(0.90296, 0.99147, 0.9978); }\r\n        else if (index == 31) { return vec3(0.92125, 0.99838, 0.99959); }\r\n        else if (index == 32) { return vec3(0.93123, 0.99988, 0.99937); }\r\n        else if (index == 33) { return vec3(0.93843, 0.9971, 0.98452); }\r\n        else if (index == 34) { return vec3(0.94534, 0.99079, 0.95178); }\r\n        else if (index == 35) { return vec3(0.95193, 0.98123, 0.90364); }\r\n        else if (index == 36) { return vec3(0.95818, 0.9687, 0.84257); }\r\n        else if (index == 37) { return vec3(0.96408, 0.95346, 0.77105); }\r\n        else if (index == 38) { return vec3(0.9696, 0.93578, 0.69157); }\r\n        else if (index == 39) { return vec3(0.97473, 0.91594, 0.6066); }\r\n        else if (index == 40) { return vec3(0.97944, 0.8942, 0.51864); }\r\n        else if (index == 41) { return vec3(0.98372, 0.87083, 0.43015); }\r\n        else if (index == 42) { return vec3(0.98755, 0.84611, 0.34362); }\r\n        else if (index == 43) { return vec3(0.9909, 0.82031, 0.26153); }\r\n        else if (index == 44) { return vec3(0.99376, 0.79369, 0.18637); }\r\n        else if (index == 45) { return vec3(0.9961, 0.76653, 0.1206); }\r\n        else if (index == 46) { return vec3(0.99792, 0.73909, 0.06672); }\r\n        else if (index == 47) { return vec3(0.99918, 0.71165, 0.0272); }\r\n        else if (index == 48) { return vec3(0.99987, 0.68448, 0.00453); }\r\n        else if (index == 49) { return vec3(0.99876, 0.65735, 0.0); }\r\n        else if (index == 50) { return vec3(0.9811, 0.62437, 0.0); }\r\n        else if (index == 51) { return vec3(0.94463, 0.5847, 0.0); }\r\n        else if (index == 52) { return vec3(0.89211, 0.53955, 0.0); }\r\n        else if (index == 53) { return vec3(0.8263, 0.49015, 0.0); }\r\n        else if (index == 54) { return vec3(0.74993, 0.43771, 0.0); }\r\n        else if (index == 55) { return vec3(0.66576, 0.38345, 0.0); }\r\n        else if (index == 56) { return vec3(0.57655, 0.32857, 0.0); }\r\n        else if (index == 57) { return vec3(0.48503, 0.2743, 0.0); }\r\n        else if (index == 58) { return vec3(0.39396, 0.22186, 0.0); }\r\n        else if (index == 59) { return vec3(0.30608, 0.17245, 0.0); }\r\n        else if (index == 60) { return vec3(0.22416, 0.12729, 0.0); }\r\n        else if (index == 61) { return vec3(0.15093, 0.0876, 0.0); }\r\n        else if (index == 62) { return vec3(0.08915, 0.0546, 0.0); }\r\n        else if (index == 63) { return vec3(0.04157, 0.02949, 0.0); }\r\n        else if (index == 64) { return vec3(0.01094, 0.0135, 0.0); }\r\n        else if (index == 65) { return vec3(0.0, 0.00784, 0.0); }\r\n        else if (index == 66) { return vec3(0.0, 0.00803, 0.00641); }\r\n        else if (index == 67) { return vec3(0.0, 0.00858, 0.02456); }\r\n        else if (index == 68) { return vec3(0.0, 0.00949, 0.05251); }\r\n        else if (index == 69) { return vec3(0.0, 0.01074, 0.08835); }\r\n        else if (index == 70) { return vec3(0.0, 0.01232, 0.13013); }\r\n        else if (index == 71) { return vec3(0.0, 0.01422, 0.17593); }\r\n        else if (index == 72) { return vec3(0.0, 0.01642, 0.22382); }\r\n        else if (index == 73) { return vec3(0.0, 0.01891, 0.27186); }\r\n        else if (index == 74) { return vec3(0.0, 0.02168, 0.31814); }\r\n        else if (index == 75) { return vec3(0.0, 0.02471, 0.36071); }\r\n    }\r\n\r\n    vec3 getPalatteOld(int index) {\r\n        if (index == 0) { return vec3(66, 30, 15) / 255.; }\r\n        else if (index == 1) { return vec3(25, 7, 26) / 255.; }\r\n        else if (index == 2) { return vec3(9, 1, 47) / 255.; }\r\n        else if (index == 3) { return vec3(4, 4, 73) / 255.; }\r\n        else if (index == 4) { return vec3(0, 7, 100) / 255.; }\r\n        else if (index == 5) { return vec3(12, 44, 138) / 255.; }\r\n        else if (index == 6) { return vec3(24, 82, 177) / 255.; }\r\n        else if (index == 7) { return vec3(57, 125, 209) / 255.; }\r\n        else if (index == 8) { return vec3(134, 181, 229) / 255.; }\r\n        else if (index == 9) { return vec3(211, 236, 248) / 255.; }\r\n        else if (index == 10) { return vec3(241, 233, 191) / 255.; }\r\n        else if (index == 11) { return vec3(248, 201, 95) / 255.; }\r\n        else if (index == 12) { return vec3(255, 170, 0) / 255.; }\r\n        else if (index == 13) { return vec3(204, 128, 0) / 255.; }\r\n        else if (index == 14) { return vec3(153, 87, 0) / 255.; }\r\n        else if (index == 15) { return vec3(106, 52, 3) / 255.; }\r\n    }\r\n\r\n    vec3 getPalatteBW(int index) {\r\n        if (index == 0) {return vec3(0.4, 0.4, 0.4); }\r\n        else if (index == 1) { return vec3(0.7, 0.7, 0.7); }\r\n        else if (index == 1) { return vec3(0.8, 0.8, 0.8); }\r\n        else if (index == 1) { return vec3(0.9, 0.9, 0.9); }\r\n        else if (index == 1) { return vec3(1.0, 1.0, 1.0); }\r\n    }\r\n\r\n    vec3 linearInterpolate(vec3 v1, vec3 v2, float f) {\r\n        return v1 * (1.0 - f) + v2 * f;\r\n    }\r\n\r\n    vec2 doubOfFloat(float upper) {\r\n        return vec2(upper, 0.0);\r\n    }\r\n\r\n    vec2 doubAddDoub(vec2 doubA, vec2 doubB) {\r\n        vec2 doubC;\r\n        float t1, t2, e;\r\n\r\n        t1 = doubA.x + doubB.x;\r\n        e = t1 - doubA.x;\r\n        t2 = ((doubB.x - e) + (doubA.x - (t1 - e))) + doubA.y + doubB.y;\r\n\r\n        doubC.x = t1 + t2;\r\n        doubC.y = t2 - (doubC.x - t1);\r\n        return doubC;\r\n    }\r\n\r\n    vec2 doubAddFloat(vec2 doub, float f) {\r\n        return doubAddDoub(doub, doubOfFloat(f));\r\n    }\r\n\r\n    vec2 doubMulDoub(vec2 doubA, vec2 doubB) {\r\n        vec2 doubC;\r\n        float c11, c21, c2, e, t1, t2;\r\n        float a1, a2, b1, b2, cona, conb, split = 8193.;\r\n\r\n        cona = doubA.x * split;\r\n        conb = doubB.x * split;\r\n        a1 = cona - (cona - doubA.x);\r\n        b1 = conb - (conb - doubB.x);\r\n        a2 = doubA.x - a1;\r\n        b2 = doubB.x - b1;\r\n\r\n        c11 = doubA.x * doubB.x;\r\n        c21 = a2 * b2 + (a2 * b1 + (a1 * b2 + (a1 * b1 - c11)));\r\n\r\n        c2 = doubA.x * doubB.y + doubA.y * doubB.x;\r\n\r\n        t1 = c11 + c2;\r\n        e = t1 - c11;\r\n        t2 = doubA.y * doubB.y + ((c2 - e) + (c11 - (t1 - e))) + c21;\r\n\r\n        doubC.x = t1 + t2;\r\n        doubC.y = t2 - (doubC.x - t1);\r\n        return doubC;\r\n    }\r\n\r\n    vec2 doubMulFloat(vec2 doub, float f) {\r\n        return doubMulDoub(doub, doubOfFloat(f));\r\n    }\r\n\r\n    vec2 doubDot(vec2 x, vec2 y) {\r\n        return doubAddDoub(doubMulDoub(x, x), doubMulDoub(y, y));\r\n    }\r\n\r\n    float mandelbrot(float x, float y) {\r\n        if (pow(x + 1.0, 2.0) + y*y \u003c= 0.0625) {\r\n            return MAX;\r\n        }\r\n\r\n        vec2 c = vec2(x, y);\r\n        vec2 z = c;\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (length(z) \u003e 4096.) {\r\n                return float(i) - log2(log2(dot(z, z))) + 4.0;\r\n            }\r\n            z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float mandelbrotDoub(vec2 x, vec2 y) {\r\n        vec2 zx = x;\r\n        vec2 zy = y;\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x \u003e 4096.0) {\r\n                return float(i) - log2(log2(doubDot(zx, zy).x)) + 4.0;\r\n            }\r\n            vec2 zxTemp = doubAddDoub(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(doubOfFloat(-1.0), doubMulDoub(zy, zy))), x);\r\n            zy = doubAddDoub(doubMulFloat(doubMulDoub(zx, zy), 2.0), y);\r\n            zx = zxTemp;\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float julia(float x, float y) {\r\n        vec2 c = vec2(uJuliaX, uJuliaY);\r\n        vec2 z = vec2(x, y);\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (length(z) \u003e 4096.) {\r\n                return float(i) - log2(log(dot(z, z)) / log(10.0)) + 2.0;\r\n            }\r\n            z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float juliaDoub(vec2 x, vec2 y) {\r\n        vec2 zx = x;\r\n        vec2 zy = y;\r\n        vec2 cx = doubOfFloat(uJuliaX);\r\n        vec2 cy = doubOfFloat(uJuliaY);\r\n\r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x \u003e 4096.0) {\r\n                return float(i) + 1.0 - log2(log(sqrt(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x)));\r\n            }\r\n            vec2 zxTemp = doubAddDoub(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(doubOfFloat(-1.0), doubMulDoub(zy, zy))), cx);\r\n            zy = doubAddDoub(doubMulFloat(doubMulDoub(zx, zy), 2.0), cy);\r\n            zx = zxTemp;\r\n        }\r\n        return MAX;\r\n    }\r\n\r\n    float mandelbox(float x, float y) {\r\n        vec2 c = vec2(x, y);\r\n        vec2 z = c;\r\n        \r\n        for (int i = 0; i \u003c= int(MAX); i++) {\r\n            if (length(z) \u003e 4096.) {\r\n                return float(i) - log2(log2(dot(z, z))) + 4.0;\r\n            }\r\n\r\n            if (z.x \u003e 1.) {\r\n                z.x = 2. - z.x;\r\n            } else if (z.x \u003c -1.) {\r\n                z.x = -2. - z.x;\r\n            }\r\n            if (z.y \u003e 1.) {\r\n                z.y = 2. - z.y;\r\n            } else if (z.y \u003c -1.) {\r\n                z.y = -2. - z.y;\r\n            }\r\n\r\n            float mag = length(z);\r\n            if (mag \u003c .5) {\r\n                z *= 4.;\r\n            } else if (mag \u003c 1.) {\r\n                z /= mag * mag;\r\n            }\r\n\r\n            z = uMandelboxScale * z + c;\r\n        }\r\n        return 0.;\r\n    }\r\n\r\n    void main() {\r\n        bool useDoub = uUseDoub == 1 ? true : false;\r\n        \r\n        float m = 0.0;\r\n        if (useDoub) {\r\n            vec2 x = doubAddDoub(xcDoub, doubMulFloat(uZoomDoub, uRatio * (vTextureCoord.x - 0.5)));\r\n            vec2 y = doubAddDoub(ycDoub, doubMulFloat(uZoomDoub, vTextureCoord.y - 0.5));\r\n            if (uGenerator == 1.0) {\r\n                m = mandelbrotDoub(x, y);\r\n            } else if (uGenerator == 2.0) {\r\n                m = juliaDoub(x, y);\r\n            }  else if (uGenerator == 3.0) {\r\n                m = mandelbox(x.x, y.x);\r\n            } else {\r\n                gl_FragColor = vec4(vTextureCoord.x, vTextureCoord.y, 0.0, 1.);\r\n                return;\r\n            }\r\n        } else {\r\n            float x = xc + (vTextureCoord.x - 0.5) * uZoom * uRatio;\r\n            float y = yc + (vTextureCoord.y - 0.5) * uZoom;\r\n            if (uGenerator == 1.0) {\r\n                m = mandelbrot(x, y);\r\n            } else if (uGenerator == 2.0) {\r\n                m = julia(x, y);\r\n            }  else if (uGenerator == 3.0) {\r\n                m = mandelbox(x, y);\r\n            } else {\r\n                gl_FragColor = vec4(vTextureCoord.x, vTextureCoord.y, 0.0, 1.);\r\n                return;\r\n            }\r\n        }\r\n\r\n        // Colouring\r\n        vec3 col = vec3(0., 0., 0.);\r\n        if (m != MAX) {\r\n            if (uGenerator == 3.0) {  // Mandelbox\r\n                m = mod(m + uPalatteOffset, 16.0);\r\n                vec3 col1 = getPalatteOld(int(m));\r\n                vec3 col2 = getPalatteOld(int(m) + 1);\r\n                col = linearInterpolate(col1, col2, mod(m, 1.0));\r\n            } else {\r\n                m = mod(m + uPalatteOffset, 75.0);\r\n                vec3 col1 = getPalatte(int(m));\r\n                vec3 col2 = getPalatte(int(m) + 1);\r\n                float f = mod(m, 1.0);\r\n                col = col1 * (1.0 - f) + col2 * f;\r\n            }\r\n        }\r\n        gl_FragColor = vec4(col.xyz, 1.0);\r\n    }\r\n    ");

