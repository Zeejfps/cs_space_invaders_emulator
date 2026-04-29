<script lang="ts">
  import { GAMES } from '../lib/games';
  import { router } from '../lib/router.svelte';
  import { unlockAudio } from '../lib/audio';
  import GameCard from './GameCard.svelte';
  import RepoBadge from '../lib/ui/RepoBadge.svelte';
  import { APP_VERSION } from '../lib/version';

  function play(id: typeof GAMES[number]['id']): void {
    // The click is itself a user gesture; unlock audio now so the first
    // beep on game start isn't suppressed by Safari/iOS autoplay rules.
    unlockAudio();
    router.navigate(id);
  }
</script>

<div class="min-h-full w-full flex flex-col items-center px-6 py-10 sm:py-16">
  <header class="w-full max-w-5xl flex flex-col items-center gap-2 mb-4">
    <h1 class="font-mono text-3xl sm:text-5xl tracking-[0.3em] text-[var(--color-crt-green)] [text-shadow:0_0_18px_rgba(0,255,0,0.5)]">
      INVADERS
    </h1>
    <p class="font-mono text-xs sm:text-sm text-zinc-500 tracking-widest uppercase">
      A Taito 8080 emulator
    </p>
    <p class="font-mono text-[0.7rem] text-zinc-500">
      Created by
      <a
              href="https://evasilyev.com"
              target="_blank"
              rel="noopener noreferrer"
              class="text-zinc-300 hover:text-[var(--color-crt-green)] transition-colors"
      >Zee Vasilyev</a>
    </p>
  </header>

  <div class="w-full max-w-5xl grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-5">
    {#each GAMES as game (game.id)}
      <GameCard {game} onclick={() => play(game.id)} />
    {/each}
  </div>

  <footer class="mt-auto pt-6 flex flex-col items-center gap-2">
    <RepoBadge />
    <span class="font-mono text-[0.6rem] text-zinc-700 tracking-widest">v{APP_VERSION}</span>
  </footer>
</div>
