import { CommonModule } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, ViewChild, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TerminalService } from '../../services/terminal.service';

@Component({
  selector: 'app-terminal-shell',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './terminal-shell.html',
  styleUrl: './terminal-shell.scss',
})
export class TerminalShellComponent implements AfterViewChecked {
  @ViewChild('terminalOutput') private terminalOutput?: ElementRef<HTMLElement>;

  private readonly terminalService = inject(TerminalService);

  protected readonly terminalLines = this.terminalService.lines$;
  protected readonly command = signal('');
  protected readonly prompt = computed(() => 'guest@terminal:~$');

  ngOnInit(): void {
    void this.terminalService.startSession('MAYAN CONSULTING');
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  protected submitCommand(): void {
    const text = this.command();

    if (text === undefined || text === null) {
      return;
    }

    void this.terminalService.sendCommand(text);
    this.command.set('');
  }

  private scrollToBottom(): void {
    const element = this.terminalOutput?.nativeElement;

    if (!element) {
      return;
    }

    element.scrollTop = element.scrollHeight;
  }
}
