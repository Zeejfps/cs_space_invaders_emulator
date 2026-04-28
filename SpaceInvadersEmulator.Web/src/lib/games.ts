export interface RomBank {
  /** Filename inside the upstream MAME zip. Matched case-insensitively. */
  name: string;
  /** Load address in the 8080 address space. */
  offset: number;
}

export type GameId = 'invaders' | 'invadpt2' | 'ballbomb' | 'lrescue';

export interface GameDef {
  id: GameId;
  title: string;
  year: number;
  publisher: string;
  /** Output filename under public/roms/. */
  romFile: string;
  /** Source zip basename on the upstream repo (without .zip). */
  upstreamZip: string;
  /** Bank-to-offset map. Build plugin merges these into a single .bin. */
  romMap: RomBank[];
  defaultDipswitches: {
    ships: 3 | 4 | 5 | 6;
    bonusLifeAt1000: boolean;
  };
  thumbnail: string;
  /** True until we've booted the game and confirmed the layout works. */
  unverified?: boolean;
}

export const GAMES: GameDef[] = [
  {
    id: 'invaders',
    title: 'Space Invaders',
    year: 1978,
    publisher: 'Taito / Midway',
    romFile: 'invaders.bin',
    upstreamZip: 'invaders',
    romMap: [
      { name: 'invaders.h', offset: 0x0000 },
      { name: 'invaders.g', offset: 0x0800 },
      { name: 'invaders.f', offset: 0x1000 },
      { name: 'invaders.e', offset: 0x1800 },
    ],
    defaultDipswitches: { ships: 3, bonusLifeAt1000: true },
    thumbnail: '/thumbs/invaders.png',
  },
  {
    id: 'invadpt2',
    title: 'Space Invaders Part II',
    year: 1980,
    publisher: 'Taito',
    romFile: 'invadpt2.bin',
    upstreamZip: 'invadpt2',
    romMap: [
      { name: 'pv01', offset: 0x0000 },
      { name: 'pv02', offset: 0x0800 },
      { name: 'pv03', offset: 0x1000 },
      { name: 'pv04', offset: 0x1800 },
      { name: 'pv05', offset: 0x4000 },
    ],
    defaultDipswitches: { ships: 3, bonusLifeAt1000: true },
    thumbnail: '/thumbs/invadpt2.png',
  },
  {
    id: 'ballbomb',
    title: 'Balloon Bomber',
    year: 1980,
    publisher: 'Taito',
    romFile: 'ballbomb.bin',
    upstreamZip: 'ballbomb',
    romMap: [
      { name: 'tn01',   offset: 0x0000 },
      { name: 'tn02',   offset: 0x0800 },
      { name: 'tn03',   offset: 0x1000 },
      { name: 'tn04',   offset: 0x1800 },
      { name: 'tn05-1', offset: 0x4000 },
      { name: 'tn06',   offset: 0x4800 },
      { name: 'tn07',   offset: 0x4c00 },
    ],
    defaultDipswitches: { ships: 3, bonusLifeAt1000: true },
    thumbnail: '/thumbs/ballbomb.png',
  },
  {
    id: 'lrescue',
    title: 'Lunar Rescue',
    year: 1979,
    publisher: 'Taito / Universal',
    romFile: 'lrescue.bin',
    upstreamZip: 'lrescue',
    romMap: [
      { name: 'lrescue.1', offset: 0x0000 },
      { name: 'lrescue.2', offset: 0x0800 },
      { name: 'lrescue.3', offset: 0x1000 },
      { name: 'lrescue.4', offset: 0x1800 },
      { name: 'lrescue.5', offset: 0x4000 },
      { name: 'lrescue.6', offset: 0x4800 },
    ],
    defaultDipswitches: { ships: 3, bonusLifeAt1000: true },
    thumbnail: '/thumbs/lrescue.png',
  },
];

export const GAMES_BY_ID: Record<GameId, GameDef> = Object.fromEntries(
  GAMES.map((g) => [g.id, g]),
) as Record<GameId, GameDef>;

export function isGameId(s: string | null | undefined): s is GameId {
  return !!s && s in GAMES_BY_ID;
}
