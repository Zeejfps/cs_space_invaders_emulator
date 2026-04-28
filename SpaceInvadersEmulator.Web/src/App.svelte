<script lang="ts">
  import { router } from './lib/router.svelte';
  import { GAMES_BY_ID } from './lib/games';
  import Launcher from './components/Launcher.svelte';
  import GameView from './components/GameView.svelte';

  const game = $derived(router.current ? GAMES_BY_ID[router.current] : null);
</script>

<main class="h-full w-full backdrop">
  {#if game}
    <GameView {game} />
  {:else}
    <Launcher />
  {/if}
</main>

<style>
  /* Subtle radial vignette: gives the cabinet visual presence on wide
     desktops where it floats centered with viewport space on either side.
     On phones the cabinet fills the viewport so the gradient is barely
     perceptible — that's fine. */
  .backdrop {
    background:
      radial-gradient(ellipse at center, #0e0e10 0%, #050507 60%, #000 100%);
  }
</style>
