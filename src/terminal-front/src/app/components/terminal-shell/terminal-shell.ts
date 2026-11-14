import { CommonModule } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, ViewChild, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { SplitAreaComponent, SplitComponent } from 'angular-split';
import { BossChartComponent } from '../boss-chart/boss-chart';
import { ClientChartComponent } from '../client-chart/client-chart';
import { EisenhowerMatrixComponent } from '../eisenhower-matrix/eisenhower-matrix';
import { JobApplication, PriorityTask, TerminalService } from '../../services/terminal.service';

@Component({
  selector: 'app-terminal-shell',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTableModule, SplitComponent, SplitAreaComponent, ClientChartComponent, BossChartComponent, EisenhowerMatrixComponent],
  templateUrl: './terminal-shell.html',
  styleUrl: './terminal-shell.scss',
})
export class TerminalShellComponent implements AfterViewChecked {
  @ViewChild('terminalOutput') private terminalOutput?: ElementRef<HTMLElement>;

  private readonly terminalService = inject(TerminalService);

  protected readonly terminalLines = this.terminalService.lines$;
  protected readonly command = signal('');
  protected readonly prompt = computed(() => `${this.terminalService.userName()}@terminal:~$`);
  protected readonly displayedColumns = [
    'id', 'timestamp', 'company', 'role', 'status', 'modality', 'priority', 'payment', 'origin',
  ];
  protected readonly isJobsPanelVisible = signal(false);
  protected readonly isTasksPanelVisible = signal(false);
  protected readonly isClientChartVisible = signal(false);
  protected readonly isBossChartVisible = signal(false);
  protected readonly isEisenhowerMatrixVisible = signal(false);
  protected readonly jobRows = signal<JobApplication[]>([]);
  protected readonly isJobsLoading = signal(false);
  protected readonly jobsError = signal<string | null>(null);
  protected readonly taskRows = signal<PriorityTask[]>([]);
  protected readonly isTasksLoading = signal(false);
  protected readonly tasksError = signal<string | null>(null);

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

    const normalized = text.trim().toLowerCase();

    if (this.terminalService.isInitialized && normalized === 'trello') {
      window.open(
        'https://trello.com/u/luiscarlosnunezguerra/boards',
        '_blank',
        'noopener,noreferrer',
      );
    }

    if (this.terminalService.isInitialized && normalized === 'github') {
      window.open('https://github.com/PanicJoker03', '_blank', 'noopener,noreferrer');
    }

    if (this.terminalService.isInitialized && normalized === 'linkedin') {
      window.open('https://www.linkedin.com/feed/foryou/', '_blank', 'noopener,noreferrer');
    }

    if (this.terminalService.isInitialized && normalized === 'computrabajo') {
      window.open('https://mx.computrabajo.com', '_blank', 'noopener,noreferrer');
    }

    if (this.terminalService.isInitialized && normalized === 'occ') {
      window.open('https://candidato.occ.com.mx/dashboard', '_blank', 'noopener,noreferrer');
    }

    if (!this.terminalService.isInitialized) {
      void this.terminalService.sendCommand(text);
      this.command.set('');
      return;
    }

    if (normalized === 'load') {
      this.isJobsPanelVisible.set(true);
      this.isTasksPanelVisible.set(false);
      this.isClientChartVisible.set(false);
      void this.loadJobApplications();
    }

    if (normalized === 'client' || normalized === 'clients' || normalized === 'chart') {
      this.isClientChartVisible.set(true);
      this.isBossChartVisible.set(false);
      this.isEisenhowerMatrixVisible.set(false);
      this.isJobsPanelVisible.set(false);
      this.isTasksPanelVisible.set(false);
    }

    if (normalized === 'boss') {
      this.isBossChartVisible.set(true);
      this.isClientChartVisible.set(false);
      this.isEisenhowerMatrixVisible.set(false);
      this.isJobsPanelVisible.set(false);
      this.isTasksPanelVisible.set(false);
    }

    if (normalized === 'eisenhower') {
      this.isEisenhowerMatrixVisible.set(true);
      this.isClientChartVisible.set(false);
      this.isBossChartVisible.set(false);
      this.isJobsPanelVisible.set(false);
      this.isTasksPanelVisible.set(false);
    }

    void this.terminalService.sendCommand(text);
    this.command.set('');
  }

  protected async showTasks(): Promise<void> {
    if (!this.terminalService.isInitialized) {
      void this.terminalService.sendCommand('tasks');
      return;
    }
    this.isTasksPanelVisible.set(true);
    this.isJobsPanelVisible.set(false);
    this.isTasksLoading.set(true);
    this.tasksError.set(null);
    try {
      this.taskRows.set(await this.terminalService.getPriorityTasks());
    } catch (error) {
      this.tasksError.set(error instanceof Error ? error.message : String(error));
    } finally {
      this.isTasksLoading.set(false);
    }
  }

  protected async toggleTask(task: PriorityTask): Promise<void> {
    if (!this.terminalService.isInitialized) {
      void this.terminalService.sendCommand('tasks');
      return;
    }
    const completed = !task.completed;
    this.taskRows.update(rows => rows.map(row => row.id === task.id ? { ...row, completed } : row));
    try {
      await this.terminalService.setPriorityTaskCompleted(task.id, completed);
    } catch (error) {
      this.taskRows.update(rows => rows.map(row => row.id === task.id ? { ...row, completed: task.completed } : row));
      this.tasksError.set(error instanceof Error ? error.message : String(error));
    }
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
