<script lang="ts">
  import { onMount, onDestroy } from 'svelte';
  import type { GameDef } from '../lib/games';
  import { initEmulator, loadRom, setShips, setBonusLife } from '../lib/emulator';
  import { initAudio, stopAllAudio } from '../lib/audio';
  import { setLastPlayed } from '../lib/lastPlayed';
  import ArcadeCabinet from './ArcadeCabinet.svelte';

  let { game }: { game: GameDef } = $props();

  type Status = 'loading' | 'ready' | 'error';
  let status = $state<Status>('loading');
  let errorMsg = $state('');

  onMount(async () => {
    document.body.classList.add('lock-scroll');
    setLastPlayed(game.id);
    try {
      // Run init + ROM fetch in parallel — WASM boot is the long pole.
      const [, romBuf] = await Promise.all([
        Promise.all([initEmulator(), initAudio()]),
        fetch(`/roms/${game.romFile}`).then((r) => {
          if (!r.ok) throw new Error(`ROM not found: ${game.romFile} (${r.status})`);
          return r.arrayBuffer();
        }),
      ]);
      loadRom(new Uint8Array(romBuf));
      setShips(game.defaultDipswitches.ships);
      setBonusLife(game.defaultDipswitches.bonusLifeAt1000);
      status = 'ready';
    } catch (e) {
      errorMsg = (e as Error).message;
      status = 'error';
    }
  });

  onDestroy(() => {
    stopAllAudio();
    document.body.classList.remove('lock-scroll');
  });
</script>

{#if status === 'loading'}
  <div class="h-full w-full flex flex-col items-center justify-center font-mono text-[var(--color-crt-green)] gap-3">
    <div class="text-2xl tracking-[0.3em] animate-pulse">LOADING</div>
    <div class="text-xs text-zinc-500">{game.title}</div>
  </div>
{:else if status === 'error'}
  <div class="h-full w-full flex flex-col items-center justify-center font-mono gap-4 px-6 text-center">
    <div class="text-xl tracking-widest text-red-500">FAILED TO START</div>
    <div class="text-sm text-zinc-400 max-w-md">{errorMsg}</div>
    <a class="text-xs text-zinc-500 underline" href="/">Back to launcher</a>
  </div>
{:else}
  <ArcadeCabinet {game} />
{/if}
