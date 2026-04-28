<script lang="ts">
  import { GAMES, GAMES_BY_ID } from '../lib/games';
  import { router } from '../lib/router.svelte';
  import { getLastPlayed } from '../lib/lastPlayed';
  import { unlockAudio } from '../lib/audio';
  import GameCard from './GameCard.svelte';
  import Button from '../lib/ui/Button.svelte';

  const lastPlayed = $derived(getLastPlayed());
  const lastGame = $derived(lastPlayed ? GAMES_BY_ID[lastPlayed] : null);

  function play(id: typeof GAMES[number]['id']): void {
    // The click is itself a user gesture; unlock audio now so the first
    // beep on game start isn't suppressed by Safari/iOS autoplay rules.
    unlockAudio();
    router.navigate(id);
  }
</script>

<div class="min-h-full w-full flex flex-col items-center px-6 py-10 sm:py-16">
  <header class="w-full max-w-5xl flex flex-col items-center gap-2 mb-10 sm:mb-14">
    <h1 class="font-mono text-3xl sm:text-5xl tracking-[0.3em] text-[var(--color-crt-green)] [text-shadow:0_0_18px_rgba(0,255,0,0.5)]">
      INVADERS
    </h1>
    <p class="font-mono text-xs sm:text-sm text-zinc-500 tracking-widest uppercase">
      A Taito 8080 emulator · Pick a game
    </p>
  </header>

  {#if lastGame}
    <div class="w-full max-w-5xl mb-8 flex items-center justify-between gap-3 px-4 py-3 rounded border border-zinc-800 bg-zinc-950/50">
      <div class="flex flex-col">
        <span class="font-mono text-xs text-zinc-500 uppercase">Continue</span>
        <span class="font-mono text-sm text-zinc-200">{lastGame.title}</span>
      </div>
      <Button variant="arcade" size="md" onclick={() => play(lastGame.id)}>Resume</Button>
    </div>
  {/if}

  <div class="w-full max-w-5xl grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-5">
    {#each GAMES as game (game.id)}
      <GameCard {game} onclick={() => play(game.id)} />
    {/each}
  </div>

  <footer class="mt-auto pt-12 font-mono text-[0.65rem] text-zinc-600 text-center">
    Keyboard: ← → SPACE · C = COIN · 1 / 2 = START
  </footer>
</div>
