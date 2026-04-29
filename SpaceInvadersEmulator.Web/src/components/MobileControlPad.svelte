<script lang="ts">
  import CoinSlot from '../lib/ui/CoinSlot.svelte';
  import CoinReturn from '../lib/ui/CoinReturn.svelte';
  import { router } from '../lib/router.svelte';
</script>

<!--
  Mobile bottom panel: dark strip beneath the cabinet face. Carries only the
  transactional/utility controls (RETURN + COIN), right-aligned. The gameplay
  primaries (joystick, P1, FIRE) live above on the cabinet face's deck.
-->
<div
  class="pad w-full flex items-center justify-end gap-5 px-5 pt-8 pb-[max(env(safe-area-inset-bottom),2rem)] bg-zinc-950 border-t-2 border-zinc-800"
>
  <CoinReturn size={52} onclick={() => router.navigate(null)} />
  <CoinSlot size={52} />
</div>

<style>
  .pad { user-select: none; -webkit-user-select: none; }
</style>
