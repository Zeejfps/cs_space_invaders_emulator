<script lang="ts">
  import RomLoader from './components/RomLoader.svelte';
  import ArcadeCabinet from './components/ArcadeCabinet.svelte';

  let gameReady = $state(false);
</script>

<main>
  {#if !gameReady}
    <RomLoader onloaded={() => (gameReady = true)} />
  {:else}
    <ArcadeCabinet />
  {/if}
</main>

<style>
  main {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100vw;
    height: 100vh;
    background: #000;
  }
</style>
