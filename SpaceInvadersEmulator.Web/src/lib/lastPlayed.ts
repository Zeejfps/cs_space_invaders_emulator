import { isGameId, type GameId } from './games';

const KEY = 'siemu:last-played';

export function getLastPlayed(): GameId | null {
  try {
    const v = localStorage.getItem(KEY);
    return isGameId(v) ? v : null;
  } catch { return null; }
}

export function setLastPlayed(id: GameId): void {
  try { localStorage.setItem(KEY, id); } catch { /* private mode etc. */ }
}
