import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export type TerminalLineType = 'system' | 'command' | 'output' | 'error';

export interface TerminalLine {
  type: TerminalLineType;
  text: string;
}

interface TerminalSessionPayload {
  id?: string;
  sessionId?: string;
  session_id?: string;
  name?: string;
  status?: string;
}

interface TerminalEventPayload {
  sequence?: number | string;
  id?: string;
  text?: string;
  message?: string;
  content?: string;
  data?: string;
  type?: string;
}

@Injectable({
  providedIn: 'root',
})
export class TerminalService {
  private readonly linesSubject = new BehaviorSubject<TerminalLine[]>([
    { type: 'system', text: 'terminal ready' },
    { type: 'system', text: 'waiting for terminal session...' },
  ]);

  readonly lines$: Observable<TerminalLine[]> = this.linesSubject.asObservable();

  private readonly baseUrl = 'http://localhost:5000';
  private sessionId: string | null = null;
  private lastEventSequence = 0;
  private pollingHandle: ReturnType<typeof globalThis.setTimeout> | null = null;

  async startSession(name = 'job assistant app'): Promise<void> {
    if (this.sessionId) {
      this.addLine('system', `session already active: ${this.sessionId}`);
      return;
    }

    this.addLine('system', `creating terminal session: ${name}`);

    try {
      const response = await fetch(`${this.baseUrl}/api/terminal-sessions`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ name }),
      });

      if (!response.ok) {
        throw new Error(`session creation failed (${response.status})`);
      }

      const payload = (await response.json()) as TerminalSessionPayload;
      const sessionId = this.extractSessionId(payload);

      if (!sessionId) {
        throw new Error('backend returned no session id');
      }

      this.sessionId = sessionId;
      this.addLine('system', `session connected: ${sessionId}`);
      await this.pollEvents();
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      this.addLine('error', `session unavailable: ${message}`);
    }
  }

  async sendCommand(command: string): Promise<void> {
    if (command === undefined || command === null) {
      return;
    }

    const raw = command;

    this.addLine('command', `$ ${raw || ' '}`);

    if (!this.sessionId) {
      await this.startSession('job assistant app');
    }

    if (!this.sessionId) {
      return;
    }

    try {
      await this.sendInput(raw === '' ? '<enter>' : raw);
      void this.pollEvents();
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      this.addLine('error', `error: ${message}`);
    }
  }

  async deleteSession(): Promise<void> {
    if (!this.sessionId) {
      return;
    }

    try {
      const response = await fetch(`${this.baseUrl}/api/terminal-sessions/${this.sessionId}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        throw new Error(`session deletion failed (${response.status})`);
      }

      this.addLine('system', `session closed: ${this.sessionId}`);
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      this.addLine('error', `session teardown failed: ${message}`);
    } finally {
      this.sessionId = null;
      this.lastEventSequence = 0;
      this.clearPolling();
    }
  }

  private addLine(type: TerminalLineType, text: string): void {
    this.linesSubject.next([
      ...this.linesSubject.value,
      { type, text },
    ]);
  }

  private clearTerminal(): void {
    this.linesSubject.next([{ type: 'system', text: 'terminal cleared' }]);
    this.lastEventSequence = 0;
  }

  private clearPolling(): void {
    if (this.pollingHandle) {
      globalThis.clearTimeout(this.pollingHandle);
      this.pollingHandle = null;
    }
  }

  private async sendInput(text: string): Promise<void> {
    if (!this.sessionId) {
      return;
    }

    const response = await fetch(`${this.baseUrl}/api/terminal-sessions/${this.sessionId}/input`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ text }),
    });

    if (!response.ok) {
      throw new Error(`input rejected (${response.status})`);
    }
  }

  private async pollEvents(): Promise<void> {
    if (!this.sessionId) {
      return;
    }

    this.clearPolling();

    try {
      const params = new URLSearchParams({ after: String(this.lastEventSequence) });
      const response = await fetch(
        `${this.baseUrl}/api/terminal-sessions/${this.sessionId}/events?${params.toString()}`,
      );

      if (!response.ok) {
        throw new Error(`event poll failed (${response.status})`);
      }

      const payload = (await response.json()) as { events?: TerminalEventPayload[] } | TerminalEventPayload[];
      const events = Array.isArray(payload) ? payload : payload.events ?? [];

      for (const event of events) {
        const text = this.extractEventText(event);
        if (text) {
          this.addLine('output', text);
        }

        const sequence = Number(event.sequence);
        if (!Number.isNaN(sequence)) {
          this.lastEventSequence = Math.max(this.lastEventSequence, sequence);
        }
      }
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      this.addLine('error', `stream unavailable: ${message}`);
    }

    if (this.sessionId) {
      this.pollingHandle = globalThis.setTimeout(() => {
        void this.pollEvents();
      }, 1500);
    }
  }

  private extractSessionId(payload: TerminalSessionPayload): string | null {
    return payload.id ?? payload.sessionId ?? payload.session_id ?? null;
  }

  private extractEventText(event: TerminalEventPayload): string | null {
    const source = [event.text, event.message, event.content, event.data]
      .map((value) => (typeof value === 'string' ? value.trim() : value))
      .find((value) => typeof value === 'string' && value.length > 0 && value !== 'undefined');

    if (typeof source !== 'string') {
      return null;
    }

    return source;
  }
}
