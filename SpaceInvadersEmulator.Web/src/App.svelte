<script lang="ts">
  import { router } from './lib/router.svelte';
  import { GAMES_BY_ID } from './lib/games';
  import Launcher from './components/Launcher.svelte';
  import GameView from './components/GameView.svelte';

  const game = $derived(router.current ? GAMES_BY_ID[router.current] : null);
</script>

<main class="h-full w-full" class:backdrop={!!game}>
  {#if game}
    <GameView {game} />
  {:else}
    <Launcher />
  {/if}
</main>

<style>
  /* Warm-toned radial halo: suggests dim ambient arcade lighting falling on
     the cabinet. Deliberately bright at center so the warm hue is clearly
     visible (not just a tint on black) — contrasts with the cabinet face's
     cool gray so the outline of the chassis reads against the surroundings. */
  .backdrop {
    background:
      radial-gradient(ellipse at center,
        rgb(82, 56, 36) 0%,
        rgb(46, 30, 20) 30%,
        rgb(18, 12, 8) 65%,
        rgb(4, 3, 2) 100%);
  }
</style>
