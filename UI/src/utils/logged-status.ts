import { ISLOGGED_KEY } from "../constants";

export type LoggedStatus = "0" | "1"
export function setLoggedStat(logged: LoggedStatus) {
    if (logged === "0" || logged == "1") {
        localStorage.setItem(ISLOGGED_KEY, logged)
    }
}

export function getLoggedStat(): LoggedStatus {
    const readKey = localStorage.getItem(ISLOGGED_KEY)
    const logged: LoggedStatus = readKey === "0" || readKey === "1" ? readKey : "0"

    return logged
}