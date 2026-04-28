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
  source.start();
}

export async function initAudio(): Promise<void> {
  // Create AudioContext early; it will be resumed on first user gesture via getCtx().
  ctx = new AudioContext();

  const entries = Object.entries(soundFiles);
  await Promise.allSettled(
    entries.map(async ([name, path]) => {
      try {
        const resp = await fetch(path);
        if (!resp.ok) return; // missing file — audio for this sound is silenced
        const buf = await resp.arrayBuffer();
        buffers.set(name, await ctx!.decodeAudioData(buf));
      } catch {
        // silently skip sounds that fail to load
      }
    }),
  );

  // Register the interface that C# [JSImport] calls into.
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
}
