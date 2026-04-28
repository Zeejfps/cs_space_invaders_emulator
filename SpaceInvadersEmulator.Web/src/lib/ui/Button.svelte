<script lang="ts">
  import type { Snippet } from 'svelte';
  import type { HTMLButtonAttributes } from 'svelte/elements';
  import { cn } from './cn';

  type Variant = 'default' | 'ghost' | 'outline' | 'arcade';
  type Size = 'sm' | 'md' | 'lg';

  interface Props extends HTMLButtonAttributes {
    variant?: Variant;
    size?: Size;
    class?: string;
    children: Snippet;
  }

  let { variant = 'default', size = 'md', class: className, children, ...rest }: Props = $props();

  const variants: Record<Variant, string> = {
    default: 'bg-zinc-900 text-zinc-100 border border-zinc-700 hover:bg-zinc-800 hover:border-zinc-600',
    ghost:   'bg-transparent text-zinc-300 hover:bg-zinc-900 hover:text-zinc-100',
    outline: 'bg-transparent text-zinc-200 border border-zinc-700 hover:bg-zinc-900',
    arcade:  'bg-zinc-950 text-[var(--color-crt-green)] border border-[var(--color-crt-green)] hover:bg-[color-mix(in_srgb,var(--color-crt-green)_15%,transparent)]',
  };
  const sizes: Record<Size, string> = {
    sm: 'h-8 px-3 text-xs',
    md: 'h-10 px-4 text-sm',
    lg: 'h-12 px-6 text-base',
  };
</script>

<button
  class={cn(
    'inline-flex items-center justify-center rounded font-mono tracking-wider uppercase transition-colors select-none focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-zinc-500 disabled:opacity-50 disabled:pointer-events-none',
    variants[variant],
    sizes[size],
    className,
  )}
  {...rest}
>
  {@render children()}
</button>
