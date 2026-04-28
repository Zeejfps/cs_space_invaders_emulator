<script lang="ts">
  import GameCanvas from './GameCanvas.svelte';
  import ControlPanel from './ControlPanel.svelte';
</script>

<div class="cabinet">
  <div class="marquee">
    <span>★&nbsp; SPACE INVADERS &nbsp;★</span>
  </div>
  <div class="bezel">
    <div class="screen">
      <GameCanvas />
    </div>
  </div>
  <ControlPanel />
</div>

<style>
  .cabinet {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0;
    font-family: monospace;
  }

  .marquee {
    background: #111;
    border: 3px solid #555;
    border-bottom: none;
    border-radius: 6px 6px 0 0;
    padding: 0.5rem 3rem;
    font-size: 1.1rem;
    letter-spacing: 0.15em;
    color: #ff0;
    text-shadow: 0 0 8px #ff0;
  }

  .bezel {
    background: #1a1a1a;
    border: 6px solid #444;
    border-top: none;
    border-bottom: none;
    padding: 1.5rem 2.5rem;
  }

  .screen {
    background: #000;
    border: 3px solid #222;
    box-shadow: inset 0 0 20px #000, 0 0 15px rgba(0, 200, 0, 0.15);
    line-height: 0;
  }
</style>
