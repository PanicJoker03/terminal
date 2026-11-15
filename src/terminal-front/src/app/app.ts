import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TerminalShellComponent } from './components/terminal-shell/terminal-shell';
import { TerminalService } from './services/terminal.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TerminalShellComponent, FormsModule],
  template: `
    @if (authenticated()) {
      <app-terminal-shell></app-terminal-shell>
    } @else {
      <main class="login">
        <form (ngSubmit)="submit()">
          <h1>Terminal</h1>
          <label>Username<input name="username" [(ngModel)]="username" autocomplete="username" required></label>
          <label>Password<input name="password" type="password" [(ngModel)]="password" autocomplete="current-password" required></label>
          @if (error()) {
            <p role="alert">{{ error() }}</p>
          }
          <button type="submit" [disabled]="busy()">{{ busy() ? 'Signing in…' : 'Sign in' }}</button>
        </form>
      </main>
    }
  `,
  styleUrl: './app.scss',
})
export class App {
  private readonly terminal = inject(TerminalService);
  protected readonly authenticated = signal(this.terminal.isAuthenticated);
  protected username = '';
  protected password = '';
  protected readonly error = signal('');
  protected readonly busy = signal(false);

  protected async submit(): Promise<void> {
    this.busy.set(true); this.error.set('');
    try { await this.terminal.login(this.username, this.password); this.authenticated.set(true); }
    catch (e) { this.error.set(e instanceof Error ? e.message : 'Sign in failed'); }
    finally { this.busy.set(false); }
  }
}
