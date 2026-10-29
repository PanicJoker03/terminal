import { firstValueFrom } from 'rxjs';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { TerminalService } from './terminal.service';

describe('TerminalService', () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it('creates a terminal session from the backend and records the initial system text', async () => {
    const fetchMock = vi.fn()
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ id: 'session-123' }),
      })
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ events: [{ sequence: 1, text: 'session connected' }] }),
      });

    vi.stubGlobal('fetch', fetchMock);

    const service = new TerminalService();
    await service.startSession('MAYAN CONSULTING');

    expect(fetchMock).toHaveBeenCalledWith(
      'http://localhost:5000/api/terminal-sessions',
      expect.objectContaining({
        method: 'POST',
        headers: expect.objectContaining({
          'Content-Type': 'application/json',
        }),
      }),
    );

    const lines = await firstValueFrom(service.lines$);
    expect(lines.some((line) => line.type === 'system' && line.text.includes('session connected: session-123'))).toBe(true);
  });

  it('sends user input to the active terminal session', async () => {
    const fetchMock = vi.fn()
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ id: 'session-456' }),
      })
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ events: [] }),
      })
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ ok: true }),
      });

    vi.stubGlobal('fetch', fetchMock);

    const service = new TerminalService();
    await service.startSession('MAYAN CONSULTING');
    await service.sendCommand('ANGULAR USER');

    expect(fetchMock).toHaveBeenCalledWith(
      'http://localhost:5000/api/terminal-sessions/session-456/input',
      expect.objectContaining({
        method: 'POST',
        body: JSON.stringify({ text: 'ANGULAR USER' }),
      }),
    );
  });

  it('ignores backend event metadata values like type=clear when they are not actual terminal output', async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      json: async () => ({ events: [{ sequence: 1, type: 'clear' }] }),
    });

    vi.stubGlobal('fetch', fetchMock);

    const service = new TerminalService();
    (service as any).sessionId = 'session-clear';
    await (service as any).pollEvents();

    const lines = await firstValueFrom(service.lines$);
    expect(lines.some((line) => line.type === 'output' && line.text === 'clear')).toBe(false);
  });

  it('enables blank Enter input when the backend prompts for a key press', async () => {
    const fetchMock = vi.fn()
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ id: 'session-prompt' }),
      })
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ events: [{ sequence: 1, text: 'Press any key to exit module...' }] }),
      })
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ ok: true }),
      });

    vi.stubGlobal('fetch', fetchMock);

    const service = new TerminalService();
    await service.startSession('MAYAN CONSULTING');

    expect((service as any).allowEmptyInput).toBe(true);

    await service.sendCommand('');

    expect(fetchMock).toHaveBeenCalledWith(
      'http://localhost:5000/api/terminal-sessions/session-prompt/input',
      expect.objectContaining({
        method: 'POST',
        body: JSON.stringify({ text: '' }),
      }),
    );
  });
});
