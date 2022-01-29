import { parse } from "./.fable/fable-library.3.1.1/Double.js";
import { Union } from "./.fable/fable-library.3.1.1/Types.js";
import { union_type, float64_type } from "./.fable/fable-library.3.1.1/Reflection.js";

export function toDouble(num) {
    return parse(num.toPrecision(32));
}

export function toFloat(num) {
    return parse(num.toPrecision(16));
}

export function splitDouble(num) {
    const upper = toFloat(num);
    const lower = toFloat(num - toDouble(upper));
    return [upper, lower];
}

export class SplitDouble extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["SplitDouble"];
    }
}

export function SplitDouble$reflection() {
    return union_type("DoublePrecision.SplitDouble", [], SplitDouble, () => [[["Item1", float64_type], ["Item2", float64_type]]]);
}

export function SplitDouble_ofFloat_5E38073B(num) {
    const tupledArg = splitDouble(num);
    return new SplitDouble(0, tupledArg[0], tupledArg[1]);
}

export function SplitDouble_Upper_189C8C6F(_arg1) {
    const hi = _arg1.fields[0];
    return hi;
}

export function SplitDouble_Lower_189C8C6F(_arg2) {
    const lo = _arg2.fields[1];
    return lo;
}

export function SplitDouble_toUniform_189C8C6F(_arg3) {
    const lo = _arg3.fields[1];
    const hi = _arg3.fields[0];
    return new Float32Array((new Float64Array([hi, lo])));
}

export function SplitDouble__get_upper(this$) {
    return SplitDouble_Upper_189C8C6F(this$);
}

export function SplitDouble__get_lower(this$) {
    return SplitDouble_Lower_189C8C6F(this$);
}

export function SplitDouble__get_toFloat(this$) {
    return SplitDouble__get_upper(this$);
}

export function SplitDouble_op_Addition_34B29620(_arg4, _arg5) {
    const u1 = _arg4.fields[0];
    const l1 = _arg4.fields[1];
    const u2 = _arg5.fields[0];
    const l2 = _arg5.fields[1];
    const t1 = u1 + u2;
    const e = t1 - u1;
    const t2 = (((u2 - e) + (u1 - (t1 - e))) + l1) + l2;
    const u3 = t1 + t2;
    const l3 = t2 - (u3 - t1);
    return new SplitDouble(0, u3, l3);
}

export function SplitDouble_op_Multiply_34B29620(_arg6, _arg7) {
    const u1 = _arg6.fields[0];
    const l1 = _arg6.fields[1];
    const u2 = _arg7.fields[0];
    const l2 = _arg7.fields[1];
    const split = 8193;
    const cona = u1 * split;
    const conb = u2 * split;
    const a1 = cona - (cona - u1);
    const b1 = conb - (conb - u2);
    const a2 = u1 - a1;
    const b2 = u2 - b1;
    const c11 = u1 * u2;
    const c21 = (a2 * b2) + ((a2 * b1) + ((a1 * b2) + ((a1 * b1) - c11)));
    const c2 = (u1 * l2) + (l1 * u2);
    const t1 = c11 + c2;
    const e = t1 - c11;
    const t2 = ((l1 * l2) + ((c2 - e) + (c11 - (t1 - e)))) + c21;
    const u3 = t1 + t2;
    const l3 = t2 - (u3 - t1);
    return new SplitDouble(0, u3, l3);
}

