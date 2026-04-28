<script lang="ts">
  import ArcadeButton from '../lib/ui/ArcadeButton.svelte';
  import CoinSlot from '../lib/ui/CoinSlot.svelte';
</script>

<div class="control-panel flex items-center justify-center gap-5 px-4 py-4 bg-zinc-950 border-x-4 border-b-4 border-zinc-700 rounded-b-md w-full">
  <CoinSlot size={52} />
  <ArcadeButton inputKey="p1Start" label="P1" tone="red"  size={52} ariaLabel="Player 1 start" />
  <ArcadeButton inputKey="p2Start" label="P2" tone="blue" size={52} ariaLabel="Player 2 start" />
</div>
