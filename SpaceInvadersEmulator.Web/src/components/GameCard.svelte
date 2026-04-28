<script lang="ts">
  import type { GameDef } from '../lib/games';
  import Card from '../lib/ui/Card.svelte';

  let { game, onclick }: { game: GameDef; onclick: () => void } = $props();
</script>

<Card {onclick} aria-label={`Play ${game.title}`}>
  <div class="aspect-[7/8] overflow-hidden bg-black border-b border-zinc-800">
    <img
      src={game.thumbnail}
      alt=""
      loading="lazy"
      class="w-full h-full object-cover [image-rendering:pixelated] transition-transform group-hover:scale-105"
      onerror={(e) => { (e.currentTarget as HTMLImageElement).style.visibility = 'hidden'; }}
    />
  </div>
  <div class="px-3 py-3 flex flex-col gap-0.5">
    <div class="font-mono text-sm tracking-wider text-zinc-100 group-hover:text-[var(--color-crt-green)] transition-colors uppercase">{game.title}</div>
    <div class="font-mono text-xs text-zinc-500">{game.year} · {game.publisher}</div>
  </div>
</Card>
