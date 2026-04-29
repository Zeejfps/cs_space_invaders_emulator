// Web Audio API implementation for Space Invaders sounds.
// Registers globalThis.siAudio which C# BrowserAudio calls via [JSImport].

const soundFiles: Record<string, string> = {
  ufo_lowpass:   '/sounds/ufo_lowpass.wav',
  shoot:         '/sounds/shoot.wav',
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
      playPlayerDied:      () => { getCtx().then(() => playBuffer('explosion')); },
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

// Synthesized coin-insert sound — two short metallic pings with bandpass
// filtering and a downward pitch bend to mimic struck metal. Self-contained
// (no audio file), and the AudioContext is created lazily so the first call
// from a user gesture (the click that triggered this) can unlock playback.
export function playCoinInsert(): void {
  void (async () => {
    const c = await getCtx();
    const now = c.currentTime;
    ping(c, now,        1320, 0.14, 0.18);  // higher "ding"
    ping(c, now + 0.06,  880, 0.18, 0.14);  // lower "clink"
  })();
}

// Synthesized coin-return click — a single brief metallic ping. Lighter
// than the two-tone coin-insert sound, but metallic rather than plastic to
// match the CoinReturn's metal-plate visual styling.
export function playCoinReturn(): void {
  void (async () => {
    const c = await getCtx();
    const now = c.currentTime;
    ping(c, now, 1500, 0.09, 0.16);
  })();
}

// Synthesized arcade-button click — a short hollow "tock" with a bright
// noise tick on attack. Tuned for "plastic" rather than mechanical-thock:
// higher pitch, faster decay, brighter transient, less low-end body. Kept
// short (~40ms) and quiet so rapid presses don't drown out gameplay sounds.
export function playArcadeClick(): void {
  void (async () => {
    const c = await getCtx();
    const now = c.currentTime;

    // Body: triangle wave (less harsh than square, more harmonic content
    // than sine = hollow plastic timbre). Quick downward bend, fast decay.
    const osc = c.createOscillator();
    osc.type = 'triangle';
    osc.frequency.setValueAtTime(700, now);
    osc.frequency.exponentialRampToValueAtTime(280, now + 0.03);
    const oscGain = c.createGain();
    oscGain.gain.setValueAtTime(0, now);
    oscGain.gain.linearRampToValueAtTime(0.18, now + 0.002);
    oscGain.gain.exponentialRampToValueAtTime(0.001, now + 0.04);
    osc.connect(oscGain).connect(c.destination);
    osc.start(now);
    osc.stop(now + 0.06);

    // Transient: ~4ms of bright bandpass-filtered noise = the snappy
    // "tick" edge that gives plastic its characteristic crispness.
    const noiseBuf = c.createBuffer(1, Math.max(1, Math.floor(0.004 * c.sampleRate)), c.sampleRate);
    const data = noiseBuf.getChannelData(0);
    for (let i = 0; i < data.length; i++) data[i] = (Math.random() * 2 - 1) * 0.6;
    const noise = c.createBufferSource();
    noise.buffer = noiseBuf;
    const noiseFilter = c.createBiquadFilter();
    noiseFilter.type = 'bandpass';
    noiseFilter.frequency.value = 4500;
    noiseFilter.Q.value = 1.2;
    const noiseGain = c.createGain();
    noiseGain.gain.value = 0.16;
    noise.connect(noiseFilter).connect(noiseGain).connect(c.destination);
    noise.start(now);
  })();
}

function ping(c: AudioContext, start: number, freq: number, duration: number, peakGain: number): void {
  const osc = c.createOscillator();
  osc.type = 'triangle';
  osc.frequency.setValueAtTime(freq, start);
  // Slight downward bend = natural decay of a struck metal piece.
  osc.frequency.exponentialRampToValueAtTime(freq * 0.92, start + duration);

  const filter = c.createBiquadFilter();
  filter.type = 'bandpass';
  filter.frequency.setValueAtTime(freq, start);
  filter.Q.value = 8;

  const gain = c.createGain();
  gain.gain.setValueAtTime(0, start);
  gain.gain.linearRampToValueAtTime(peakGain, start + 0.005);
  gain.gain.exponentialRampToValueAtTime(0.001, start + duration);

  osc.connect(filter).connect(gain).connect(c.destination);
  osc.start(start);
  osc.stop(start + duration + 0.02);
}
