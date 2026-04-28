<script lang="ts">
  import { router } from './lib/router.svelte';
  import { GAMES_BY_ID } from './lib/games';
  import Launcher from './components/Launcher.svelte';
  import GameView from './components/GameView.svelte';

  const game = $derived(router.current ? GAMES_BY_ID[router.current] : null);
</script>

<main class="h-full w-full">
  {#if game}
    <GameView {game} />
  {:else}
    <Launcher />
  {/if}
</main>
