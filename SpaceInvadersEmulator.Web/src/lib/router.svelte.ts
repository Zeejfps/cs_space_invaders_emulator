import { isGameId, type GameId } from './games';

function parseFromLocation(): GameId | null {
  const id = new URLSearchParams(window.location.search).get('game');
  return isGameId(id) ? id : null;
}

class Router {
  current = $state<GameId | null>(parseFromLocation());

  constructor() {
    window.addEventListener('popstate', () => {
      this.current = parseFromLocation();
    });
  }

  navigate(id: GameId | null): void {
    history.pushState({}, '', id ? `?game=${id}` : '/');
    this.current = id;
  }
}

export const router = new Router();
