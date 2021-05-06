module Shaders

open Fetch
open Browser.Dom

let vsMandel = """
    precision highp float;
    precision highp int;

    attribute vec4 aVertexPosition;
    attribute vec2 aTextureCoord;
    varying vec2 vTextureCoord;
    void main() {
        gl_Position = aVertexPosition;
        vTextureCoord = aTextureCoord;
    }"""


let fsMandel = """
    precision highp float;
    precision highp int;
    
    varying vec2 vTextureCoord;
    uniform float uZoom;
    uniform float xc;
    uniform float yc;
    uniform float uRatio;

    const float MAX = 500.;

    vec3 getPalatte(int index) {
        if (index == 0) { return vec3(66, 30, 15) / 255.; }
        if (index == 1) { return vec3(25, 7, 26) / 255.; }
        if (index == 2) { return vec3(9, 1, 47) / 255.; }
        if (index == 3) { return vec3(4, 4, 73) / 255.; }
        if (index == 4) { return vec3(0, 7, 100) / 255.; }
        if (index == 5) { return vec3(12, 44, 138) / 255.; }
        if (index == 6) { return vec3(24, 82, 177) / 255.; }
        if (index == 7) { return vec3(57, 125, 209) / 255.; }
        if (index == 8) { return vec3(134, 181, 229) / 255.; }
        if (index == 9) { return vec3(211, 236, 248) / 255.; }
        if (index == 10) { return vec3(241, 233, 191) / 255.; }
        if (index == 11) { return vec3(248, 201, 95) / 255.; }
        if (index == 12) { return vec3(255, 170, 0) / 255.; }
        if (index == 13) { return vec3(204, 128, 0) / 255.; }
        if (index == 14) { return vec3(153, 87, 0) / 255.; }
        if (index == 15) { return vec3(106, 52, 3) / 255.; }
    }

    float mandelbrot(float x, float y) {
        vec2 c = vec2(x, y);
        vec2 z = c;

        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > 2.) {
                return float(i);
                // float smoothed = log2(log2(dot(z, z)) / 1.);
                // return sqrt(float(i) + 10. - smoothed) * 2.;
            }
            z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);
        }
        return MAX;
    }

    const float SCALE = 2.5;
    float mandelbox(float x, float y) {
        vec2 c = vec2(x, y);
        vec2 z = c;
        
        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > 400.) {
                return float(i);
            }

            if (z.x > 1.) {
                z.x = 2. - z.x;
            } else if (z.x < -1.) {
                z.x = -2. - z.x;
            }
            if (z.y > 1.) {
                z.y = 2. - z.y;
            } else if (z.y < -1.) {
                z.y = -2. - z.y;
            }

            float mag = length(z);
            if (mag < .5) {
                z *= 4.;
            } else if (mag < 1.) {
                z /= mag * mag;
            }

            z = SCALE * z + c;
        }

        return 0.;
    }

    void main() {
        float x = xc + (vTextureCoord.x - 0.5) * uZoom * uRatio;
        float y = yc + (vTextureCoord.y - 0.5) * uZoom;
        float m = mandelbrot(x, y);

        if (m == MAX) {
            gl_FragColor = vec4(0., 0., 0., 1.);
        } else {
            vec3 col = getPalatte(int(mod(m, 16.)));
            gl_FragColor = vec4(col.x, col.y, col.z, 1.);
        }
    }"""