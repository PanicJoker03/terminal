import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export type TerminalLineType = 'system' | 'command' | 'output' | 'error';

export interface TerminalLine {
  type: TerminalLineType;
  text: string;
}

@Injectable({
  providedIn: 'root',
})
export class TerminalService {
  private readonly linesSubject = new BehaviorSubject<TerminalLine[]>([
    { type: 'system', text: 'terminal ready' },
    { type: 'system', text: 'backend bridge: webapi / worker / websocket' },
  ]);

  readonly lines$: Observable<TerminalLine[]> = this.linesSubject.asObservable();

  sendCommand(command: string): void {
    const trimmed = command.trim();

    if (!trimmed) {
      return;
    }

    const existing = this.linesSubject.value;
    this.linesSubject.next([
      ...existing,
      { type: 'command', text: `$ ${trimmed}` },
    ]);

    void this.processCommand(trimmed);
  }

  private async processCommand(command: string): Promise<void> {
    const existing = this.linesSubject.value;

    try {
      const result = await this.callBackend(command);

      this.linesSubject.next([
        ...existing,
        { type: 'output', text: result },
      ]);
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);

      this.linesSubject.next([
        ...existing,
        { type: 'error', text: `error: ${message}` },
      ]);
    }
  }

  private async callBackend(command: string): Promise<string> {
    const normalized = command.toLowerCase();

    if (normalized === 'help') {
      return 'available commands: help, status, api, worker, socket, clear';
    }

    if (normalized === 'status') {
      return 'system online — webapi, webworker, and websocket adapters initialized';
    }

    if (normalized === 'api') {
      return 'webapi bridge connected to /api/terminal';
    }

    if (normalized === 'worker') {
      return 'webworker bridge listening for background tasks';
    }

    if (normalized === 'socket') {
      return 'websocket channel connected and ready for live events';
    }

    if (normalized === 'clear') {
      this.linesSubject.next([
        { type: 'system', text: 'terminal cleared' },
      ]);
      return 'terminal buffer reset';
    }

    return `command not recognized: ${command}`;
  }
}
