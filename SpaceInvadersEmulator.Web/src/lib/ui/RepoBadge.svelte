<script lang="ts">
  interface Props {
    /** Anchor href. */
    href?: string;
    /** Visible label after the GitHub mark. */
    label?: string;
  }
  let {
    href = 'https://github.com/Zeejfps/cs_space_invaders_emulator',
    label = 'Source on GitHub',
  }: Props = $props();
</script>

<a
  {href}
  target="_blank"
  rel="noopener noreferrer"
  class="badge inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md border border-zinc-700 bg-zinc-900 text-zinc-300 hover:text-[var(--color-crt-green)] hover:border-zinc-500 transition-colors font-mono text-xs"
>
  <svg
    aria-hidden="true"
    viewBox="0 0 24 24"
    fill="currentColor"
    class="w-4 h-4 shrink-0"
  >
    <!-- GitHub Octicon mark (MIT, github/feather) -->
    <path d="M12 0.297c-6.63 0-12 5.373-12 12 0 5.303 3.438 9.8 8.205 11.385 0.6 0.113 0.82-0.258 0.82-0.577 0-0.285-0.01-1.04-0.015-2.04-3.338 0.724-4.042-1.61-4.042-1.61-0.546-1.387-1.333-1.756-1.333-1.756-1.089-0.745 0.084-0.729 0.084-0.729 1.205 0.084 1.838 1.236 1.838 1.236 1.07 1.835 2.809 1.305 3.495 0.998 0.108-0.776 0.418-1.305 0.762-1.605-2.665-0.305-5.467-1.334-5.467-5.93 0-1.31 0.469-2.381 1.236-3.221-0.124-0.303-0.535-1.523 0.117-3.176 0 0 1.008-0.322 3.301 1.23 0.957-0.266 1.983-0.399 3.003-0.404 1.02 0.005 2.047 0.138 3.006 0.404 2.291-1.552 3.297-1.23 3.297-1.23 0.653 1.653 0.242 2.873 0.118 3.176 0.77 0.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921 0.43 0.371 0.823 1.102 0.823 2.222 0 1.606-0.014 2.898-0.014 3.293 0 0.322 0.216 0.694 0.825 0.576 4.766-1.589 8.199-6.085 8.199-11.386 0-6.627-5.373-12-12-12z"/>
  </svg>
  <span>{label}</span>
</a>

<style>
  .badge {
    /* Prevent the browser from wrapping the label/icon onto two lines on phones. */
    white-space: nowrap;
  }
</style>
