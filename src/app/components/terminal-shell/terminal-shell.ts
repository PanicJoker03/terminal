import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TerminalService } from '../../services/terminal.service';

@Component({
  selector: 'app-terminal-shell',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './terminal-shell.html',
  styleUrl: './terminal-shell.scss',
})
export class TerminalShellComponent {
  private readonly terminalService = inject(TerminalService);

  protected readonly terminalLines = this.terminalService.lines$;
  protected readonly command = signal('');
  protected readonly prompt = computed(() => 'guest@terminal:~$');

  ngOnInit(): void {
    void this.terminalService.startSession('MAYAN CONSULTING');
  }

  protected submitCommand(): void {
    const text = this.command().trim();

    if (!text) {
      return;
    }

    void this.terminalService.sendCommand(text);
    this.command.set('');
  }
}
