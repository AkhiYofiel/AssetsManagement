import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Api, EmployeeRequest, Employee } from '../../core/api';

@Component({
  selector: 'app-employees',
  imports: [ReactiveFormsModule],
  templateUrl: './employees.html',
  styleUrl: './employees.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Employees implements OnInit {
  private readonly api = inject(Api);
  private readonly fb = inject(FormBuilder);

  protected readonly employees = signal<Employee[]>([]);
  protected readonly errorMessage = signal('');
  protected readonly feedback = signal('');
  protected readonly isSubmitting = signal(false);

  protected readonly employeeForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
  });

  ngOnInit(): void {
    this.loadEmployees();
  }

  protected createEmployee(): void {
    this.feedback.set('');
    this.errorMessage.set('');

    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    const payload: EmployeeRequest = this.employeeForm.getRawValue();
    this.isSubmitting.set(true);

    this.api.createEmployee(payload).subscribe({
      next: () => {
        this.feedback.set('Employee created successfully.');
        this.employeeForm.reset({
          firstName: '',
          lastName: '',
          email: '',
        });
        this.loadEmployees();
      },
      error: (error: unknown) => this.errorMessage.set(this.getErrorMessage(error)),
      complete: () => this.isSubmitting.set(false),
    });
  }

  private loadEmployees(): void {
    this.api.getEmployees().subscribe({
      next: (employees) => this.employees.set(employees),
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
