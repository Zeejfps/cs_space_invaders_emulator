// Web Audio API implementation for Space Invaders sounds.
// Registers globalThis.siAudio which C# BrowserAudio calls via [JSImport].

const soundFiles: Record<string, string> = {
  ufo_lowpass:   '/sounds/ufo_lowpass.wav',
  shoot:         '/sounds/shoot.wav',
  player_die:    '/sounds/player_die.wav',
  invader_die:   '/sounds/invader_die.wav',
  fastinvader1:  '/sounds/fastinvader1.wav',
  fastinvader2:  '/sounds/fastinvader2.wav',
  fastinvader3:  '/sounds/fastinvader3.wav',
  fastinvader4:  '/sounds/fastinvader4.wav',
  explosion:     '/sounds/explosion.wav',
};

let ctx: AudioContext | null = null;
const buffers = new Map<string, AudioBuffer>();
let ufoSource: AudioBufferSourceNode | null = null;
let _initPromise: Promise<void> | null = null;
const liveSources = new Set<AudioBufferSourceNode>();

async function getCtx(): Promise<AudioContext> {
  if (!ctx) ctx = new AudioContext();
  if (ctx.state === 'suspended') await ctx.resume();
  return ctx;
}

function playBuffer(name: string): void {
  const buf = buffers.get(name);
  if (!buf || !ctx) return;
  const source = ctx.createBufferSource();
  source.buffer = buf;
  source.connect(ctx.destination);
  source.onended = () => liveSources.delete(source);
  liveSources.add(source);
  source.start();
}

export function initAudio(): Promise<void> {
  if (_initPromise) return _initPromise;
  _initPromise = (async () => {
    ctx = new AudioContext();

    await Promise.allSettled(
      Object.entries(soundFiles).map(async ([name, path]) => {
        try {
          const resp = await fetch(path);
          if (!resp.ok) return;
          const buf = await resp.arrayBuffer();
          buffers.set(name, await ctx!.decodeAudioData(buf));
        } catch { /* missing sound — silenced */ }
      }),
    );

    (globalThis as Record<string, unknown>).siAudio = {
      setUfoLoopActive: (active: boolean): void => {
        getCtx().then(() => {
          const buf = buffers.get('ufo_lowpass');
          if (!buf || !ctx) return;
          if (active && !ufoSource) {
            ufoSource = ctx.createBufferSource();
            ufoSource.buffer = buf;
            ufoSource.loop = true;
            ufoSource.connect(ctx.destination);
            ufoSource.start();
          } else if (!active && ufoSource) {
            ufoSource.stop();
            ufoSource.disconnect();
            ufoSource = null;
          }
        });
      },
      playShot:            () => { getCtx().then(() => playBuffer('shoot')); },
      playPlayerDied:      () => { getCtx().then(() => playBuffer('player_die')); },
      playInvaderDied:     () => { getCtx().then(() => playBuffer('invader_die')); },
      playExtraLifeGained: () => { getCtx().then(() => playBuffer('invader_die')); },
      playFleetMoved:      (step: number) => { getCtx().then(() => playBuffer(`fastinvader${step}`)); },
      playUfoHit:          () => { getCtx().then(() => playBuffer('explosion')); },
    };
  })();
  return _initPromise;
}

// Silence everything currently playing. Called when navigating away from a game
// so the looping UFO source doesn't survive into the launcher.
export function stopAllAudio(): void {
  if (ufoSource) {
    try { ufoSource.stop(); } catch { /* already stopped */ }
    ufoSource.disconnect();
    ufoSource = null;
  }
  for (const src of liveSources) {
    try { src.stop(); } catch { /* already stopped */ }
    src.disconnect();
  }
  liveSources.clear();
}

// Resume the AudioContext within a user gesture (call from a click/touch handler).
export async function unlockAudio(): Promise<void> {
  await getCtx();
}
