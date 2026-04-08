import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Api, StatusItem, StatusRequest } from '../../core/api';

@Component({
  selector: 'app-status',
  imports: [ReactiveFormsModule],
  templateUrl: './status.html',
  styleUrl: './status.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Status implements OnInit {
  private readonly api = inject(Api);
  private readonly fb = inject(FormBuilder);

  protected readonly statuses = signal<StatusItem[]>([]);
  protected readonly errorMessage = signal('');
  protected readonly feedback = signal('');
  protected readonly isSubmitting = signal(false);

  protected readonly statusForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
  });

  ngOnInit(): void {
    this.loadStatus();
  }

  protected createStatus(): void {
    this.feedback.set('');
    this.errorMessage.set('');

    if (this.statusForm.invalid) {
      this.statusForm.markAllAsTouched();
      return;
    }

    const payload: StatusRequest = this.statusForm.getRawValue();
    this.isSubmitting.set(true);

    this.api.createStatus(payload).subscribe({
      next: () => {
        this.feedback.set('Status created successfully.');
        this.statusForm.reset({ name: '' });
        this.loadStatus();
      },
      error: (error: unknown) => this.errorMessage.set(this.getErrorMessage(error)),
      complete: () => this.isSubmitting.set(false),
    });
  }

  private loadStatus(): void {
    this.api.getStatus().subscribe({
      next: (status) => this.statuses.set(status),
      error: (error: unknown) => this.errorMessage.set(this.getErrorMessage(error)),
    });
  }

  private getErrorMessage(error: unknown): string {
    if (error && typeof error === 'object' && 'message' in error) {
      return String(error.message);
    }

    return 'Request failed. Please check API availability and try again.';
  }
}
