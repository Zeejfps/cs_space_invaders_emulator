<script lang="ts">
  import ArcadeButton from '../lib/ui/ArcadeButton.svelte';
  import CoinSlot from '../lib/ui/CoinSlot.svelte';
</script>

<!--
  Three-column layout:
    - Left: empty (visual balance with the right column).
    - Center: P1/P2 cluster, optically centered in the panel.
    - Right: coin slot, right-aligned.
  Each column is flex-1 (grid-cols-3 with equal tracks), so P1/P2 land
  on the panel's true center regardless of coin slot width.
-->
<div class="control-panel grid grid-cols-3 items-center px-4 py-4 bg-zinc-950 rounded-b-md w-full">
  <div></div>
  <div class="flex items-center justify-center gap-5">
    <ArcadeButton inputKey="p1Start" label="P1" tone="red"  size={52} ariaLabel="Player 1 start" />
    <ArcadeButton inputKey="p2Start" label="P2" tone="blue" size={52} ariaLabel="Player 2 start" />
  </div>
  <div class="flex items-center justify-end">
    <CoinSlot size={52} />
  </div>
</div>
