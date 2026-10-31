import { CommonModule } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, ViewChild, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { JobApplication, TerminalService } from '../../services/terminal.service';

@Component({
  selector: 'app-terminal-shell',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTableModule],
  templateUrl: './terminal-shell.html',
  styleUrl: './terminal-shell.scss',
})
export class TerminalShellComponent implements AfterViewChecked {
  @ViewChild('terminalOutput') private terminalOutput?: ElementRef<HTMLElement>;

  private readonly terminalService = inject(TerminalService);

  protected readonly terminalLines = this.terminalService.lines$;
  protected readonly command = signal('');
  protected readonly prompt = computed(() => 'guest@terminal:~$');
  protected readonly displayedColumns = ['company', 'role', 'status'];
  protected readonly isJobsPanelVisible = signal(false);
  protected readonly jobRows = signal<JobApplication[]>([]);
  protected readonly isJobsLoading = signal(false);
  protected readonly jobsError = signal<string | null>(null);

  ngOnInit(): void {
    void this.terminalService.startSession('job assistant app');
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  protected submitCommand(): void {
    const text = this.command();

    if (text === undefined || text === null) {
      return;
    }

    if (text.trim().toLowerCase() === 'load') {
      this.isJobsPanelVisible.set(true);
      void this.loadJobApplications();
    }

    void this.terminalService.sendCommand(text);
    this.command.set('');
  }

  private async loadJobApplications(): Promise<void> {
    this.isJobsLoading.set(true);
    this.jobsError.set(null);

    try {
      this.jobRows.set(await this.terminalService.getJobApplications());
    } catch (error) {
      this.jobsError.set(error instanceof Error ? error.message : String(error));
    } finally {
      this.isJobsLoading.set(false);
    }
  }

  private scrollToBottom(): void {
    const element = this.terminalOutput?.nativeElement;

    if (!element) {
      return;
    }

    element.scrollTop = element.scrollHeight;
  }
}
