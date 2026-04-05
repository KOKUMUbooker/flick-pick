import { hubIsDisconnected, appState } from "../store";

export const movieNightHub = {
    async start() {
        if (hubIsDisconnected()) {
            await appState.hubConnection.start();
        }
    },

    async join(movieNightId: string) {
        await this.start();
        await appState.hubConnection.invoke("JoinMovieNight", movieNightId)
    },

    async leave(movieNightId: string) {
        await appState.hubConnection.invoke("LeaveMovieNight", movieNightId)
    },
}