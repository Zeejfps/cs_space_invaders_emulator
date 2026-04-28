import { mount } from 'svelte';
import App from './App.svelte';
import { initEmulator } from './lib/emulator';
import { initAudio } from './lib/audio';

async function bootstrap(): Promise<void> {
  // Initialize emulator runtime and audio in parallel.
  // Audio buffers load while the WASM runtime boots.
  await Promise.all([initEmulator(), initAudio()]);

  mount(App, { target: document.getElementById('app')! });
}

bootstrap().catch(console.error);
