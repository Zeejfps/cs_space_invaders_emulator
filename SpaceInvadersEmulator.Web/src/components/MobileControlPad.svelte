<script lang="ts">
  import ArcadeButton from '../lib/ui/ArcadeButton.svelte';
  import CoinSlot from '../lib/ui/CoinSlot.svelte';
  import Joystick from '../lib/ui/Joystick.svelte';
</script>

<!--
  Mobile control pad layout:
    - Movement cluster (left): two neutral-tone arrow buttons.
    - FIRE (right): bigger red button. Tapped 5-10× more than direction;
      larger thumb target is honest UX.
    - Movement and FIRE share a row, vertically centered so the smaller arrows
      line up with the middle of the bigger FIRE button.
    - Coin / P1 row spans below. P2 omitted — physical 2P on a single touchscreen
      isn't feasible.
-->
<div
  class="pad w-full grid grid-cols-[1fr_auto] items-center gap-4 px-4 pt-3 pb-[max(env(safe-area-inset-bottom),0.75rem)] bg-zinc-950 border-t-2 border-zinc-800"
>
  <!-- Movement: drag the knob left/right; releases recenter and clear input. -->
  <div class="flex justify-start items-center">
    <Joystick size={112} />
  </div>

  <!-- Fire -->
  <ArcadeButton inputKey="fire" label="FIRE" tone="red" size={88} ariaLabel="Fire" />

  <!-- Coin / P1 row -->
  <div class="col-span-2 flex gap-4 justify-center items-center">
    <CoinSlot size={44} />
    <ArcadeButton inputKey="p1Start" label="P1" tone="red" size={44} ariaLabel="Player 1 start" />
  </div>
</div>

<style>
  .pad { user-select: none; -webkit-user-select: none; }
</style>
