<script lang="ts">
  import type { Snippet } from 'svelte';
  import type { HTMLButtonAttributes } from 'svelte/elements';
  import { cn } from './cn';

  interface Props extends HTMLButtonAttributes {
    class?: string;
    children: Snippet;
  }

  let { class: className, children, ...rest }: Props = $props();
</script>

<button
  class={cn(
    'group relative flex flex-col items-stretch overflow-hidden rounded-lg border border-zinc-800 bg-zinc-950 text-left transition-all hover:border-zinc-600 hover:-translate-y-0.5 hover:shadow-[0_0_24px_rgba(0,255,0,0.08)] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[var(--color-crt-green)]',
    className,
  )}
  {...rest}
>
  {@render children()}
</button>
