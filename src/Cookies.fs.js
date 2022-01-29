import { map, equalsWith } from "./.fable/fable-library.3.1.1/Array.js";
import { comparePrimitives } from "./.fable/fable-library.3.1.1/Util.js";
import { ofArray, tryFind } from "./.fable/fable-library.3.1.1/Map.js";

export function findCookieValue(name) {
    const kvArrToPair = (kvArr) => {
        if ((!equalsWith(comparePrimitives, kvArr, null)) ? (kvArr.length === 2) : false) {
            const v = kvArr[1];
            const k = kvArr[0];
            return [k, v];
        }
        else {
            return ["", ""];
        }
    };
    const rawCookies = document.cookie;
    return tryFind(name, ofArray(map((s) => kvArrToPair(s.trim().split("=")), rawCookies.split(";"))));
}

